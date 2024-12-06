using Microsoft.EntityFrameworkCore;
using Pet_Adoption_Project.Data;
using PetAdoption.Interfaces;
using PetAdoption.Models;

namespace PetAdoption.Services
{
    public class LocationService : ILocationService
    {
        private readonly ApplicationDbContext _context;

        public LocationService(ApplicationDbContext context)
        {
            _context = context;
        }

        // List all locations
        public async Task<IEnumerable<LocationDto>> ListLocations()
        {
            var locations = await _context.Locations.ToListAsync();

            return locations.Select(location => new LocationDto
            {
                LocationId = location.LocationId,
                Address = location.Address,
                City = location.City,
                State = location.State
            });
        }

        // Find a location by ID
        public async Task<LocationDto?> FindLocation(int id)
        {
            var location = await _context.Locations
                .FirstOrDefaultAsync(l => l.LocationId == id);

            if (location == null) return null;

            return new LocationDto
            {
                LocationId = location.LocationId,
                Address = location.Address,
                City = location.City,
                State = location.State
            };
        }

        // Add a new location
        public async Task<ServiceResponse> AddLocation(LocationDto locationDto)
        {
            ServiceResponse response = new();

            var location = new Location
            {
                Address = locationDto.Address,
                City = locationDto.City,
                State = locationDto.State
            };

            try
            {
                _context.Locations.Add(location);
                await _context.SaveChangesAsync();

                response.Status = ServiceResponse.ServiceStatus.Created;
                response.CreatedId = location.LocationId;
            }
            catch (Exception ex)
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("Error adding location.");
                response.Messages.Add(ex.Message);
            }

            return response;
        }

        // Update an existing location
        public async Task<ServiceResponse> UpdateLocation(LocationDto locationDto)
        {
            ServiceResponse response = new();

            var location = await _context.Locations.FindAsync(locationDto.LocationId);
            if (location == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                response.Messages.Add("Location not found.");
                return response;
            }

            location.Address = locationDto.Address;
            location.City = locationDto.City;
            location.State = locationDto.State;

            try
            {
                _context.Entry(location).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                response.Status = ServiceResponse.ServiceStatus.Updated;
            }
            catch (DbUpdateConcurrencyException)
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("Error updating location.");
            }

            return response;
        }

        // Delete a location
        public async Task<ServiceResponse> DeleteLocation(int id)
        {
            ServiceResponse response = new();

            var location = await _context.Locations.FindAsync(id);
            if (location == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                response.Messages.Add("Location not found.");
                return response;
            }

            try
            {
                _context.Locations.Remove(location);
                await _context.SaveChangesAsync();

                response.Status = ServiceResponse.ServiceStatus.Deleted;
            }
            catch (Exception ex)
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("Error deleting location.");
                response.Messages.Add(ex.Message);
            }

            return response;
        }

        // Assign a food truck to a location
        public async Task<ServiceResponse> AssignFoodTruckToLocation(int locationId, int foodTruckId)
        {
            ServiceResponse response = new();

            var location = await _context.Locations.FindAsync(locationId);
            var foodTruck = await _context.FoodTrucks.FindAsync(foodTruckId);

            if (location == null || foodTruck == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                if (location == null) response.Messages.Add("Location not found.");
                if (foodTruck == null) response.Messages.Add("Food truck not found.");
                return response;
            }

            foodTruck.LocationId = locationId;

            try
            {
                _context.Entry(foodTruck).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                response.Status = ServiceResponse.ServiceStatus.Updated;
            }
            catch (Exception ex)
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("Error assigning food truck to location.");
                response.Messages.Add(ex.Message);
            }

            return response;
        }

        // Remove a food truck from a location
        public async Task<ServiceResponse> RemoveFoodTruckFromLocation(int locationId, int foodTruckId)
        {
            ServiceResponse response = new();

            var location = await _context.Locations.FindAsync(locationId);
            var foodTruck = await _context.FoodTrucks.FindAsync(foodTruckId);

            if (location == null || foodTruck == null || foodTruck.LocationId != locationId)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                response.Messages.Add("Food truck or location not found, or food truck is not assigned to the specified location.");
                return response;
            }

            foodTruck.LocationId = 0; // Unassign the food truck

            try
            {
                _context.Entry(foodTruck).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                response.Status = ServiceResponse.ServiceStatus.Updated;
            }
            catch (Exception ex)
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("Error removing food truck from location.");
                response.Messages.Add(ex.Message);
            }

            return response;
        }
        

        public async Task<IEnumerable<FoodTruckDto>> ListFoodTrucksByLocation(int locationId)
        {
            var foodTrucks = await _context.FoodTrucks
                .Where(ft => ft.LocationId == locationId)
                .ToListAsync();

            return foodTrucks.Select(ft => new FoodTruckDto
            {
                FoodTruckId = ft.FoodTruckId,
                Name = ft.Name,
                LocationId = ft.LocationId
            });
        }

        // Implementation of UpdateLocation
        public async Task<ServiceResponse> UpdateLocation(int id, LocationDto locationDto)
        {
            var response = new ServiceResponse();

            // Find the location by its ID
            var location = await _context.Locations.FirstOrDefaultAsync(l => l.LocationId == id);
            if (location == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                response.Messages.Add("Location not found");
                return response;
            }

            // Map the updated values from LocationDto to the existing Location
            location.Address = locationDto.Address;
            location.City = locationDto.City;
            location.State = locationDto.State;

            // Save the changes to the database
            try
            {
                await _context.SaveChangesAsync();
                response.Status = ServiceResponse.ServiceStatus.Updated;
                response.Messages.Add("Location updated successfully");
            }
            catch (Exception ex)
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("An error occurred while updating the location: " + ex.Message);
            }

            return response;
        }
    }
}
