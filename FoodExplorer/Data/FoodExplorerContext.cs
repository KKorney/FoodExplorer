using FoodExplorer.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;

namespace FoodExplorer.Data
{
    public class FoodExplorerContext : DbContext
    {
        // DbSets represent your tables in the database
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<NutrientLevels> NutrientLevels { get; set; } = null!;
        public DbSet<History> HistoryEntries { get; set; } = null!;
        public DbSet<Favorite> Favorites { get; set; } = null!;

        public FoodExplorerContext() { }

        // Configuration method for migrations and runtime options
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // We store the database in the Local Application Data folder.
            // This ensures the app has write permissions and the DB persists across updates.
            string appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string dbDirectory = Path.Combine(appDataFolder, "FoodExplorer");

            // Create the directory if it doesn't exist yet
            if (!Directory.Exists(dbDirectory))
            {
                Directory.CreateDirectory(dbDirectory);
            }

            string dbPath = Path.Combine(dbDirectory, "food_explorer.db");

            // Use SQLite as the database engine
            optionsBuilder.UseSqlite($"Data Source={dbPath}");

            // Note: For migrations, use the following command in the terminal:
            // dotnet ef migrations add InitialCreate --project FoodExplorer --startup-project FoodExplorer
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // --- 1. Product Entity Configuration ---
            // Using Barcode as the Primary Key
            modelBuilder.Entity<Product>().HasKey(p => p.BarCode);

            // Convert DateTime to long (ticks) for SQLite compatibility (INTEGER storage)
            modelBuilder.Entity<Product>()
                .Property(p => p.LastUpdatedDate)
                .HasConversion<long>();

            // --- 2. 1:1 Relationship Configuration (Product <-> NutrientLevels) ---
            modelBuilder.Entity<NutrientLevels>()
                .HasOne(nl => nl.Product)
                .WithOne(p => p.NutrientLevels)
                .HasForeignKey<NutrientLevels>(nl => nl.ProductBarCode)
                .IsRequired();

            // --- 3. 1:N Relationship Configuration (Product <-> History) ---
            modelBuilder.Entity<History>()
                .HasOne(h => h.Product)
                .WithMany(p => p.HistoryEntries)
                .HasForeignKey(h => h.ProductBarCode)
                .IsRequired();

            modelBuilder.Entity<History>()
                .Property(h => h.ConsultationDate)
                .HasConversion<long>();

            // --- 4. 0:1 Relationship Configuration (Product <-> Favorite) ---
            modelBuilder.Entity<Favorite>()
                .HasOne(f => f.Product)
                .WithOne(p => p.FavoriteEntry)
                .HasForeignKey<Favorite>(f => f.ProductBarCode)
                .IsRequired();

            modelBuilder.Entity<Favorite>()
                .Property(f => f.AddedDate)
                .HasConversion<long>();
        }
    }
}