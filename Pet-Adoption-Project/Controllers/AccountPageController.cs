using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PetAdoption.Interfaces;
using PetAdoption.Models;
using PetAdoption.Models.ViewModels;
using ErrorViewModel = PetAdoption.Models.ViewModels.ErrorViewModel;

namespace PetAdoption.Controllers
{

  
    public class AccountPageController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly IPetService _petService;
        private readonly ILocationService _locationService;

        // Dependency injection of service interfaces
        public AccountPageController(IAccountService accountService, IPetService petService, ILocationService locationService)
        {
            _accountService = accountService;
            _petService = petService;
            _locationService = locationService;
        }

        // Default action: redirect to list of accounts
        public IActionResult Index()
        {
            return RedirectToAction("Profile");
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var userRole = HttpContext.Session.GetString("UserRole");

            if (userRole == "Admin")
            {
                // Redirect admin to a different page if necessary
                return RedirectToAction("Index", "Home");
            }

            var accountId = HttpContext.Session.GetInt32("AccountId");
            if (accountId == null)
            {
                return RedirectToAction("Index", "LoginPage");
            }

            var account = await _accountService.FindAccount(accountId.Value);
            if (account == null)
            {
                return View("Error", new ErrorViewModel { Errors = new List<string> { "Account not found" } });
            }

            return View(account);
        }




        // GET: AccountPage/List
        public async Task<IActionResult> List()
        {
            IEnumerable<AccountDto?> accountDtos = await _accountService.ListAccounts();
            return View(accountDtos);
        }

        // GET: AccountPage/Details/{id}
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            AccountDto? accountDto = await _accountService.FindAccount(id);
            IEnumerable<PetDto> associatedPets = await _petService.ListPetsForAccount(id);

            if (accountDto == null)
            {
                return View("Error", new ErrorViewModel() { Errors = new List<string> { "Could not find account" } });
            }
            else
            {
                // Account page details view model
                AccountDetails accountInfo = new AccountDetails()
                {
                    Account = accountDto,
                    AssociatedPets = associatedPets
                };
                return View(accountInfo);
            }
        }

        // GET AccountPage/New
        public async Task<IActionResult> New()
        {
            var locations = await _locationService.ListLocations(); // Fetch locations
            ViewBag.Locations = locations.Select(location => new SelectListItem
            {
                Value = location.LocationId.ToString(),
                Text = location.City.ToString(),
            }).ToList();

            return View(new AccountDto()); // Return an empty DTO for the form
        }
        public async Task<IActionResult> NewAccount()
        {
            var locations = await _locationService.ListLocations(); // Fetch locations
            ViewBag.Locations = locations.Select(location => new SelectListItem
            {
                Value = location.LocationId.ToString(),
                Text = location.City.ToString(),
            }).ToList();

            return View(new AccountDto()); // Return an empty DTO for the form
        }

        // POST AccountPage/Add
        [HttpPost]
        public async Task<IActionResult> Add(AccountDto accountDto)
        {
            ServiceResponse response = await _accountService.AddAccount(accountDto);

            if (response.Status == ServiceResponse.ServiceStatus.Created)
            {
                return RedirectToAction("Index", "LoginPage", new { id = response.CreatedId });
            }
            else
            {
                return View("Error", new ErrorViewModel() { Errors = response.Messages });
            }
        }

        // GET AccountPage/Edit/{id}
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            AccountDto? accountDto = await _accountService.FindAccount(id);
            if (accountDto == null)
            {
                return View("Error");
            }
            else
            {
                return View(accountDto);
            }
        }

        // POST AccountPage/Update/{id}
        [HttpPost]
        public async Task<IActionResult> Update(int id, AccountDto accountDto)
        {
            ServiceResponse response = await _accountService.UpdateAccount(id, accountDto);

            if (response.Status == ServiceResponse.ServiceStatus.Updated)
            {
                return RedirectToAction("Profile", "AccountPage", new { id = id });
            }
            else
            {
                return View("Error", new ErrorViewModel() { Errors = response.Messages });
            }
        }

        // GET AccountPage/ConfirmDelete/{id}
        [HttpGet]
        public async Task<IActionResult> ConfirmDelete(int id)
        {
            AccountDto? accountDto = await _accountService.FindAccount(id);
            if (accountDto == null)
            {
                return View("Error");
            }
            else
            {
                return View(accountDto);
            }
        }

        // POST AccountPage/Delete/{id}
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            ServiceResponse response = await _accountService.DeleteAccount(id);

            if (response.Status == ServiceResponse.ServiceStatus.Deleted)
            {
                return RedirectToAction("List", "AccountPage");
            }
            else
            {
                return View("Error", new ErrorViewModel() { Errors = response.Messages });
            }
        }
    }
}
