using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BlueManagerPlatform.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;


namespace BlueManager.Data
{


    public class BlueManagerContext : DbContext
    {
        public BlueManagerContext(DbContextOptions<BlueManagerContext> options)
            : base(options)
        {
        }

        public DbSet<BlueManagerPlatform.Models.Hub> Hubs { get; set; }

        public DbSet<BlueManagerPlatform.Models.Tool> Tools { get; set; }
        public DbSet<BlueManagerPlatform.Models.ToolAtHub> ToolAtHubs { get; set; }
        public DbSet<BlueManagerPlatform.Models.ToolBatteryReadout> ToolBatteryReadouts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ToolAtHub>()
                .HasNoKey();
            modelBuilder.Entity<ToolBatteryReadout>()
                .HasNoKey();
        }
    }
}
