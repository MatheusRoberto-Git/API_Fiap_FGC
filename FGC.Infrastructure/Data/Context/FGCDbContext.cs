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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new UserConfiguration());
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