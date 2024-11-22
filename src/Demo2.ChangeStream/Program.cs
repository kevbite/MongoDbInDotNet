// See https://aka.ms/new-console-template for more information

using Demo2.Models;
using MongoDB.Driver;

Console.WriteLine("Running...");

var connectionString = "mongodb://localhost:27017";

var client =
    new MongoClient(
        connectionString);

var database = client.GetDatabase("insurance-demo2");

var collection = database.GetCollection<Policy>("policies");

var pipeline = new EmptyPipelineDefinition<ChangeStreamDocument<Policy>>()
    .Match(x => x.OperationType 
                == ChangeStreamOperationType.Insert);

var cursor = collection.Watch(pipeline, new ChangeStreamOptions
{
    // FullDocument = ChangeStreamFullDocumentOption,
});

await cursor.ForEachAsync(change =>
{
    Console.WriteLine($"Order has been created");
});

