using Bogus;
using CSharpFunctionalExtensions;
using SOFTURE.Typesense.Abstractions;
using SOFTURE.Typesense.Playground.Examples;

namespace SOFTURE.Typesense.Playground;

public sealed class Playground(IDocumentClient documentClient)
{
    public async Task Run()
    {
        var currentId = 1;
        var exampleDocuments = new Faker<ExampleDocument>()
            .StrictMode(true)
            .RuleFor(o => o.Id, f => (currentId++).ToString())
            .RuleFor(o => o.Name, f => f.Name.FullName())
            .RuleFor(o => o.Identifier, f => f.Random.String2(2))
            .RuleFor(o => o.City, f => f.Address.City())
            .Generate(50);

        foreach (var exampleDocument in exampleDocuments)
        {
            await documentClient.UpsertDocument(exampleDocument)
                .Tap(() => Console.WriteLine("Document upserted"))
                .TapError(error => Console.WriteLine($"Error creating document: {error}"));
        }


        var exampleQuery = new ExampleQuery
        {
            Name = "John Doe"
        };

        await documentClient.Search<ExampleDocument, ExampleQuery>(exampleQuery)
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