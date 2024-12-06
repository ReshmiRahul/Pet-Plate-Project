using PetAdoption.Models;

namespace PetAdoption.Interfaces
{
    public interface IFoodTruckService
    {
        // Base CRUD operations
        Task<IEnumerable<FoodTruckDto>> ListFoodTrucks();
        Task<FoodTruckDto?> FindFoodTruck(int id);
        Task<ServiceResponse> UpdateFoodTruck(int id, FoodTruckDto foodTruckDto);
        Task<ServiceResponse> AddFoodTruck(FoodTruckDto foodTruckDto);
        Task<ServiceResponse> DeleteFoodTruck(int id);

        // Related methods
        Task<IEnumerable<FoodTruckDto>> ListFoodTrucksByLocation(int locationId);
        Task<ServiceResponse> AssignFoodTruckToLocation(int foodTruckId, int locationId);
        Task<ServiceResponse> RemoveFoodTruckFromLocation(int foodTruckId, int locationId);
    }
}
