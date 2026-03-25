using FoodExplorer.Data;
using FoodExplorer.Interfaces;
using FoodExplorer.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FoodExplorer.Repositories
{
    public class ProductRepository(FoodExplorerContext context) : IProductRepository
    {
        private readonly FoodExplorerContext _context = context;

        /// <summary>
        /// Retrieves a product by barcode, including its associated nutrient levels.
        /// </summary>
        public async Task<Product?> GetByBarcodeAsync(string barcode)
        {
            return await _context.Products
                .Include(p => p.NutrientLevels)
                .FirstOrDefaultAsync(p => p.BarCode == barcode);
        }

        

        /// <summary>
        /// Handles the "Upsert" logic: Adds a new product or updates an existing one.
        /// Sets the LastUpdatedDate to the current timestamp.
        /// </summary>
        public async Task SaveOrUpdateAsync(Product product)
        {
            // Check if the product already exists using AsNoTracking to avoid conflict during Update
            var exists = await _context.Products.AnyAsync(p => p.BarCode == product.BarCode);

            product.LastUpdatedDate = DateTime.Now;

            if (!exists)
            {
                // New product entry
                _context.Products.Add(product);
            }
            else
            {
                // Update existing product details (Nutri-Score, ingredients, etc.)
                _context.Products.Update(product);
            }

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Quick check to see if a product is already cached locally.
        /// </summary>
        public async Task<bool> ExistsAsync(string barcode)
        {
            return await _context.Products.AnyAsync(p => p.BarCode == barcode);
        }
    }
}