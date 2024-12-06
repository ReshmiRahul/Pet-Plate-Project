using PetAdoption.Interfaces;
using PetAdoption.Models;
using Microsoft.EntityFrameworkCore;
using Pet_Adoption_Project.Data;

namespace PetAdoption.Services
{
    public class PetService : IPetService
    {
        private readonly ApplicationDbContext _context;
        // dependency injection of database context
        public PetService(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<IEnumerable<PetDto>> ListPets()
        {
            // all Pets
            List<Pet> Pets = await _context.Pets
                .ToListAsync();
            // empty list of data transfer object PetDto
            List<PetDto> PetDtos = new List<PetDto>();
            // foreach Application application record in database
            foreach (Pet Pet in Pets)
            {
                // create new instance of PetDto, add to list
                PetDtos.Add(new PetDto()
                {
                    PetId = Pet.PetId,
                    PetAge = Pet.PetAge,
                    PetName = Pet.PetName,
                    PetType = Pet.PetType,
                    PetBreed = Pet.PetBreed,
                    PetDescription = Pet.PetDescription,
                    PetStatus = Pet.PetStatus
                });
            }
            // return PetDtos
            return PetDtos;

        }


        public async Task<PetDto?> FindPet(int id)
        {
            // include will join order(i)tem with 1 Pet, 1 order, 1 customer
            // first or default async will get the first order(i)tem matching the {id}
            var Pet = await _context.Pets
                .FirstOrDefaultAsync(p => p.PetId == id);

            // no order application found
            if (Pet == null)
            {
                return null;
            }
            // create an instance of PetDto
            PetDto PetDto = new PetDto()
            {
                PetId = Pet.PetId,
                PetAge = Pet.PetAge,
                PetName = Pet.PetName,
                PetType = Pet.PetType,
                PetBreed = Pet.PetBreed,
                PetDescription = Pet.PetDescription,
                PetStatus = Pet.PetStatus
            };
            return PetDto;

        }


        public async Task<ServiceResponse> UpdatePet(PetDto PetDto)
        {
            ServiceResponse serviceResponse = new();


            // Create instance of Pet
            Pet Pet = new Pet()
            {
                PetId = (int)PetDto.PetId,
                PetAge = (int)PetDto.PetAge,
                PetName = PetDto.PetName,
                PetType = PetDto.PetType,
                PetBreed = PetDto.PetBreed,
                PetDescription = PetDto.PetDescription,
                PetStatus = (int)PetDto.PetStatus
            };
            // flags that the object has changed
            _context.Entry(Pet).State = EntityState.Modified;

            try
            {
                // SQL Equivalent: Update Pets set ... where PetId={id}
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.Error;
                serviceResponse.Messages.Add("An error occurred updating the record");
                return serviceResponse;
            }

            serviceResponse.Status = ServiceResponse.ServiceStatus.Updated;
            return serviceResponse;
        }


        public async Task<ServiceResponse> AddPet(PetDto PetDto)
        {
            ServiceResponse serviceResponse = new();


            // Create instance of Pet
            Pet Pet = new Pet()
            {
                PetName = PetDto.PetName,
                PetAge = (int)PetDto.PetAge,
                PetType = PetDto.PetType,
                PetBreed = PetDto.PetBreed,
                PetDescription = PetDto.PetDescription,
                PetStatus = (int)PetDto.PetStatus
            };
            // SQL Equivalent: Insert into Pets (..) values (..)

            try
            {
                _context.Pets.Add(Pet);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.Error;
                serviceResponse.Messages.Add("There was an error adding the Pet.");
                serviceResponse.Messages.Add(ex.Message);
            }


            serviceResponse.Status = ServiceResponse.ServiceStatus.Created;
            serviceResponse.CreatedId = Pet.PetId;
            return serviceResponse;
        }


        public async Task<ServiceResponse> DeletePet(int id)
        {
            ServiceResponse response = new();
            // Order application must exist in the first place
            var Pet = await _context.Pets.FindAsync(id);
            if (Pet == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                response.Messages.Add("Pet cannot be deleted because it does not exist.");
                return response;
            }

            try
            {
                _context.Pets.Remove(Pet);
                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("Error encountered while deleting the Pet");
                return response;
            }

            response.Status = ServiceResponse.ServiceStatus.Deleted;

            return response;

        }

        public async Task<IEnumerable<PetDto>> ListPetsForAccount(int id)
        {
            // join AccountPet on Pets.Petid = AccountPet.Petid WHERE AccountPet.Accountid = {id}
            List<Pet> Pets = await _context.Pets
                .Where(p => p.Accounts.Any(c => c.AccountId == id))
                .ToListAsync();

            // empty list of data transfer object PetDto
            List<PetDto> PetDtos = new List<PetDto>();
            // foreach application record in database
            foreach (Pet Pet in Pets)
            {
                // create new instance of PetDto, add to list
                PetDtos.Add(new PetDto()
                {
                    PetId = Pet.PetId,
                    PetAge = Pet.PetAge,
                    PetName = Pet.PetName,
                    PetType = Pet.PetType,
                    PetBreed = Pet.PetBreed,
                    PetDescription = Pet.PetDescription,
                    PetStatus = Pet.PetStatus
                });
            }
            // return PetDtos
            return PetDtos;

        }


    }
}
