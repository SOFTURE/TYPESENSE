using CSharpFunctionalExtensions;
using SOFTURE.Typesense.Abstractions;
using SOFTURE.Typesense.Models;
using Typesense;

namespace SOFTURE.Typesense.Clients;

public class DocumentClient(ITypesenseClient client) : IDocumentClient
{
    public async Task<Result<List<TDocument>>> Search<TDocument, TQuery, TFilters>(
        TQuery query,
        TFilters? filters = null,
        bool typoTolerance = true,
        int records = 10)
        where TDocument : DocumentBase
        where TQuery : QueryBase
        where TFilters : FilterBase
    {
        var text = query.Text();
        var queryBy = query.QueryBy();
        var filterBy = filters?.FilterBy();

        var searchParameters = new SearchParameters(text, queryBy)
        {
            PerPage = records,
            FilterBy = filterBy,
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
        catch (TypesenseApiNotFoundException)
        {
            await client.CreateDocument(document.Collection, document);
        }

        return Result.Success();
    }
}