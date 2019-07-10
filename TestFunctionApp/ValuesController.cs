using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace TestFunctionApp
{
    public class HttpPostTimer : IDisposable
    {
        DateTime start;
        public HttpPostTimer()
        {
            start = DateTime.Now;
            if (ValuesController.HttpPostStartTime == DateTime.MinValue)
                ValuesController.HttpPostStartTime = start;
        }
        public void Dispose()
        {
            ValuesController.HttpPostEndTime = DateTime.Now;
            TimeSpan ts = ValuesController.HttpPostEndTime - start;
            ValuesController.HttpPostCounter++;
            ValuesController.HttpPostTimer += ts.TotalMilliseconds;
        }

    }
    public class TestResponse
    {
        public string name { get; set; }
        public string value { get; set; }

    };
    public static class ValuesController
    {
        public static double HttpPostCounter;
        public static double HttpPostTimer;
        public static DateTime HttpPostStartTime = DateTime.MinValue;
        public static DateTime HttpPostEndTime = DateTime.MinValue;
        [FunctionName("values")]

        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string value = new StreamReader(req.Body).ReadToEnd();
            using (HttpPostTimer hpt = new HttpPostTimer())
            {
                TestResponse t = new TestResponse();
                t.name = "testResponse";
                t.value = value;
                return await Task.FromResult(new JsonResult(t));
            }
        }
    }
    /*
    public static class Function1
    {
        [FunctionName("Function1")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            return name != null
                ? (ActionResult)new OkObjectResult($"Hello, {name}")
                : new BadRequestObjectResult("Please pass a name on the query string or in the request body");
        }
    }*/
}
