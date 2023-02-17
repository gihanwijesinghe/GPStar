using GPStar.Services.Invoices;
using Microsoft.Extensions.Configuration;

namespace GPStar.Infrastructure
{
    public class InvoiceCosmosClient
    {
        public static async Task<InvoiceService> InitializeCosmosClientInstanceAsync(IConfigurationSection configurationSection)
        {
            var databaseName = configurationSection["DatabaseName"];
            var containerName = configurationSection["ContainerName"];
            var account = configurationSection["URL"];
            var key = configurationSection["PrimaryKey"];

            var client = new Microsoft.Azure.Cosmos.CosmosClient(account, key);
            var database = await client.CreateDatabaseIfNotExistsAsync(databaseName);

            await database.Database.CreateContainerIfNotExistsAsync(containerName, "/id");
            var cosmosDbService = new InvoiceService(client, databaseName, containerName);

            return cosmosDbService;
        }
    }
}