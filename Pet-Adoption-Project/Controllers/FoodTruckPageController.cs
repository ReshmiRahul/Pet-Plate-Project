using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PetAdoption.Interfaces;
using PetAdoption.Models;
using PetAdoption.Models.ViewModels;
using PetAdoption.Services;
using ErrorViewModel = PetAdoption.Models.ViewModels.ErrorViewModel;

namespace PetAdoption.Controllers
{
    public class FoodTruckPageController : Controller
    {
        private readonly IFoodTruckService _foodTruckService;
        private readonly IMenuItemService _menuItemService;
        private readonly ILocationService _locationService;

        // Dependency injection of the FoodTruck service
        public FoodTruckPageController(IFoodTruckService foodTruckService, IMenuItemService menuItemService, ILocationService locationService)
        {
            _foodTruckService = foodTruckService;
            _menuItemService = menuItemService;
            _locationService = locationService;
        }

        // Default action: redirect to list of food trucks
        public IActionResult Index()
        {
            return RedirectToAction("List");
        }

        // GET: FoodTruckPage/List
        public async Task<IActionResult> List()
        {
            IEnumerable<FoodTruckDto?> foodTruckDtos = await _foodTruckService.ListFoodTrucks();
            return View(foodTruckDtos);
        }

        // GET: FoodTruckPage/Details/{id}
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            // Fetch the food truck details using the service layer
            FoodTruckDto? foodTruckDto = await _foodTruckService.FindFoodTruck(id);

            // Fetch associated menu items (or other related entities)
            IEnumerable<MenuItemDto> associatedMenuItems = await _menuItemService.ListMenuItems();

            // Handle null case for the primary object
            if (foodTruckDto == null)
            {
                return View("Error", new ErrorViewModel
                {
                    Errors = new List<string> { "Could not find food truck" }
                });
            }

            // Create the view model and pass it to the view
            FoodTruckDetails foodTruckInfo = new FoodTruckDetails
            {
                FoodTruck = foodTruckDto,
                MenuItems = associatedMenuItems
            };

            return View(foodTruckInfo); // Make sure the correct model is passed here
        }



        // POST FoodTruckPage/Add
        [HttpPost]
        public async Task<IActionResult> Add(FoodTruckDto foodTruckDto)
        {
            ServiceResponse response = await _foodTruckService.AddFoodTruck(foodTruckDto);

            if (response.Status == ServiceResponse.ServiceStatus.Created)
            {
                return RedirectToAction("Details", "FoodTruckPage", new { id = response.CreatedId });
            }
            else
            {
                return View("Error", new ErrorViewModel() { Errors = response.Messages });
            }
        }


        // GET FoodTruckPage/Edit/{id}
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            FoodTruckDto? foodTruckDto = await _foodTruckService.FindFoodTruck(id);
            if (foodTruckDto == null)
            {
                return View("Error");
            }
            else
            {
                return View(foodTruckDto);
            }
        }

        // POST FoodTruckPage/Update/{id}
        [HttpPost]
        public async Task<IActionResult> Update(int id, FoodTruckDto foodTruckDto)
        {
            ServiceResponse response = await _foodTruckService.UpdateFoodTruck(id, foodTruckDto);

            if (response.Status == ServiceResponse.ServiceStatus.Updated)
            {
                return RedirectToAction("Details", "FoodTruckPage", new { id = id });
            }
            else
            {
                return View("Error", new ErrorViewModel() { Errors = response.Messages });
            }
        }


        // GET FoodTruckPage/ConfirmDelete/{id}
        [HttpGet]
        public async Task<IActionResult> ConfirmDelete(int id)
        {
            FoodTruckDto? foodTruckDto = await _foodTruckService.FindFoodTruck(id);
            if (foodTruckDto == null)
            {
                return View("Error");
            }
            else
            {
                return View(foodTruckDto);
            }
        }

        // POST FoodTruckPage/Delete/{id}
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            ServiceResponse response = await _foodTruckService.DeleteFoodTruck(id);

            if (response.Status == ServiceResponse.ServiceStatus.Deleted)
            {
                return RedirectToAction("List", "FoodTruckPage");
            }
            else
            {
                return View("Error", new ErrorViewModel() { Errors = response.Messages });
            }
        }
        public async Task<IActionResult> New()
        {
            var locations = await _locationService.ListLocations(); // Fetch locations
            ViewBag.Locations = locations.Select(location => new SelectListItem
            {
                Value = location.LocationId.ToString(),
                Text = location.City.ToString(),
            }).ToList();

            return View(new FoodTruckDto()); // Return an empty DTO for the form
        }


    }
}
