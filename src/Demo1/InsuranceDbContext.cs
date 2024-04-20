using Demo1.Models;
using Microsoft.EntityFrameworkCore;
using MongoDB.EntityFrameworkCore.Extensions;

public class InsuranceDbContext : DbContext
{
    public DbSet<Policy> Policies { get; init; }

    public InsuranceDbContext(DbContextOptions<InsuranceDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Policy>(builder =>
        {
            builder.ToCollection("policies");
            // builder.Property(x => x.Id);
            // builder.Property(x => x.PolicyHolder).HasElementName("policyHolder");
            // builder.Property(x => x.PolicyNumber).HasElementName("policyNumber");
            // builder.Property(x => x.Premium).HasElementName("premium");
            // builder.Property(x => x.Claims).HasElementName("claims");
        });
    }
}