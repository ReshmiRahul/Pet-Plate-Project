using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;

namespace PetAdoption.Models
{
    public class Pet
    {
        // PetID is primary key
        [Key]
        public int PetId { get; set; }

        public string PetName { get; set; }

        public string PetType { get; set; }

        public string PetBreed { get; set; }

        public int PetAge { get; set; }

        public string PetDescription { get; set; }

        public int PetStatus { get; set; }

        // Navigation property for many-to-many relationship with Account
        public ICollection<Account> Accounts { get; set; }

        // Navigation property for the one-to-many relationship with Application
        public ICollection<Application> Applications { get; set; }
    }

    public class PetDto
    {
        public int? PetId { get; set; }

        public string PetName { get; set; }

        public string? PetType { get; set; }

        public string? PetBreed { get; set; }

        public int? PetAge { get; set; }

        public string? PetDescription { get; set; }

        public int? PetStatus { get; set; }
    }
}
