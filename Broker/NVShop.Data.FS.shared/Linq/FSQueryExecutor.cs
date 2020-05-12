namespace NVShop.Data.FS.Linq
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
	using NVShop.Data.FS.Linq.Query;
	using NVShop.Data.FS.Linq.Visitors;
	using Remotion.Linq;

    internal class FSQueryExecutor<TFSEntity> : IQueryExecutor
    {
        private readonly IFSRepository<TFSEntity> _rep;
		private readonly FSCommand _command;

		public FSQueryExecutor(IFSRepository<TFSEntity> rep)
        {
            _rep = rep;
		}

		public FSQueryExecutor(IFSRepository<TFSEntity> rep, FSCommand command)
		{
			_rep = rep;
			_command = command;
		}

		/// <summary>
		///     Executes the given <paramref name="queryModel" /> as a scalar query, i.e. as a query returning a scalar value of
		///     type <typeparamref name="T" />.
		///     The query ends with a scalar result operator, for example a
		///     <see cref="T:Remotion.Linq.Clauses.ResultOperators.CountResultOperator" /> or a
		///     <see cref="T:Remotion.Linq.Clauses.ResultOperators.SumResultOperator" />.
		/// </summary>
		/// <typeparam name="T">The type of the scalar value returned by the query.</typeparam>
		/// <param name="queryModel">
		///     The <see cref="T:Remotion.Linq.QueryModel" /> representing the query to be executed. Analyze this via an
		///     <see cref="T:Remotion.Linq.IQueryModelVisitor" />.
		/// </param>
		/// <returns>A scalar value of type <typeparamref name="T" /> that represents the query's result.</returns>
		/// <remarks>
		///     The difference between
		///     <see cref="M:Remotion.Linq.IQueryExecutor.ExecuteSingle``1(Remotion.Linq.QueryModel,System.Boolean)" /> and
		///     <see cref="M:Remotion.Linq.IQueryExecutor.ExecuteScalar``1(Remotion.Linq.QueryModel)" /> is in the kind of object
		///     that is returned.
		///     <see cref="M:Remotion.Linq.IQueryExecutor.ExecuteSingle``1(Remotion.Linq.QueryModel,System.Boolean)" /> is used
		///     when a query that would otherwise return a collection result set should pick a single value from the
		///     set, for example the first, last, minimum, maximum, or only value in the set.
		///     <see cref="M:Remotion.Linq.IQueryExecutor.ExecuteScalar``1(Remotion.Linq.QueryModel)" /> is used when a value is
		///     calculated or aggregated from all the values in the collection result set. This applies to, for example, item
		///     counts, average calculations,
		///     checks for the existence of a specific item, and so on.
		/// </remarks>
		T IQueryExecutor.ExecuteScalar<T>(QueryModel queryModel)
        {
            var command = FSQueryModelVisitor.GenerateFSQuery(queryModel);

            if (_command != null)
            {
                var whereClause = command.WhereClause.HasValue() && _command.WhereClause.HasValue() 
                    ? command.WhereClause + " AND " + _command.WhereClause
                    : command.WhereClause.HasValue() 
                        ? command.WhereClause
                        : _command.WhereClause.HasValue() 
                            ? _command.WhereClause
                            : string.Empty;

                if (command.IsCountQuery)
                {
                    var count = _rep.Count(whereClause);

                    if (typeof(T) == typeof(int))
                    {
                        return (T)(object)count;
                    }
                    return default(T);
                }

                if (command.IsLongCountQuery)
                {
                    var count = _rep.Count(whereClause);

                    if (typeof(T) == typeof(long))
                    {
                        return (T)(object)(long)count;
                    }
                    return default(T);
                }
            }


            return ExecuteCollection<T>(queryModel)
                .Single();
        }

        /// <summary>
        ///     Executes the given <paramref name="queryModel" /> as a single object query, i.e. as a query returning a single
        ///     object of type
        ///     <typeparamref name="T" />.
        ///     The query ends with a single result operator, for example a
        ///     <see cref="T:Remotion.Linq.Clauses.ResultOperators.FirstResultOperator" /> or a
        ///     <see cref="T:Remotion.Linq.Clauses.ResultOperators.SingleResultOperator" />.
        /// </summary>
        /// <typeparam name="T">The type of the single value returned by the query.</typeparam>
        /// <param name="queryModel">
        ///     The <see cref="T:Remotion.Linq.QueryModel" /> representing the query to be executed. Analyze this via an
        ///     <see cref="T:Remotion.Linq.IQueryModelVisitor" />.
        /// </param>
        /// <param name="returnDefaultWhenEmpty">
        ///     If <see langword="true" />, the executor must return a default value when its result set is empty;
        ///     if <see langword="false" />, it should throw an <see cref="T:System.InvalidOperationException" /> when its result
        ///     set is empty.
        /// </param>
        /// <returns>A single value of type <typeparamref name="T" /> that represents the query's result.</returns>
        /// <remarks>
        ///     The difference between
        ///     <see cref="M:Remotion.Linq.IQueryExecutor.ExecuteSingle``1(Remotion.Linq.QueryModel,System.Boolean)" /> and
        ///     <see cref="M:Remotion.Linq.IQueryExecutor.ExecuteScalar``1(Remotion.Linq.QueryModel)" /> is in the kind of object
        ///     that is returned.
        ///     <see cref="M:Remotion.Linq.IQueryExecutor.ExecuteSingle``1(Remotion.Linq.QueryModel,System.Boolean)" /> is used
        ///     when a query that would otherwise return a collection result set should pick a single value from the
        ///     set, for example the first, last, minimum, maximum, or only value in the set.
        ///     <see cref="M:Remotion.Linq.IQueryExecutor.ExecuteScalar``1(Remotion.Linq.QueryModel)" /> is used when a value is
        ///     calculated or aggregated from all the values in the collection result set. This applies to, for example, item
        ///     counts, average calculations,
        ///     checks for the existence of a specific item, and so on.
        /// </remarks>
        T IQueryExecutor.ExecuteSingle<T>(QueryModel queryModel, bool returnDefaultWhenEmpty)
        {
            return returnDefaultWhenEmpty
                ? ExecuteCollection<T>(queryModel)
                    .SingleOrDefault()
                : ExecuteCollection<T>(queryModel)
                    .Single();
        }

        /// <summary>
        ///     Executes the given <paramref name="queryModel" /> as a collection query, i.e. as a query returning objects of type
        ///     <typeparamref name="T" />.
        ///     The query does not end with a scalar result operator, but it can end with a single result operator, for example
        ///     <see cref="T:Remotion.Linq.Clauses.ResultOperators.SingleResultOperator" /> or
        ///     <see cref="T:Remotion.Linq.Clauses.ResultOperators.FirstResultOperator" />. In such a case, the returned enumerable
        ///     must yield exactly
        ///     one object (or none if the last result operator allows empty result sets).
        /// </summary>
        /// <typeparam name="T">The type of the items returned by the query.</typeparam>
        /// <typeparam name="TFSEntity"></typeparam>
        /// <param name="queryModel">
        ///     The <see cref="T:Remotion.Linq.QueryModel" /> representing the query to be executed. Analyze this via an
        ///     <see cref="T:Remotion.Linq.IQueryModelVisitor" />.
        /// </param>
        /// <returns>A scalar value of type <typeparamref name="TFSEntity" /> that represents the query's result.</returns>
        public IEnumerable<T> ExecuteCollection<T>(QueryModel queryModel)
        {
			var command = FSQueryModelVisitor.GenerateFSQuery(queryModel);

			if (_command != null)
			{
				var whereClause = command.WhereClause.HasValue() && _command.WhereClause.HasValue() 
                    ? command.WhereClause + " AND " + _command.WhereClause
					: command.WhereClause.HasValue() 
                        ? command.WhereClause
					    : _command.WhereClause.HasValue() 
                            ? _command.WhereClause
					        : string.Empty;

                var orderBy = _command.OrderByClause.HasValue()
                    ? string.Join(",", _command.OrderByClause, command.OrderByClause).Trim(',')
                    : command.OrderByClause;

                command = new FSCommand(
                    whereClause: whereClause,
                    orderByClause: orderBy,
					offset: command.Offset ?? _command.Offset,
					limit: command.Limit ?? _command.Limit
				);
			}

			var fsQuery = _rep.Get(command.WhereClause, command.OrderByClause, command.Offset, command.Limit);

			var projection = FSProjectionVisitor<TFSEntity, T>.BuildProjector(queryModel.SelectClause.Selector);

			return fsQuery.Select(projection);
        }
    }
}