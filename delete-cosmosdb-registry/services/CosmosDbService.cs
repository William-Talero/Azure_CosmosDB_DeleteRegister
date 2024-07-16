using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace delete_cosmosdb_registry;

public class CosmosDbService : ICosmosDbService
{
    private CosmosClient cosmosClient;
    private readonly IAuthService authService;
    private readonly string databaseId;
    private readonly string containerId;
    private readonly string cosmosEndpoint;
    private Task initializationTask;

    public CosmosDbService(IConfiguration configuration, IAuthService authService)
    {
        this.authService = authService;
        cosmosEndpoint = configuration["cosmosEndpoint"];
        databaseId = configuration["databaseId"];
        containerId = configuration["containerId"];

        initializationTask = InitializeAsync();
    }

    private async Task InitializeAsync()
    {
        string accessToken = await authService.GetAccessTokenAsync();
        Console.WriteLine(accessToken);
        var tokenCredential = new AzureADTokenCredential(accessToken);
        cosmosClient = new CosmosClient(cosmosEndpoint, tokenCredential);

        Console.WriteLine(cosmosEndpoint);
    }

    private async Task EnsureInitializedAsync()
    {
        await initializationTask;
    }

    public async Task<bool> CheckUserPermissionsAsync(string accessToken)
    {
        await EnsureInitializedAsync();
        var container = cosmosClient.GetContainer(databaseId, containerId);

        try
        {
            await container.ReadItemAsync<dynamic>("nonexistent-id", new PartitionKey("nonexistent-partition-key"));
            return true;
        }
        catch (CosmosException ex)
        {
            Console.WriteLine(ex.Message);
            if (ex.StatusCode == System.Net.HttpStatusCode.Forbidden || ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return false;
            }
            return true;
        }
    }

    public async Task<bool> ItemExistsAsync(string itemId, string partitionKey)
    {
        await EnsureInitializedAsync();
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
        await EnsureInitializedAsync();
        var container = cosmosClient.GetContainer(databaseId, containerId);
        await container.DeleteItemAsync<dynamic>(itemId, new PartitionKey(partitionKey));
    }
}