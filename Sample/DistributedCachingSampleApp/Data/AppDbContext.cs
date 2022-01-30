using DistributedCachingSampleApp.Data.Config;
using DistributedCachingSampleApp.DomainModel;
using Microsoft.EntityFrameworkCore;

namespace DistributedCachingSampleApp.Data;

public class AppDbContext : DbContext
{
  public AppDbContext(DbContextOptions options) : base(options)
  {
    Database.MigrateAsync();
  }
  public DbSet<Customer> Customer { get; set; } = null!;
  
  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);
    modelBuilder.ApplyConfiguration(new CustomerConfig());
  }
}

