using ManageStoresApi.Implementations;
using ManageStoresApi.Interfaces;
using ManageStoresData.Implementations;
using ManageStoresData.Interfaces;
using System.Web.Mvc;
using Unity;
using Unity.Mvc5;

namespace ManageStoresApi
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
			var container = new UnityContainer();

            container.RegisterType<IProductData, ProductData>();
            container.RegisterType<IProductApi, ProductApi>();
            container.RegisterType<IStoreData, StoreData>();
            container.RegisterType<IStoreApi, StoreApi>();

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }
    }
}