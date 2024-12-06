namespace PetAdoption.Models.ViewModels
{
    public class PetDetails
    {
        // Pet details must have a PetDto object
        public required PetDto Pet { get; set; }

        // A pet can have many accounts associated with it (e.g., adopters)
        public IEnumerable<AccountDto>? AssociatedAccounts { get; set; }

        // A pet can have many applications associated with it
        public IEnumerable<ApplicationDto>? PetApplications { get; set; }
    }
}
