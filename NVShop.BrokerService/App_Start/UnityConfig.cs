using NVShop.Core.Configuration;
using NVShop.Core.Infrastructure;
using NVShop.Core.Infrastructure.DependencyManagement;
using System;
using System.Linq;
using Unity;

namespace NVShop.BrokerService
{
    /// <summary>
    /// Specifies the Unity configuration for the main container.
    /// </summary>
    public static class UnityConfig
    {
        #region Unity Container
        private static Lazy<IUnityContainer> container =
          new Lazy<IUnityContainer>(() =>
          {
              var container = new UnityContainer();
              RegisterTypes(container);
              return container;
          });

        /// <summary>
        /// Configured Unity Container.
        /// </summary>
        public static IUnityContainer Container => container.Value;
        #endregion

        /// <summary>
        /// Registers the type mappings with the Unity container.
        /// </summary>
        /// <param name="container">The unity container to configure.</param>
        /// <remarks>
        /// There is no need to register concrete types such as controllers or
        /// API controllers (unless you want to change the defaults), as Unity
        /// allows resolving a concrete type even if it was not previously
        /// registered.
        /// </remarks>
        public static void RegisterTypes(IUnityContainer container)
        {
            var typeFinder = new WebAppTypeFinder();

            container.RegisterInstance<ITypeFinder>(typeFinder);

            // Exclude FS-Dependencies if Broker-Path is not set
            if (!BrokerSettings.IsValid())
            {
                typeFinder.AssemblySkipLoadingPattern += "|NVShop.Data.FS|NVShop.Data.eNVenta";
            }
            
            var types = typeFinder.FindClassesOfType<IDependencyRegistrar>();

            var instances = types.Select(Activator.CreateInstance)
                .Cast<IDependencyRegistrar>()
                .OrderBy(t => t.Order)
                .ToList();

            foreach (var instance in instances)
            {
                instance.Register(container);
            }
        }
    }
}