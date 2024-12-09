using Microsoft.AspNetCore.Mvc;
using PetAdoption.Interfaces;
using PetAdoption.Models.ViewModels;

namespace PetAdoption.Controllers
{
    public class LoginPageController : Controller
    {
        private readonly IAccountService _accountService;

        public LoginPageController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        public IActionResult Index()
        {
            return View(new LoginViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var account = await _accountService.Login(model.Username, model.Password);
            if (account == null)
            {
                ModelState.AddModelError("", "Invalid username or password");
                return View(model);
            }

            // Store AccountId in session
            HttpContext.Session.SetInt32("AccountId", account.AccountId);

            return RedirectToAction("Profile", "AccountPage");
        }
    }
}
