using FGC.Domain.GameManagement.Entities;
using FGC.Domain.PaymentManagement.Entities;
using FGC.Domain.UserManagement.Entities;
using FGC.Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;

namespace FGC.Infrastructure.Data.Context
{
    public class FGCDbContext : DbContext
    {
        public FGCDbContext(DbContextOptions<FGCDbContext> options) : base(options) { }

        public FGCDbContext() : base() { }

        public DbSet<User> Users { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<Payment> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply configurations
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new GameConfiguration());
            modelBuilder.ApplyConfiguration(new PaymentConfiguration());

            ConfigureGlobalSettings(modelBuilder);
        }

        private static void ConfigureGlobalSettings(ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(decimal) || property.ClrType == typeof(decimal?))
                    {
                        property.SetPrecision(18);
                        property.SetScale(2);
                    }
                }
            }
        }
    }
}
