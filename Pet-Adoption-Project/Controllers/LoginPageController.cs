using Microsoft.AspNetCore.Mvc;
using PetAdoption.Models.ViewModels;
using PetAdoption.Interfaces;

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
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Validate user credentials (now using plain password)
                bool isAuthenticated = await _accountService.ValidateUserAsync(model.Username, model.Password);

                if (isAuthenticated)
                {
                    // Proceed with authentication
                    Console.WriteLine("succes");
                    return RedirectToAction("List", "AccountPage");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid username or password");
                }
            }

            return View(model);
        }

    }
}
