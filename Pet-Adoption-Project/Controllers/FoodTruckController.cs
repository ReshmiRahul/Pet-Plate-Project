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
    public class FoodTruckController : ControllerBase
    {
        private readonly IFoodTruckService _foodTruckService;

        // Dependency injection of service interfaces
        public FoodTruckController(IFoodTruckService foodTruckService)
        {
            _foodTruckService = foodTruckService;
        }

        /// <summary>
        /// Returns a list of FoodTrucks
        /// </summary>
        /// <returns>
        /// 200 OK
        /// [{FoodTruckDto},{FoodTruckDto},..]
        /// </returns>
        /// <example>
        /// GET: api/FoodTruck/List -> [{FoodTruckDto},{FoodTruckDto},..]
        /// </example>
        [HttpGet("List")]
        public async Task<ActionResult<IEnumerable<FoodTruckDto>>> ListFoodTrucks()
        {
            var foodTruckDtos = await _foodTruckService.ListFoodTrucks();
            return Ok(foodTruckDtos);
        }

        /// <summary>
        /// Returns a single FoodTruck specified by its {id}
        /// </summary>
        /// <param name="id">The FoodTruck id</param>
        /// <returns>
        /// 200 OK
        /// {FoodTruckDto}
        /// or
        /// 404 Not Found
        /// </returns>
        /// <example>
        /// GET: api/FoodTruck/Find/1 -> {FoodTruckDto}
        /// </example>
        [HttpGet("Find/{id}")]
        public async Task<ActionResult<FoodTruckDto>> FindFoodTruck(int id)
        {
            var foodTruck = await _foodTruckService.FindFoodTruck(id);

            if (foodTruck == null)
            {
                return NotFound();
            }

            return Ok(foodTruck);
        }

        /// <summary>
        /// Updates a FoodTruck
        /// </summary>
        /// <param name="id">The ID of the FoodTruck to update</param>
        /// <param name="foodTruckDto">The required information to update the FoodTruck</param>
        /// <returns>
        /// 400 Bad Request
        /// or
        /// 404 Not Found
        /// or
        /// 204 No Content
        /// </returns>
        /// <example>
        /// PUT: api/FoodTruck/Update/5
        /// </example>
        [HttpPut("Update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] FoodTruckDto foodTruckDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _foodTruckService.UpdateFoodTruck(id, foodTruckDto);
            if (response.Status == ServiceResponse.ServiceStatus.Updated)
            {
                return Ok(new { Message = "FoodTruck updated successfully" });
            }
            else
            {
                return BadRequest(new { Message = "Failed to update FoodTruck", Errors = response.Messages });
            }
        }



        /// <summary>
        /// Adds a FoodTruck
        /// </summary>
        /// <param name="foodTruckDto">The required information to add the FoodTruck</param>
        /// <returns>
        /// 201 Created
        /// or
        /// 400 Bad Request
        /// </returns>
        /// <example>
        /// POST: api/FoodTruck/Add
        /// </example>
        // POST: api/FoodTruck/Add
        [HttpPost("Add")]
        public async Task<ActionResult<FoodTruck>> AddFoodTruck(FoodTruckDto foodTruckDto)
        {
            var response = await _foodTruckService.AddFoodTruck(foodTruckDto);

            if (response.Status == ServiceResponse.ServiceStatus.Error)
            {
                return StatusCode(500, new { Message = "Error creating FoodTruck", Errors = response.Messages });
            }

            return CreatedAtAction("FindFoodTruck", new { id = response.CreatedId }, foodTruckDto);
        }


        /// <summary>
        /// Deletes a FoodTruck
        /// </summary>
        /// <param name="id">The id of the FoodTruck to delete</param>
        /// <returns>
        /// 204 No Content
        /// or
        /// 404 Not Found
        /// </returns>
        /// <example>
        /// DELETE: api/FoodTruck/Delete/7
        /// </example>
        [HttpDelete("Delete/{id}")]
        public async Task<ActionResult> DeleteFoodTruck(int id)
        {
            var response = await _foodTruckService.DeleteFoodTruck(id);

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
