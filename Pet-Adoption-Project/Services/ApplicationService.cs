using PetAdoption.Interfaces;
using PetAdoption.Models;
using Microsoft.EntityFrameworkCore;
using Pet_Adoption_Project.Data;

namespace PetAdoption.Services
{
    public class ApplicationService : IApplicationService
    {

        private readonly ApplicationDbContext _context;
        // dependency injection of database context
        public ApplicationService(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<IEnumerable<ApplicationDto>> ListApplications()
        {
            // include will join the Account(i)tem with 1 account, 1 pet
            List<Application> Applications = await _context.Applications
                .Include(i => i.Account)
                .Include(i => i.Pet)
                .Include(i => i.Pet.Accounts)
                .ToListAsync();
            // empty list of data transfer object ApplicationDto
            List<ApplicationDto> ApplicationDtos = new List<ApplicationDto>();
            // foreach Account Item record in database
            foreach (Application Application in Applications)
            {
                // create new instance of ApplicationDto, add to list
                ApplicationDtos.Add(new ApplicationDto()
                {
                    ApplicationID = Application.ApplicationID,
                    ApplicationDate = Application.ApplicationDate,
                    ApplicationStatus = Application.ApplicationStatus,
                    ApplicationExperience = Application.ApplicationExperience,
                    ApplicationComments = Application.ApplicationComments,
                    ApplicationReason = Application.ApplicationReason,
                    AccountId = Application.AccountId,
                    PetId = Application.PetId,
                    AccountName = Application.Account.AccountName,
                    PetName = Application.Pet.PetName
                });
            }
            // return ApplicationDtos
            return ApplicationDtos;

        }


        public async Task<ApplicationDto?> FindApplication(int id)
        {
            // include will join Account(i)tem with 1 Pet, 1 Account
            // first or default async will get the first account(i)tem matching the {id}
            var Application = await _context.Applications
                .Include(i => i.Account)
                .Include(i => i.Pet)
                .Include(i => i.Pet.Accounts)
                .FirstOrDefaultAsync(i => i.ApplicationID == id);

            // no item found
            if (Application == null)
            {
                return null;
            }
            // create an instance of ApplicationDto
            ApplicationDto ApplicationDto = new ApplicationDto()
            {
                ApplicationID = Application.ApplicationID,
                ApplicationDate = Application.ApplicationDate,
                ApplicationStatus = Application.ApplicationStatus,
                ApplicationExperience = Application.ApplicationExperience,
                ApplicationComments = Application.ApplicationComments,
                ApplicationReason = Application.ApplicationReason,
                AccountId = Application.AccountId,
                PetId = Application.PetId,
                AccountName = Application.Account.AccountName,
                PetName = Application.Pet.PetName
            };
            return ApplicationDto;

        }


        public async Task<ServiceResponse> UpdateApplication(ApplicationDto ApplicationDto)
        {
            ServiceResponse serviceResponse = new();
            var existingApplication = await _context.Applications.FindAsync(ApplicationDto.ApplicationID);

            if (existingApplication == null)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.NotFound;
                return serviceResponse;
            }

            // Update properties
            existingApplication.ApplicationReason = ApplicationDto.ApplicationReason;
            existingApplication.ApplicationComments = ApplicationDto.ApplicationComments;
            existingApplication.ApplicationDate = (DateTime)ApplicationDto.ApplicationDate;
            existingApplication.ApplicationExperience = ApplicationDto.ApplicationExperience;
            existingApplication.ApplicationStatus = (int)ApplicationDto.ApplicationStatus;
            existingApplication.PetId = ApplicationDto.PetId;
            existingApplication.AccountId = ApplicationDto.AccountId;

            try
            {
                await _context.SaveChangesAsync();
                serviceResponse.Status = ServiceResponse.ServiceStatus.Updated;
            }
            catch (DbUpdateConcurrencyException)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.Error;
                serviceResponse.Messages.Add("An error occurred updating the record");
                return serviceResponse;
            }

            return serviceResponse;
        }


        public async Task<ServiceResponse> AddApplication(ApplicationDto ApplicationDto)
        {
            ServiceResponse serviceResponse = new();
            Pet? Pet = await _context.Pets.FindAsync(ApplicationDto.PetId);
            Account? Account = await _context.Accounts.FindAsync(ApplicationDto.AccountId);

            // Data must link to a valid entity
            if (Pet == null || Account == null)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.NotFound;
                if (Pet == null)
                {
                    serviceResponse.Messages.Add("Pet was not found. ");
                }
                if (Account == null)
                {
                    serviceResponse.Messages.Add("Account was not found.");
                }
                return serviceResponse;
            }

            Application Application = new Application()
            {
                ApplicationReason = ApplicationDto.ApplicationReason,
                ApplicationComments = ApplicationDto.ApplicationComments,
                ApplicationDate = (DateTime)ApplicationDto.ApplicationDate,
                ApplicationExperience = ApplicationDto.ApplicationExperience,
                ApplicationStatus = (int)ApplicationDto.ApplicationStatus,
                PetId = ApplicationDto.PetId,
                Pet = Pet,
                Account = Account,
                AccountId = ApplicationDto.AccountId
            };
            // SQL Equivalent: Insert into Applications (..) values (..)

            try
            {
                _context.Applications.Add(Application);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.Error;
                serviceResponse.Messages.Add("There was an error adding the Account Item.");
                serviceResponse.Messages.Add(ex.Message);
            }


            serviceResponse.Status = ServiceResponse.ServiceStatus.Created;
            serviceResponse.CreatedId = Application.ApplicationID;
            return serviceResponse;
        }


        public async Task<ServiceResponse> DeleteApplication(int id)
        {
            ServiceResponse response = new();
            // Account Item must exist in the first place
            var Application = await _context.Applications.FindAsync(id);
            if (Application == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                response.Messages.Add("Account Item cannot be deleted because it does not exist.");
                return response;
            }

            try
            {
                _context.Applications.Remove(Application);
                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("Error encountered while deleting Account item");
                return response;
            }

            response.Status = ServiceResponse.ServiceStatus.Deleted;

            return response;

        }

        public async Task<IEnumerable<ApplicationDto>> ListApplicationsForAccount(int id)
        {
            // WHERE Accountid == id
            List<Application> Applications = await _context.Applications
                .Include(i => i.Pet)
                .Include(i => i.Account)
                .Include(i => i.Account.Pets)
                .Where(i => i.AccountId == id)
                .ToListAsync();

            // empty list of data transfer object ApplicationDto
            List<ApplicationDto> ApplicationDtos = new List<ApplicationDto>();
            // foreach Account Item record in database
            foreach (Application Application in Applications)
            {
                // create new instance of ApplicationDto, add to list
                ApplicationDtos.Add(new ApplicationDto()
                {
                    ApplicationID = Application.ApplicationID,
                    ApplicationDate = Application.ApplicationDate,
                    ApplicationStatus = Application.ApplicationStatus,
                    ApplicationExperience = Application.ApplicationExperience,
                    ApplicationComments = Application.ApplicationComments,
                    ApplicationReason = Application.ApplicationReason,
                    AccountId = Application.AccountId,
                    PetId = Application.PetId,
                    AccountName = Application.Account.AccountName,
                    PetName = Application.Pet.PetName
                });
            }
            // return 200 OK with ApplicationDtos
            return ApplicationDtos;

        }

        public async Task<IEnumerable<ApplicationDto>> ListApplicationsForPet(int id)
        {
            // WHERE PetId == id
            List<Application> Applications = await _context.Applications
                .Include(i => i.Pet)
                .Include(i => i.Account)
                .Include(i => i.Account.Pets)
                .Where(i => i.PetId == id)
                .ToListAsync();

            // empty list of data transfer object ApplicationDto
            List<ApplicationDto> ApplicationDtos = new List<ApplicationDto>();
            // foreach Account Item record in database
            foreach (Application Application in Applications)
            {
                // create new instance of ApplicationDto, add to list
                ApplicationDtos.Add(new ApplicationDto()
                {
                    ApplicationID = Application.ApplicationID,
                    ApplicationDate = Application.ApplicationDate,
                    ApplicationStatus = Application.ApplicationStatus,
                    ApplicationExperience = Application.ApplicationExperience,
                    ApplicationComments = Application.ApplicationComments,
                    ApplicationReason = Application.ApplicationReason,
                    AccountId = Application.AccountId,
                    PetId = Application.PetId,
                    AccountName = Application.Account.AccountName,
                    PetName = Application.Pet.PetName
                });
            }
            // return 200 OK with ApplicationDtos
            return ApplicationDtos;

        }
    }
}


