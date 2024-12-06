using PetAdoption.Models;

namespace PetAdoption.Interfaces
{
    public interface IAccountService
    {


        // base CRUD
        Task<IEnumerable<AccountDto>> ListAccounts();


        Task<AccountDto?> FindAccount(int id);


        Task<ServiceResponse> UpdateAccount(AccountDto AccountDto);

        Task<ServiceResponse> AddAccount(AccountDto AccountDto);

        Task<ServiceResponse> DeleteAccount(int id);

        // related methods

        Task<IEnumerable<AccountDto>> ListAccountsForPet(int id);


        Task<ServiceResponse> LinkAccountToPet(int AccountId, int PetId);

        Task<ServiceResponse> UnlinkAccountFromPet(int AccountId, int PetId);
    }
}
