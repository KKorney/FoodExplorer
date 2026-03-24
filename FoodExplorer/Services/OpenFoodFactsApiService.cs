using FoodExplorer.Interfaces;
using FoodExplorer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace FoodExplorer.Services
{
    public class OpenFoodFactsApiService : IOpenFoodFactsApi
    {
        private readonly HttpClient _httpClient;
        private readonly SemaphoreSlim _semaphore = new(1, 1);
        private DateTime _lastRequestTime = DateTime.MinValue;
        
        // Open Food Facts requests a limit of roughly 10 requests per minute.
        // We set a 6-second interval to respect this limit safely.
        private readonly TimeSpan _minInterval = TimeSpan.FromSeconds(6);

        public OpenFoodFactsApiService()
        {
            _httpClient = new HttpClient();
            // IMPORTANT: Open Food Facts requires a descriptive User-Agent
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "FoodExplorer - Windows - Version 1.0");
        }

        /// <summary>
        /// Ensures we don't exceed the API rate limit by waiting between requests.
        /// </summary>
        private async Task WaitIfNeededAsync()
        {
            await _semaphore.WaitAsync();
            try
            {
                var elapsed = DateTime.Now - _lastRequestTime;
                if (elapsed < _minInterval)
                {
                    await Task.Delay(_minInterval - elapsed);
                }
                _lastRequestTime = DateTime.Now;
            }
            finally 
            { 
                _semaphore.Release(); 
            }
        }

        public async Task<Product?> FetchProductByBarcodeAsync(string barcode)
        {
            await WaitIfNeededAsync();

            // We request specific fields to reduce data usage and improve speed
            string url = $"https://world.openfoodfacts.org/api/v2/product/{barcode}.json?fields=product_name_en,nutriscore_grade,ingredients_text_en,additives_tags,traces,nutrient_levels,image_front_small_url,status";

            try
            {
                var response = await _httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
                    var root = doc.RootElement;

                    // 'status' property: 1 means found, 0 means not found
                    if (root.TryGetProperty("status", out var statusProp) && statusProp.GetInt32() == 1)
                    {
                        if (root.TryGetProperty("product", out var productJson))
                        {
                            return MapJsonToProduct(productJson, barcode);
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Product {barcode} not found on the server.");
                    }
                }
            }
            catch (Exception ex) 
            { 
                Console.WriteLine($"API Error (Barcode): {ex.Message}"); 
            }
            return null;
        }

      
        /// <summary>
        /// Maps raw JSON data from Open Food Facts into our local Product model.
        /// </summary>
        private static Product MapJsonToProduct(JsonElement p, string barcode)
        {
            var levels = new NutrientLevels();
            if (p.TryGetProperty("nutrient_levels", out var nl))
            {
                levels.Fat = nl.TryGetProperty("fat", out var f) ? f.GetString() ?? "unknown" : "unknown";
                levels.Salt = nl.TryGetProperty("salt", out var s) ? s.GetString() ?? "unknown" : "unknown";
                levels.SaturatedFat = nl.TryGetProperty("saturated-fat", out var sf) ? sf.GetString() ?? "unknown" : "unknown";
                levels.Sugars = nl.TryGetProperty("sugars", out var sug) ? sug.GetString() ?? "unknown" : "unknown";
                levels.ProductBarCode = barcode;
            }

            // Clean up the 'traces' string (removing language prefixes like en: or fr:)
            string rawTraces = p.TryGetProperty("traces", out var tr) ? tr.GetString() ?? "" : "";
            string cleanedTraces = string.IsNullOrWhiteSpace(rawTraces) 
                ? "None" 
                : rawTraces.Replace("en:", "").Replace("fr:", "").Trim();

            return new Product
            {
                BarCode = barcode,
                Name = p.TryGetProperty("product_name_en", out var n) ? n.GetString() ?? "Unknown Name" : "Unknown Name",

                // Empty string allows the 'TargetNullValue' in XAML to trigger the fallback image
                ImageFrontSmallUrl = p.TryGetProperty("image_front_small_url", out var img) ? img.GetString() ?? "" : "",

                NutriScore = p.TryGetProperty("nutriscore_grade", out var ng) ? ng.GetString()?.ToUpper() ?? "N/A" : "N/A",
                Ingredients = p.TryGetProperty("ingredients_text_en", out var ing) ? ing.GetString() ?? "No ingredients info" : "No ingredients info",

            
                Additives = p.TryGetProperty("additives_tags", out var add)
                    ? string.Join(", ", add.EnumerateArray()
                        .Select(a => a.GetString()?.Split(':').Last().ToUpper() ?? "")) 
                    : "None",

                Traces = cleanedTraces,
                NutrientLevels = levels,
                LastUpdatedDate = DateTime.Now
            };
        }
    }
}