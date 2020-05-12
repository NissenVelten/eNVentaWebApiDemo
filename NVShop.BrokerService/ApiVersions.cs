using Microsoft.Web.Http;

namespace NVShop.BrokerService
{
    public static class ApiVersions
    {
        public static ApiVersion V1 => ApiVersion.Parse("1.0");
    }
}