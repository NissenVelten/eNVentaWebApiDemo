namespace NVShop.Data.FS
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using FrameworkSystems.FrameworkBase;
    using FrameworkSystems.FrameworkBase.Metadatatype;
    using FrameworkSystems.FrameworkDataProvider;

    public class FSQueryHelper<TFSEntity>
        where TFSEntity : class, IDevFrameworkDataObject
    {
        private string _where;

        public FSQueryHelper(FSUtil util)
        {
            FSUtil = util;
            Entity = util.Create<TFSEntity>();
        }


        protected FSUtil FSUtil { get; }
        protected TFSEntity Entity { get; }

        public FSQueryHelper<TFSEntity> Where(string where, Func<FSQueryHelper<TFSEntity>, string>[] selectParams, Func<FSQueryHelper<TFSEntity>, string>[] whereParams)
        {
            _where = where;

            return this;
        }

        public string Prop<TProperty>(Expression<Func<TFSEntity, TProperty>> property)
        {
            return Entity.Prop(property);
        }

        public string Value(dynamic value) => DB.SqlString(value);

        public string Value(Guid value) => DB.SqlString(value);

        public string Value(int value) => DB.SqlString(value);

        public string Value(long value) => DB.SqlString(value);

        public string Value(double value) => DB.SqlString(value);

        public string Value(float value) => DB.SqlString(value);

        public string Value(decimal value) => DB.SqlString(value);

        public string Value(string value) => DB.SqlString(value);

        public string Value(DateTime value) => DB.SqlString(value);

        public string Value(short value) => DB.SqlString(value);

        public string Value(byte value) => DB.SqlString(value);

        public string Value(string op, object value) => DB.SqlString(op, value);

        public string Value(IFSSystemGuid rowId) => DB.SqlStringRowID(rowId);

        public string Value(string op, IFSSystemGuid rowId) => DB.SqlStringRowID(op, rowId);

        public string ValueObject(object value) => value.ToString();

        public string Table()
        {
            if (IsJoinTable())
            {
                return Entity.QueryInfo.OverrideFromClause;
            }

            return Entity.Table();
        }

        public bool IsJoinTable() => Entity.GetQueryJoinTables().Count > 1;

        public string Table(TFSEntity entity) => entity.Table();

        public string Table(Expression<Func<TFSEntity, object>> property)
        {
            return Entity.Table(property);
        }

        public string Exists<TFSSubEntity>(
            Expression<Func<TFSSubEntity, object>> from = null,
            FSSubQueryList<TFSSubEntity, TFSEntity> relations = null,
            FSQueryList<TFSSubEntity> conditions = null
        ) where TFSSubEntity : class, IDevFrameworkDataObject
        {
            return SubQuery(" EXISTS ", () => new FSSubQuery<TFSEntity, TFSSubEntity>(
                FSUtil,
                from: from,
                relations: relations,
                conditions: conditions
            ).ToString());
        }

        public string Exists<TFSSubEntity>(Func<FSSubQuery<TFSEntity, TFSSubEntity>, string> expr)
            where TFSSubEntity : class, IDevFrameworkDataObject
        {
            return SubQuery(" EXISTS ", () => expr(new FSSubQuery<TFSEntity, TFSSubEntity>(FSUtil)));
        }

        public string Exists(string subQuery)
        {
            return SubQuery(" EXISTS ", () => subQuery);
        }

        public string NotExists<TFSSubEntity>(Func<FSSubQuery<TFSEntity, TFSSubEntity>, string> expr)
            where TFSSubEntity : class, IDevFrameworkDataObject
        {
            return SubQuery(" NOT EXISTS ", () => expr(new FSSubQuery<TFSEntity, TFSSubEntity>(FSUtil)));
        }

        public string NotExists(string subQuery)
        {
            return SubQuery(" NOT EXISTS ", () => subQuery);
        }

        public string In(Expression<Func<TFSEntity, object>> prop, IEnumerable<object> values, bool useDbSql = true)
        {
            var elements = values;
            if (useDbSql)
            {
                elements = elements.Select(DB.SqlStringObject);
            }
            return $" {Entity.Prop(prop)} IN ({elements.Join(",")})";
        }

        public string In(Expression<Func<TFSEntity, dynamic>> prop, string inClause)
        {
            return $" {Entity.Prop(prop)} IN ({inClause})";
        }

        public string In<TFSSubEntity>(Expression<Func<TFSEntity, object>> prop, FSSubQuery<TFSEntity, TFSSubEntity> expr)
            where TFSSubEntity : class, IDevFrameworkDataObject
        {
            return SubQuery($" {Entity.Prop(prop)} IN ", expr.ToString);
        }

        public string In<TFSSubEntity>(
            Expression<Func<TFSEntity, object>> prop,
            Expression<Func<TFSSubEntity, object>> field = null,
            Expression<Func<TFSSubEntity, object>> from = null,
            string alias = null,
            FSSubQueryList<TFSSubEntity, TFSEntity> relations = null,
            FSQueryList<TFSSubEntity> conditions = null
        ) where TFSSubEntity : class, IDevFrameworkDataObject
        {
            return SubQuery($" {Entity.Prop(prop)} IN ", () => new FSSubQuery<TFSEntity, TFSSubEntity>(
                FSUtil,
                field,
                from,
                alias,
                relations,
                conditions
            ).ToString());
        }

        public string NotIn<TFSSubEntity>(Expression<Func<TFSEntity, object>> prop, FSSubQuery<TFSEntity, TFSSubEntity> expr)
            where TFSSubEntity : class, IDevFrameworkDataObject
        {
            return SubQuery($" {Entity.Prop(prop)} NOT IN ", expr.ToString);
        }

        private string SubQuery(string subQuery, Func<string> expr)
        {
            return $" {subQuery} ({expr()})";
        }

        public string Eq(Expression<Func<TFSEntity, object>> prop, object val, bool upperCase = false)
        {
            if (upperCase)
            {
                return $" UPPER({Entity.Prop(prop)}) = UPPER({DB.SqlStringObject(val)})";
            }
            return $" {Entity.Prop(prop)} = {DB.SqlStringObject(val)}";
        }

        public string Eq(Expression<Func<TFSEntity, object>> prop, Expression<Func<TFSEntity, object>> prop2, bool upperCase = false)
        {
            if (upperCase)
            {
                return $" UPPER({Entity.Prop(prop)}) = UPPER({Entity.Prop(prop2)})";
            }
            return $" {Entity.Prop(prop)} = {Entity.Prop(prop2)}";
        }

        public string Eq(string propName, object val, bool upperCase = false)
        {
            if (upperCase)
            {
                return $" UPPER({propName}) = UPPER({DB.SqlStringObject(val)})";
            }
            return $" {propName} = {DB.SqlStringObject(val)}";
        }

        public string Between(Expression<Func<TFSEntity, object>> prop, object lower, object upper)
        {
            return $" {Entity.Prop(prop)} >= {DB.SqlStringObject(lower)} AND {Entity.Prop(prop)} < {DB.SqlStringObject(upper)}";
        }

        public string GreaterThanOrEq(Expression<Func<TFSEntity, object>> prop, object val)
        {
            return $" {Entity.Prop(prop)} >= {DB.SqlStringObject(val)}";
        }

        public string GreaterThan(Expression<Func<TFSEntity, object>> prop, object val)
        {
            return $" {Entity.Prop(prop)} > {DB.SqlStringObject(val)}";
        }

        public string LessThanOrEq(Expression<Func<TFSEntity, object>> prop, object val)
        {
            return $" {Entity.Prop(prop)} <= {DB.SqlStringObject(val)}";
        }

        public string LessThan(Expression<Func<TFSEntity, object>> prop, object val)
        {
            return $" {Entity.Prop(prop)} < {DB.SqlStringObject(val)}";
        }

        public string NotEq(Expression<Func<TFSEntity, object>> prop, object val)
        {
            return $" {Entity.Prop(prop)} != {DB.SqlStringObject(val)}";
        }

        public string NotEq(Expression<Func<TFSEntity, object>> prop, Expression<Func<TFSEntity, object>> prop2)
        {
            return $" {Entity.Prop(prop)} != {Entity.Prop(prop2)}";
        }

        public string Like(Expression<Func<TFSEntity, object>> prop, object val, bool upperCase = false)
        {
            if (upperCase)
            {
                return $" UPPER({Entity.Prop(prop)}) LIKE UPPER({DB.SqlStringObject(val + "%")})";
            }

            return $" {Entity.Prop(prop)} LIKE {DB.SqlStringObject(val + "%")}";
        }

        public string Like<TFSSubEntity>(Expression<Func<TFSEntity, object>> prop, FSSubQuery<TFSEntity, TFSSubEntity> expr)
            where TFSSubEntity : class, IDevFrameworkDataObject
        {
            return SubQuery($" {Entity.Prop(prop)} LIKE ", expr.ToString);
        }

        public string Like<TFSSubEntity>(Expression<Func<TFSEntity, object>> prop, Expression<Func<TFSSubEntity, object>> field = null,
            Expression<Func<TFSSubEntity, object>> from = null,
            string alias = null,
            FSSubQueryList<TFSSubEntity, TFSEntity> relations = null,
            FSQueryList<TFSSubEntity> conditions = null)
            where TFSSubEntity : class, IDevFrameworkDataObject
        {
            return SubQuery($" {Entity.Prop(prop)} LIKE ", () => new FSSubQuery<TFSEntity, TFSSubEntity>(
                FSUtil,
                field: field,
                fieldModifier: x => $"{x} + '%'", 
                from: from,
                alias: alias,
                relations: relations,
                conditions: conditions
            ).ToString());
        }

        public string NotLike(Expression<Func<TFSEntity, object>> prop, object val)
        {
            return $" {Entity.Prop(prop)} NOT LIKE {DB.SqlStringObject(val + "%")}";
        }

        public string Coalesce(Expression<Func<TFSEntity, object>> prop, object defaultValue, object value)
        {
            return $" COALESCE({Entity.Prop(prop)}, {defaultValue}) = {value}";
        }

        public string NotCoalesce(Expression<Func<TFSEntity, object>> prop, object defaultValue, object value)
        {
            return $" NOT COALESCE({Entity.Prop(prop)}, {defaultValue}) = {value}";
        }
       

        public string NotNull(Expression<Func<TFSEntity, object>> prop)
        {
            return $" {Entity.Prop(prop)} IS NOT NULL";
        }

        public string Null(Expression<Func<TFSEntity, object>> prop)
        {
            return $" {Entity.Prop(prop)} IS NULL";
        }

        public string BranchCondition(bool native = false)
        {
            return FSUtil.BranchCondition(Entity, native);
        }

        public string BranchCondition<TProperty>(Expression<Func<TFSEntity, TProperty>> property, string alias = null, bool native = false)
        {
            return FSUtil.BranchCondition(Entity, property, alias, native);
        }

        public string BranchCondition(string tableName, string alias = null, bool native = false)
        {
            return FSUtil.BranchCondition(tableName, alias, native);
        }

        public string GetWhereBranch(string tableName, string prefix = "AND", string alias = null, bool native = false)
        {
            return tableName.IsNullOrEmpty()
                ? FSUtil.GetWhereBranch(Entity, prefix, alias, native)
                : FSUtil.GetWhereBranch(tableName, prefix, alias, native);
        }


        public string GetWhereBranch(Expression<Func<TFSEntity, object>> property, string prefix = "AND", string alias = null, bool native = false)
        {
            var tableName = Table(property);

            return FSUtil.GetWhereBranch(tableName, prefix, alias, native);
        }

        public string ExtendBranchCondition()
        {
            return FSUtil.ExtendBranch(Entity);
        }
    }

    public enum SubQueryOperator
    {
        Exists,
        NotExists,
        In,
        NotIn
    }
}