using System;
using System.Collections.Generic;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Company.Function
{
    public class CosmosTriggerToDoList
    {
        private readonly ILogger _logger;

        public CosmosTriggerToDoList(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<CosmosTriggerToDoList>();
        }

        [Function("CosmosTriggerToDoList")]
        [CosmosDBOutput(
            "ToDoList", "ItemsCopy", Connection = "cosmosrmoreirao_DOCUMENTDB", CreateIfNotExists = true, PartitionKey = "/partitionKey")]
        public object Run([CosmosDBTrigger(
            databaseName: "ToDoList",
            containerName: "Items",
            Connection = "cosmosrmoreirao_DOCUMENTDB",
            LeaseContainerName = "leases2",
            CreateLeaseContainerIfNotExists = true)] IReadOnlyList<ToDoListItemDocument> input)
        {
            if (input != null && input.Count > 0)
            {
                _logger.LogInformation("Documents modified: " + input.Count);
                _logger.LogInformation("First document Id: " + input[0].Id);
                _logger.LogInformation("First document Description: " + input[0].Description);

                return input.Select(p => new { id = p.Id, partitionKey = p.PartitionKey, description = p.Description, newElemenet = Guid.NewGuid().ToString() });
            }

            return null;
        }
    }

}
