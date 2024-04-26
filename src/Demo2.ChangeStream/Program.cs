// See https://aka.ms/new-console-template for more information

using Demo2.Models;
using MongoDB.Driver;

Console.WriteLine("Running...");

var client =
    new MongoClient(
        "mongodb+srv://leedssharp:X7N8IDTJou96tetg@cluster0.adp3smg.mongodb.net/?retryWrites=true&w=majority&appName=Cluster0&appname=Demo2");

var database = client.GetDatabase("insurance-demo2");

var collection = database.GetCollection<Policy>("policies");

var pipeline = new EmptyPipelineDefinition<ChangeStreamDocument<Policy>>()
    .Match(x => x.OperationType == ChangeStreamOperationType.Insert);

var cursor = collection.Watch(pipeline, new ChangeStreamOptions
{
    FullDocument = ChangeStreamFullDocumentOption.UpdateLookup
});

await cursor.ForEachAsync(change =>
{
    var policy = change.FullDocument;
    Console.WriteLine($"Order {policy.PolicyNumber} has been created");
});

