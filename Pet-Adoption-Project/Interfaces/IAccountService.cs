using PetAdoption.Models;

namespace PetAdoption.Interfaces
{
    public interface IAccountService
    {


        // base CRUD
        Task<IEnumerable<AccountDto>> ListAccounts();


        Task<AccountDto?> FindAccount(int id);


        Task<ServiceResponse> UpdateAccount(int id, AccountDto accountDto);

        Task<ServiceResponse> AddAccount(AccountDto AccountDto);

        Task<ServiceResponse> DeleteAccount(int id);

        // related methods

        Task<IEnumerable<AccountDto>> ListAccountsForPet(int id);

        // Authentication methods
        Task<(bool IsSuccess, string ErrorMessage)> LoginAsync(string username, string password);
        Task LogoutAsync();
        Task<bool> ValidateUserAsync(string username, string password);
        Task<AccountDto?> FindByUsernameAsync(string username);
        Task<Account> Login(string username, string password);

    }
}
