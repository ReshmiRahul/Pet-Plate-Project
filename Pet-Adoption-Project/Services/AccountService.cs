using PetAdoption.Interfaces;
using PetAdoption.Models;
using Microsoft.EntityFrameworkCore;
using Pet_Adoption_Project.Data;
using BCrypt.Net;

namespace PetAdoption.Services
{
    public class AccountService : IAccountService
    {
        private readonly ApplicationDbContext _context;

        // Dependency injection of database context
        public AccountService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Login method
        public async Task<(bool IsSuccess, string ErrorMessage)> LoginAsync(string username, string password)
        {
            bool isValidUser = await ValidateUserAsync(username, password);

            if (!isValidUser)
            {
                return (false, "Invalid username or password.");
            }

            return (true, string.Empty);
        }

        public async Task<Account> Login(string username, string password)
        {
            // Your login logic here, for example:
            // Query the database to validate the username and password
            var account = await _context.Accounts
                                          .FirstOrDefaultAsync(a => a.AccountName == username && a.AccountPassword == password);

            return account;  // Return the found account, or null if not found
        }
        // Logout method
        public async Task LogoutAsync()
        {
            // Clear session or cookies for logout
            await Task.CompletedTask;
        }

        // List all accounts
        public async Task<IEnumerable<AccountDto>> ListAccounts()
        {
            List<Account> accounts = await _context.Accounts
                .Include(a => a.Location) // Eagerly load the Location entity
                .ToListAsync();

            List<AccountDto> accountDtos = accounts.Select(account => new AccountDto()
            {
                AccountId = account.AccountId,
                AccountName = account.AccountName,
                AccountEmail = account.AccountEmail,
                AccountRole = account.AccountRole,
                AccountPassword = account.AccountPassword,
                LocationId = account.LocationId,
                LocationAddress = account.Location?.Address
            }).ToList();

            return accountDtos;
        }

        // Find account by ID
        public async Task<AccountDto?> FindAccount(int id)
        {
            var account = await _context.Accounts
                .Include(a => a.Location)
                .FirstOrDefaultAsync(c => c.AccountId == id);

            if (account == null)
            {
                return null;
            }

            return new AccountDto()
            {
                AccountId = account.AccountId,
                AccountName = account.AccountName,
                AccountEmail = account.AccountEmail,
                AccountRole = account.AccountRole,
                AccountPassword = account.AccountPassword,
                LocationId = account.LocationId,
                LocationAddress = account.Location?.Address
            };
        }

        // Update an account
        public async Task<ServiceResponse> UpdateAccount(int id, AccountDto accountDto)
        {
            var response = new ServiceResponse();

            // Find the food truck by id
            var account = await _context.Accounts
                .Include(ft => ft.Location)  // Include related entities if necessary
                .FirstOrDefaultAsync(ft => ft.AccountId == id);

            if (account == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                response.Messages.Add("Account not found.");
                return response;
            }

            // Update the food truck properties with the data from the FoodTruckDto
            account.AccountName = accountDto.AccountName;
            account.AccountPassword = accountDto.AccountPassword;
            account.AccountEmail = accountDto.AccountEmail;
            account.AccountRole = accountDto.AccountRole;
            account.LocationId = accountDto.LocationId;

            // Save changes to the database
            await _context.SaveChangesAsync();

            response.Status = ServiceResponse.ServiceStatus.Updated;
            return response;
        }

        // Add a new account
        public async Task<ServiceResponse> AddAccount(AccountDto accountDto)
        {
            ServiceResponse response = new();

            var account = new Account
            {
                AccountName = accountDto.AccountName,
                AccountPassword = accountDto.AccountPassword,
                AccountEmail = accountDto.AccountEmail,
                AccountRole = accountDto.AccountRole,
                LocationId = accountDto.LocationId
            };

            try
            {
                _context.Accounts.Add(account);
                await _context.SaveChangesAsync();

                response.Status = ServiceResponse.ServiceStatus.Created;
                response.CreatedId = account.AccountId;
            }
            catch (Exception ex)
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("Error adding account.");
                response.Messages.Add(ex.Message);
            }

            return response;
        }

        // Delete an account
        public async Task<ServiceResponse> DeleteAccount(int id)
        {
            ServiceResponse response = new();

            var account = await _context.Accounts.FindAsync(id);
            if (account == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                response.Messages.Add("Account not found.");
                return response;
            }

            try
            {
                _context.Accounts.Remove(account);
                await _context.SaveChangesAsync();

                response.Status = ServiceResponse.ServiceStatus.Deleted;
            }
            catch (Exception ex)
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add($"An error occurred: {ex.Message}");
            }

            return response;
        }

        // List accounts linked to a pet
        public async Task<IEnumerable<AccountDto>> ListAccountsForPet(int id)
        {
            var accounts = await _context.Accounts
                .Where(c => c.Pets.Any(p => p.PetId == id))
                .ToListAsync();

            return accounts.Select(account => new AccountDto()
            {
                AccountId = account.AccountId,
                AccountName = account.AccountName,
                AccountEmail = account.AccountEmail,
                AccountRole = account.AccountRole,
                AccountPassword = account.AccountPassword,
                LocationId = account.LocationId,
                LocationAddress = account.Location?.Address
            }).ToList();
        }

        // Link account to a pet
        public async Task<ServiceResponse> LinkAccountToPet(int accountId, int petId)
        {
            ServiceResponse serviceResponse = new();

            var account = await _context.Accounts.Include(c => c.Pets).FirstOrDefaultAsync(c => c.AccountId == accountId);
            var pet = await _context.Pets.FindAsync(petId);

            if (account == null || pet == null)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.NotFound;
                if (account == null) serviceResponse.Messages.Add("Account not found.");
                if (pet == null) serviceResponse.Messages.Add("Pet not found.");
                return serviceResponse;
            }

            try
            {
                account.Pets.Add(pet);
                await _context.SaveChangesAsync();
                serviceResponse.Status = ServiceResponse.ServiceStatus.Created;
            }
            catch (Exception ex)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.Error;
                serviceResponse.Messages.Add($"An error occurred: {ex.Message}");
            }

            return serviceResponse;
        }

        // Unlink account from a pet
        public async Task<ServiceResponse> UnlinkAccountFromPet(int accountId, int petId)
        {
            ServiceResponse serviceResponse = new();

            var account = await _context.Accounts.Include(c => c.Pets).FirstOrDefaultAsync(c => c.AccountId == accountId);
            var pet = await _context.Pets.FindAsync(petId);

            if (account == null || pet == null)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.NotFound;
                if (account == null) serviceResponse.Messages.Add("Account not found.");
                if (pet == null) serviceResponse.Messages.Add("Pet not found.");
                return serviceResponse;
            }

            try
            {
                account.Pets.Remove(pet);
                await _context.SaveChangesAsync();
                serviceResponse.Status = ServiceResponse.ServiceStatus.Deleted;
            }
            catch (Exception ex)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.Error;
                serviceResponse.Messages.Add($"An error occurred: {ex.Message}");
            }

            return serviceResponse;
        }

        // Validate user credentials
        public async Task<bool> ValidateUserAsync(string username, string password)
        {
            // Find the account by username from the database
            var account = await _context.Accounts.SingleOrDefaultAsync(a => a.AccountName == username);

            if (account == null)
                return false;

            // Compare the plain password directly with the one stored in the database
            return account.AccountPassword == password;
        }

        public async Task CreateAccountAsync(string username, string password)
        {
            var account = new Account
            {
                AccountName = username,
                AccountPassword = password // Storing plain password directly
            };

            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();
        }

        public async Task<AccountDto?> FindByUsernameAsync(string username)
        {
            var account = await _context.Accounts
                .Where(a => a.AccountName == username)
                .Select(a => new AccountDto
                {
                    AccountId = a.AccountId,
                    AccountName = a.AccountName,
                    AccountEmail = a.AccountEmail,
                    AccountRole = a.AccountRole,
                    LocationId = a.LocationId,
                    LocationAddress = a.Location.Address
                })
                .FirstOrDefaultAsync();

            return account;
        }

        public Task<ServiceResponse> UpdateAccount(AccountDto AccountDto)
        {
            throw new NotImplementedException();
        }


    }
}
