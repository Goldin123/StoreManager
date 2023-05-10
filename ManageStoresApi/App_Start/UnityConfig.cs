using ManageStoresApi.Implementations;
using ManageStoresApi.Interfaces;
using ManageStoresData.Implementations;
using ManageStoresData.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Unity;

namespace ManageStoresApi.App_Start
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
        }
    }
}