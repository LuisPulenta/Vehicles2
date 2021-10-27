using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Vehicles2.Api.Data.Entities;

namespace Vehicles2.Api.Data
{
    public class DataContext : IdentityDbContext<User>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }
        
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Detail> Details { get; set; }
        public DbSet<DocumentType> DocumentTypes { get; set; }
        public DbSet<History> Histories { get; set; }
        public DbSet<Procedure> Procedures { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<VehiclePhoto> VehiclePhotos { get; set; }
        public DbSet<VehicleType> VehicleTypes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<VehicleType>().HasIndex(x => x.Description).IsUnique();
        }
    }
}