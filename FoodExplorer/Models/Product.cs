using System;
using System.Collections.Generic;

namespace FoodExplorer.Models
{
    public class Product
    {
        
        public string BarCode { get; set; } = string.Empty;

       
        public string Name { get; set; } = string.Empty;
        public string ImageFrontSmallUrl { get; set; } = string.Empty;
        public string Ingredients { get; set; } = string.Empty;
        public string NutriScore { get; set; } = string.Empty;

        
        public string Additives { get; set; } = string.Empty;
        public string Traces { get; set; } = string.Empty;

        public NutrientLevels? NutrientLevels { get; set; }

        public DateTime LastUpdatedDate { get; set; }

     
        public ICollection<History> HistoryEntries { get; set; }
        public Favorite? FavoriteEntry { get; set; } 
     

        public Product()
        {

            HistoryEntries = new List<History>();

            LastUpdatedDate = DateTime.MinValue;
        }
    }
}