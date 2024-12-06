using Microsoft.AspNetCore.Mvc;
using PetAdoption.Interfaces;
using PetAdoption.Models;
using PetAdoption.Models.ViewModels;
using ErrorViewModel = PetAdoption.Models.ViewModels.ErrorViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;
using PetAdoption.Services;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace PetAdoption.Controllers
{
    public class MenuItemPageController : Controller
    {
        private readonly IMenuItemService _menuItemService;
        private readonly IFoodTruckService _foodTruckService;

        // Dependency injection of the MenuItem service
        public MenuItemPageController(IMenuItemService menuItemService, IFoodTruckService foodTruckService)
        {
            _menuItemService = menuItemService;
            _foodTruckService = foodTruckService;
        }

        // Default action: redirect to list of menu items
        public IActionResult Index()
        {
            return RedirectToAction("List");
        }

        // GET: MenuItemPage/List
        public async Task<IActionResult> List()
        {
            IEnumerable<MenuItemDto?> menuItemDtos = await _menuItemService.ListMenuItems();
            return View(menuItemDtos);
        }

        // GET: MenuItemPage/Details/{id}
        // GET: MenuItemPage/Details/{id}
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            // Fetch the menu item details using the service layer
            var menuItemDto = await _menuItemService.FindMenuItem(id);

            if (menuItemDto == null)
            {
                return View("Error", new ErrorViewModel
                {
                    Errors = new List<string> { "Could not find menu item" }
                });
            }

            // Fetch associated food truck details
            var foodTruckDto = await _foodTruckService.FindFoodTruck(menuItemDto.FoodTruckId);

            // Create the view model and pass it to the view
            var menuItemDetails = new MenuItemDetails
            {
                MenuItem = menuItemDto,
                AssociatedFoodTruck = foodTruckDto
            };

            return View(menuItemDetails);
        }

        // GET MenuItemPage/New
        public async Task<IActionResult> New()
        {
            var foodTrucks = await _foodTruckService.ListFoodTrucks(); // Fetch foodTrucks
            ViewBag.FoodTrucks = foodTrucks.Select(foodTruck => new SelectListItem
            {
                Value = foodTruck.FoodTruckId.ToString(),
                Text = foodTruck.Name.ToString(),
            }).ToList();

            return View(new MenuItemDto()); // Return an empty DTO for the form
        }

        // POST MenuItemPage/Add
        [HttpPost]
        public async Task<IActionResult> Add(MenuItemDto menuItemDto)
        {
            if (ModelState.IsValid)
            {
                ServiceResponse response = await _menuItemService.AddMenuItem(menuItemDto);

                if (response.Status == ServiceResponse.ServiceStatus.Created)
                {
                    return RedirectToAction("Details", "MenuItemPage", new { id = response.CreatedId });
                }
                else
                {
                    return View("Error", new ErrorViewModel() { Errors = response.Messages });
                }
            }
            else
            {
                return View(menuItemDto);
            }
        }

        // GET MenuItemPage/Edit/{id}
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            MenuItemDto? menuItemDto = await _menuItemService.FindMenuItem(id);
            if (menuItemDto == null)
            {
                return View("Error");
            }
            else
            {
                return View(menuItemDto);
            }
        }

        // POST MenuItemPage/Update/{id}
        [HttpPost]
        public async Task<IActionResult> Update(int id, MenuItemDto menuItemDto)
        {
            ServiceResponse response = await _menuItemService.UpdateMenuItem(menuItemDto);

            if (response.Status == ServiceResponse.ServiceStatus.Updated)
            {
                return RedirectToAction("Details", "MenuItemPage", new { id = id });
            }
            else
            {
                return View("Error", new ErrorViewModel() { Errors = response.Messages });
            }
        }

        // GET MenuItemPage/ConfirmDelete/{id}
        [HttpGet]
        public async Task<IActionResult> ConfirmDelete(int id)
        {
            MenuItemDto? menuItemDto = await _menuItemService.FindMenuItem(id);
            if (menuItemDto == null)
            {
                return View("Error");
            }
            else
            {
                return View(menuItemDto);
            }
        }

        // POST MenuItemPage/Delete/{id}
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            ServiceResponse response = await _menuItemService.DeleteMenuItem(id);

            if (response.Status == ServiceResponse.ServiceStatus.Deleted)
            {
                return RedirectToAction("List", "MenuItemPage");
            }
            else
            {
                return View("Error", new ErrorViewModel() { Errors = response.Messages });
            }
        }
    }
}