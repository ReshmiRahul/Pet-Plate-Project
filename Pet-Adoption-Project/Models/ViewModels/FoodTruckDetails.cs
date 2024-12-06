namespace PetAdoption.Models.ViewModels
{
    public class FoodTruckDetails
    {
        // FoodTruck details must have a FoodTruckDto object
        public required FoodTruckDto FoodTruck { get; set; }

        // List of menu items served by the food truck
        public IEnumerable<MenuItemDto>? MenuItems { get; set; }

        // Applications that include this food truck
        public IEnumerable<ApplicationDto>? RelatedApplications { get; set; }
    }
}
