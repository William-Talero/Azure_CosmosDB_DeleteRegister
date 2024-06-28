namespace delete_cosmosdb_registry;

public interface ICosmosDbService
{
    Task<bool> CheckUserPermissionsAsync(string accessToken);
    Task<bool> ItemExistsAsync(string itemId, string partitionKey);
    Task DeleteItemAsync(string itemId, string partitionKey);
}