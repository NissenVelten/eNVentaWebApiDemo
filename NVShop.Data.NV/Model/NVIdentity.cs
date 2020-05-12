using System.Security.Principal;

namespace NVShop.Data.NV.Model
{
    public class NVIdentity : IIdentity
    {
        public NVIdentity(string userName, string password, string businessUnit)
        {
            Name = userName;
            Password = password;
            BusinessUnit = businessUnit;
        }

        public string Name { get; }
        public string AuthenticationType => "NV";
        public string BusinessUnit { get; }
        public string Password { get; }

        public bool IsAuthenticated { get; set; }

        public string GetToken() => $"{BusinessUnit}:{Name}:{Password}";
    }
}
