namespace PetAdoption.Models.ViewModels
{
    public class ApplicationDetails
    {
        // Application details must have an ApplicationDto object
        public required ApplicationDto Application { get; set; }

        // The application is associated with a specific account
        public required AccountDto Account { get; set; }

        // The application is also associated with a specific pet
        public required PetDto Pet { get; set; }

        // The application is also associated with FoodTruck
        public FoodTruckDto? AssociatedFoodTruck { get; set; }
    }
}
