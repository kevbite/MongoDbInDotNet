using Demo1.Models;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

var mongoClient = 
    new MongoClient("mongodb://localhost:27017");

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<InsuranceDbContext>(
    options =>
        options.UseMongoDB(mongoClient,
            databaseName: "insurance-demo1"));

var app = builder.Build();

var group = app.MapGroup("/policies");

group.MapPost("/", async (Policy policy, 
    InsuranceDbContext context) =>
{
    policy = policy with { Id = Guid.NewGuid() };
    context.Policies.Add(policy);

    await context.SaveChangesAsync();

    return TypedResults.Ok(policy);
});

group.MapGet("/", async (InsuranceDbContext context,
    string? policyNumber) =>
{
    var policies =
        policyNumber is { Length: > 0 }
            ? context.Policies
                .Where(x => x.PolicyNumber == policyNumber)
            : context.Policies;

    return TypedResults.Ok(await policies.ToListAsync());
});

group.MapGet("/{policyId:guid}", async (Guid policyId,
    InsuranceDbContext context) =>
{
    var policy = await context.Policies
        .Where(x => x.Id == policyId)
        .FirstOrDefaultAsync();

    return policy is null ? Results.NotFound() : Results.Ok(policy);
});

group.MapPost("/{policyId:guid}/claims",
    async (Guid policyId, 
    Claim claim, InsuranceDbContext context) =>
{
    var policy = await context.Policies
        .Where(x => x.Id == policyId)
        .FirstOrDefaultAsync();

    if (policy is null)
        return Results.NotFound();

    claim = claim with { Id = Guid.NewGuid() };
    policy.Claims.Add(claim);

    await context.SaveChangesAsync();

    return TypedResults.Ok(claim);
});

app.Run();