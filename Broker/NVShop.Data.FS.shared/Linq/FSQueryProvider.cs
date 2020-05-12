namespace NVShop.Data.FS.Linq
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;

    internal class FSQueryProvider : IQueryProvider
    {
        private readonly IFSQueryContext _queryContext;

        public FSQueryProvider(IFSQueryContext queryContext)
        {
            _queryContext = queryContext;
        }

        /// <summary>Konstruiert ein <see cref="T:System.Linq.IQueryable" />-Objekt, das die Abfrage auswerten kann, die von einer angegebenen Ausdrucksbaumstruktur dargestellt wird.</summary>
        /// <returns>Ein <see cref="T:System.Linq.IQueryable" />-Objekt, das die Abfrage auswerten kann, die von der angegebenen Ausdrucksbaumstruktur dargestellt wird.</returns>
        /// <param name="expression">Eine Ausdrucksbaumstruktur, die eine LINQ-Abfrage darstellt.</param>
        public IQueryable CreateQuery(Expression expression)
        {
            var elementType = expression.Type.GetElementType();
            try
            {
                return (IQueryable)Activator.CreateInstance(typeof(FSQueryable<>).MakeGenericType(elementType), this, expression);
            }
            catch (System.Reflection.TargetInvocationException tie)
            {
                throw tie.InnerException;
            }

        }

        /// <summary>Konstruiert ein <see cref="T:System.Linq.IQueryable`1" />-Objekt, das die Abfrage auswerten kann, die von einer angegebenen Ausdrucksbaumstruktur dargestellt wird.</summary>
        /// <returns>Ein <see cref="T:System.Linq.IQueryable`1" />-Objekt, das die Abfrage auswerten kann, die von der angegebenen Ausdrucksbaumstruktur dargestellt wird.</returns>
        /// <param name="expression">Eine Ausdrucksbaumstruktur, die eine LINQ-Abfrage darstellt.</param>
        /// <typeparam name="TElement">Der Typ der Elemente des <see cref="T:System.Linq.IQueryable`1" />-Objekts, das zurückgegeben wird.</typeparam>
        public IQueryable<TResult> CreateQuery<TResult>(Expression expression)
        {
            return new FSQueryable<TResult>(this, expression);
        }

        /// <summary>Führt die Abfrage aus, die von einer angegebenen Ausdrucksbaumstruktur dargestellt wird.</summary>
        /// <returns>Der Wert, der aus der Ausführung der angegebenen Abfrage resultiert.</returns>
        /// <param name="expression">Eine Ausdrucksbaumstruktur, die eine LINQ-Abfrage darstellt.</param>
        public object Execute(Expression expression)
        {
            return _queryContext.Execute(expression, false);
        }

        /// <summary>Führt die stark typisierte Abfrage aus, die von einer angegebenen Ausdrucksbaumstruktur dargestellt wird.</summary>
        /// <returns>Der Wert, der aus der Ausführung der angegebenen Abfrage resultiert.</returns>
        /// <param name="expression">Eine Ausdrucksbaumstruktur, die eine LINQ-Abfrage darstellt.</param>
        /// <typeparam name="TResult">Der Typ des Werts, der aus der Ausführung der Abfrage resultiert.</typeparam>
        public TResult Execute<TResult>(Expression expression)
        {
            var isEnumerable = typeof(TResult).Name == "IEnumerable`1";

            return (TResult)_queryContext.Execute(expression, isEnumerable);
        }
    }
}