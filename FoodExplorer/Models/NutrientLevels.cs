using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace FoodExplorer.Models
{
    public class NutrientLevels
    {
       

        [Key]
        public string ProductBarCode { get; set; } = string.Empty;

        public string Fat { get; set; } = string.Empty;
        public string SaturatedFat { get; set; } = string.Empty;
        public string Salt { get; set; } = string.Empty;
        public string Sugars { get; set; } = string.Empty;


        public Product Product { get; set; } = null!; 
    }
}