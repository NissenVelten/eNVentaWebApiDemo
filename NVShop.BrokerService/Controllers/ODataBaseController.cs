namespace NVShop.BrokerService.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Http;
    using Microsoft.AspNet.OData;
    using Microsoft.AspNet.OData.Query;
    using Microsoft.AspNet.OData.Routing;
    using NVShop.Data.NV.Repository;

    [HttpBasicAuthorize]
    public abstract class ODataBaseController : ODataController
    {
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <seealso cref="Microsoft.AspNet.OData.ODataController" />
    [HttpBasicAuthorize]
    public abstract class ODataBaseController<TEntity> : ODataBaseController<TEntity, string>
    {
        public ODataBaseController(INVReadRepository<TEntity, string> nvRepository) : base(nvRepository)
        {

        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <seealso cref="Microsoft.AspNet.OData.ODataController" />
    [HttpBasicAuthorize]
    public abstract class ODataBaseController<TEntity, TKey> : ODataController
    {
        public ODataBaseController(INVReadRepository<TEntity, TKey> nvRepository)
        {
            NVRepository = nvRepository;
        }

        protected INVReadRepository<TEntity, TKey> NVRepository { get; }

        /// <summary>
        /// Gets the entities.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns></returns>
        [HttpGet]
        [ODataRoute("")]
        [EnableQuery(PageSize = 1000, 
            AllowedQueryOptions = AllowedQueryOptions.Filter | AllowedQueryOptions.OrderBy | AllowedQueryOptions.Skip | AllowedQueryOptions.Top | AllowedQueryOptions.Select | AllowedQueryOptions.Count,
            MaxExpansionDepth = 0)]
        public virtual IQueryable<TEntity> Get([FromODataUri] string query = "")
        {
            if (NVRepository is INVQueryRepository<TEntity> queryRep)
            {
                var builder = queryRep.Query();

                if (string.IsNullOrEmpty(query))
                {
                    QueryParamHelper.Apply(builder, query);
                }

                return builder.Queryable();
            }

            return NVRepository.Queryable();
        }

        /// <summary>
        /// Gets the entity.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        [HttpGet]
        [ODataRoute("({key})")]
        public virtual async Task<IHttpActionResult> GetEntity([FromODataUri] TKey key)
        {
            var result = await NVRepository.FindAsync(key);
            if (result == null)
            {
                result = await GetByKey(key);
            }

            return result != null 
                ? (IHttpActionResult)Ok(result)
                : NotFound();
        }

        protected virtual Task<TEntity> GetByKey(TKey key) => Task.FromResult(default(TEntity));
    }
}