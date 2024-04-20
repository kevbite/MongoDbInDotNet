using Demo2.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

#pragma warning disable CS0618
BsonDefaults.GuidRepresentation = GuidRepresentation.Standard;
BsonDefaults.GuidRepresentationMode = GuidRepresentationMode.V3;
BsonSerializer.RegisterSerializer(GuidSerializer.StandardInstance);
#pragma warning restore CS0618
//
// BsonClassMap.RegisterClassMap<Policy>(map =>
// {
//     map.AutoMap();
// });


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton(_
    => new MongoClient("mongodb://localhost:27017"));
builder.Services.AddScoped<IMongoDatabase>(provider =>
    provider.GetRequiredService<MongoClient>()
        .GetDatabase("insurance-demo2"));
builder.Services.AddScoped<IMongoCollection<Policy>>(provider =>
    provider.GetRequiredService<IMongoDatabase>()
        .GetCollection<Policy>("policies"));


var app = builder.Build();


var group = app.MapGroup("/policies");

group.MapPost("/", async (Policy policy, IMongoCollection<Policy> collection) =>
{
    policy = policy with { Id = Guid.NewGuid() };
    await collection.InsertOneAsync(policy);

    return TypedResults.Ok(policy);
});

group.MapGet("/", async (string? policyNumber, IMongoCollection<Policy> collection) =>
{
    var aggregateFluent = collection.Aggregate();
    if (policyNumber is { Length: > 0 })
    {
        aggregateFluent = aggregateFluent.Match(x => x.PolicyNumber == policyNumber);
    }

    return TypedResults.Ok(await aggregateFluent.ToListAsync());
});

group.MapGet("/{policyId:guid}", async (Guid policyId, IMongoCollection<Policy> collection) =>
{
    var policy = await collection.Find(x => x.Id == policyId)
        .FirstOrDefaultAsync();

    return policy is null ? Results.NotFound() : Results.Ok(policy);
});

group.MapPost("/{policyId:guid}/claims", async (Guid policyId, Claim claim, IMongoCollection<Policy> collection) =>
{
    claim = claim with { Id = Guid.NewGuid() };

    var result = await collection.UpdateOneAsync(x => x.Id == policyId,
        Builders<Policy>.Update.Push(x => x.Claims, claim));

    if (result is { MatchedCount: 0 })
        return Results.NotFound();
    
    return TypedResults.Ok(claim);
});

app.Run();