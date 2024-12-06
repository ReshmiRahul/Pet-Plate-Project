using System.ComponentModel.DataAnnotations;

namespace PetAdoption.Models
{
    public class Location
    {
        [Key]
        public int LocationId { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }

        public ICollection<FoodTruck> FoodTrucks { get; set; }
    }

    public class LocationDto
    {
        public int LocationId { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
    }
}
