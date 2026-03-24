using FoodExplorer.Data;
using FoodExplorer.Interfaces;
using FoodExplorer.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FoodExplorer.Repositories
{
    public class FavoriteRepository(FoodExplorerContext context) : IFavoriteRepository
    {
        private readonly FoodExplorerContext _context = context;

        /// <summary>
        /// Adds a product to the favorites list. 
        /// Checks for existing entries first to prevent duplicate favorites.
        /// </summary>
        public async Task AddAsync(Favorite favorite)
        {
            if (!await IsFavoriteAsync(favorite.ProductBarCode))
            {
                _context.Favorites.Add(favorite);
                await _context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Updates the Note column for a favorite entry identified by barcode.
        /// </summary>
        public async Task UpdateNoteAsync(string barcode, string note)
        {
            var favorite = await _context.Favorites.FirstOrDefaultAsync(f => f.ProductBarCode == barcode);
            if (favorite != null)
            {
                favorite.Note = note ?? string.Empty;
                _context.Favorites.Update(favorite);
                await _context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Retrieves all favorites. 
        /// Includes the associated Product details (Name, Image, Nutri-Score, etc.).
        /// </summary>
        public async Task<List<Favorite>> GetAllAsync()
        {
            return await _context.Favorites
                .AsNoTracking() // Performance boost for read-only lists
                .Include(f => f.Product)
                .ToListAsync();
        }

        /// <summary>
        /// Removes a favorite entry using its unique database ID.
        /// </summary>
        public async Task DeleteAsync(int favoriteId)
        {
            var favorite = await _context.Favorites.FindAsync(favoriteId);
            if (favorite != null)
            {
                _context.Favorites.Remove(favorite);
                await _context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Removes a favorite entry based on the product's barcode.
        /// Useful for the "unstar" action in the search results.
        /// </summary>
        public async Task DeleteByBarcodeAsync(string barcode)
        {
            var favorite = await _context.Favorites
                .FirstOrDefaultAsync(f => f.ProductBarCode == barcode);

            if (favorite != null)
            {
                _context.Favorites.Remove(favorite);
                await _context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Checks if a product is already marked as a favorite.
        /// </summary>
        public async Task<bool> IsFavoriteAsync(string barcode)
        {
            return await _context.Favorites
                .AsNoTracking()
                .AnyAsync(f => f.ProductBarCode == barcode);
        }
    }
}