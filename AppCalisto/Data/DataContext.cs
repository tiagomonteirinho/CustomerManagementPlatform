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
                .Property(e => e.Id)
                .UseIdentityColumn(seed: 1000, increment: 1);

            modelBuilder.Entity<Product>()
                .Property(e => e.Id)
                .UseIdentityColumn(seed: 1000, increment: 1);

            modelBuilder.Entity<Order>()
                .Property(e => e.Id)
                .UseIdentityColumn(seed: 1000, increment: 1);

            modelBuilder.Entity<Appointment>()
                .Property(e => e.Id)
                .UseIdentityColumn(seed: 1000, increment: 1);

            modelBuilder.Entity<Budget>()
                .Property(e => e.Id)
                .UseIdentityColumn(seed: 1000, increment: 1);

            modelBuilder.Entity<Observation>()
                .Property(e => e.Id)
                .UseIdentityColumn(seed: 1000, increment: 1);

            modelBuilder.Entity<ObservationImage>()
                .Property(e => e.Id)
                .UseIdentityColumn(seed: 1000, increment: 1);
        }

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<Company> Companies { get; set; }

        public DbSet<Service> Services { get; set; }

        public DbSet<Client> Clients { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<Appointment> Appointments { get; set; }

        public DbSet<Observation> Observations { get; set; }

        public DbSet<ObservationImage> ObservationImages { get; set; }

        public DbSet<Budget> Budgets { get; set; }

        public DbSet<Product> Products { get; set; }

        public DbSet<Item> Items { get; set; }
    }
}
