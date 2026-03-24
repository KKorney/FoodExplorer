using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodExplorer.Models
{
    public class Favorite
    {

        
        [Key]
        public string ProductBarCode { get; set; } = string.Empty;
        public string Note { get; set; } = string.Empty;
        public DateTime AddedDate { get; set; }

        public Product Product { get; set; } = null!;


        public Favorite()
        {

            AddedDate = DateTime.UtcNow;
            Note = string.Empty;
        }
    }
}