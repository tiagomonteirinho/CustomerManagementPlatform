using AppCalisto.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace AppCalisto.Data
{
    public class DataContext : IdentityDbContext<User>
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }

            modelBuilder.Entity<Client>()
                .Property(o => o.Id)
                .UseIdentityColumn(seed: 1000, increment: 1);

            modelBuilder.Entity<Order>()
                .Property(o => o.Id)
                .UseIdentityColumn(seed: 1000, increment: 1);

            modelBuilder.Entity<Product>()
                .Property(o => o.Id)
                .UseIdentityColumn(seed: 1000, increment: 1);

            modelBuilder.Entity<Budget>()
                .Property(o => o.Id)
                .UseIdentityColumn(seed: 1000, increment: 1);

            modelBuilder.Entity<BudgetProduct>()
                .HasKey(bp => new { bp.BudgetId, bp.ProductId });
        }

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<Client> Clients { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<Company> Companies { get; set; }

        public DbSet<Service> Services { get; set; }

        public DbSet<Product> Products { get; set; }

        public DbSet<Budget> Budgets { get; set; }
    }
}
