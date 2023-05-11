using ExcelDataReader;
using log4net;
using ManageStores.Helpers;
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
using System.Runtime.Remoting.Messaging;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using System.Xml.Serialization;

namespace ManageStores.Implementations
{
    public class ProductSite : IProductSite
    {
        private readonly HttpClient _client = Helpers.ApiHttpClient.GetApiClient();
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public async Task<Tuple<bool, string, List<ProductDetail>>> GetProductsAsync()
        {
            try
            {
                _log.Info($"{nameof(GetProductsAsync)} attempting to get products from api.");
                HttpResponseMessage response = await _client.GetAsync($"api/Product/GetProducts");
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    if (!string.IsNullOrEmpty(content))
                    {
                        var products = JsonConvert.DeserializeObject<ProductDetailResponse>(content);
                        _log.Info($"{nameof(GetProductsAsync)} successfully returned {products.Result.Count()} product(s) from api.");
                        return new Tuple<bool, string, List<ProductDetail>>(true, "successful", products.Result);
                    }
                }
                _log.Info($"{nameof(GetProductsAsync)} no products found from api.");
                return new Tuple<bool, string, List<ProductDetail>>(false, "no products", new List<ProductDetail>());
            }
            catch (Exception ex)
            {
                _log.Error($"{nameof(GetProductsAsync)} {ex.Message} {ex.StackTrace}.");
                return new Tuple<bool, string, List<ProductDetail>>(false, ex.Message, new List<ProductDetail>());
            }
        }
        public async Task<Tuple<bool, string>> AddProductFileAsync(ProductUpload productUpload)
        {
            try
            {
                //Check if products exits then update 


                _log.Info($"{nameof(AddProductFileAsync)} attempting to add file {productUpload.File.FileName}.");

                string fullFilePath = Path.Combine(ConfigurationManager.AppSettings["FileLocation"], $"{DateTime.Now.ToString("MMddyyyyHHmmssfff")}_{productUpload.File.FileName}");
                
                string extension = Path.GetExtension(productUpload.File.FileName);
                
                if (!Directory.Exists(ConfigurationManager.AppSettings["FileLocation"]))
                    Directory.CreateDirectory(ConfigurationManager.AppSettings["FileLocation"]);
                    
                productUpload.File.SaveAs(fullFilePath);

                switch (extension)
                {
                    case ".csv":
                        if (productUpload.FileType == 1)
                        {
                            _log.Info($"{nameof(AddProductFileAsync)} about to process csv file {productUpload.File.FileName}.");
                            var productRequest = ProcessCSVFile(fullFilePath);
                            return await AddProductsAsync(productRequest);
                        }
                        else
                        return FailedUploadDueToSelection(productUpload, fullFilePath);
                    case ".json":
                        if (productUpload.FileType == 2)
                        {
                            _log.Info($"{nameof(AddProductFileAsync)} about to process json file {productUpload.File.FileName}.");
                            var productRequest = ProcessJsonFile(fullFilePath);
                            return await AddProductsAsync(productRequest);
                        }
                        else
                            return FailedUploadDueToSelection(productUpload, fullFilePath);
                    case ".xml":
                        if (productUpload.FileType == 3)
                        {
                            _log.Info($"{nameof(AddProductFileAsync)} about to process xml file {productUpload.File.FileName}.");
                            var productRequest = ProcessXmlFile(fullFilePath);
                            return await AddProductsAsync(productRequest);
                        }
                        else
                            return FailedUploadDueToSelection(productUpload, fullFilePath);
                    default: //unknown file type
                        File.Delete(fullFilePath);
                        _log.Info($"{nameof(AddProductFileAsync)} attempting to add file {productUpload.File.FileName} failed, unknown file format.");
                        return new Tuple<bool, string>(false, $"File {productUpload.File.FileName} could not be imported, please make the file is either csv,json or xml.");
                }

            }
            catch (Exception ex)
            {
                _log.Error($"{nameof(AddProductFileAsync)} {ex.Message} {ex.StackTrace}.");
                return new Tuple<bool, string>(false, "");
            }
        }
        private static Tuple<bool, string> FailedUploadDueToSelection(ProductUpload productUpload, string fullFilePath)
        {
            File.Delete(fullFilePath);
            _log.Info($"{nameof(AddProductFileAsync)} attempting to add file {productUpload.File.FileName} failed, selection vs format do not match.");
            return new Tuple<bool, string>(false, $"File {productUpload.File.FileName} could not be uploaded, please the selection matches the file format.");
        }
        private List<AddProductRequest> ProcessCSVFile(string fullFilePath)
        {
            var addProductRequests = new List<AddProductRequest>();
            try
            {
                var lines = File.ReadAllLines(fullFilePath).Skip(1).ToList();

                foreach (var line in lines)
                {
                    string[] values = line.Split(',');
                    if (values.Length > 0)
                    {
                        decimal? amount = null;
                        bool? weightedItem = null;

                        Int32.TryParse(values[0].ToString(), out int id);
                        string productName = values[1]?.ToString();

                        if (!string.IsNullOrEmpty(values[3]))
                        {
                            decimal.TryParse(values[3]?.ToString().Replace(',', '.'), out decimal amnt);
                            amount = amnt;
                        }

                        if (!string.IsNullOrEmpty(values[2]))
                            weightedItem = values[2].ToLower().Equals("y") ? true : false;

                        AddItemsToList(addProductRequests, amount, weightedItem, id, productName);
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error($"{nameof(ProcessExcelFile)} {ex.Message} {ex.StackTrace}.");
            }
            return addProductRequests;
        }
        private static void AddItemsToList(List<AddProductRequest> addProductRequests, decimal? amount, bool? weightedItem, int id, string productName)
        {
            addProductRequests.Add(new AddProductRequest
            {
                ID = id,
                ProductName = productName,
                WeightedItem = weightedItem,
                SuggestedSellingPrice = amount,
            });
        }
        private List<AddProductRequest> ProcessExcelFile(string fullFilePath)
        {
            var addProductRequests = new List<AddProductRequest>();
            try
            {
                using (var stream = new FileStream(fullFilePath, FileMode.Open, FileAccess.Read))
                {
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        int i = 0;
                        do
                        {
                            while (reader.Read())                             {
                                if (i > 0) //skip header
                                {
                                    Int32.TryParse(reader.GetValue(0).ToString(), out int id);
                                    string productName = reader.GetValue(1)?.ToString();
                                    string weightedItem = reader.GetValue(2)?.ToString();
                                    decimal.TryParse(reader.GetValue(3).ToString().Replace(',', '.'), out decimal amount);
                                    addProductRequests.Add(new AddProductRequest
                                    {
                                        ID = id,
                                        ProductName = productName,
                                        WeightedItem = weightedItem.ToLower().Equals("y") ? true : false,
                                        SuggestedSellingPrice = amount,
                                    });
                                }
                                i++;
                            }
                        } while (reader.NextResult());
                    }
                }
                return addProductRequests;
            }
            catch (Exception ex)
            {
                _log.Error($"{nameof(ProcessExcelFile)} {ex.Message} {ex.StackTrace}.");
                return addProductRequests;
            }

        }
        private List<AddProductRequest> ProcessJsonFile(string fullFilePath)
        {
            try
            {
                var addProductRequests = new List<AddProductRequest>();
                var addProductRequestsJson = new List<AddProductRequestJson>();
                using (StreamReader sr = File.OpenText(fullFilePath))
                {
                    addProductRequestsJson = JsonConvert.DeserializeObject<List<AddProductRequestJson>>(sr.ReadToEnd());
                }
                ConvertProductJsonToAddRequests(addProductRequests, addProductRequestsJson);
                return addProductRequests;
            }
            catch (Exception ex)
            {
                _log.Error($"{nameof(ProcessExcelFile)} {ex.Message} {ex.StackTrace}.");
                return new List<AddProductRequest>();
            }
        }
        private List<AddProductRequest> ProcessXmlFile(string fullFilePath)
        {
            try
            {
                var addProductRequests = new List<AddProductRequest>();
                XmlReader xmlReader = XmlReader.Create(fullFilePath);
                while (xmlReader.Read())
                {
                    if ((xmlReader.NodeType == XmlNodeType.Element) && (xmlReader.Name == "Product"))
                    {
                        if (xmlReader.HasAttributes)
                        {

                            decimal? amount = null;
                            bool? weightedItem = null;

                            Int32.TryParse(xmlReader.GetAttribute("ID"), out int id);
                            string productName = xmlReader.GetAttribute("Name");

                            if (!string.IsNullOrEmpty(xmlReader.GetAttribute("SuggestedSellingPrice")))
                            {
                                decimal.TryParse(xmlReader.GetAttribute("SuggestedSellingPrice").ToString().Replace(',', '.'), out decimal amnt);
                                amount = amnt;
                            }

                            if (!string.IsNullOrEmpty(xmlReader.GetAttribute("WeightedItem")))
                                weightedItem = xmlReader.GetAttribute("WeightedItem").ToLower().Equals("y") ? true : false;

                            AddItemsToList(addProductRequests, amount, weightedItem, id, productName);

                        }
                    }
                }
                return addProductRequests;
            }
            catch (Exception ex)
            {
                _log.Error($"{nameof(ProcessExcelFile)} {ex.Message} {ex.StackTrace}.");
                return new List<AddProductRequest>();
            }
        }
        private static void ConvertProductJsonToAddRequests(List<AddProductRequest> addProductRequests, List<AddProductRequestJson> addProductRequestsJson)
        {
            foreach (var item in addProductRequestsJson)
            {
                decimal? amount = null;
                bool? weightedItem = null;
                if (!string.IsNullOrEmpty(item.SuggestedSellingPrice))
                {
                    decimal.TryParse(item.SuggestedSellingPrice.ToString().Replace(',', '.'), out decimal amnt);
                    amount = amnt;
                }
               
                if (!string.IsNullOrEmpty(item.WeightedItem)) 
                    weightedItem = item.WeightedItem.ToLower().Equals("y") ? true : false;

                AddItemsToList(addProductRequests, amount, weightedItem, int.Parse(item.ID), item.ProductName);

            }
        }
        private async Task<Tuple<bool,string>> AddProductsAsync(List<AddProductRequest> addProductRequests) 
        {
            try
            {
                _log.Info($"{nameof(AddProductsAsync)} attempting to add products using api.");
               
                HttpResponseMessage response = await _client.PostAsync($"api/Product/AddProducts",
                          new StringContent(JsonConvert.SerializeObject(addProductRequests), System.Text.Encoding.Unicode, "application/json"));
               
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    if (!string.IsNullOrEmpty(content))
                    {
                        var results = JsonConvert.DeserializeObject<AddProductResponse>(content);
                        _log.Info($"{nameof(GetProductsAsync)} {results}.");
                        return new Tuple<bool, string>(true, results.Result);
                    }
                }
                return new Tuple<bool, string>(false,response.ReasonPhrase);
            }
            catch (Exception ex)
            {
                _log.Error($"{nameof(AddProductsAsync)} {ex.Message} {ex.StackTrace}.");
                return new Tuple<bool, string>(false, ex.Message);
            }
        }
    }
}