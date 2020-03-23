using Microsoft.EntityFrameworkCore;
using BlueManagerPlatform.Models;
using BlueManager.Models;

namespace BlueManager.Data
{
    public class BlueManagerContext : DbContext
    {
        public BlueManagerContext(DbContextOptions<BlueManagerContext> options)
            : base(options)
        {
        }

        public DbSet<Hub> Hubs { get; set; }

        public DbSet<Tool> Tools { get; set; }
        public DbSet<ToolAtHub> ToolAtHubs { get; set; }
        public DbSet<ToolBatteryReadout> ToolBatteryReadouts { get; set; }

        public DbSet<ToolLastLocation> ToolLastLocations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ToolBatteryReadout>()
                .HasNoKey();

            modelBuilder.Entity<ToolLastLocation>()
                .HasNoKey()
                .ToView(nameof(ToolLastLocations));
        }
    }
}