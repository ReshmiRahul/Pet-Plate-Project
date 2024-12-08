using PetAdoption.Interfaces;
using PetAdoption.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using PetAdoption.Services;
using PetAdoption.Models.ViewModels;

namespace PetAdoption.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;

        // dependency injection of service interfaces
        public AccountController(IAccountService AccountService)
        {
            _accountService = AccountService;
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

            // Redirect to Profile page after login
            return RedirectToAction("GetProfile");
        }

        [HttpGet("Profile")]
        public async Task<IActionResult> GetProfile()
        {
            // Retrieve AccountId from session
            var accountId = HttpContext.Session.GetInt32("AccountId");

            if (accountId == null)
            {
                // If no AccountId in session, redirect to login
                return RedirectToAction("Login");
            }

            // Get the account details using the AccountId
            var account = await _accountService.FindAccount(accountId.Value);

            if (account == null)
            {
                return NotFound(new { Message = "Account not found" });
            }

            // Return the profile view with account details
            return View("Profile", account);  // Make sure you have a Profile.cshtml view in the correct location
        }

        /// <summary>
        /// Returns a list of Accounts
        /// </summary>
        /// <returns>
        /// 200 OK
        /// [{AccountDto},{AccountDto},..]
        /// </returns>
        /// <example>
        /// GET: api/Account/List -> [{AccountDto},{AccountDto},..]
        /// </example>
        [HttpGet(template: "List")]
        public async Task<ActionResult<IEnumerable<AccountDto>>> ListAccounts()
        {
            // empty list of data transfer object AccountDto
            IEnumerable<AccountDto> AccountDtos = await _accountService.ListAccounts();
            // return 200 OK with AccountDtos
            return Ok(AccountDtos);
        }

        /// <summary>
        /// Returns a single Account specified by its {id}
        /// </summary>
        /// <param name="id">The Account id</param>
        /// <returns>
        /// 200 OK
        /// {AccountDto}
        /// or
        /// 404 Not Found
        /// </returns>
        /// <example>
        /// GET: api/Account/Find/1 -> {AccountDto}
        /// </example>
        [HttpGet(template: "Find/{id}")]
        public async Task<ActionResult<AccountDto>> FindAccount(int id)
        {

            var Account = await _accountService.FindAccount(id);

            // if the Account could not be located, return 404 Not Found
            if (Account == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(Account);
            }
        }

        /// <summary>
        /// Updates a Account
        /// </summary>
        /// <param name="id">The ID of the Account to update</param>
        /// <param name="AccountDto">The required information to update the Account (AccountName, AccountColor)</param>
        /// <returns>
        /// 400 Bad Request
        /// or
        /// 404 Not Found
        /// or
        /// 204 No Content
        /// </returns>
        /// <example>
        /// PUT: api/Account/Update/5
        /// Request Headers: Content-Type: application/json
        /// Request Body: {AccountDto}
        /// ->
        /// Response Code: 204 No Content
        /// </example>
        [HttpPut("Update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] AccountDto accountDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _accountService.UpdateAccount(id, accountDto);
            if (response.Status == ServiceResponse.ServiceStatus.Updated)
            {
                return Ok(new { Message = "Account updated successfully" });
            }
            else
            {
                return BadRequest(new { Message = "Failed to update Account", Errors = response.Messages });
            }
        }

        /// <summary>
        /// Adds a Account
        /// </summary>
        /// <param name="AccountDto">The required information to add the Account (AccountName, AccountColor)</param>
        /// <returns>
        /// 201 Created
        /// Location: api/Account/Find/{AccountId}
        /// {AccountDto}
        /// or
        /// 404 Not Found
        /// </returns>
        /// <example>
        /// POST: api/Account/Add
        /// Request Headers: Content-Type: application/json
        /// Request Body: {AccountDto}
        /// ->
        /// Response Code: 201 Created
        /// Response Headers: Location: api/Account/Find/{AccountId}
        /// </example>
        [HttpPost("Add")]
        public async Task<ActionResult<Account>> AddAccount(AccountDto accountDto)
        {
            // Call the service method to add the account
            ServiceResponse response = await _accountService.AddAccount(accountDto);

            // Handle the case when the account is not found or there's an error
            if (response.Status == ServiceResponse.ServiceStatus.NotFound)
            {
                return NotFound(response.Messages); // Return 404 if not found
            }
            else if (response.Status == ServiceResponse.ServiceStatus.Error)
            {
                return StatusCode(500, response.Messages); // Return 500 for error in the service
            }

            // If successful, return a 201 Created status with the location of the new resource
            return CreatedAtAction(
                nameof(FindAccount), // Action method name to retrieve the created resource (adjust to your action)
                new { id = response.CreatedId }, // Route parameters (e.g., account ID)
                accountDto // The body of the response contains the created AccountDto
            );
        }


        /// <summary>
        /// Deletes the Account
        /// </summary>
        /// <param name="id">The id of the Account to delete</param>
        /// <returns>
        /// 204 No Content
        /// or
        /// 404 Not Found
        /// </returns>
        /// <example>
        /// DELETE: api/Account/Delete/7
        /// ->
        /// Response Code: 204 No Content
        /// </example>
        [HttpDelete("Delete/{id}")]
        public async Task<ActionResult> DeleteAccount(int id)
        {
            ServiceResponse response = await _accountService.DeleteAccount(id);

            if (response.Status == ServiceResponse.ServiceStatus.NotFound)
            {
                return NotFound();
            }
            else if (response.Status == ServiceResponse.ServiceStatus.Error)
            {
                return StatusCode(500, response.Messages);
            }

            return NoContent();

        }
    }
}
