namespace PetAdoption.Models.ViewModels
{
    public class LocationDetails
    {
        // Location details must have a LocationDto object
        public required LocationDto Location { get; set; }

        // A location can have many food trucks associated with it
        public IEnumerable<FoodTruckDto>? AssociatedFoodTrucks { get; set; }
    }
}
