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
    public class HistoryRepository(FoodExplorerContext context) : IHistoryRepository
    {
        private readonly FoodExplorerContext _context = context;

        /// <summary>
        /// Adds a product to the history or updates the timestamp if it was already scanned.
        /// This prevents duplicate entries for the same product.
        /// </summary>
        public async Task AddEntryAsync(History entry)
        {
            // Check if the product has been scanned before to avoid duplicates
            var existingEntry = await _context.HistoryEntries
                .FirstOrDefaultAsync(h => h.ProductBarCode == entry.ProductBarCode);

            if (existingEntry != null)
            {
                // If it exists, simply update the viewing timestamp
                existingEntry.ConsultationDate = DateTime.Now;
                _context.HistoryEntries.Update(existingEntry);
            }
            else
            {
                // Otherwise, create a new history record
                entry.ConsultationDate = DateTime.Now;
                _context.HistoryEntries.Add(entry);
            }

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Retrieves the full viewing history, including product details.
        /// Sorted by the most recent scans first.
        /// </summary>
        public async Task<List<History>> GetFullHistoryAsync()
        {
            return await _context.HistoryEntries
                .AsNoTracking() // Performance optimization for read-only display
                .Include(h => h.Product)
                .OrderByDescending(h => h.ConsultationDate) // Most recent at the top
                .ToListAsync();
        }

      

       
    }
}