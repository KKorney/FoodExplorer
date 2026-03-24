using FoodExplorer.Interfaces;
using FoodExplorer.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FoodExplorer.Services
{
    public class FavoriteService(IFavoriteRepository favoriteRepository) : IFavoriteManager
    {
        private readonly IFavoriteRepository _favoriteRepository = favoriteRepository;

        /// <summary>
        /// Adds a product to the favorites. 
        /// Converts the Product object into a Favorite entity with the current timestamp.
        /// </summary>
        public async Task AddFavoriteAsync(Product product, string? note = null)
        {
            // Prepare the Favorite object from the Product details
            var favorite = new Favorite
            {
                ProductBarCode = product.BarCode,
                AddedDate = DateTime.Now,
                Note = note ?? string.Empty // Accept a note passed by caller
            };

            await _favoriteRepository.AddAsync(favorite);
        }

        public async Task UpdateFavoriteNoteAsync(string barcode, string note)
        {
            await _favoriteRepository.UpdateNoteAsync(barcode, note);
        }

        /// <summary>
        /// Retrieves the complete list of favorites.
        /// The repository handles the .Include(p => p.Product) to ensure data is complete.
        /// </summary>
        public async Task<List<Favorite>> GetFullFavoritesAsync()
        {
            return await _favoriteRepository.GetAllAsync();
        }

        /// <summary>
        /// Removes a favorite entry using the product's barcode.
        /// This is the most common way to "unfavorite" a product from the UI.
        /// </summary>
        public async Task DeleteFavoriteEntryAsync(string barcode)
        {
            await _favoriteRepository.DeleteByBarcodeAsync(barcode);
        }

        /// <summary>
        /// Checks if a product is currently in the favorites list.
        /// Primarily used to determine the state of the "Star" icon in the UI.
        /// </summary>
        public async Task<bool> IsFavoriteAsync(string barcode)
        {
            return await _favoriteRepository.IsFavoriteAsync(barcode);
        }

        /// <summary>
        /// Helper method to remove a favorite by barcode.
        /// Useful for implementing a "Toggle" button (Add if not exists, Remove if exists).
        /// </summary>
        public async Task RemoveFavoriteByBarcodeAsync(string barcode)
        {
            await _favoriteRepository.DeleteByBarcodeAsync(barcode);
        }
    }
}