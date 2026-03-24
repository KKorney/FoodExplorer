using FoodExplorer.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FoodExplorer.Interfaces
{
    /// <summary>
    /// Manages persistence operations (CRUD) for favorite products in the local database.
    /// </summary>
    public interface IFavoriteRepository
    {
        /// <summary>
        /// Adds a product to the favorites table.
        /// </summary>
        /// <param name="favorite">The favorite entry to persist.</param>
        Task AddAsync(Favorite favorite);

        /// <summary>
        /// Retrieves the list of all favorites, including associated product details.
        /// </summary>
        /// <returns>A list of favorite entries with their related product information.</returns>
        Task<List<Favorite>> GetAllAsync();

        /// <summary>
        /// Removes a specific favorite entry using its unique database identifier.
        /// </summary>
        /// <param name="favoriteId">The primary key ID of the favorite entry.</param>
        Task DeleteAsync(int favoriteId);

        /// <summary>
        /// Removes a favorite entry using the product's barcode. 
        /// Useful for "unstarring" a product directly from the search results.
        /// </summary>
        /// <param name="barcode">The unique barcode of the product to remove.</param>
        Task DeleteByBarcodeAsync(string barcode);

        /// <summary>
        /// Checks if a product already exists in the favorites list.
        /// </summary>
        /// <param name="barcode">The barcode of the product to verify.</param>
        /// <returns>True if the product is already a favorite; otherwise, false.</returns>
        Task<bool> IsFavoriteAsync(string barcode);

        /// <summary>
        /// Updates the Note field for a favorite entry identified by barcode.
        /// </summary>
        Task UpdateNoteAsync(string barcode, string note);
    }
}