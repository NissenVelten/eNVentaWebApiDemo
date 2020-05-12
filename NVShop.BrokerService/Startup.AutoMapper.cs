using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Unity;
using Unity.Lifetime;

namespace NVShop.BrokerService
{
    using AutoMapper;

    using Owin;

    using System;

    public partial class Startup
    {
        public static void ConfigureAutoMapper(IAppBuilder app)
        {
            var typeFinder = UnityConfig.Container.Resolve<ITypeFinder>();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.ConstructServicesUsing(type => UnityConfig.Container.Resolve(type));

                foreach (var profile in GetProfiles(typeFinder))
                {
                    cfg.AddProfile(profile);
                }
            });

            var mapper = config.CreateMapper();

            UnityConfig.Container.RegisterInstance(mapper);
        }

        private static IEnumerable<Profile> GetProfiles(ITypeFinder typeFinder)
        {
            var profiles = typeFinder.FindClassesOfType<Profile>()
                .Where(t => typeof(Profile).IsAssignableFrom(t))
                .Select(Activator.CreateInstance)
                .Cast<Profile>();

            return profiles;
        }
    }
}