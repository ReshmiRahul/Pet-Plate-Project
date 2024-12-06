using PetAdoption.Models;

namespace PetAdoption.Interfaces
{
    public interface IMenuItemService
    {
        // Base CRUD operations

        /// <summary>
        /// Retrieves a list of all menu items.
        /// </summary>
        Task<IEnumerable<MenuItemDto>> ListMenuItems();

        /// <summary>
        /// Finds a specific menu item by its ID.
        /// </summary>
        Task<MenuItemDto?> FindMenuItem(int id);

        /// <summary>
        /// Updates a menu item with the provided data.
        /// </summary>
        Task<ServiceResponse> UpdateMenuItem(MenuItemDto menuItemDto);

        /// <summary>
        /// Adds a new menu item to the system.
        /// </summary>
        Task<ServiceResponse> AddMenuItem(MenuItemDto menuItemDto);

        /// <summary>
        /// Deletes a menu item by its ID.
        /// </summary>
        Task<ServiceResponse> DeleteMenuItem(int id);

        // Related methods (if applicable)

        /// <summary>
        /// Lists menu items associated with a specific category.
        /// </summary>
        Task<IEnumerable<MenuItemDto>> ListMenuItemsByCategory(int categoryId);

        /// <summary>
        /// Links a menu item to a category.
        /// </summary>
        Task<ServiceResponse> LinkMenuItemToCategory(int menuItemId, int categoryId);

        /// <summary>
        /// Unlinks a menu item from a category.
        /// </summary>
        Task<ServiceResponse> UnlinkMenuItemFromCategory(int menuItemId, int categoryId);
    }
}