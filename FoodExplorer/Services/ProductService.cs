using FoodExplorer.Interfaces;
using FoodExplorer.Models;
using System;
using System.Threading.Tasks;

namespace FoodExplorer.Services
{
    public class ProductService(
        IProductRepository productRepository,
        IHistoryRepository historyRepository,
        IOpenFoodFactsApi apiService) : IProductFinder
    {
        private readonly IProductRepository _productRepository = productRepository;
        private readonly IHistoryRepository _historyRepository = historyRepository;
        private readonly IOpenFoodFactsApi _apiService = apiService;

        /// <summary>
        /// Retrieves a product by barcode. 
        /// Uses a "Cache-Aside" pattern with a 30-day expiration policy to ensure data accuracy.
        /// </summary>
        public async Task<Product?> GetProductByBarcodeAsync(string barcode)
        {
            // Step 1: Try to retrieve the product from the local SQLite database
            var product = await _productRepository.GetByBarcodeAsync(barcode);

            bool needsUpdate = false;

            if (product != null)
            {
                // Step 2: Check if the locally stored data is "stale" (older than 30 days)
                // Manufacturers often update ingredients or nutritional values.
                var dataAge = DateTime.Now - product.LastUpdatedDate;
                if (dataAge.TotalDays > 30)
                {
                    needsUpdate = true;
                }
            }

            // Step 3: Fetch from API if the product is missing locally OR if data is outdated
            if (product == null || needsUpdate)
            {
                try
                {
                    var freshProduct = await _apiService.FetchProductByBarcodeAsync(barcode);

                    if (freshProduct != null)
                    {
                        product = freshProduct;
                        // Update the local database with fresh data and a new timestamp
                        await _productRepository.SaveOrUpdateAsync(product);
                    }
                }
                catch (Exception)
                {
                    // If API call fails but we have old data, we keep the old data as a fallback
                    // This allows the app to work offline or during API downtime.
                }
            }

            // Step 4: Record this consultation in the history table
            if (product != null)
            {
                var historyEntry = new History
                {
                    ProductBarCode = product.BarCode,
                    ConsultationDate = DateTime.Now
                };
                await _historyRepository.AddEntryAsync(historyEntry);
            }

            return product;
        }
    }
}