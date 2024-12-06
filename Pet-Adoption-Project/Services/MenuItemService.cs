using PetAdoption.Interfaces;
using PetAdoption.Models;
using Microsoft.EntityFrameworkCore;
using Pet_Adoption_Project.Data;

namespace PetAdoption.Services
{
    public class MenuItemService : IMenuItemService
    {
        private readonly ApplicationDbContext _context;

        // Dependency injection of the database context
        public MenuItemService(ApplicationDbContext context)
        {
            _context = context;
        }

        // List all MenuItems
        public async Task<IEnumerable<MenuItemDto>> ListMenuItems()
        {
            // Retrieve all MenuItems from the database
            List<MenuItem> menuItems = await _context.MenuItems.ToListAsync();

            // Create a list of MenuItemDto to return
            List<MenuItemDto> menuItemDtos = new List<MenuItemDto>();

            // Map each MenuItem to a MenuItemDto
            foreach (var menuItem in menuItems)
            {
                menuItemDtos.Add(new MenuItemDto()
                {
                    MenuItemId = menuItem.MenuItemId,
                    Name = menuItem.Name,
                    Price = menuItem.Price,
                    FoodTruckId = menuItem.FoodTruckId,
                });
            }

            return menuItemDtos;
        }

        // Find a specific MenuItem by ID
        public async Task<MenuItemDto?> FindMenuItem(int id)
        {
            var menuItem = await _context.MenuItems
                .Include(m => m.FoodTruck)  // Include related FoodTruck information
                .FirstOrDefaultAsync(m => m.MenuItemId == id);

            if (menuItem == null)
                return null;

            // Return the found MenuItem as a MenuItemDto
            return new MenuItemDto()
            {
                MenuItemId = menuItem.MenuItemId,
                Name = menuItem.Name,
                Price = menuItem.Price,
                FoodTruckId = menuItem.FoodTruckId
            };
        }

        // Update a MenuItem
        public async Task<ServiceResponse> UpdateMenuItem(MenuItemDto menuItemDto)
        {
            ServiceResponse serviceResponse = new();
            var menuItem = await _context.MenuItems.FindAsync(menuItemDto.MenuItemId);
            if (menuItem == null)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.NotFound;
                serviceResponse.Messages.Add("MenuItem not found.");
                return serviceResponse;
            }

            menuItem.Name = menuItemDto.Name;
            menuItem.Price = menuItemDto.Price;
            menuItem.FoodTruckId = menuItemDto.FoodTruckId;

            try
            {
                await _context.SaveChangesAsync();
                serviceResponse.Status = ServiceResponse.ServiceStatus.Updated;
            }
            catch (Exception ex)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.Error;
                serviceResponse.Messages.Add("Error updating MenuItem.");
                serviceResponse.Messages.Add(ex.Message);
            }

            return serviceResponse;
        }

        // Add a new MenuItem
        public async Task<ServiceResponse> AddMenuItem(MenuItemDto menuItemDto)
        {
            ServiceResponse serviceResponse = new();

            // Create a new MenuItem from the provided MenuItemDto
            var menuItem = new MenuItem()
            {
                Name = menuItemDto.Name,
                Price = menuItemDto.Price,
                FoodTruckId = menuItemDto.FoodTruckId
            };

            try
            {
                // Add the new MenuItem to the database
                _context.MenuItems.Add(menuItem);
                await _context.SaveChangesAsync();

                serviceResponse.Status = ServiceResponse.ServiceStatus.Created;
                serviceResponse.CreatedId = menuItem.MenuItemId;
            }
            catch (Exception ex)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.Error;
                serviceResponse.Messages.Add("There was an error adding the MenuItem.");
                serviceResponse.Messages.Add(ex.Message);
            }

            return serviceResponse;
        }

        // Delete a MenuItem
        public async Task<ServiceResponse> DeleteMenuItem(int id)
        {
            ServiceResponse serviceResponse = new();

            // Find the MenuItem to delete
            var menuItem = await _context.MenuItems.FindAsync(id);
            if (menuItem == null)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.NotFound;
                serviceResponse.Messages.Add("MenuItem not found.");
                return serviceResponse;
            }

            try
            {
                // Remove the MenuItem from the database
                _context.MenuItems.Remove(menuItem);
                await _context.SaveChangesAsync();

                serviceResponse.Status = ServiceResponse.ServiceStatus.Deleted;
            }
            catch (Exception ex)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.Error;
                serviceResponse.Messages.Add("Error encountered while deleting the MenuItem.");
                serviceResponse.Messages.Add(ex.Message);
            }

            return serviceResponse;
        }

        // List MenuItems for a specific FoodTruck
        public async Task<IEnumerable<MenuItemDto>> ListMenuItemsByFoodTruck(int foodTruckId)
        {
            // Find MenuItems linked to the FoodTruck
            List<MenuItem> menuItems = await _context.MenuItems
                .Where(m => m.FoodTruckId == foodTruckId)
                .ToListAsync();

            // Map the MenuItems to MenuItemDto
            List<MenuItemDto> menuItemDtos = menuItems.Select(menuItem => new MenuItemDto()
            {
                MenuItemId = menuItem.MenuItemId,
                Name = menuItem.Name,
                Price = menuItem.Price,
                FoodTruckId = menuItem.FoodTruckId
            }).ToList();

            return menuItemDtos;
        }


        public Task<IEnumerable<MenuItemDto>> ListMenuItemsByCategory(int categoryId)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse> LinkMenuItemToCategory(int menuItemId, int categoryId)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse> UnlinkMenuItemFromCategory(int menuItemId, int categoryId)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse> UpdateMenuItem(int id, MenuItemDto menuItemDto)
        {
            throw new NotImplementedException();
        }
    }
}
