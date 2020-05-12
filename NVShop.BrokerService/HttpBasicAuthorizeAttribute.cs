using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using NVShop.Data.NV.Model;
using NVShop.Data.NV.Services;

using Unity;

namespace NVShop.BrokerService
{
    public class HttpBasicAuthorizeAttribute : AuthorizeAttribute
    {
        private readonly IECAuthService _authService;

        public HttpBasicAuthorizeAttribute() : this(UnityConfig.Container.Resolve<IECAuthService>())
        {
        }

        public HttpBasicAuthorizeAttribute(IECAuthService authService)
        {
            _authService = authService;
        }

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if (IsValid(actionContext.Request.Headers.Authorization))
            {
                IsAuthorized(actionContext);
            }
            else
            {
                HandleUnauthorizedRequest(actionContext);
            }
        }

        private bool IsValid(AuthenticationHeaderValue auth)
        {
            if (auth == null)
                return false;

            var identity = ParseAuthorizationHeader(auth);
            if (identity == null)
                return false;

            if (_authService.Authenticate(identity))
            {
                HttpContext.Current.Items["NVIdentity"] = identity;
                return true;
            }

            return false;
        }

        protected override void HandleUnauthorizedRequest(HttpActionContext actionContext)
        {
            actionContext.Response = new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.Unauthorized,
                ReasonPhrase = "Unauthorized",
                Headers = {
                    { "WWW-Authenticate", "Basic" }
                }
            };
        }

        /// <summary>
        /// Parses the Authorization header and creates user credentials
        /// </summary>
        /// <param name="actionContext"></param>
        protected virtual NVIdentity ParseAuthorizationHeader(AuthenticationHeaderValue auth)
        {
            if (auth == null || auth.Scheme != "Basic")
                return null;

            var authHeader = auth.Parameter;

            if (string.IsNullOrWhiteSpace(authHeader))
                return null;

            authHeader = Encoding.Default.GetString(Convert.FromBase64String(authHeader));

            // find first : as password allows for :
            var tokens = authHeader.Split(':');
            if (tokens.Length != 2)
                return null;

            string usernameToken = tokens[0];
            string passwordToken = tokens[1];

            string username = usernameToken;
            string businessUnit = null;

            var businessUnitSeparatorIdx = usernameToken.LastIndexOf("@");
            if (businessUnitSeparatorIdx != -1)
            {
                username = usernameToken.Substring(0, businessUnitSeparatorIdx);
                businessUnit = usernameToken.Substring(businessUnitSeparatorIdx + 1);
            }

            return new NVIdentity(username, passwordToken, businessUnit);
        }
    }
}