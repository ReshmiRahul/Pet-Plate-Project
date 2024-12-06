using PetAdoption.Models;
using Microsoft.AspNetCore.Mvc;

namespace PetAdoption.Interfaces
{
    public interface IPetService
    {
        // base CRUD
        Task<IEnumerable<PetDto>> ListPets();


        Task<PetDto?> FindPet(int id);


        Task<ServiceResponse> UpdatePet(PetDto PetDto);

        Task<ServiceResponse> AddPet(PetDto PetDto);

        Task<ServiceResponse> DeletePet(int id);

        // related methods

        Task<IEnumerable<PetDto>> ListPetsForAccount(int id);

    }
}
