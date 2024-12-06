using PetAdoption.Interfaces;
//using PetAdoption.Migrations;
using PetAdoption.Models;
using Microsoft.EntityFrameworkCore;
using Pet_Adoption_Project.Data;

namespace PetAdoption.Services
{
    public class AccountService : IAccountService
    {
        private readonly ApplicationDbContext _context;
        // dependency injection of database context
        public AccountService(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<IEnumerable<AccountDto>> ListAccounts()
        {
            // Fetch all accounts, including their related Location data
            List<Account> Accounts = await _context.Accounts
                .Include(a => a.Location) // Eagerly load the Location entity
                .ToListAsync();

            // Map the accounts to AccountDto
            List<AccountDto> AccountDtos = Accounts.Select(Account => new AccountDto()
            {
                AccountId = Account.AccountId,
                AccountName = Account.AccountName,
                AccountEmail = Account.AccountEmail,
                AccountRole = Account.AccountRole,
                AccountPassword = Account.AccountPassword,
                LocationId = Account.LocationId,
                LocationAddress = Account.Location?.Address // Safe access with null check
            }).ToList();

            return AccountDtos;
        }



        public async Task<AccountDto?> FindAccount(int id)
        {
            // Include Location in the query to fetch the related data
            var Account = await _context.Accounts
                .Include(a => a.Location) // Eagerly load the Location entity
                .FirstOrDefaultAsync(c => c.AccountId == id);

            // If no account is found, return null
            if (Account == null)
            {
                return null;
            }

            // Map the account to AccountDto
            AccountDto AccountDto = new AccountDto()
            {
                AccountId = Account.AccountId,
                AccountName = Account.AccountName,
                AccountEmail = Account.AccountEmail,
                AccountRole = Account.AccountRole,
                AccountPassword = Account.AccountPassword,
                LocationId = Account.LocationId,
                LocationAddress = Account.Location?.Address // Safe null check
            };

            return AccountDto;
        }



        public async Task<ServiceResponse> UpdateAccount(AccountDto AccountDto)
        {
            ServiceResponse serviceResponse = new();

            // Check if the account exists in the database
            var existingAccount = await _context.Accounts
                .FirstOrDefaultAsync(a => a.AccountId == AccountDto.AccountId);

            if (existingAccount == null)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.Error;
                serviceResponse.Messages.Add("Account not found");
                return serviceResponse;
            }

            // Update the existing account with new values
            existingAccount.AccountName = AccountDto.AccountName;
            existingAccount.AccountEmail = AccountDto.AccountEmail;
            existingAccount.AccountRole = AccountDto.AccountRole;
            existingAccount.LocationId = AccountDto.LocationId;

            try
            {
                // Save changes to the database
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.Error;
                serviceResponse.Messages.Add("A concurrency error occurred while updating the record");
                return serviceResponse;
            }
            catch (Exception ex)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.Error;
                serviceResponse.Messages.Add($"An unexpected error occurred: {ex.Message}");
                return serviceResponse;
            }

            // Success response
            serviceResponse.Status = ServiceResponse.ServiceStatus.Updated;
            serviceResponse.Messages.Add("Account updated successfully");
            return serviceResponse;
        }



        public async Task<ServiceResponse> AddAccount(AccountDto AccountDto)
        {
            ServiceResponse serviceResponse = new();


            // Create instance of Account
            Account Account = new Account()
            {
                AccountName = AccountDto.AccountName,
                AccountEmail = AccountDto.AccountEmail,
                AccountRole = AccountDto.AccountRole,
                AccountPassword = AccountDto.AccountPassword,
                LocationId = AccountDto.LocationId,
            };
            // SQL Equivalent: Insert into Accounts (..) values (..)

            try
            {
                _context.Accounts.Add(Account);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.Error;
                serviceResponse.Messages.Add("There was an error adding the Account.");
                serviceResponse.Messages.Add(ex.Message);
            }


            serviceResponse.Status = ServiceResponse.ServiceStatus.Created;
            serviceResponse.CreatedId = Account.AccountId;
            return serviceResponse;
        }


        public async Task<ServiceResponse> DeleteAccount(int id)
        {
            ServiceResponse response = new();
            // account Item must exist in the first place
            var Account = await _context.Accounts.FindAsync(id);
            if (Account == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                response.Messages.Add("Account cannot be deleted because it does not exist.");
                return response;
            }

            try
            {
                _context.Accounts.Remove(Account);
                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("Error encountered while deleting the Account");
                return response;
            }

            response.Status = ServiceResponse.ServiceStatus.Deleted;

            return response;

        }

        public async Task<IEnumerable<AccountDto>> ListAccountsForPet(int id)
        {
            // join AccountPet on Accounts.Accountid = AccountPet.Accountid WHERE AccountPet.Petid = {id}
            List<Account> Accounts = await _context.Accounts
                .Where(c => c.Pets.Any(p => p.PetId==id))
                .ToListAsync();

            // empty list of data transfer object AccountDto
            List<AccountDto> AccountDtos = new List<AccountDto>();
            // foreach account record in database
            foreach (Account Account in Accounts)
            {
                // create new instance of AccountDto, add to list
                AccountDtos.Add(new AccountDto()
                {
                    AccountId = Account.AccountId,
                    AccountName = Account.AccountName,
                    AccountEmail = Account.AccountEmail,
                    AccountRole = Account.AccountRole,
                    AccountPassword = Account.AccountPassword,
                    LocationId = Account.LocationId,
                    LocationAddress = Account.Location?.Address

                });
            }
            // return AccountDtos
            return AccountDtos;

        }

        public async Task<ServiceResponse> LinkAccountToPet(int AccountId, int PetId)
        {
            ServiceResponse serviceResponse = new();

            Account? Account = await _context.Accounts
                .Include(c => c.Pets)
                .Where(c => c.AccountId== AccountId)
                .FirstOrDefaultAsync();
            Pet? Pet = await _context.Pets.FindAsync(PetId);

            // Data must link to a valid entity
            if (Pet == null || Account == null) { 
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
            try
            {
                Account.Pets.Add(Pet);
                _context.SaveChanges();
            }
            catch(Exception Ex)
            {
                serviceResponse.Messages.Add("There was an issue linking the Pet to the Account");
                serviceResponse.Messages.Add(Ex.Message);
            }


            serviceResponse.Status = ServiceResponse.ServiceStatus.Created;
            return serviceResponse;
        }

        public async Task<ServiceResponse> UnlinkAccountFromPet(int AccountId, int PetId)
        {
            ServiceResponse serviceResponse = new();

            Account? Account = await _context.Accounts
                .Include(c => c.Pets)
                .Where(c => c.AccountId == AccountId)
                .FirstOrDefaultAsync();
            Pet? Pet = await _context.Pets.FindAsync(PetId);

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
            try
            {
                Account.Pets.Remove(Pet);
                _context.SaveChanges();
            }
            catch (Exception Ex)
            {
                serviceResponse.Messages.Add("There was an issue unlinking the Pet to the Account");
                serviceResponse.Messages.Add(Ex.Message);
            }


            serviceResponse.Status = ServiceResponse.ServiceStatus.Deleted;
            return serviceResponse;
        }
    }
}
