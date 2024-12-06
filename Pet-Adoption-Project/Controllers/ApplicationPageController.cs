using Microsoft.AspNetCore.Mvc;
using PetAdoption.Interfaces;
using PetAdoption.Models.ViewModels;
using PetAdoption.Models;
using ErrorViewModel = PetAdoption.Models.ViewModels.ErrorViewModel;

[Route("ApplicationPage")] // Add this at the class level if not present
public class ApplicationPageController : Controller
{
    private readonly IApplicationService _applicationService;
    private readonly IAccountService _accountService;
    private readonly IPetService _petService;

    public ApplicationPageController(IApplicationService applicationService, IAccountService accountService, IPetService petService)
    {
        _applicationService = applicationService;
        _accountService = accountService;
        _petService = petService;
    }

    // Default action: redirect to list of applications
    [HttpGet("")] // Default to index route
    public IActionResult Index() => RedirectToAction("List");

    // GET: ApplicationPage/List
    [HttpGet("List")]
    public async Task<IActionResult> List()
    {
        var applicationDtos = await _applicationService.ListApplications();
        return View(applicationDtos);
    }

    // GET: ApplicationPage/Details/{id}
    [HttpGet("Details/{id}")]
    public async Task<IActionResult> Details(int id)
    {
        var applicationDto = await _applicationService.FindApplication(id);
        if (applicationDto == null)
        {
            return View("Error", new ErrorViewModel { Errors = new List<string> { "Application not found." } });
        }

        var accountDto = await _accountService.FindAccount(applicationDto.AccountId);
        var petDto = await _petService.FindPet(applicationDto.PetId);

        var applicationDetails = new ApplicationDetails
        {
            Application = applicationDto,
            Account = accountDto,
            Pet = petDto
        };

        return View(applicationDetails);
    }

    // GET: ApplicationPage/New
    [HttpGet("New")]
    public async Task<IActionResult> New()
    {
        ViewData["Accounts"] = await _accountService.ListAccounts();
        ViewData["Pets"] = await _petService.ListPets();
        return View();
    }

    // POST: ApplicationPage/Add
    [HttpPost("Add")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(ApplicationDto applicationDto)
    {
        if (!ModelState.IsValid)
        {
            ViewData["Accounts"] = await _accountService.ListAccounts();
            ViewData["Pets"] = await _petService.ListPets();
            return View(applicationDto);
        }

        applicationDto.ApplicationStatus = 0; // Pending status
        applicationDto.ApplicationDate = DateTime.UtcNow;

        var response = await _applicationService.AddApplication(applicationDto);
        if (response.Status == ServiceResponse.ServiceStatus.Created)
        {
            return RedirectToAction("Details", new { id = response.CreatedId });
        }

        return View("Error", new ErrorViewModel { Errors = response.Messages });
    }

    // GET: ApplicationPage/Edit/{id}
    [HttpGet("Edit/{id}")]
    public async Task<IActionResult> Edit(int id)
    {
        var applicationDto = await _applicationService.FindApplication(id);
        if (applicationDto == null)
        {
            return View("Error", new ErrorViewModel { Errors = new List<string> { "Application not found." } });
        }

        return View(applicationDto);
    }

    // POST: ApplicationPage/Update/{id}
    [HttpPost("Update/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(int id, ApplicationDto applicationDto)
    {
        if (id != applicationDto.ApplicationID)
        {
            return BadRequest("Application ID mismatch.");
        }

        if (!ModelState.IsValid)
        {
            return View(applicationDto); // Return with validation errors
        }

        var existingApplication = await _applicationService.FindApplication(id);
        if (existingApplication == null)
        {
            return View("Error", new ErrorViewModel { Errors = new List<string> { "Application not found for update." } });
        }

        existingApplication.ApplicationStatus = applicationDto.ApplicationStatus;

        var response = await _applicationService.UpdateApplication(existingApplication);
        if (response.Status == ServiceResponse.ServiceStatus.NotFound)
        {
            return View("Error", new ErrorViewModel { Errors = response.Messages });
        }

        return RedirectToAction("Details", new { id = existingApplication.ApplicationID });
    }

    // GET: ApplicationPage/ConfirmDelete/{id}
    [HttpGet("ConfirmDelete/{id}")]
    public async Task<IActionResult> ConfirmDelete(int id)
    {
        var applicationDto = await _applicationService.FindApplication(id);
        if (applicationDto == null)
        {
            return View("Error", new ErrorViewModel { Errors = new List<string> { "Application not found." } });
        }

        return View(applicationDto); // Show a confirmation page with application details
    }

    // POST: ApplicationPage/Delete/{id}
    [HttpPost("Delete/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var response = await _applicationService.DeleteApplication(id);

        if (response.Status == ServiceResponse.ServiceStatus.Deleted)
        {
            return RedirectToAction("List"); // Redirect to the List page after successful deletion
        }

        return View("Error", new ErrorViewModel { Errors = response.Messages });
    }
}
