using CSharpFunctionalExtensions;
using SOFTURE.Typesense.Abstractions;
using SOFTURE.Typesense.Playground.Examples;

namespace SOFTURE.Typesense.Playground;

public sealed class Playground(IDocumentClient documentClient)
{
    public async Task Run()
    {
        var exampleDocument = new ExampleDocument
        {
            Id = "1",
            ClientId = "1",
            Name = "John Doe",
            Identifier = "JD",
            City = "New York",
        };

        await documentClient.UpsertDocument(exampleDocument)
            .Tap(() => Console.WriteLine("Document upserted"))
            .TapError(error => Console.WriteLine($"Error creating document: {error}"));

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