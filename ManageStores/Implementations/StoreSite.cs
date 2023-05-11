using log4net;
using ManageStores.Interfaces;
using ManageStoresModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;

namespace ManageStores.Implementations
{
    public class StoreSite : IStoreSite
    {
        private readonly HttpClient _client = Helpers.ApiHttpClient.GetApiClient();
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public async Task<Tuple<bool, string, List<StoreDetail>>> GetStoresAsync()
        {
            try
            {
                _log.Info($"{nameof(GetStoresAsync)} attempting to get stores from api.");
                HttpResponseMessage response = await _client.GetAsync($"api/Store/GetStores");
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    if (!string.IsNullOrEmpty(content))
                    {
                        var products = JsonConvert.DeserializeObject<StoreDetailResponse>(content);
                        _log.Info($"{nameof(GetStoresAsync)} successfully returned {products.Result.Count()} stores(s) from api.");
                        return new Tuple<bool, string, List<StoreDetail>>(true, "successful", products.Result);
                    }
                }
                _log.Info($"{nameof(GetStoresAsync)} no stores found from api.");
                return new Tuple<bool, string, List<StoreDetail>>(false, "no stores", new List<StoreDetail>());
            }
            catch (Exception ex)
            {
                _log.Error($"{nameof(GetStoresAsync)} {ex.Message} {ex.StackTrace}.");
                return new Tuple<bool, string, List<StoreDetail>>(false, ex.Message, new List<StoreDetail>());
            }
        }
    }
}