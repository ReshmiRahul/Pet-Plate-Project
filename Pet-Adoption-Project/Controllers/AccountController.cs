using PetAdoption.Interfaces;
using PetAdoption.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace PetAdoption.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        // dependency injection of service interfaces
        public AccountController(IAccountService AccountService)
        {
            _accountService = AccountService;
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
        [HttpPut(template: "Update/{id}")]
        public async Task<ActionResult> UpdateAccount(int id, AccountDto AccountDto)
        {
            // {id} in URL must match AccountId in POST Body
            if (id != AccountDto.AccountId)
            {
                //400 Bad Request
                return BadRequest();
            }

            ServiceResponse response = await _accountService.UpdateAccount(AccountDto);

            if (response.Status == ServiceResponse.ServiceStatus.NotFound)
            {
                return NotFound(response.Messages);
            }
            else if (response.Status == ServiceResponse.ServiceStatus.Error)
            {
                return StatusCode(500, response.Messages);
            }

            //Status = Updated
            return NoContent();

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
        [HttpPost(template: "Add")]
        public async Task<ActionResult<Account>> AddAccount(AccountDto AccountDto)
        {
            ServiceResponse response = await _accountService.AddAccount(AccountDto);

            if (response.Status == ServiceResponse.ServiceStatus.NotFound)
            {
                return NotFound(response.Messages);
            }
            else if (response.Status == ServiceResponse.ServiceStatus.Error)
            {
                return StatusCode(500, response.Messages);
            }

            // returns 201 Created with Location
            return Created($"api/Account/FindAccount/{response.CreatedId}",AccountDto);
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
