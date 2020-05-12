namespace NVShop.Data.FS
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using FrameworkSystems.FrameworkBase;

    public class FSSubQuery<TFSEntity, TFSSubEntity>
        where TFSEntity : class, IDevFrameworkDataObject
        where TFSSubEntity : class, IDevFrameworkDataObject
    {
        private readonly FSUtil _util;

        public FSSubQuery(FSUtil util) {
            _util = util;

            Relations = new FSSubQueryList<TFSSubEntity, TFSEntity>(_util);
            Conditions = new FSQueryList<TFSSubEntity>(_util);
        }

        public FSSubQuery(
            FSUtil util,
            Expression<Func<TFSSubEntity, object>> field = null,
            Expression<Func<TFSSubEntity, object>> from = null,
            string alias = null,
            FSSubQueryList<TFSSubEntity, TFSEntity> relations = null,
            FSQueryList<TFSSubEntity> conditions = null,
            Func<string, string> fieldModifier = null
        ) : this(util)
        {
            _util = util;

            if (field != null)
            {
                FieldExpr = field;
            }

            if (fieldModifier != null)
            {
                FieldModifier = fieldModifier;
            }

            if (from != null)
            {
                FromExpr = from;
            }

            if (alias != null)
            {
                Alias = alias;
            }

            if (relations != null)
            {
                Relations = relations;
            }

            if (conditions != null)
            {
                Conditions = conditions;
            }
        }

        public string Field { get; set; } = "1";
        public string Alias { get; set; } = "";

        public Expression<Func<TFSSubEntity, object>> FieldExpr { get; set; }
        public Func<string, string> FieldModifier { get; set; }
        public Expression<Func<TFSSubEntity, object>> FromExpr { get; set; }

        public FSSubQueryList<TFSSubEntity, TFSEntity> Relations { get; set; }
            

        public FSQueryList<TFSSubEntity> Conditions { get; set; }

        public override string ToString()
        {
            var helper = _util.QueryHelper<TFSSubEntity>();

            var field = FieldExpr != null
                ? helper.Prop(FieldExpr)
                : Field;

            if (FieldModifier != null)
            {
                field = FieldModifier(field);
            }

            var branchCondition = helper.ExtendBranchCondition();

            var from = FromExpr != null ? helper.Table(FromExpr) : helper.Table();


            if (branchCondition.HasValue())
            {
                Conditions.Add(branchCondition);
            }

            if (!from.StartsWith(" FROM ", StringComparison.InvariantCultureIgnoreCase))
            {
                from = " FROM " + from;
            }

            var where = Relations.Concat(Conditions).Join(" AND ");
            if (!where.HasValue())
            {
                where = " 1 = 1 "; 
            }

            var query = $"SELECT {field} {from} {Alias} WHERE {where}";

            if (Alias.HasValue())
            {
                query = query.Replace($" {from}.", $" {Alias}.");
            }

            return query;
        }
    }
}