using System.ComponentModel.DataAnnotations;
using static System.Net.Mime.MediaTypeNames;
using System.Drawing;

namespace PetAdoption.Models
{
    public class Account
    {
        //accountid is the primary key
        [Key]
        public int AccountId { get; set; }
        public string AccountName { get; set; }
        public string AccountPassword { get; set; }
        public string AccountEmail { get; set; }
        public string AccountRole { get; set; }
        // Foreign key for Location
        public int LocationId { get; set; }

        // Navigation property for Location
        public Location Location { get; set; }


        // Navigation property for many-to-many relationship with Pet
        public ICollection<Pet> Pets { get; set; }

        // Navigation property for one-to-many relationship with Application
        public ICollection<Application> Applications { get; set; }

    }

    public class AccountDto
    {
        public int? AccountId { get; set; }

        public string AccountName { get; set; }

        public string? AccountEmail { get; set; }

        public string? AccountRole { get; set; }

        public string? AccountPassword { get; set; }
        public int LocationId { get; set; }
        public string? LocationAddress { get; set; }
    }
}
