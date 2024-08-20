using Bogus;
using CSharpFunctionalExtensions;
using SOFTURE.Typesense.Abstractions;
using SOFTURE.Typesense.Playground.Examples;

namespace SOFTURE.Typesense.Playground;

public sealed class Playground(IDocumentClient documentClient)
{
    public async Task Run()
    {
        var exampleCites = new[] { "Warsaw", "Wroclaw" };
        var exampleIdentifiers = new[] { "AB", "CD" };

        var currentId = 1;
        var exampleDocuments = new Faker<ExampleDocument>()
            .StrictMode(true)
            .RuleFor(o => o.Id, _ => (currentId++).ToString())
            .RuleFor(o => o.Name, f => f.Name.FullName())
            .RuleFor(o => o.Identifier, f => f.PickRandom(exampleIdentifiers))
            .RuleFor(o => o.City, f => f.PickRandom(exampleCites))
            .Generate(50);

        foreach (var exampleDocument in exampleDocuments)
        {
            await documentClient.UpsertDocument(exampleDocument)
                .Tap(() => Console.WriteLine("Document upsert"))
                .TapError(error => Console.WriteLine($"Error creating document: {error}"));
        }


        var exampleQuery = new ExampleQuery
        {
            Name = "J"
        };

        var exampleFilters = new ExampleFilters
        {
            City = "Wroclaw",
            Identifier = "AB"
        };

        await documentClient.Search<ExampleDocument, ExampleQuery, ExampleFilters>(
                query: exampleQuery,
                filters: exampleFilters
            )
            .Tap(result =>
            {
                foreach (var document in result)
                {
                    Console.WriteLine($"Document found: {document}");
                }
            })
            .TapError(error => Console.WriteLine($"Error searching documents: {error}"));
    }
}