using System;
using System.Collections.Generic;
using System.Net.Http;

namespace PCE_Web.Classes
{
    public class GetProductsFromApi
    {
        public static List<Item> GetProducts(string productName)
        {
            var clientHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
            };
            var client = new HttpClient(clientHandler);
            try
            {
                var resultList = new List<Item>();
                var getDataTask = client.GetAsync("https://localhost:44320/api/Products/" + productName).ContinueWith(
                    response =>
                    {
                        var result = response.Result;
                        if (result.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            var readResult = result.Content.ReadAsAsync<List<Item>>();
                            readResult.Wait();
                            resultList = readResult.Result;
                        }
                    });
                getDataTask.Wait();
                return resultList;
            }
            catch (Exception)
            {
                Console.WriteLine();
                throw;
            }
        }
    }
}
