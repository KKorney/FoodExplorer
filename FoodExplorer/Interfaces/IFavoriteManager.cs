using FoodExplorer.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FoodExplorer.Interfaces
{
    public interface IFavoriteManager
    {
        /// <summary>
        /// Adds a product to the user's favorites list.
        /// </summary>
        /// <param name="product">The product object to be saved.</param>
        Task AddFavoriteAsync(Product product, string? note = null);

        /// <summary>
        /// Retrieves the complete list of all favorite entries.
        /// </summary>
        /// <returns>A list containing all favorite products and their metadata.</returns>
        Task<List<Favorite>> GetFullFavoritesAsync();

        /// <summary>
        /// Removes a product from the favorites list using its unique barcode.
        /// </summary>
        /// <param name="barcode">The unique barcode identifier of the product.</param>
        Task DeleteFavoriteEntryAsync(string barcode);

        /// <summary>
        /// Updates the note attached to a favorite product.
        /// </summary>
        Task UpdateFavoriteNoteAsync(string barcode, string note);

        /// <summary>
        /// Checks if a specific product is already marked as a favorite.
        /// Useful for toggling the star icon or preventing duplicates in the UI.
        /// </summary>
        /// <param name="barcode">The barcode of the product to check.</param>
        /// <returns>True if the product exists in favorites, otherwise false.</returns>
        Task<bool> IsFavoriteAsync(string barcode);
    }
}