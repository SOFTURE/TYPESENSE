using CSharpFunctionalExtensions;
using SOFTURE.Typesense.Abstractions.Models;

namespace SOFTURE.Typesense.Abstractions;

public interface IDocumentClient
{
    Task<Result<SearchItems<TDocument>>> Search<TDocument, TQuery, TFilters>(
        TQuery query,
        int page = 1,
        TFilters? filters = null,
        bool typoTolerance = true,
        int records = 10)
        where TDocument : DocumentBase
        where TQuery : QueryBase
        where TFilters : FilterBase;

    Task<Result> CreateDocument<TDocument>(TDocument document)
        where TDocument : DocumentBase;

    Task<Result> UpsertDocument<TDocument>(TDocument document)
        where TDocument : DocumentBase;
    
    Task<Result> DeleteDocument<TDocument>(TDocument document)
        where TDocument : DocumentBase;
    
    Task<Result> ImportDocuments<TDocument>(List<TDocument> documents, int batchSize = 40)
        where TDocument : DocumentBase;
}