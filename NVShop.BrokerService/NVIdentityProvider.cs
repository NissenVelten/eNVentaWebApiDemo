using System.Web;
using NVShop.Data.NV.Model;

namespace NVShop.BrokerService
{
    public class AuthNVIdentityProvider : INVIdentityProvider
    {
        public NVIdentity Get => HttpContext.Current.Items["NVIdentity"] as NVIdentity;
    }
}