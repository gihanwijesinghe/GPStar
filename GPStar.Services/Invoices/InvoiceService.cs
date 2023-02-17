using GPStar.Model;
using Microsoft.Azure.Cosmos;

namespace GPStar.Services.Invoices
{
    public class InvoiceService : IInvoiceService
    {
        private Container _container;
        public InvoiceService(
            CosmosClient cosmosDbClient,
            string databaseName,
            string containerName)
        {
            _container = cosmosDbClient.GetContainer(databaseName, containerName);
        }
        public async Task AddAsync(Invoice invoice)
        {
            await _container.CreateItemAsync(invoice, new PartitionKey(invoice.Id.ToString()));
        }
        //public async Task DeleteAsync(string id)
        //{
        //    await _container.DeleteItemAsync<Item>(id, new PartitionKey(id));
        //}
        public async Task<Invoice> GetAsync(string id)
        {
            try
            {
                var response = await _container.ReadItemAsync<Invoice>(id, new PartitionKey(id));
                return response.Resource;
            }
            catch (CosmosException) //For handling item not found and other exceptions
            {
                return null;
            }
        }
        //public async Task<IEnumerable<Item>> GetMultipleAsync(string queryString)
        //{
        //    var query = _container.GetItemQueryIterator<Item>(new QueryDefinition(queryString));
        //    var results = new List<Item>();
        //    while (query.HasMoreResults)
        //    {
        //        var response = await query.ReadNextAsync();
        //        results.AddRange(response.ToList());
        //    }
        //    return results;
        //}
        public async Task UpdateAsync(string id, Invoice item)
        {
            await _container.UpsertItemAsync(item, new PartitionKey(id));
        }
    }
}
