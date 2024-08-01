using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Cosmos;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Company.Function
{
    public class HttpTriggerCreateNew
    {
        private readonly ILogger<HttpTriggerCreateNew> _logger;
        private readonly CosmosClient _cosmosClient;
        private readonly Container _container;

        public HttpTriggerCreateNew(ILogger<HttpTriggerCreateNew> logger)
        {
            _logger = logger;
            var connectionString = Environment.GetEnvironmentVariable("cosmosrmoreirao_DOCUMENTDB");
            _cosmosClient = new CosmosClient(connectionString);
            var database = _cosmosClient.GetDatabase("ToDoList");
            _container = database.CreateContainerIfNotExistsAsync("NewItems", "/id").GetAwaiter().GetResult();
        }

         [Function("HttpTriggerCreateNew")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            if (data == null || data.id == null)
            {
                return new BadRequestObjectResult("Invalid item data.");
            }

            try
            {
                JObject item = JObject.FromObject(data);
                ItemResponse<JObject> response = await _container.CreateItemAsync(item, new PartitionKey(item["id"].ToString()));
                return new OkObjectResult(response.Resource);
            }
            catch (CosmosException ex)
            {
                _logger.LogError($"Error creating item: {ex.Message}");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}