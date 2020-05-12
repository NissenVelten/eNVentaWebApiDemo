using NVShop.Data.NV.Model;

namespace NVShop.Data.NV.Services
{
    public interface IECAuthService
    {
        bool Authenticate(NVIdentity identity);
    }
}
