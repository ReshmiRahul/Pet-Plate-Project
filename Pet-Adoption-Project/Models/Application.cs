using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Drawing;

namespace PetAdoption.Models
{
    public class Application
    {
        // ApplicationID is primary key
        [Key]
        public int ApplicationID { get; set; }

        // Foreign key referencing Account
        [ForeignKey("Account")]
        public int AccountId { get; set; }

        // Foreign key referencing Pet
        [ForeignKey("Pet")]
        public int PetId { get; set; }

        [ForeignKey("FoodTruck")]
        public int? FoodTruckId { get; set; }

        public DateTime ApplicationDate { get; set; }

        public int ApplicationStatus { get; set; }

        public string ApplicationReason { get; set; }

        public string ApplicationExperience { get; set; }

        public string ApplicationComments { get; set; }

        // Navigation property for the relationship with Account
        public Account Account { get; set; }

        // Navigation property for the relationship with Pet
        public Pet Pet { get; set; }

        // Navigation property for the relationship with FoodTruck
        public FoodTruck? FoodTruck { get; set; }
    }
    public class ApplicationDto
    {
        public int? ApplicationID { get; set; }

        public int AccountId { get; set; }  // Required to establish relationship with Account

        public int PetId { get; set; }  // Required to establish relationship with Pet

        public DateTime? ApplicationDate { get; set; }

        public int? ApplicationStatus { get; set; }

        public string? ApplicationReason { get; set; }

        public string? ApplicationExperience { get; set; }

        public string? ApplicationComments { get; set; }

        //  Account and Pet names for reference
        public string? AccountName { get; set; }
        public string? PetName { get; set; }

        public int? FoodTruckId { get; set; }
        public string? FoodTruckName { get; set; }
    }
}
