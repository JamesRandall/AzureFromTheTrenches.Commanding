
using System.IO;
using System.Threading.Tasks;
using FunctionTemplates.Commands;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace FunctionTemplates
{
    public static class SimpleHttpTrigger
    {
        [FunctionName("SimpleHttpTrigger")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequest req, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = new StreamReader(req.Body).ReadToEnd();
            EchoMessageCommand echoMessageCommand = JsonConvert.DeserializeObject<EchoMessageCommand>(requestBody);
            string result = await Infrastructure.CommandDispatcher.DispatchAsync(echoMessageCommand);
            string jsonResult = JsonConvert.SerializeObject(result);
            return new OkObjectResult(jsonResult);
        }
    }
}
