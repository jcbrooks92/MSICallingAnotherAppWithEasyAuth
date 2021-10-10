using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Azure.Services.AppAuthentication;

namespace Company.Function
{

    public static class MSIFunction
    {
        private static readonly HttpClient client = new HttpClient();

        [FunctionName("MSIFunction")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            var tokenProvider = new AzureServiceTokenProvider();

            var endpoint = Environment.GetEnvironmentVariable("endpoint"); // Env variable - The audience of the app registration or identifier in AAD
            var url = Environment.GetEnvironmentVariable("url"); //Env variable - URL of the web app or function app you're calling including https ie : https://www.microsoft.com

                        try
                {
                string accessToken = await tokenProvider.GetAccessTokenAsync(endpoint); //example: https://database.usgovcloudapi.net

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                    HttpResponseMessage response = await client.GetAsync(url);
                    var contents = await response.Content.ReadAsStringAsync();

                    log.LogInformation(response.ToString());
                //log.Info(contents);
                return new OkObjectResult(response.ToString());

            }
            catch (Exception e)
                {
                    log.LogInformation(e.ToString());
                //return req.CreateErrorResponse(HttpStatusCode.BadRequest, e.ToString());
                return new BadRequestObjectResult(e.ToString());
                }
           
        }
    }
}
