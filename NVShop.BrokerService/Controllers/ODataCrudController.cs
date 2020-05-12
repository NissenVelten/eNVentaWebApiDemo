namespace NVShop.BrokerService.Controllers
{
    using System;
    using System.Threading.Tasks;
    using System.Web.Http;

    using Microsoft.AspNet.OData;
    using Microsoft.AspNet.OData.Routing;

    using NVShop.Data.NV.Model;
    using NVShop.Data.NV.Repository;

    public abstract class ODataCrudController<TNVEntity> : ODataBaseController<TNVEntity>
        where TNVEntity : NVEntity
    {
        public ODataCrudController(INVCrudRepository<TNVEntity> nvRepository) : base(nvRepository)
        {
            NVRepository = nvRepository;
        }

        protected new INVCrudRepository<TNVEntity> NVRepository { get; }

        /// <summary>
        /// Creates a new entity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        [ODataRoute("")]
        public virtual async Task<IHttpActionResult> Post([FromBody] TNVEntity entity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var newEntity = await NVRepository.InsertAsync(entity);
                return Created(newEntity);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Puts the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        [HttpPut]
        [ODataRoute("({key})")]
        public virtual async Task<IHttpActionResult> Put([FromODataUri] string key, [FromBody] Delta<TNVEntity> entity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var dbEntity = await NVRepository.FindAsync(key);
            if (dbEntity == null)
            {
                return NotFound();
            }

            var orgVersion = dbEntity.RowVersion;

            entity.Put(dbEntity);

            if (dbEntity.RowVersion != orgVersion)
            {
                return Conflict();
            }

            return Updated(await NVRepository.UpdateAsync(dbEntity));
        }

        /// <summary>
        /// Allowed Partial updates to an Entity
        /// </summary>
        /// <param name="key"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPatch]
        [ODataRoute("({key})")]
        public virtual async Task<IHttpActionResult> Patch([FromODataUri] string key, [FromBody] Delta<TNVEntity> entity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var dbEntity = NVRepository.Find(key);
            if (dbEntity == null)
            {
                return NotFound();
            }

            var orgVersion = dbEntity.RowVersion;

            entity.Patch(dbEntity);

            if (dbEntity.RowVersion != orgVersion)
            {
                return Conflict();
            }

            return Updated(await NVRepository.UpdateAsync(dbEntity));
        }

        /// <summary>
        /// Deletes by the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        [ODataRoute("({key})")]
        public virtual IHttpActionResult Delete([FromODataUri] string key)
        {
            try
            {
                var entity = NVRepository.Find(key);
                if (entity == null)
                {
                    return NotFound();
                }
                
                NVRepository.Delete(entity);

                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}