using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PetAdoption.Models
{
    public class MenuItem
    {
        [Key]
        public int MenuItemId { get; set; }

        public string Name { get; set; }
        public decimal Price { get; set; }

        [ForeignKey("FoodTruck")]
        public int FoodTruckId { get; set; }

        // Navigation property
        public FoodTruck FoodTruck { get; set; }
    }
    public class MenuItemDto
    {
        public int MenuItemId { get; set; }

        public string Name { get; set; }
        public decimal Price { get; set; }

        // Foreign Key to FoodTruck
        public int FoodTruckId { get; set; }
    }
}
