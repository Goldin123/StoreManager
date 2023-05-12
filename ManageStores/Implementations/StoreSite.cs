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
using System.Xml;

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
        public async Task<Tuple<bool, string>> AddStoreFileAsync(StoreUpload storeUpload)
        {
            try
            {
                _log.Info($"{nameof(AddStoreFileAsync)} attempting to add file {storeUpload.File.FileName}.");

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
                            _log.Info($"{nameof(AddStoreFileAsync)} about to process csv file {storeUpload.File.FileName}.");
                            var storesRequest = ProcessCSVFile(fullFilePath);
                            if (storesRequest.Item1)
                                return await AddStoresAsync(storesRequest.Item3);
                            else
                                return new Tuple<bool, string>(false, storesRequest.Item2);
                        }
                        else
                            return FailedUploadDueToSelection(storeUpload, fullFilePath);
                    case ".json":
                        if (storeUpload.FileType == 2)
                        {
                            _log.Info($"{nameof(AddStoreFileAsync)} about to process json file {storeUpload.File.FileName}.");
                            var storesRequest = ProcessJsonFile(fullFilePath);
                            return await AddStoresAsync(storesRequest);
                        }
                        else
                            return FailedUploadDueToSelection(storeUpload, fullFilePath);
                    case ".xml":
                        if (storeUpload.FileType == 3)
                        {
                            _log.Info($"{nameof(AddStoreFileAsync)} about to process xml file {storeUpload.File.FileName}.");
                            var storesRequest = ProcessXmlFile(fullFilePath);
                            if (storesRequest.Item1)
                                return await AddStoresAsync(storesRequest.Item3);
                            else
                                return new Tuple<bool, string>(false, storesRequest.Item2);
                        }
                        else
                            return FailedUploadDueToSelection(storeUpload, fullFilePath);
                    default: //unknown file type
                        File.Delete(fullFilePath);
                        _log.Info($"{nameof(AddStoreFileAsync)} attempting to add file {storeUpload.File.FileName} failed, unknown file format.");
                        return new Tuple<bool, string>(false, $"File {storeUpload.File.FileName} could not be imported, please make the file is either csv,json or xml.");
                }

            }
            catch (Exception ex)
            {
                _log.Error($"{nameof(AddStoreFileAsync)} {ex.Message} {ex.StackTrace}.");
                return new Tuple<bool, string>(false, "");
            }
        }
        private Tuple<bool, string, List<AddStoresRequest>> ProcessCSVFile(string fullFilePath)
        {
            var addStoreRequests = new List<AddStoresRequest>();
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
                    return new Tuple<bool, string, List<AddStoresRequest>>(success, header, addStoreRequests);
                }
                else
                {
                    if (!headers[0].ToString().Equals("ID"))
                    {
                        message = "No ID on file, please use template";
                        success = false;
                        _log.Info($"{nameof(ProcessCSVFile)} {message}");
                        return new Tuple<bool, string, List<AddStoresRequest>>(success, header, addStoreRequests);
                    }
                    if (!headers[1].ToString().Equals("Name"))
                    {
                        message = "No Name on file, please use template";
                        success = false;
                        _log.Info($"{nameof(ProcessCSVFile)} {message}");
                        return new Tuple<bool, string, List<AddStoresRequest>>(success, header, addStoreRequests);
                    }
                    if (!headers[2].ToString().Equals("TelephoneNumber"))
                    {
                        message = "No TelephoneNumber on file, please use template";
                        success = false;
                        _log.Info($"{nameof(ProcessCSVFile)} {message}");
                        return new Tuple<bool, string, List<AddStoresRequest>>(success, header, addStoreRequests);
                    }
                    if (!headers[3].ToString().Equals("OpenDate"))
                    {
                        message = "No OpenDate on file, please use template";
                        success = false;
                        _log.Info($"{nameof(ProcessCSVFile)} {message}");
                        return new Tuple<bool, string, List<AddStoresRequest>>(success, header, addStoreRequests);
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
                            Int32.TryParse(values[0].ToString(), out int id);
                            if (id > 0)
                            {
                                DateTime? openDate = null;
                                string storeName = values[1]?.ToString();
                                string telephoneNumber = values[2]?.ToString();

                                if (!string.IsNullOrEmpty(values[3]))
                                {
                                    DateTime.TryParse(values[3]?.ToString().Replace(',', '.'), out DateTime date);
                                    openDate = date;
                                }                               
                                AddItemsToList(addStoreRequests, openDate, telephoneNumber, id, storeName);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error($"{nameof(ProcessCSVFile)} {ex.Message} {ex.StackTrace}.");
                return new Tuple<bool, string, List<AddStoresRequest>>(false, ex.Message, addStoreRequests);
            }
            return new Tuple<bool, string, List<AddStoresRequest>>(success, message, addStoreRequests);
        }
        private static void AddItemsToList(List<AddStoresRequest> addStoreRequests, DateTime? openDate, string telephoneNumber, int id, string storeName)
        {
            DateTime dateTime = DateTime.MinValue;
            addStoreRequests.Add(new AddStoresRequest
            {
                ID = id,
                StoreName = storeName,
                OpenDate = openDate ?? dateTime,
                TelephoneNumber = telephoneNumber,
            });
        }
        private async Task<Tuple<bool, string>> AddStoresAsync(List<AddStoresRequest> addStoresRequests)
        {
            try
            {
                var addRequests = await ValidateAddStoresRequests(addStoresRequests);
                if (addRequests.Item1)
                {
                    if (addRequests.Item2 == null)
                        return new Tuple<bool, string>(true, "the stores all updated with latest values.");


                    _log.Info($"{nameof(AddStoresAsync)} attempting to add stores using api.");

                    HttpResponseMessage response = await _client.PostAsync($"api/Store/AddStores",
                              new StringContent(JsonConvert.SerializeObject(addRequests.Item2), System.Text.Encoding.Unicode, "application/json"));

                    if (response.IsSuccessStatusCode)
                    {
                        string content = await response.Content.ReadAsStringAsync();
                        if (!string.IsNullOrEmpty(content))
                        {
                            var results = JsonConvert.DeserializeObject<AddStoresResponse>(content);
                            _log.Info($"{nameof(AddStoresAsync)} {results}.");
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
                _log.Error($"{nameof(AddStoresAsync)} {ex.Message} {ex.StackTrace}.");
                return new Tuple<bool, string>(false, ex.Message);
            }
        }
        private async Task<Tuple<bool, List<AddStoresRequest>>> ValidateAddStoresRequests(List<AddStoresRequest> addStoresRequests)
        {
            try
            {
                _log.Info($"{nameof(ValidateAddStoresRequests)} attempting to validate store api requests.");

                var stores = await GetStoresAsync();

                if (stores.Item1)
                {
                    if (stores.Item3 == null)//No stores on db.
                        return new Tuple<bool, List<AddStoresRequest>>(true, addStoresRequests);

                    var existingStores = new List<AddStoresRequest>();

                    foreach (var product in stores.Item3)
                    {
                        var request = addStoresRequests.Where(a => a.ID == product.ID).FirstOrDefault();
                       
                    }

                    if (existingStores?.Count() > 0)
                    {
                        _log.Info($"{nameof(ValidateAddStoresRequests)} found {existingStores.Count()} existing products.");

                        addStoresRequests = addStoresRequests.Except(existingStores).ToList();

                    }

                }
                return new Tuple<bool, List<AddStoresRequest>>(true, addStoresRequests);
            }
            catch (Exception ex)
            {
                _log.Error($"{nameof(ValidateAddStoresRequests)} {ex.Message} {ex.StackTrace}.");
                return new Tuple<bool, List<AddStoresRequest>>(false, addStoresRequests);
            }
        }
        private static Tuple<bool, string> FailedUploadDueToSelection(StoreUpload storeUpload, string fullFilePath)
        {
            File.Delete(fullFilePath);
            _log.Info($"{nameof(FailedUploadDueToSelection)} attempting to add file {storeUpload.File.FileName} failed, selection vs format do not match.");
            return new Tuple<bool, string>(false, $"File {storeUpload.File.FileName} could not be uploaded, please the selection matches the file format.");
        }
        private List<AddStoresRequest> ProcessJsonFile(string fullFilePath)
        {
            try
            {
                var addStoreRequests = new List<AddStoresRequest>();
                var addStoreRequestsJson = new List<AddStoresRequestJson>();
                using (StreamReader sr = File.OpenText(fullFilePath))
                {
                    addStoreRequestsJson = JsonConvert.DeserializeObject<List<AddStoresRequestJson>>(sr.ReadToEnd());
                }
                ConvertProductJsonToAddRequests(addStoreRequests, addStoreRequestsJson);
                return addStoreRequests;
            }
            catch (Exception ex)
            {
                _log.Error($"{nameof(ProcessJsonFile)} {ex.Message} {ex.StackTrace}.");
                return new List<AddStoresRequest>();
            }
        }
        private Tuple<bool, string, List<AddStoresRequest>> ProcessXmlFile(string fullFilePath)
        {
            try
            {
                var addStoreRequests = new List<AddStoresRequest>();
                bool success = true;
                string message = string.Empty;

                XmlReader xmlReader = XmlReader.Create(fullFilePath);
                while (xmlReader.Read())
                {
                    if ((xmlReader.NodeType == XmlNodeType.Element) && (xmlReader.Name == "Store"))
                    {
                        if (xmlReader.HasAttributes)
                        {
                            Int32.TryParse(xmlReader.GetAttribute("ID"), out int id);
                            if (id > 0)
                            {
                                DateTime? openDate = null;
                                string storeName = xmlReader.GetAttribute("Name");
                                string telephoneNumber = xmlReader.GetAttribute("TelephoneNumber");

                                if (!string.IsNullOrEmpty(xmlReader.GetAttribute("OpenDate")))
                                {
                                    DateTime.TryParse(xmlReader.GetAttribute("OpenDate").ToString().Replace(',', '.'), out DateTime date);
                                    openDate = date;
                                }

                                AddItemsToList(addStoreRequests, openDate, telephoneNumber, id, storeName);

                            }
                        }
                    }
                }
                return new Tuple<bool, string, List<AddStoresRequest>>(success, message, addStoreRequests);
            }
            catch (Exception ex)
            {
                _log.Error($"{nameof(ProcessXmlFile)} {ex.Message} {ex.StackTrace}.");
                return new Tuple<bool, string, List<AddStoresRequest>>(false, ex.Message, new List<AddStoresRequest>());
            }
        }
        private static void ConvertProductJsonToAddRequests(List<AddStoresRequest> addStoreRequests, List<AddStoresRequestJson> addStoreRequestsJson)
        {
            foreach (var item in addStoreRequestsJson)
            {
                Int32.TryParse(item.ID, out int id);

                if (id > 0)
                {
                    DateTime? openDate = null;
                    string storeName = item.StoreName;
                    string telephoneNumber = item.TelephoneNumber;
                    if (!string.IsNullOrEmpty(item.OpenDate))
                    {
                        DateTime.TryParse(item.OpenDate.ToString().Replace(',', '.'), out DateTime date);
                        openDate = date;
                    }
                    AddItemsToList(addStoreRequests, openDate, telephoneNumber, id, storeName);
                }
            }
        }

    }
}