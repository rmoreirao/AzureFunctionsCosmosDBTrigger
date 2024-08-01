using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Cosmos;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace Company.Function
{
    public class HttpTriggerList
    {
        private readonly ILogger<HttpTriggerList> _logger;
        private readonly CosmosClient _cosmosClient;
        private readonly Container _container;

        public HttpTriggerList(ILogger<HttpTriggerList> logger)
        {
            _logger = logger;
            var connectionString = Environment.GetEnvironmentVariable("cosmosrmoreirao_DOCUMENTDB");
            _cosmosClient = new CosmosClient(connectionString);
            _container = _cosmosClient.GetContainer("ToDoList", "Items");
        }

        [Function("HttpTriggerList")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
        {
            var query = "SELECT * FROM c";
            var iterator = _container.GetItemQueryIterator<ToDoListItemDocument>(new QueryDefinition(query));
            var results = new List<ToDoListItemDocument>();

            while (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync();

                foreach (var item in response)
                {
                    _logger.LogInformation("Item: {0}", item.Description);
                }

                results.AddRange(response.ToList());
            }

            return new OkObjectResult(results);
        }
    }
}
