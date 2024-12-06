using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetAdoption;
using PetAdoption.Models;
using PetAdoption.Interfaces;

namespace PetAdoption.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PetController : ControllerBase
    {
        private readonly IPetService _PetService;

        // dependency injection of service interfaces
        public PetController(IPetService PetService)
        {
            _PetService = PetService;
        }


        /// <summary>
        /// Returns a list of Pets
        /// </summary>
        /// <returns>
        /// 200 OK
        /// [{PetDto},{PetDto},..]
        /// </returns>
        /// <example>
        /// GET: api/Pet/List -> [{PetDto},{PetDto},..]
        /// </example>
        [HttpGet(template: "List")]
        public async Task<ActionResult<IEnumerable<PetDto>>> ListPets()
        {
            // empty list of data transfer object PetDto
            IEnumerable<PetDto> PetDtos = await _PetService.ListPets();
            // return 200 OK with PetDtos
            return Ok(PetDtos);
        }

        /// <summary>
        /// Returns a single Pet specified by its {id}
        /// </summary>
        /// <param name="id">The Pet id</param>
        /// <returns>
        /// 200 OK
        /// {PetDto}
        /// or
        /// 404 Not Found
        /// </returns>
        /// <example>
        /// GET: api/Pet/Find/1 -> {PetDto}
        /// </example>
        [HttpGet(template: "Find/{id}")]
        public async Task<ActionResult<PetDto>> FindPet(int id)
        {

            var Pet = await _PetService.FindPet(id);

            // if the Pet could not be located, return 404 Not Found
            if (Pet == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(Pet);
            }
        }

        /// <summary>
        /// Updates a Pet
        /// </summary>
        /// <param name="id">The ID of the Pet to update</param>
        /// <param name="PetDto">The required information to update the Pet (PetName, PetColor)</param>
        /// <returns>
        /// 400 Bad Request
        /// or
        /// 404 Not Found
        /// or
        /// 204 No Content
        /// </returns>
        /// <example>
        /// PUT: api/Pet/Update/5
        /// Request Headers: Content-Type: application/json
        /// Request Body: {PetDto}
        /// ->
        /// Response Code: 204 No Content
        /// </example>
        [HttpPut(template: "Update/{id}")]
        public async Task<ActionResult> UpdatePet(int id, PetDto PetDto)
        {
            // {id} in URL must match PetId in POST Body
            if (id != PetDto.PetId)
            {
                //400 Bad Request
                return BadRequest();
            }

            ServiceResponse response = await _PetService.UpdatePet(PetDto);

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
        /// Adds a Pet
        /// </summary>
        /// <param name="PetDto">The required information to add the Pet (PetName, PetColor)</param>
        /// <returns>
        /// 201 Created
        /// Location: api/Pet/Find/{PetId}
        /// {PetDto}
        /// or
        /// 404 Not Found
        /// </returns>
        /// <example>
        /// POST: api/Pet/Add
        /// Request Headers: Content-Type: application/json
        /// Request Body: {PetDto}
        /// ->
        /// Response Code: 201 Created
        /// Response Headers: Location: api/Pet/Find/{PetId}
        /// </example>
        [HttpPost(template: "Add")]
        public async Task<ActionResult<Pet>> AddPet(PetDto PetDto)
        {
            ServiceResponse response = await _PetService.AddPet(PetDto);

            if (response.Status == ServiceResponse.ServiceStatus.NotFound)
            {
                return NotFound(response.Messages);
            }
            else if (response.Status == ServiceResponse.ServiceStatus.Error)
            {
                return StatusCode(500, response.Messages);
            }

            // returns 201 Created with Location
            return Created($"api/Pet/FindPet/{response.CreatedId}", PetDto);
        }

        /// <summary>
        /// Deletes the Pet
        /// </summary>
        /// <param name="id">The id of the Pet to delete</param>
        /// <returns>
        /// 204 No Content
        /// or
        /// 404 Not Found
        /// </returns>
        /// <example>
        /// DELETE: api/Pet/Delete/7
        /// ->
        /// Response Code: 204 No Content
        /// </example>
        [HttpDelete("Delete/{id}")]
        public async Task<ActionResult> DeletePet(int id)
        {
            ServiceResponse response = await _PetService.DeletePet(id);

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



        /// <summary>
        /// Returns a list of Pets for a specific Account by its {id}
        /// </summary>
        /// <returns>
        /// 200 OK
        /// [{PetDto},{PetDto},..]
        /// </returns>
        /// <example>
        /// GET: api/Pet/ListForAccount/3 -> [{PetDto},{PetDto},..]
        /// </example>
        [HttpGet(template: "ListForAccount/{id}")]
        public async Task<IActionResult> ListPetsForAccount(int id)
        {
            // empty list of data transfer object PetDto
            IEnumerable<PetDto> PetDtos = await _PetService.ListPetsForAccount(id);
            // return 200 OK with CateDtos
            return Ok(PetDtos);
        }
    }
}
