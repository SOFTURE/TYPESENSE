using CSharpFunctionalExtensions;
using SOFTURE.Typesense.Models;

namespace SOFTURE.Typesense.Abstractions;

public interface IDocumentClient
{
    Task<Result<List<TDocument>>> Search<TDocument, TQuery>(
        TQuery query,
        bool typoTolerance = true,
        int records = 10)
        where TDocument : DocumentBase
        where TQuery : QueryBase;

    Task<Result> CreateDocument<TDocument>(TDocument document)
        where TDocument : DocumentBase;

    Task<Result> UpsertDocument<TDocument>(TDocument document)
        where TDocument : DocumentBase;
}