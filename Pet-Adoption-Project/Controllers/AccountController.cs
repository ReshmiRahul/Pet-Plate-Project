using PetAdoption.Interfaces;
using PetAdoption.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace PetAdoption.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(string username, string password)
        {
            var account = await _accountService.Login(username, password);
            if (account == null)
            {
                return Unauthorized(new { Message = "Invalid credentials" });
            }

            // Store AccountId in session
            HttpContext.Session.SetInt32("AccountId", account.AccountId);

            // Redirect to Profile page
            return RedirectToAction("Profile", "AccountPage");
        }

        [HttpGet("List")]
        public async Task<ActionResult<IEnumerable<AccountDto>>> ListAccounts()
        {
            var accounts = await _accountService.ListAccounts();
            return Ok(accounts);
        }

        [HttpGet("Find/{id}")]
        public async Task<ActionResult<AccountDto>> FindAccount(int id)
        {
            var account = await _accountService.FindAccount(id);
            return account == null ? NotFound() : Ok(account);
        }

        [HttpPost("Add")]
        public async Task<ActionResult> AddAccount(AccountDto accountDto)
        {
            var response = await _accountService.AddAccount(accountDto);

            if (response.Status == ServiceResponse.ServiceStatus.Created)
            {
                return CreatedAtAction(nameof(FindAccount), new { id = response.CreatedId }, accountDto);
            }

            return BadRequest(response.Messages);
        }

        [HttpPut("Update/{id}")]
        public async Task<IActionResult> UpdateAccount(int id, AccountDto accountDto)
        {
            var response = await _accountService.UpdateAccount(id, accountDto);
            if (response.Status == ServiceResponse.ServiceStatus.Updated)
            {
                return NoContent();
            }

            return BadRequest(response.Messages);
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteAccount(int id)
        {
            var response = await _accountService.DeleteAccount(id);

            if (response.Status == ServiceResponse.ServiceStatus.Deleted)
            {
                return NoContent();
            }

            return NotFound();
        }
    }
}
