using log4net;
using ManageStores.Interfaces;
using ManageStoresModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace ManageStores.Implementations
{
    public class StoreProductSite : IStoreProductSite
    {
        private readonly HttpClient _client = Helpers.ApiHttpClient.GetApiClient();
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public async Task<Tuple<bool, string, List<StoreProductDetail>>> GetStoresProductsAsync()
        {
            try
            {
                _log.Info($"{nameof(GetStoresProductsAsync)} attempting to get store(s) and product(s) from api.");
                HttpResponseMessage response = await _client.GetAsync($"api/Store/GetStoresProducts");
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    if (!string.IsNullOrEmpty(content))
                    {
                        var storesProducts = JsonConvert.DeserializeObject<StoreProductDetailResponse>(content);
                        _log.Info($"{nameof(GetStoresProductsAsync)} successfully returned {storesProducts.Result.Count()} stores(s) and product(s) from api.");
                        return new Tuple<bool, string, List<StoreProductDetail>>(true, "successful", storesProducts.Result);
                    }
                }
                _log.Info($"{nameof(GetStoresProductsAsync)} no stores found from api.");
                return new Tuple<bool, string, List<StoreProductDetail>>(false, "no stores", new List<StoreProductDetail>());
            }
            catch (Exception ex)
            {
                _log.Error($"{nameof(GetStoresProductsAsync)} {ex.Message} {ex.StackTrace}.");
                return new Tuple<bool, string, List<StoreProductDetail>>(false, ex.Message, new List<StoreProductDetail>());
            }
        }

        public async Task<Tuple<bool, string>> AddStoreProductFileAsync(StoreUpload storeUpload)
        {
            try
            {
                _log.Info($"{nameof(AddStoreProductFileAsync)} attempting to add file {storeUpload.File.FileName}.");

                string fullFilePath = Path.Combine(ConfigurationManager.AppSettings["FileLocation"], $"{DateTime.Now.ToString("MMddyyyyHHmmssfff")}_{storeUpload.File.FileName}");

                string extension = Path.GetExtension(storeUpload.File.FileName);

                if (!Directory.Exists(ConfigurationManager.AppSettings["FileLocation"]))
                    Directory.CreateDirectory(ConfigurationManager.AppSettings["FileLocation"]);

                storeUpload.File.SaveAs(fullFilePath);

                switch (extension)
                {
                    case ".csv":
                        if (storeUpload.FileType == 1)
                        {
                            _log.Info($"{nameof(AddStoreProductFileAsync)} about to process csv file {storeUpload.File.FileName}.");
                            var storesRequest = ProcessCSVFile(fullFilePath);
                            if (storesRequest.Item1)
                                return await AddStoresProductsAsync(storesRequest.Item3);
                            else
                                return new Tuple<bool, string>(false, storesRequest.Item2);
                        }
                        else
                            return FailedUploadDueToSelection(storeUpload, fullFilePath);
                    case ".json":
                        if (storeUpload.FileType == 2)
                        {
                            _log.Info($"{nameof(AddStoreProductFileAsync)} about to process json file {storeUpload.File.FileName}.");
                            var storesRequest = ProcessJsonFile(fullFilePath);
                            return await AddStoresProductsAsync(storesRequest);
                        }
                        else
                            return FailedUploadDueToSelection(storeUpload, fullFilePath);
                    case ".xml":
                        if (storeUpload.FileType == 3)
                        {
                            _log.Info($"{nameof(AddStoreProductFileAsync)} about to process xml file {storeUpload.File.FileName}.");
                            var storesRequest = ProcessXmlFile(fullFilePath);
                            if (storesRequest.Item1)
                                return await AddStoresProductsAsync(storesRequest.Item3);
                            else
                                return new Tuple<bool, string>(false, storesRequest.Item2);
                        }
                        else
                            return FailedUploadDueToSelection(storeUpload, fullFilePath);
                    default: //unknown file type
                        File.Delete(fullFilePath);
                        _log.Info($"{nameof(AddStoreProductFileAsync)} attempting to add file {storeUpload.File.FileName} failed, unknown file format.");
                        return new Tuple<bool, string>(false, $"File {storeUpload.File.FileName} could not be imported, please make the file is either csv,json or xml.");
                }

            }
            catch (Exception ex)
            {
                _log.Error($"{nameof(AddStoreProductFileAsync)} {ex.Message} {ex.StackTrace}.");
                return new Tuple<bool, string>(false, "");
            }
        }

        private Tuple<bool, string, List<StoreProductRequest>> ProcessCSVFile(string fullFilePath)
        {
            var addStoreRequests = new List<StoreProductRequest>();
            bool success = true;
            string message = string.Empty;
            try
            {
                var header = File.ReadAllLines(fullFilePath).First();
                string[] headers = header.Split(',');

                if (headers.Length == 0)
                {
                    message = "No headers available, please use template";
                    success = false;
                    _log.Info($"{nameof(ProcessCSVFile)} {message}");
                    return new Tuple<bool, string, List<StoreProductRequest>>(success, header, addStoreRequests);
                }
                else
                {
                    if (!headers[0].ToString().Equals("BranchID"))
                    {
                        message = "No BranchID on file, please use template";
                        success = false;
                        _log.Info($"{nameof(ProcessCSVFile)} {message}");
                        return new Tuple<bool, string, List<StoreProductRequest>>(success, header, addStoreRequests);
                    }
                    if (!headers[1].ToString().Equals("ProductID"))
                    {
                        message = "No ProductID on file, please use template";
                        success = false;
                        _log.Info($"{nameof(ProcessCSVFile)} {message}");
                        return new Tuple<bool, string, List<StoreProductRequest>>(success, header, addStoreRequests);
                    }
                   
                }

                if (success)
                {
                    var lines = File.ReadAllLines(fullFilePath).Skip(1).ToList();

                    foreach (var line in lines)
                    {
                        string[] values = line.Split(',');
                        if (values.Length > 0)
                        {
                            Int32.TryParse(values[0].ToString(), out int sid);
                            Int32.TryParse(values[1].ToString(), out int pid);
                            if (sid > 0 && pid > 0)
                            {
                                AddItemsToList(addStoreRequests, sid, pid);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error($"{nameof(ProcessCSVFile)} {ex.Message} {ex.StackTrace}.");
                return new Tuple<bool, string, List<StoreProductRequest>>(false, ex.Message, addStoreRequests);
            }
            return new Tuple<bool, string, List<StoreProductRequest>>(success, message, addStoreRequests);
        }

        private static void AddItemsToList(List<StoreProductRequest> storeProductsRequests, int sid, int pid)
        {
            storeProductsRequests.Add(new StoreProductRequest
            {
                 SID = sid,
                 PID = pid,
                 Active = true,
            });
        }

        private async Task<Tuple<bool, string>> AddStoresProductsAsync(List<StoreProductRequest> storeProductsRequests)
        {
            try
            {
                var addRequests = await ValidateAddStoresRequests(storeProductsRequests);
                if (addRequests.Item1)
                {
                    if (addRequests.Item2 == null)
                        return new Tuple<bool, string>(true, "the stores and product all updated with latest values.");


                    _log.Info($"{nameof(AddStoresProductsAsync)} attempting to add stores and products using api.");

                    HttpResponseMessage response = await _client.PostAsync($"api/Store/AddStoresProducts",
                              new StringContent(JsonConvert.SerializeObject(addRequests.Item2), System.Text.Encoding.Unicode, "application/json"));

                    if (response.IsSuccessStatusCode)
                    {
                        string content = await response.Content.ReadAsStringAsync();
                        if (!string.IsNullOrEmpty(content))
                        {
                            var results = JsonConvert.DeserializeObject<AddStoresResponse>(content);
                            _log.Info($"{nameof(AddStoresProductsAsync)} {results}.");
                            return new Tuple<bool, string>(true, results.Result);
                        }
                    }
                    return new Tuple<bool, string>(false, response.ReasonPhrase);
                }
                else
                {
                    return new Tuple<bool, string>(false, "validations failed");
                }
            }
            catch (Exception ex)
            {
                _log.Error($"{nameof(AddStoresProductsAsync)} {ex.Message} {ex.StackTrace}.");
                return new Tuple<bool, string>(false, ex.Message);
            }
        }

        private async Task<Tuple<bool, List<StoreProductRequest>>> ValidateAddStoresRequests(List<StoreProductRequest> storesProductsRequests)
        {
            try
            {
                _log.Info($"{nameof(ValidateAddStoresRequests)} attempting to validate store api requests.");

                var storesProducts = await GetStoresProductsAsync();

                if (storesProducts.Item1)
                {
                    if (storesProducts.Item3?.Count()==0)//No stores and products on db.
                        return new Tuple<bool, List<StoreProductRequest>>(true, storesProductsRequests);

                    var existingStores = new List<StoreProductRequest>();

                    foreach (var storeProduct in storesProducts.Item3)
                    {
                        var request = storesProductsRequests.Where(a => a.SID == storeProduct.SID && a.PID == storeProduct.PID).FirstOrDefault();

                    }

                    if (existingStores?.Count() > 0)
                    {
                        _log.Info($"{nameof(ValidateAddStoresRequests)} found {existingStores.Count()} existing store(s) and product(s).");

                        storesProductsRequests = storesProductsRequests.Except(existingStores).ToList();

                    }

                }
                return new Tuple<bool, List<StoreProductRequest>>(true, storesProductsRequests);
            }
            catch (Exception ex)
            {
                _log.Error($"{nameof(ValidateAddStoresRequests)} {ex.Message} {ex.StackTrace}.");
                return new Tuple<bool, List<StoreProductRequest>>(false, storesProductsRequests);
            }
        }

        private static Tuple<bool, string> FailedUploadDueToSelection(StoreUpload storeUpload, string fullFilePath)
        {
            File.Delete(fullFilePath);
            _log.Info($"{nameof(FailedUploadDueToSelection)} attempting to add file {storeUpload.File.FileName} failed, selection vs format do not match.");
            return new Tuple<bool, string>(false, $"File {storeUpload.File.FileName} could not be uploaded, please the selection matches the file format.");
        }

        private List<StoreProductRequest> ProcessJsonFile(string fullFilePath)
        {
            try
            {
                var StoreProductRequests = new List<StoreProductRequest>();
                var addStoreProductRequestsJson = new List<AddStoresProductsRequestJson>();
                using (StreamReader sr = File.OpenText(fullFilePath))
                {
                    addStoreProductRequestsJson = JsonConvert.DeserializeObject<List<AddStoresProductsRequestJson>>(sr.ReadToEnd());
                }
                ConvertStoresProductsJsonToRequests(StoreProductRequests, addStoreProductRequestsJson);
                return StoreProductRequests;
            }
            catch (Exception ex)
            {
                _log.Error($"{nameof(ProcessJsonFile)} {ex.Message} {ex.StackTrace}.");
                return new List<StoreProductRequest>();
            }
        }

        private static void ConvertStoresProductsJsonToRequests(List<StoreProductRequest> StoreProductsRequests, List<AddStoresProductsRequestJson> StoreProductsRequestsJson)
        {
            foreach (var item in StoreProductsRequestsJson)
            {
                Int32.TryParse(item.SID, out int sid);
                Int32.TryParse(item.PID, out int pid);

                if (sid > 0 && pid >0)
                    AddItemsToList(StoreProductsRequests,sid,pid);
            }
        }

        private Tuple<bool, string, List<StoreProductRequest>> ProcessXmlFile(string fullFilePath)
        {
            try
            {
                var storeProductsRequests = new List<StoreProductRequest>();
                bool success = true;
                string message = string.Empty;

                XmlReader xmlReader = XmlReader.Create(fullFilePath);
                while (xmlReader.Read())
                {
                    if ((xmlReader.NodeType == XmlNodeType.Element) && (xmlReader.Name == "BranchProduct"))
                    {
                        if (xmlReader.HasAttributes)
                        {
                            Int32.TryParse(xmlReader.GetAttribute("BranchID"), out int sid);
                            Int32.TryParse(xmlReader.GetAttribute("ProductID"), out int pid);
                            if (sid > 0 && pid >0)
                                AddItemsToList(storeProductsRequests, sid, pid);
                        }
                    }
                }
                return new Tuple<bool, string, List<StoreProductRequest>>(success, message, storeProductsRequests);
            }
            catch (Exception ex)
            {
                _log.Error($"{nameof(ProcessXmlFile)} {ex.Message} {ex.StackTrace}.");
                return new Tuple<bool, string, List<StoreProductRequest>>(false, ex.Message, new List<StoreProductRequest>());
            }
        }
    }
}