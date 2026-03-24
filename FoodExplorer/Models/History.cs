using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodExplorer.Models
{


    public class History
    {
        public int Id { get; set; } // Clé Primaire de History
        public DateTime ConsultationDate { get; set; }

        // Clé Étrangère vers Product
        public string ProductBarCode { get; set; } = string.Empty;

        // Propriété de Navigation
        public Product Product { get; set; } = null!;

        public History()
        {

            ConsultationDate = DateTime.UtcNow;
        }
    }
}