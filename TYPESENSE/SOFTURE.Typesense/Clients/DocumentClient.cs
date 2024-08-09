using CSharpFunctionalExtensions;
using SOFTURE.Typesense.Abstractions;
using SOFTURE.Typesense.Models;
using Typesense;

namespace SOFTURE.Typesense.Clients;

public class DocumentClient(ITypesenseClient client) : IDocumentClient
{
    public async Task<Result<List<TDocument>>> Search<TDocument, TQuery>(
        TQuery query,
        bool typoTolerance = true,
        int records = 10)
        where TDocument : DocumentBase
        where TQuery : QueryBase
    {
        if (!query.CanSearch()) return new List<TDocument>();

        var searchParameters = new SearchParameters(query.Text(), query.QueryBy())
        {
            PerPage = records,
            NumberOfTypos = typoTolerance ? 2 : 0
        };

        try
        {
            var searchResult = await client.Search<TDocument>(query.Collection, searchParameters);

            return searchResult.Hits.Select(x => x.Document).ToList();
        }
        catch (Exception e)
        {
            return Result.Failure<List<TDocument>>(e.Message);
        }
    }

    public async Task<Result> CreateDocument<TDocument>(TDocument document)
        where TDocument : DocumentBase
    {
        try
        {
            await client.CreateDocument(document.Collection, document);

            return Result.Success();
        }
        catch (Exception e)
        {
            return Result.Failure(e.Message);
        }
    }

    public async Task<Result> UpsertDocument<TDocument>(TDocument document)
        where TDocument : DocumentBase
    {
        try
        {
            await client.UpdateDocument(document.Collection, document.Id, document);
        }
        catch (TypesenseApiNotFoundException e)
        {
            await client.CreateDocument(document.Collection, document);
        }

        return Result.Success();
    }
}