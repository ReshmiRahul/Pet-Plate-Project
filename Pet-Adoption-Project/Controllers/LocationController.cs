using PetAdoption.Interfaces;
using PetAdoption.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using PetAdoption.Services;

namespace PetAdoption.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationController : ControllerBase
    {
        private readonly ILocationService _locationService;

        // Dependency injection of service interfaces
        public LocationController(ILocationService locationService)
        {
            _locationService = locationService;
        }

        /// <summary>
        /// Returns a list of Locations
        /// </summary>
        /// <returns>
        /// 200 OK
        /// [{LocationDto},{LocationDto},..]
        /// </returns>
        /// <example>
        /// GET: api/Location/List -> [{LocationDto},{LocationDto},..]
        /// </example>
        [HttpGet("List")]
        public async Task<ActionResult<IEnumerable<LocationDto>>> ListLocations()
        {
            var locationDtos = await _locationService.ListLocations();
            return Ok(locationDtos);
        }

        /// <summary>
        /// Returns a single Location specified by its {id}
        /// </summary>
        /// <param name="id">The Location id</param>
        /// <returns>
        /// 200 OK
        /// {LocationDto}
        /// or
        /// 404 Not Found
        /// </returns>
        /// <example>
        /// GET: api/Location/Find/1 -> {LocationDto}
        /// </example>
        [HttpGet("Find/{id}")]
        public async Task<ActionResult<LocationDto>> FindLocation(int id)
        {
            var location = await _locationService.FindLocation(id);

            if (location == null)
            {
                return NotFound();
            }

            return Ok(location);
        }

        /// <summary>
        /// Updates a Location
        /// </summary>
        /// <param name="id">The ID of the Location to update</param>
        /// <param name="locationDto">The required information to update the Location</param>
        /// <returns>
        /// 400 Bad Request
        /// or
        /// 404 Not Found
        /// or
        /// 204 No Content
        /// </returns>
        /// <example>
        /// PUT: api/Location/Update/5
        /// </example>
        [HttpPut("Update/{id}")]
        public async Task<ActionResult> UpdateLocation(int id, LocationDto locationDto)
        {
            // Ensure the URL id matches the body LocationId
            if (id != locationDto.LocationId)
            {
                return BadRequest("The ID in the URL does not match the LocationId in the body.");
            }

            // Call the service method with both 'id' and 'locationDto'
            ServiceResponse response = await _locationService.UpdateLocation(id, locationDto);

            if (response.Status == ServiceResponse.ServiceStatus.NotFound)
            {
                return NotFound(new { Message = "Location not found.", Details = response.Messages });
            }

            if (response.Status == ServiceResponse.ServiceStatus.Error)
            {
                return StatusCode(500, new { Message = "An error occurred while updating the location.", Details = response.Messages });
            }

            return NoContent();
        }


        /// <summary>
        /// Adds a Location
        /// </summary>
        /// <param name="locationDto">The required information to add the Location</param>
        /// <returns>
        /// 201 Created
        /// or
        /// 400 Bad Request
        /// </returns>
        /// <example>
        /// POST: api/Location/Add
        /// </example>
        [HttpPost("Add")]
        public async Task<ActionResult<LocationDto>> AddLocation(LocationDto locationDto)
        {
            var response = await _locationService.AddLocation(locationDto);

            if (response.Status == ServiceResponse.ServiceStatus.Error)
            {
                return StatusCode(500, response.Messages);
            }

            return Created($"api/Location/Find/{response.CreatedId}", locationDto);
        }

        /// <summary>
        /// Deletes a Location
        /// </summary>
        /// <param name="id">The id of the Location to delete</param>
        /// <returns>
        /// 204 No Content
        /// or
        /// 404 Not Found
        /// </returns>
        /// <example>
        /// DELETE: api/Location/Delete/7
        /// </example>
        [HttpDelete("Delete/{id}")]
        public async Task<ActionResult> DeleteLocation(int id)
        {
            var response = await _locationService.DeleteLocation(id);

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

        /// <summary>
        /// Returns a list of FoodTrucks by a specific Location
        /// </summary>
        /// <param name="locationId">The Location id</param>
        /// <returns>
        /// 200 OK
        /// [{FoodTruckDto},{FoodTruckDto},..]
        /// </returns>
        /// <example>
        /// GET: api/Location/FoodTrucks/1 -> [{FoodTruckDto},{FoodTruckDto},..]
        /// </example>
        [HttpGet("FoodTrucks/{locationId}")]
        public async Task<ActionResult<IEnumerable<FoodTruckDto>>> ListFoodTrucksByLocation(int locationId)
        {
            var foodTrucks = await _locationService.ListFoodTrucksByLocation(locationId);
            return Ok(foodTrucks);
        }

    }
}
