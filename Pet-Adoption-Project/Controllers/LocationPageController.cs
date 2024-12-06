using Microsoft.AspNetCore.Mvc;
using PetAdoption.Interfaces;
using PetAdoption.Models;
using PetAdoption.Models.ViewModels;
using PetAdoption.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using ErrorViewModel = PetAdoption.Models.ViewModels.ErrorViewModel;


namespace PetAdoption.Controllers
{
    public class LocationPageController : Controller
    {
        private readonly ILocationService _locationService;

        // Dependency injection of the Location service
        public LocationPageController(ILocationService locationService)
        {
            _locationService = locationService;
        }

        // Default action: redirect to list of locations
        public IActionResult Index()
        {
            return RedirectToAction("List");
        }

        // GET: LocationPage/List
        public async Task<IActionResult> List()
        {
            IEnumerable<LocationDto?> locationDtos = await _locationService.ListLocations();
            return View(locationDtos);
        }

        // GET: Location/Details/{id}
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            // Fetch the location details
            LocationDto? locationDto = await _locationService.FindLocation(id);

            // Fetch associated entities (if applicable, e.g., food trucks or pets)
            IEnumerable<FoodTruckDto> associatedFoodTrucks = await _locationService.ListFoodTrucksByLocation(id);

            // Handle null case for the primary object
            if (locationDto == null)
            {
                return View("Error", new ErrorViewModel
                {
                    Errors = new List<string> { "Could not find location" }
                });
            }

            // Create the view model and pass it to the view
            LocationDetails locationInfo = new LocationDetails
            {
                Location = locationDto,
                AssociatedFoodTrucks = associatedFoodTrucks
            };

            return View(locationInfo);
        }


        // GET LocationPage/New
        public ActionResult New()
        {
            return View();
        }

        // POST LocationPage/Add
        [HttpPost]
        public async Task<IActionResult> Add(LocationDto locationDto)
        {
            ServiceResponse response = await _locationService.AddLocation(locationDto);

            if (response.Status == ServiceResponse.ServiceStatus.Created)
            {
                return RedirectToAction("Details", "LocationPage", new { id = response.CreatedId });
            }
            else
            {
                return View("Error", new ErrorViewModel() { Errors = response.Messages });
            }
        }

        // GET LocationPage/Edit/{id}
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            LocationDto? locationDto = await _locationService.FindLocation(id);
            if (locationDto == null)
            {
                return View("Error");
            }
            else
            {
                return View(locationDto);
            }
        }

        // POST LocationPage/Update/{id}
        [HttpPost]
        public async Task<IActionResult> Update(int id, LocationDto locationDto)
        {
            ServiceResponse response = await _locationService.UpdateLocation(id, locationDto);

            if (response.Status == ServiceResponse.ServiceStatus.Updated)
            {
                return RedirectToAction("Details", "LocationPage", new { id = id });
            }
            else
            {
                return View("Error", new ErrorViewModel() { Errors = response.Messages });
            }
        }

        // GET LocationPage/ConfirmDelete/{id}
        [HttpGet]
        public async Task<IActionResult> ConfirmDelete(int id)
        {
            LocationDto? locationDto = await _locationService.FindLocation(id);
            if (locationDto == null)
            {
                return View("Error");
            }
            else
            {
                return View(locationDto);
            }
        }

        // POST LocationPage/Delete/{id}
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            ServiceResponse response = await _locationService.DeleteLocation(id);

            if (response.Status == ServiceResponse.ServiceStatus.Deleted)
            {
                return RedirectToAction("List", "LocationPage");
            }
            else
            {
                return View("Error", new ErrorViewModel() { Errors = response.Messages });
            }
        }
    }
}
