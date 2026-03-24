using FoodExplorer.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FoodExplorer.Interfaces
{
    /// <summary>
    /// Technical service for direct communication with the Open Food Facts API.
    /// </summary>
    public interface IOpenFoodFactsApi
    {
        /// <summary>
        /// Retrieves raw product data from the API using its unique barcode.
        /// </summary>
        /// <param name="barcode">The product's barcode identifier.</param>
        /// <returns>The product mapped to the C# model, or null if the API does not find it.</returns>
        Task<Product?> FetchProductByBarcodeAsync(string barcode);

        
    }
}