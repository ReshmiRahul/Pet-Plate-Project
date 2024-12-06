namespace PetAdoption.Models.ViewModels
{
    public class MenuItemDetails
    {
        // MenuItem details must have a MenuItemDto object
        public required MenuItemDto MenuItem { get; set; }

        // FoodTruck associated with the menu item
        public FoodTruckDto? AssociatedFoodTruck { get; set; }
    }
}
