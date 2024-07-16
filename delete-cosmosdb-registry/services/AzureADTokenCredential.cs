namespace delete_cosmosdb_registry;


using Microsoft.Azure.Cosmos;
using Azure.Core;
using System;
using System.Threading;
using System.Threading.Tasks;

// Primero, creamos una clase personalizada que implementa TokenCredential
public class AzureADTokenCredential : TokenCredential
{
    private readonly string _token;

    public AzureADTokenCredential(string token)
    {
        _token = token;
    }

    public override AccessToken GetToken(TokenRequestContext requestContext, CancellationToken cancellationToken)
    {
        return new AccessToken(_token, DateTimeOffset.UtcNow.AddHours(1));
    }

    public override ValueTask<AccessToken> GetTokenAsync(TokenRequestContext requestContext, CancellationToken cancellationToken)
    {
        return new ValueTask<AccessToken>(GetToken(requestContext, cancellationToken));
    }
}