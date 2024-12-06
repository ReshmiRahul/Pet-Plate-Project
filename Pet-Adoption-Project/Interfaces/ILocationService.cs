using PetAdoption.Models;

namespace PetAdoption.Interfaces
{
    public interface ILocationService
    {
        // Base CRUD operations
        Task<IEnumerable<LocationDto>> ListLocations();
        Task<LocationDto?> FindLocation(int id);
        Task<ServiceResponse> AddLocation(LocationDto locationDto);
        Task<ServiceResponse> UpdateLocation(int id, LocationDto locationDto);
        Task<ServiceResponse> DeleteLocation(int id);

        // Related methods
        Task<IEnumerable<FoodTruckDto>> ListFoodTrucksByLocation(int locationId);
    }
}
