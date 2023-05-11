using ManageStores.Implementations;
using ManageStores.Interfaces;
using System.Web.Mvc;
using Unity;
using Unity.Mvc5;

namespace ManageStores
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
			var container = new UnityContainer();
            container.RegisterType<IProductSite, ProductSite>();
            // register all your components with the container here
            // it is NOT necessary to register your controllers

            // e.g. container.RegisterType<ITestService, TestService>();

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }
    }
}