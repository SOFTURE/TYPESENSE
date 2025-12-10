using Bogus;
using CSharpFunctionalExtensions;
using SOFTURE.Typesense.Abstractions;
using SOFTURE.Typesense.Abstractions.Models;
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
        var statuses = new[] { "active", "inactive", "deleted", "pending" };
        var availableTags = new[] { "electronics", "furniture", "clothing", "books", "toys" };

        var currentId = 1;
        var exampleDocuments = new Faker<ExampleDocument>()
            .StrictMode(true)
            .RuleFor(o => o.Id, _ => (currentId++).ToString())
            .RuleFor(o => o.Name, f => f.Name.FullName())
            .RuleFor(o => o.Identifier, f => f.PickRandom(exampleIdentifiers))
            .RuleFor(o => o.City, f => f.PickRandom(exampleCites))
            .RuleFor(o => o.IsActive, f => f.Random.Bool())
            .RuleFor(o => o.VoivodeshipId, f => f.PickRandom(voivodeshipIds))
            .RuleFor(o => o.Status, f => f.PickRandom(statuses))
            .RuleFor(o => o.Price, f => f.Random.Decimal(10, 1000))
            .RuleFor(o => o.Quantity, f => f.Random.Int(0, 100))
            .RuleFor(o => o.MaxValue, f => f.Random.Double(100, 1000))
            .RuleFor(o => o.MinValue, f => f.Random.Double(0, 100))
            .RuleFor(o => o.Timestamp, f => f.Date.Between(new DateTime(2024, 1, 1), new DateTime(2025, 12, 31)).Ticks)
            .RuleFor(o => o.Tags, f => f.PickRandom(availableTags, f.Random.Int(1, 3)).ToArray())
            .RuleFor(o => o.ExcludedIds, _ => null)
            .Generate(recordsCount);
        
        await documentClient.ImportDocuments(exampleDocuments, batchSize: recordsCount + 1)
            .Tap(() => Console.WriteLine("Documents upsert"))
            .TapError(error => Console.WriteLine($"Error creating document: {error}"));

        var exampleQuery = new ExampleQuery
        {
            //Name = matchingDocument.Name[..4]
        };

        var timestampFrom = exampleDocuments.Min(d => d.Timestamp);
        var timestampTo = exampleDocuments.Max(d => d.Timestamp);

        var exampleFilters = new ExampleFilters
        {
            //City = "Wroclaw",
            //Identifier = "AB",
            //IsActive = false,
            //VoivodeshipId = 3,
            //Status = "deleted",
            //Price = 500,
            //Quantity = 50,
            //MaxValue = 500,
            //MinValue = 50,
            //Timestamp = (timestampFrom, timestampTo),
            Tags = ["electronics", "books"],
            ExcludedIds = ["1", "2", "3"],
            //Name = "John Doe"
        };
        
        var exampleSort = new ExampleSort()
            .WithName(SortDirection.Desc)
            .WithCity(SortDirection.Asc);

        Console.WriteLine($"[DEBUG] QueryBy: '{exampleQuery.QueryBy()}'");
        Console.WriteLine($"[DEBUG] Filters: '{exampleFilters.FilterBy()}'");
        Console.WriteLine($"[DEBUG] SortBy: '{exampleSort.SortBy()}'");
        
        await documentClient.Search<ExampleDocument, ExampleQuery, ExampleFilters, ExampleSort>(
                query: exampleQuery,
                filters: exampleFilters,
                sortBy: exampleSort
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