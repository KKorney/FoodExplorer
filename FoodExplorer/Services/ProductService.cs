using FoodExplorer.Interfaces;
using FoodExplorer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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
        /// Orchestrates product retrieval using a "Cache-Aside" pattern.
        /// Checks local storage first, falls back to API, and updates history.
        /// </summary>
        public async Task<Product?> GetProductByBarcodeAsync(string barcode)
        {
            // Step 1: Check the local database (Cache)
            var product = await _productRepository.GetByBarcodeAsync(barcode);

            // Step 2: If not found in cache, fetch from external API
            if (product == null)
            {
                product = await _apiService.FetchProductByBarcodeAsync(barcode);

                if (product != null)
                {
                    // Step 3: Save to local DB for future offline/fast access
                    await _productRepository.SaveOrUpdateAsync(product);
                }
            }

            // Step 4: If product is found (locally or via API), record the visit in history
            if (product != null)
            {
                var historyEntry = new History
                {
                    ProductBarCode = product.BarCode,
                    ConsultationDate = DateTime.Now
                };
                await _historyRepository.AddEntryAsync(historyEntry);
            }

            // Step 5: Return the final product object to the ViewModel
            return product;
        }

        
      
    }
}