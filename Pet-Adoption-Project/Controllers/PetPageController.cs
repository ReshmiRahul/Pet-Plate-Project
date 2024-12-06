using Microsoft.AspNetCore.Mvc;
using PetAdoption.Interfaces;
using PetAdoption.Models;
using PetAdoption.Models.ViewModels;
using System.Threading.Tasks;
using System.Collections.Generic;
using ErrorViewModel = PetAdoption.Models.ViewModels.ErrorViewModel;

namespace PetAdoption.Controllers
{
    public class PetPageController : Controller
    {
        private readonly IPetService _petService;

        // Dependency injection of the pet service interface
        public PetPageController(IPetService petService)
        {
            _petService = petService;
        }

        // Default action: redirect to the list of pets
        public IActionResult Index()
        {
            return RedirectToAction("List");
        }

        // GET: PetPage/List
        public async Task<IActionResult> List()
        {
            IEnumerable<PetDto> petDtos = await _petService.ListPets();
            return View(petDtos);
        }

        // GET: PetPage/Details/{id}
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            PetDto? petDto = await _petService.FindPet(id);

            if (petDto == null)
            {
                return View("Error", new ErrorViewModel { Errors = new List<string> { "Could not find pet" } });
            }
            else
            {
                // Create a new PetDetails object
                PetDetails petDetails = new PetDetails
                {
                    Pet = petDto,
                    AssociatedAccounts = null, // Populate with actual data if needed
                    PetApplications = null // Populate with actual data if needed
                };

                return View(petDetails);
            }
        }

        // GET: PetPage/New
        public ActionResult New()
        {
            return View();
        }

        // POST: PetPage/Add
        [HttpPost]
        public async Task<IActionResult> Add(PetDto petDto)
        {
            if (!ModelState.IsValid)
            {
                return View("New");
            }

            ServiceResponse response = await _petService.AddPet(petDto);

            if (response.Status == ServiceResponse.ServiceStatus.Created)
            {
                return RedirectToAction("Details", new { id = response.CreatedId });
            }
            else
            {
                return View("Error", new ErrorViewModel { Errors = response.Messages });
            }
        }

        // GET: PetPage/Edit/{id}
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            PetDto? petDto = await _petService.FindPet(id);

            if (petDto == null)
            {
                return View("Error");
            }
            else
            {
                return View(petDto);
            }
        }

        // POST: PetPage/Update/{id}
        [HttpPost]
        public async Task<IActionResult> Update(int id, PetDto petDto)
        {
            if (!ModelState.IsValid)
            {
                return View("Edit", petDto);
            }

            if (id != petDto.PetId)
            {
                return View("Error", new ErrorViewModel { Errors = new List<string> { "Pet ID mismatch" } });
            }

            ServiceResponse response = await _petService.UpdatePet(petDto);

            if (response.Status == ServiceResponse.ServiceStatus.Updated)
            {
                return RedirectToAction("Details", new { id = id });
            }
            else
            {
                return View("Error", new ErrorViewModel { Errors = response.Messages });
            }
        }

        // GET: PetPage/ConfirmDelete/{id}
        [HttpGet]
        public async Task<IActionResult> ConfirmDelete(int id)
        {
            PetDto? petDto = await _petService.FindPet(id);
            if (petDto == null)
            {
                return View("Error");
            }
            else
            {
                return View(petDto);
            }
        }

        // POST: PetPage/Delete/{id}
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            ServiceResponse response = await _petService.DeletePet(id);

            if (response.Status == ServiceResponse.ServiceStatus.Deleted)
            {
                return RedirectToAction("List");
            }
            else
            {
                return View("Error", new ErrorViewModel { Errors = response.Messages });
            }
        }
    }
}
