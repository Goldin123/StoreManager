using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ManageStores.Helpers
{
    public static class SiteHelper
    {
        public static List<T> Deserialize<T>(string SerializedJSONString)
        {
            var obj = JsonConvert.DeserializeObject<List<T>>(SerializedJSONString);
            return obj;
        }
    }
}