namespace NVShop.Data.FS
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public partial interface IFSRepository<T>
    {
        IQueryable<T> Queryable(string where = "", string orderby = "");

        /// <summary>
        /// Gets the specified where.
        /// </summary>
        /// <param name="where">The where.</param>
        /// <param name="orderby">The orderby.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="loadSize">Size of the load.</param>
        /// <param name="progress">The progress.</param>
        /// <param name="syncState">State of the synchronize.</param>
        /// <returns></returns>
        IEnumerable<T> Get(
            string where = "",
            string orderby = "",
            int? offset = null,
            int? limit = null,
            int? loadSize = null
        );

        /// <summary>
        /// Gets the asynchronous.
        /// </summary>
        /// <param name="where">The where.</param>
        /// <param name="orderby">The orderby.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="loadSize">Size of the load.</param>
        /// <param name="progress">The progress.</param>
        /// <param name="syncState">State of the synchronize.</param>
        /// <returns></returns>
        Task<IEnumerable<T>> GetAsync(
            string where = "",
            string orderby = "",
            int? offset = null,
            int? limit = null,
            int? loadSize = null
        );

        /// <summary>
        /// Idents the specified where.
        /// </summary>
        /// <param name="where">The where.</param>
        /// <param name="orderBy">The order by.</param>
        /// <param name="progress">The progress.</param>
        /// <param name="ct">The ct.</param>
        /// <returns></returns>
        IEnumerable<FSIdent> Ident(
            string where = "",
            string orderBy = "");

        /// <summary>
        ///     Firsts the or default.
        /// </summary>
        /// <param name="where">The where.</param>
        /// <returns></returns>
        T FirstOrDefault(string where);

        /// <summary>
        ///     Firsts the or default asynchronous.
        /// </summary>
        /// <param name="where">The where.</param>
        /// <returns></returns>
        Task<T> FirstOrDefaultAsync(string where);

        /// <summary>
        ///     Load the record by rowId
        /// </summary>
        /// <param name="rowId"></param>
        /// <returns></returns>
        T Find(string rowId);

        /// <summary>
        ///     Load the record by rowId
        /// </summary>
        /// <param name="rowId"></param>
        /// <returns></returns>
        Task<T> FindAsync(string rowId);

        /// <summary>
        ///     Creates a new instance. Instance must be saved with SaveChanges
        /// </summary>
        /// <returns></returns>
        T Create();

        /// <summary>
        ///     Returns the count
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        int Count(string where = "");

        /// <summary>
        ///     Returns the count
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        Task<int> CountAsync(string where = "");

        /// <summary>
        ///     Checks if a record for the condition exists
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        bool Exists(string where = "");

        /// <summary>
        ///     Checks if a record for the condition exists
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        Task<bool> ExistsAsync(string where = "");

        /// <summary>
        ///     Delete entity from erp database
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        void Delete(T entity);

        /// <summary>
        ///     Delete entity from erp database
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        Task DeleteAsync(T entity);

        /// <summary>
        ///     Saves the entity to the erp database.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        T Save(T entity);

        /// <summary>
        ///     Saves the entity to the erp database.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<T> SaveAsync(T entity);

        /// <summary>
        ///     Determines whether [has entity changed] [the specified entity].
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        bool HasEntityChanged(T entity);

        T ReloadEntity(T entity);

        bool IsReadOnly { get; }
    }

    public partial interface IFSRepository
    {
        IQueryable Queryable();

		IEnumerable Get(
			string where = "",
			string orderby = "",
			int? offset = null,
			int? limit = null,
			int? loadSize = -1);
	}
}