using FoodExplorer.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FoodExplorer.Interfaces
{
    /// <summary>
    /// Defines the contract for product searching, including caching mechanisms and API integration.
    /// </summary>
    public interface IProductFinder
    {
        /// <summary>
        /// Searches for a product by its barcode.
        /// Implements smart caching logic: Check Local DB first; if not found, 
        /// perform API call, then create or update the local cache.
        /// </summary>
        /// <param name="barcode">The unique barcode of the product to search for.</param>
        /// <returns>The complete Product object, or null if not found.</returns>
        Task<Product?> GetProductByBarcodeAsync(string barcode);

      
    }
}