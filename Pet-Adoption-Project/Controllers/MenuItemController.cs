using PetAdoption.Interfaces;
using PetAdoption.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PetAdoption.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MenuItemController : ControllerBase
    {
        private readonly IMenuItemService _menuItemService;

        // Dependency injection of service interfaces
        public MenuItemController(IMenuItemService menuItemService)
        {
            _menuItemService = menuItemService;
        }

        /// <summary>
        /// Returns a list of MenuItems
        /// </summary>
        /// <returns>
        /// 200 OK
        /// [{MenuItemDto},{MenuItemDto},..]
        /// </returns>
        /// <example>
        /// GET: api/MenuItem/List -> [{MenuItemDto},{MenuItemDto},..]
        /// </example>
        [HttpGet("List")]
        public async Task<ActionResult<IEnumerable<MenuItemDto>>> ListMenuItems()
        {
            var menuItemDtos = await _menuItemService.ListMenuItems();
            return Ok(menuItemDtos);
        }

        /// <summary>
        /// Returns a single MenuItem specified by its {id}
        /// </summary>
        /// <param name="id">The MenuItem id</param>
        /// <returns>
        /// 200 OK
        /// {MenuItemDto}
        /// or
        /// 404 Not Found
        /// </returns>
        /// <example>
        /// GET: api/MenuItem/Find/1 -> {MenuItemDto}
        /// </example>
        [HttpGet("Find/{id}")]
        public async Task<ActionResult<MenuItemDto>> FindMenuItem(int id)
        {
            var menuItem = await _menuItemService.FindMenuItem(id);

            if (menuItem == null)
            {
                return NotFound();
            }

            return Ok(menuItem);
        }

        /// <summary>
        /// Updates a MenuItem
        /// </summary>
        /// <param name="id">The ID of the MenuItem to update</param>
        /// <param name="menuItemDto">The required information to update the MenuItem</param>
        /// <returns>
        /// 400 Bad Request
        /// or
        /// 404 Not Found
        /// or
        /// 204 No Content
        /// </returns>
        /// <example>
        /// PUT: api/MenuItem/Update/5
        /// </example>
        [HttpPut("Update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] MenuItemDto menuItemDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _menuItemService.UpdateMenuItem(menuItemDto);
            if (response.Status == ServiceResponse.ServiceStatus.Updated)
            {
                return Ok(new { Message = "MenuItem updated successfully" });
            }
            else
            {
                return BadRequest(new { Message = "Failed to update MenuItem", Errors = response.Messages });
            }
        }

        /// <summary>
        /// Adds a MenuItem
        /// </summary>
        /// <param name="menuItemDto">The required information to add the MenuItem</param>
        /// <returns>
        /// 201 Created
        /// or
        /// 400 Bad Request
        /// </returns>
        /// <example>
        /// POST: api/MenuItem/Add
        /// </example>
        [HttpPost("Add")]
        public async Task<ActionResult<MenuItem>> AddMenuItem(MenuItemDto menuItemDto)
        {
            var response = await _menuItemService.AddMenuItem(menuItemDto);

            if (response.Status == ServiceResponse.ServiceStatus.Error)
            {
                return StatusCode(500, response.Messages);
            }

            return Created($"api/MenuItem/Find/{response.CreatedId}", menuItemDto);
        }

        /// <summary>
        /// Deletes a MenuItem
        /// </summary>
        /// <param name="id">The id of the MenuItem to delete</param>
        /// <returns>
        /// 204 No Content
        /// or
        /// 404 Not Found
        /// </returns>
        /// <example>
        /// DELETE: api/MenuItem/Delete/7
        /// </example>
        [HttpDelete("Delete/{id}")]
        public async Task<ActionResult> DeleteMenuItem(int id)
        {
            var response = await _menuItemService.DeleteMenuItem(id);

            if (response.Status == ServiceResponse.ServiceStatus.NotFound)
            {
                return NotFound();
            }

            if (response.Status == ServiceResponse.ServiceStatus.Error)
            {
                return StatusCode(500, response.Messages);
            }

            return NoContent();
        }
    }
}