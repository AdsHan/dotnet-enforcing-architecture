using EnforcingArchitecture.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace EnforcingArchitecture.API.Infrastructure;

public class CatalogDbContext : DbContext
{
    public CatalogDbContext()
    {

    }

    public CatalogDbContext(DbContextOptions<CatalogDbContext> options) : base(options)
    {

    }

    public DbSet<ProductModel> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(modelBuilder);
    }

}

