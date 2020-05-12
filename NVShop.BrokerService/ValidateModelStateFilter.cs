
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace NVShop.BrokerService
{
    public class ValidateModelStateFilter : ActionFilterAttribute
    {
        /// <summary>
        /// Tritt vor dem Aufrufen der Aktionsmethode auf.
        /// </summary>
        /// <param name="actionContext">Der Aktionskontext.</param>
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (!actionContext.ModelState.IsValid)
            {
                actionContext.Response = actionContext.Request.CreateErrorResponse(HttpStatusCode.BadRequest, actionContext.ModelState);
            }
        }
    }
}