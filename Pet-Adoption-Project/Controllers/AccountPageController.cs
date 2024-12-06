using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        // Dependency injection of service interfaces
        public AccountPageController(IAccountService accountService, IPetService petService)
        {
            _accountService = accountService;
            _petService = petService;
        }

        // Default action: redirect to list of accounts
        public IActionResult Index()
        {
            return RedirectToAction("List");
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
        public ActionResult New()
        {
            return View();
        }

        // POST AccountPage/Add
        [HttpPost]
        public async Task<IActionResult> Add(AccountDto accountDto)
        {
            ServiceResponse response = await _accountService.AddAccount(accountDto);

            if (response.Status == ServiceResponse.ServiceStatus.Created)
            {
                return RedirectToAction("Details", "AccountPage", new { id = response.CreatedId });
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
            ServiceResponse response = await _accountService.UpdateAccount(accountDto);

            if (response.Status == ServiceResponse.ServiceStatus.Updated)
            {
                return RedirectToAction("Details", "AccountPage", new { id = id });
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
