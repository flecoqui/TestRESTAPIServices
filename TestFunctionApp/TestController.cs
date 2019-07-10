using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace TestFunctionApp
{
    public static class TestController
    {

        [FunctionName("test")]


        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "delete", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            if (req.Method.ToLower() == "get")
                return await Task.FromResult(new JsonResult(new List<TestResponse>
            {
                new TestResponse {name="HttpPostTimerMs", value=ValuesController.HttpPostTimer.ToString()},
                new TestResponse {name="HttpPostCounter", value=ValuesController.HttpPostCounter.ToString()},
                new TestResponse {name="HttpPostStartTime", value=ValuesController.HttpPostStartTime.ToString()},
                new TestResponse {name="HttpPostEndTime", value=ValuesController.HttpPostEndTime.ToString()}

            }));
            else if (req.Method.ToLower() == "delete")
            {
                ValuesController.HttpPostTimer = 0;
                ValuesController.HttpPostCounter = 0;
                ValuesController.HttpPostStartTime = DateTime.MinValue;
                ValuesController.HttpPostEndTime = DateTime.MinValue;
                return await Task.FromResult(new JsonResult(new List<TestResponse>
                {
                    new TestResponse {name="HttpPostTimerMs", value=ValuesController.HttpPostTimer.ToString()},
                    new TestResponse {name="HttpPostCounter", value=ValuesController.HttpPostCounter.ToString()},
                    new TestResponse {name="HttpPostStartTime", value=ValuesController.HttpPostStartTime.ToString()},
                    new TestResponse {name="HttpPostEndTime", value=ValuesController.HttpPostEndTime.ToString()}

                }));
            }
            return null;

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
