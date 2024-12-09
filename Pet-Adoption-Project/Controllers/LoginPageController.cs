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

            // Check if admin credentials are provided
            if (model.Username == "admin" && model.Password == "admin")
            {
                // Optionally, store a session variable indicating admin login
                HttpContext.Session.SetString("UserRole", "Admin");

                // Redirect to Home/Index for admin
                return RedirectToAction("Index", "Home");
            }

            // Regular user login
            var account = await _accountService.Login(model.Username, model.Password);
            if (account == null)
            {
                ModelState.AddModelError("", "Invalid username or password");
                return View(model);
            }

            // Store AccountId in session for regular users
            HttpContext.Session.SetInt32("AccountId", account.AccountId);
            HttpContext.Session.SetString("UserRole", "User");

            // Redirect to Profile for regular users
            return RedirectToAction("Profile", "AccountPage");
        }

    }
}
