namespace NVShop.Data.FS
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using FrameworkSystems.FrameworkBase;
    using System.Linq;

    public class FSQueryList<TFSEntity> : List<string>
        where TFSEntity : class, IDevFrameworkDataObject
    {
        public FSQueryList(FSUtil util)
        {
            FSUtil = util;
        }

        protected FSUtil FSUtil { get; }

        public void Add(params Func<FSQueryHelper<TFSEntity>, string>[] builders)
        {
            foreach (var builder in builders) {
                Add(builder(new FSQueryHelper<TFSEntity>(FSUtil)));
            }
        }
        
        public FSQueryList<TFSEntity> Exists<TFSSubEntity>(
            Expression<Func<TFSSubEntity, object>> from = null,
            Func<FSSubQueryList<TFSSubEntity, TFSEntity>, FSSubQueryList<TFSSubEntity, TFSEntity>> relations = null,
            Func<FSQueryList<TFSSubEntity>, FSQueryList<TFSSubEntity>> conditions = null
        ) where TFSSubEntity : class, IDevFrameworkDataObject
        {
            Add(x => x.Exists(from, 
                relations?.Invoke(new FSSubQueryList<TFSSubEntity, TFSEntity>(FSUtil)), 
                conditions?.Invoke(new FSQueryList<TFSSubEntity>(FSUtil))
            ).ToString());

            return this;
        }
        
        public FSQueryList<TFSEntity> NotExists<TFSSubEntity>(Func<FSSubQuery<TFSEntity, TFSSubEntity>, string> expr)
            where TFSSubEntity : class, IDevFrameworkDataObject
        {
            Add(x => x.NotExists(expr));

            return this;
        }

        public FSQueryList<TFSEntity> Eq(Expression<Func<TFSEntity, object>> prop, object val, bool upperCase = false)
        {
            Add(x => x.Eq(prop, val, upperCase));

            return this;
        }

        public FSQueryList<TFSEntity> Eq(Expression<Func<TFSEntity, object>> prop, Expression<Func<TFSEntity, object>> prop2, bool upperCase = false)
        {
            Add(x => x.Eq(prop, prop2, upperCase));

            return this;
        }

        public FSQueryList<TFSEntity> Eq(string propName, object val, bool upperCase = false)
        {
            Add(x => x.Eq(propName, val, upperCase));

            return this;
        }

        public FSQueryList<TFSEntity> Between(Expression<Func<TFSEntity, object>> prop, object lower, object upper)
        {
            Add(x => x.Between(prop, lower, upper));

            return this;
        }

        public FSQueryList<TFSEntity> GreaterThanOrEq(Expression<Func<TFSEntity, object>> prop, object val)
        {
            Add(x => x.GreaterThanOrEq(prop, val));

            return this;
        }

        public FSQueryList<TFSEntity> GreaterThan(Expression<Func<TFSEntity, object>> prop, object val)
        {
            Add(x => x.GreaterThan(prop, val));

            return this;
        }

        public FSQueryList<TFSEntity> LessThanOrEq(Expression<Func<TFSEntity, object>> prop, object val)
        {
            Add(x => x.LessThanOrEq(prop, val));

            return this;
        }

        public FSQueryList<TFSEntity> LessThan(Expression<Func<TFSEntity, object>> prop, object val)
        {
            Add(x => x.LessThan(prop, val));

            return this;
        }

        public FSQueryList<TFSEntity> NotEq(Expression<Func<TFSEntity, object>> prop, object val)
        {
            Add(x => x.NotEq(prop, val));

            return this;
        }

        public FSQueryList<TFSEntity> NotEq(Expression<Func<TFSEntity, object>> prop, Expression<Func<TFSEntity, object>> prop2)
        {
            Add(x => x.NotEq(prop, prop2));

            return this;
        }

        public FSQueryList<TFSEntity> Like(Expression<Func<TFSEntity, object>> prop, object val, bool upperCase = false)
        {
            Add(x => x.Like(prop, val, upperCase));

            return this;
        }

        public FSQueryList<TFSEntity> Like<TFSSubEntity>(
            Expression<Func<TFSEntity, object>> prop,
            Expression<Func<TFSSubEntity, object>> field = null,
            Expression<Func<TFSSubEntity, object>> from = null,
            string alias = null,
            Func<FSSubQueryList<TFSSubEntity, TFSEntity>, FSSubQueryList<TFSSubEntity, TFSEntity>> relations = null,
            Func<FSQueryList<TFSSubEntity>, FSQueryList<TFSSubEntity>> conditions = null
        ) where TFSSubEntity : class, IDevFrameworkDataObject
        {
            Add(x => x.Like(prop, field, from, alias,
                relations?.Invoke(new FSSubQueryList<TFSSubEntity, TFSEntity>(FSUtil)),
                conditions?.Invoke(new FSQueryList<TFSSubEntity>(FSUtil)))
            );

            return this;
        }

        public FSQueryList<TFSEntity> Not(Func<FSQueryHelper<TFSEntity>, string> expr)
        {
            Add($"NOT {expr(new FSQueryHelper<TFSEntity>(FSUtil))}");

            return this;
        }

        public FSQueryList<TFSEntity> NotLike(Expression<Func<TFSEntity, object>> prop, object val)
        {
            Add(x => x.NotLike(prop, val));

            return this;
        }

        public FSQueryList<TFSEntity> Coalesce(Expression<Func<TFSEntity, object>> prop, object defaultValue, object val)
        {
            Add(x => x.Coalesce(prop, defaultValue, val));

            return this;
        }

        public FSQueryList<TFSEntity> NotCoalesce(Expression<Func<TFSEntity, object>> prop, object defaultValue, object val)
        {
            Add(x => x.NotCoalesce(prop, defaultValue, val));

            return this;
        }

        public FSQueryList<TFSEntity> NotNull(Expression<Func<TFSEntity, object>> prop)
        {
            Add(x => x.NotNull(prop));

            return this;
        }

        public FSQueryList<TFSEntity> Null(Expression<Func<TFSEntity, object>> prop)
        {
            Add(x => x.Null(prop));

            return this;
        }

        public FSQueryList<TFSEntity> In(Expression<Func<TFSEntity, object>> prop, IEnumerable<object> values, bool useDbSql = false)
        {
            if (!values.Any())
                return this;

            Add(x => x.In(prop, values, useDbSql));

            return this;
        }
        
        public FSQueryList<TFSEntity> In(Expression<Func<TFSEntity, dynamic>> prop, string inClause)
        {
            Add(x => x.In(prop, inClause));

            return this;
        }


        public FSQueryList<TFSEntity> In<TFSSubEntity>(
            Expression<Func<TFSEntity, object>> prop,
            Expression<Func<TFSSubEntity, object>> field = null,
            Expression<Func<TFSSubEntity, object>> from = null,
            string alias = null,
            Func<FSSubQueryList<TFSSubEntity, TFSEntity>, FSSubQueryList<TFSSubEntity, TFSEntity>> relations = null,
            Func<FSQueryList<TFSSubEntity>, FSQueryList<TFSSubEntity>> conditions = null
        ) where TFSSubEntity : class, IDevFrameworkDataObject
        {
            Add(x => x.In(prop, field, from, alias,
                relations?.Invoke(new FSSubQueryList<TFSSubEntity, TFSEntity>(FSUtil)),
                conditions?.Invoke(new FSQueryList<TFSSubEntity>(FSUtil)))
            );

            return this;
        }

        public FSQueryList<TFSEntity> Block(Func<FSQueryList<TFSEntity>, FSQueryList<TFSEntity>> expr, FSQueryOperator op = FSQueryOperator.Or)
        {
            Add($"({expr(new FSQueryList<TFSEntity>(FSUtil)).Join(op == FSQueryOperator.Or ? " OR " : " AND ")})");

            return this;
        }

        public FSQueryList<TFSEntity> NotBlock(Func<FSQueryList<TFSEntity>, FSQueryList<TFSEntity>> expr, FSQueryOperator op = FSQueryOperator.Or)
        {
            Add($"NOT ({expr(new FSQueryList<TFSEntity>(FSUtil)).Join(op == FSQueryOperator.Or ? " OR " : " AND ")})");

            return this;
        }

        public FSQueryList<TFSEntity> Where(string whereClause)
        {
            Add(whereClause);

            return this;
        }
    }
}