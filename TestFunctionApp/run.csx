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

namespace TestFunctionApp.Function
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
        
        public static async  Task<IActionResult> Run(
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
    public static class TestController
    {

        [FunctionName("test")]
        

        public static async  Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get","delete", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            if(req.Method.ToLower() == "get" )
            return await Task.FromResult( new JsonResult(new List<TestResponse>
            {
                new TestResponse {name="HttpPostTimerMs", value=ValuesController.HttpPostTimer.ToString()},
                new TestResponse {name="HttpPostCounter", value=ValuesController.HttpPostCounter.ToString()},
                new TestResponse {name="HttpPostStartTime", value=ValuesController.HttpPostStartTime.ToString()},
                new TestResponse {name="HttpPostEndTime", value=ValuesController.HttpPostEndTime.ToString()}

            }));
            else if(req.Method.ToLower() == "delete" )
            {
                ValuesController.HttpPostTimer = 0;
                ValuesController.HttpPostCounter = 0;
                ValuesController.HttpPostStartTime = DateTime.MinValue;
                ValuesController.HttpPostEndTime = DateTime.MinValue;
                return await Task.FromResult( new JsonResult(new List<TestResponse>
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
}
