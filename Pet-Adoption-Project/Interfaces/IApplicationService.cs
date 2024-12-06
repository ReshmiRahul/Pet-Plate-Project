using PetAdoption.Models;

namespace PetAdoption.Interfaces
{
  public interface IApplicationService
        {

            // describes the ways we interact with ApplicationService
            // Very useful if "ApplicationService" is replaced with "ApplicationTester"
            Task<IEnumerable<ApplicationDto>> ListApplications();


            Task<ApplicationDto?> FindApplication(int id);


            Task<ServiceResponse> UpdateApplication(ApplicationDto ApplicationDto);

            Task<ServiceResponse> AddApplication(ApplicationDto ApplicationDto);

            Task<ServiceResponse> DeleteApplication(int id);

            // related methods

            Task<IEnumerable<ApplicationDto>> ListApplicationsForAccount(int id);


            Task<IEnumerable<ApplicationDto>> ListApplicationsForPet(int id);

        
    }
}
