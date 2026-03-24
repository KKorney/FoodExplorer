using FoodExplorer.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FoodExplorer.Interfaces
{
    /// <summary>
    /// Handles low-level database operations for product entities.
    /// Manages the local cache of products retrieved from the API.
    /// </summary>
    public interface IProductRepository
    {
        /// <summary>
        /// Retrieves a product from the local database using its barcode.
        /// </summary>
        /// <param name="barcode">The unique barcode identifier.</param>
        /// <returns>The cached product if found; otherwise, null.</returns>
        Task<Product?> GetByBarcodeAsync(string barcode);

      

        /// <summary>
        /// Saves a new product to the database or updates an existing one.
        /// Useful for keeping the Nutri-Score and ingredients up to date.
        /// </summary>
        /// <param name="product">The product entity to persist.</param>
        Task SaveOrUpdateAsync(Product product);

        /// <summary>
        /// Quickly checks if a product is already cached in the local database.
        /// </summary>
        /// <param name="barcode">The barcode to check.</param>
        /// <returns>True if the product exists locally; otherwise, false.</returns>
        Task<bool> ExistsAsync(string barcode);
    }
}