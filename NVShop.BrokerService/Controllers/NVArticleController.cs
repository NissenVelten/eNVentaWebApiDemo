namespace NVShop.BrokerService.Controllers
{
    using System.Threading.Tasks;
    using Microsoft.AspNet.OData.Routing;
    using Microsoft.Web.Http;
    using NVShop.Data.NV;
    using NVShop.Data.NV.Model;

    [ApiVersion("1.0")]
	[ODataRoutePrefix("NVArticle")]
    public class NVArticleController : ODataBaseController<NVArticle>
    {
        private readonly INVContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="NVArticleController"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public NVArticleController(INVContext context) : base(context.Article) {
            _context = context;
        }

        /// <summary>
        /// Gets the by key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        protected override async Task<NVArticle> GetByKey(string key)
        {
            var article = await base.GetByKey(key);

            if (article == null)
            {
                article = await _context.Article.Query()
                    .ByArticleId(key)
                    .FirstOrDefaultAsync();
            }

            return article;
        }
    }
}