using PetAdoption.Interfaces;
using PetAdoption.Models;
using Microsoft.EntityFrameworkCore;
using Pet_Adoption_Project.Data;

namespace PetAdoption.Services
{
    public class FoodTruckService : IFoodTruckService
    {
        private readonly ApplicationDbContext _context;

        public FoodTruckService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<FoodTruckDto>> ListFoodTrucks()
        {
            var foodTrucks = await _context.FoodTrucks
                .Include(ft => ft.Location)
                .ToListAsync();

            return foodTrucks.Select(ft => new FoodTruckDto
            {
                FoodTruckId = ft.FoodTruckId,
                Name = ft.Name,
                Description = ft.Description,
                Contact = ft.Contact,
                LocationId = ft.LocationId,
                LocationAddress = ft.Location?.Address
            });
        }

        public async Task<FoodTruckDto?> FindFoodTruck(int id)
        {
            var foodTruck = await _context.FoodTrucks
                .Include(ft => ft.Location)
                .FirstOrDefaultAsync(ft => ft.FoodTruckId == id);

            if (foodTruck == null) return null;

            return new FoodTruckDto
            {
                FoodTruckId = foodTruck.FoodTruckId,
                Name = foodTruck.Name,
                Description = foodTruck.Description,
                Contact = foodTruck.Contact,
                LocationId = foodTruck.LocationId,
                LocationAddress = foodTruck.Location?.Address
            };
        }

        public async Task<ServiceResponse> AddFoodTruck(FoodTruckDto foodTruckDto)
        {
            ServiceResponse response = new();

            var foodTruck = new FoodTruck
            {
                Name = foodTruckDto.Name,
                Description = foodTruckDto.Description,
                Contact = foodTruckDto.Contact,
                LocationId = foodTruckDto.LocationId
            };

            try
            {
                _context.FoodTrucks.Add(foodTruck);
                await _context.SaveChangesAsync();

                response.Status = ServiceResponse.ServiceStatus.Created;
                response.CreatedId = foodTruck.FoodTruckId;
            }
            catch (Exception ex)
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("Error adding food truck.");
                response.Messages.Add(ex.Message);
            }

            return response;
        }

        public async Task<ServiceResponse> UpdateFoodTruck(FoodTruckDto foodTruckDto)
        {
            ServiceResponse response = new();

            var foodTruck = await _context.FoodTrucks.FindAsync(foodTruckDto.FoodTruckId);
            if (foodTruck == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                response.Messages.Add("Food truck not found.");
                return response;
            }

            foodTruck.Name = foodTruckDto.Name;
            foodTruck.Description = foodTruckDto.Description;
            foodTruck.Contact = foodTruckDto.Contact;
            foodTruck.LocationId = foodTruckDto.LocationId;

            try
            {
                _context.Entry(foodTruck).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                response.Status = ServiceResponse.ServiceStatus.Updated;
            }
            catch (DbUpdateConcurrencyException)
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("Error updating food truck.");
            }

            return response;
        }

        public async Task<ServiceResponse> DeleteFoodTruck(int id)
        {
            ServiceResponse response = new();

            var foodTruck = await _context.FoodTrucks.FindAsync(id);
            if (foodTruck == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                response.Messages.Add("Food truck not found.");
                return response;
            }

            try
            {
                _context.FoodTrucks.Remove(foodTruck);
                await _context.SaveChangesAsync();

                response.Status = ServiceResponse.ServiceStatus.Deleted;
            }
            catch (Exception ex)
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("Error deleting food truck.");
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
                Description = ft.Description,
                Contact = ft.Contact,
                LocationId = ft.LocationId
            });
        }

        public async Task<ServiceResponse> AssignFoodTruckToLocation(int foodTruckId, int locationId)
        {
            ServiceResponse response = new();

            var foodTruck = await _context.FoodTrucks.FindAsync(foodTruckId);
            var location = await _context.Locations.FindAsync(locationId);

            if (foodTruck == null || location == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                if (foodTruck == null) response.Messages.Add("Food truck not found.");
                if (location == null) response.Messages.Add("Location not found.");
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

        public async Task<ServiceResponse> RemoveFoodTruckFromLocation(int foodTruckId, int locationId)
        {
            ServiceResponse response = new();

            var foodTruck = await _context.FoodTrucks.FindAsync(foodTruckId);

            if (foodTruck == null || foodTruck.LocationId != locationId)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                response.Messages.Add("Food truck not found or not associated with the specified location.");
                return response;
            }

            foodTruck.LocationId = 0; // Unassigning the location

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

        public async Task<ServiceResponse> UpdateFoodTruck(int id, FoodTruckDto foodTruckDto)
        {
            var response = new ServiceResponse();

            // Find the food truck by id
            var foodTruck = await _context.FoodTrucks
                .Include(ft => ft.Location)  // Include related entities if necessary
                .FirstOrDefaultAsync(ft => ft.FoodTruckId == id);

            if (foodTruck == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                response.Messages.Add("FoodTruck not found.");
                return response;
            }

            // Update the food truck properties with the data from the FoodTruckDto
            foodTruck.Name = foodTruckDto.Name;
            foodTruck.Description = foodTruckDto.Description;
            foodTruck.Contact = foodTruckDto.Contact;
            foodTruck.LocationId = foodTruckDto.LocationId;

            // Save changes to the database
            await _context.SaveChangesAsync();

            response.Status = ServiceResponse.ServiceStatus.Updated;
            return response;
        }
    }
}
