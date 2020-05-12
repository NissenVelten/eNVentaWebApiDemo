namespace NVShop.Core.Infrastructure.DependencyManagement
{
    using Unity;

    public interface IDependencyRegistrar
    {
        #region Properties

        int Order { get; }

        #endregion

        void Register(IUnityContainer container);
    }
}