namespace PetAdoption.Models.ViewModels
{
    public class AccountDetails
    {
        // Account details must have an AccountDto object
        public required AccountDto Account { get; set; }

        // An account can have many pets associated with it
        public IEnumerable<PetDto>? AssociatedPets { get; set; }

        // An account can have many applications associated with it
        public IEnumerable<ApplicationDto>? AccountApplications { get; set; }
        public List<LocationDto> Locations { get; set; }
    }
}
