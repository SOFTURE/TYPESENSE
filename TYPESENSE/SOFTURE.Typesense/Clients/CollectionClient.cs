using CSharpFunctionalExtensions;
using SOFTURE.Typesense.Abstractions;
using SOFTURE.Typesense.Interfaces;
using SOFTURE.Typesense.ValueObjects;
using Typesense;

namespace SOFTURE.Typesense.Clients;

public class CollectionClient(ITypesenseClient client) : ICollectionClient
{
    public async Task<Result<CreationState>> CreateCollection(CollectionConfiguration configuration)
    {
        try
        {
            var collection = await client.RetrieveCollection(configuration.Collection);
            
            var iSchemaModify = configuration.ISchemaModified(collection.Fields);
            if (!iSchemaModify) return CreationState.NotModified;
            
            await client.DeleteCollection(configuration.Collection);
            await client.CreateCollection(configuration.GetSchema());

            return CreationState.Updated;
        }
        catch (TypesenseApiNotFoundException)
        {
            try
            {
                await client.CreateCollection(configuration.GetSchema());
            }
            catch (Exception e)
            {
                return Result.Failure<CreationState>(e.Message);
            }
   
            return CreationState.Created;
        }
        catch (Exception e)
        {
            return Result.Failure<CreationState>(e.Message);
        }
    }

    public async Task<Result> DeleteCollection(Collection collection)
    {
        try
        {
            await client.DeleteCollection(collection);

            return Result.Success();
        }
        catch (Exception e)
        {
            return Result.Failure(e.Message);
        }
    }
}