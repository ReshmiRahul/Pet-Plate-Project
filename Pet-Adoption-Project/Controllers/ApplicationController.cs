using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetAdoption.Interfaces;
using PetAdoption.Models;

namespace PetAdoption.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationController : ControllerBase
    {
        private readonly IApplicationService _applicationService;

        // Dependency injection of service interfaces
        public ApplicationController(IApplicationService applicationService)
        {
            _applicationService = applicationService;
        }

        /// <summary>
        /// Returns a list of applications.
        /// </summary>
        [HttpGet("List")]
        public async Task<ActionResult<IEnumerable<ApplicationDto>>> ListApplications()
        {
            IEnumerable<ApplicationDto> applicationDtos = await _applicationService.ListApplications();
            return Ok(applicationDtos);
        }

        /// <summary>
        /// Returns a single application specified by its id.
        /// </summary>
        [HttpGet("Find/{id}")]
        public async Task<ActionResult<ApplicationDto>> FindApplication(int id)
        {
            var application = await _applicationService.FindApplication(id);
            if (application == null)
            {
                return NotFound();
            }
            return Ok(application);
        }

        /// <summary>
        /// Updates an application.
        /// </summary>
        [HttpPut("Update/{id}")]
        public async Task<ActionResult> UpdateApplication(int id, ApplicationDto applicationDto)
        {
            if (id != applicationDto.ApplicationID)
            {
                return BadRequest();
            }

            var response = await _applicationService.UpdateApplication(applicationDto);
            if (response.Status == ServiceResponse.ServiceStatus.NotFound)
            {
                return NotFound(response.Messages);
            }

            return NoContent();
        }

        /// <summary>
        /// Adds a new application.
        /// </summary>
        [HttpPost("Add")]
        public async Task<ActionResult<ApplicationDto>> AddApplication(ApplicationDto applicationDto)
        {
            var response = await _applicationService.AddApplication(applicationDto);
            if (response.Status == ServiceResponse.ServiceStatus.NotFound)
            {
                return NotFound(response.Messages);
            }

            return CreatedAtAction(nameof(FindApplication), new { id = response.CreatedId }, applicationDto);
        }

        /// <summary>
        /// Deletes an application.
        /// </summary>
        [HttpDelete("Delete/{id}")]
        [Authorize]
        public async Task<ActionResult> DeleteApplication(int id)
        {
            ServiceResponse response = await _applicationService.DeleteApplication(id);

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

        // List applications for account
        [HttpGet("ListForAccount/{id}")]
        public async Task<IActionResult> ListApplicationsForAccount(int id)
        {
            IEnumerable<ApplicationDto> applicationDtos = await _applicationService.ListApplicationsForAccount(id);
            return Ok(applicationDtos);
        }

        // List applications for pet
        [HttpGet("ListForPet/{id}")]
        public async Task<IActionResult> ListApplicationsForPet(int id)
        {
            IEnumerable<ApplicationDto> applicationDtos = await _applicationService.ListApplicationsForPet(id);
            return Ok(applicationDtos);
        }
    }
}
