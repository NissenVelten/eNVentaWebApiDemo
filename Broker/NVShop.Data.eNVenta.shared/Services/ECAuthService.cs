using NVShop.Data.NV.Model;

namespace NVShop.Data.eNVenta.Services
{
    using NVShop.Data.FS;
    using NVShop.Data.NV.Services;

    public class ECAuthService : IECAuthService
    {
        public bool Authenticate(NVIdentity identity)
        {
            if (string.IsNullOrEmpty(identity.Name) || string.IsNullOrEmpty(identity.Password))
            {
                return false;
            }

            return FSUtil.Authenticate(identity);
        }
    }
}
