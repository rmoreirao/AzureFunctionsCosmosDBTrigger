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
    public class HttpTriggerTransformationList
    {
        private readonly ILogger<HttpTriggerTransformationList> _logger;
        private readonly CosmosClient _cosmosClient;
        private readonly Container _container;

        public HttpTriggerTransformationList(ILogger<HttpTriggerTransformationList> logger)
        {
            _logger = logger;
            var connectionString = Environment.GetEnvironmentVariable("cosmosrmoreirao_DOCUMENTDB");
            _cosmosClient = new CosmosClient(connectionString);
            _container = _cosmosClient.GetContainer("ToDoList", "Items");
        }

        [Function("HttpTriggerTransformationList")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
        {
            var query = "SELECT * FROM c";
            var iterator = _container.GetItemQueryIterator<ToDoListItemDocument>(new QueryDefinition(query));
            var results = new List<dynamic>();

            while (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync();

                foreach (var item in response)
                {
                    var transformationItem = new
                    {
                        id = item.Id,
                        partitionKey = item.PartitionKey,
                        description = item.Description,
                        // description transversed
                        descriptionTransversed = new string(item.Description.Reverse().ToArray())
                    };

                    results.Add(transformationItem);
                }
            }

            return new OkObjectResult(results);
        }
    }
}
