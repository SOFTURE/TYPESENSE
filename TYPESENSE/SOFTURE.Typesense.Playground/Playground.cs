using Bogus;
using CSharpFunctionalExtensions;
using SOFTURE.Typesense.Abstractions;
using SOFTURE.Typesense.Playground.Examples;

namespace SOFTURE.Typesense.Playground;

public sealed class Playground(IDocumentClient documentClient)
{
    public async Task Run()
    {
        const int recordsCount = 50;
        
        var exampleCites = new[] { "Warsaw", "Wroclaw" };
        var exampleIdentifiers = new[] { "AB", "CD" };
        var voivodeshipIds = new[] { 1, 2, 3, 4, 5 };

        var currentId = 1;
        var exampleDocuments = new Faker<ExampleDocument>()
            .StrictMode(true)
            .RuleFor(o => o.Id, _ => (currentId++).ToString())
            .RuleFor(o => o.Name, f => f.Name.FullName())
            .RuleFor(o => o.Identifier, f => f.PickRandom(exampleIdentifiers))
            .RuleFor(o => o.City, f => f.PickRandom(exampleCites))
            .RuleFor(o => o.IsActive, f => f.Random.Bool())
            .RuleFor(o => o.VoivodeshipId, f => f.PickRandom(voivodeshipIds))
            .Generate(recordsCount);
        
        await documentClient.ImportDocuments(exampleDocuments, batchSize: recordsCount)
            .Tap(() => Console.WriteLine("Documents upsert"))
            .TapError(error => Console.WriteLine($"Error creating document: {error}"));

        var randomPick = new Random().Next(0, recordsCount);
        var randomDocument = exampleDocuments[randomPick];
        
        var exampleQuery = new ExampleQuery
        {
            Name = randomDocument.Name?[..5]
        };

        var exampleFilters = new ExampleFilters
        {
            City = randomDocument.City,
            Identifier = randomDocument.Identifier,
            IsActive = randomDocument.IsActive,
            VoivodeshipId = randomDocument.VoivodeshipId
        };

        await documentClient.Search<ExampleDocument, ExampleQuery, ExampleFilters>(
                query: exampleQuery,
                filters: exampleFilters
            )
            .Tap(result =>
            {
                Console.WriteLine($"Search result: {result}");
                foreach (var document in result.Items)
                {
                    Console.WriteLine($"Document found: {document}");
                }
            })
            .TapError(error => Console.WriteLine($"Error searching documents: {error}"));
    }
}