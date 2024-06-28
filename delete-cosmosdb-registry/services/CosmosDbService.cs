using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;

namespace delete_cosmosdb_registry;

public class CosmosDbService : ICosmosDbService
{
    private readonly CosmosClient cosmosClient;
    private readonly string databaseId;
    private readonly string containerId;

    public CosmosDbService(IConfiguration configuration)
    {
        var cosmosEndpoint = configuration["cosmosEndpoint"];
        var cosmosKey = configuration["cosmosKey"];
        databaseId = configuration["databaseId"];
        containerId = configuration["containerId"];
        cosmosClient = new CosmosClient(cosmosEndpoint, cosmosKey);
    }

    public async Task<bool> CheckUserPermissionsAsync(string accessToken)
    {
        var container = cosmosClient.GetContainer(databaseId, containerId);
        try
        {
            await container.ReadItemAsync<dynamic>("nonexistent-id", new PartitionKey("nonexistent-partition-key"));
            return true;
        }
        catch (CosmosException ex)
        {
            if (ex.StatusCode == System.Net.HttpStatusCode.Forbidden || ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return false;
            }
            return true;
        }
    }

    public async Task<bool> ItemExistsAsync(string itemId, string partitionKey)
    {
        var container = cosmosClient.GetContainer(databaseId, containerId);
        try
        {
            var response = await container.ReadItemAsync<dynamic>(itemId, new PartitionKey(partitionKey));
            return response != null;
        }
        catch (CosmosException ex)
        {
            if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return false;
            }
            throw;
        }
    }

    public async Task DeleteItemAsync(string itemId, string partitionKey)
    {
        var container = cosmosClient.GetContainer(databaseId, containerId);
        await container.DeleteItemAsync<dynamic>(itemId, new PartitionKey(partitionKey));
    }
}