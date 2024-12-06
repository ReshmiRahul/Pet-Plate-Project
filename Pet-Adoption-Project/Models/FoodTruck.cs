using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PetAdoption.Models
{
    public class FoodTruck
    {
        [Key]
        public int FoodTruckId { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public string Contact { get; set; }

        [ForeignKey("Location")]
        public int LocationId { get; set; }

        // Navigation properties
        public Location Location { get; set; }
        public ICollection<MenuItem> MenuItems { get; set; } = new List<MenuItem>();
        public ICollection<Application> Applications { get; set; } = new List<Application>();
    }

    public class FoodTruckDto
    {
        public int FoodTruckId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Contact { get; set; }
        public int LocationId { get; set; }
        public string? LocationAddress { get; set; }
    }
}
