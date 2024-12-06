using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PetAdoption.Models;

namespace Pet_Adoption_Project.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Pet> Pets { get; set; }
        public DbSet<Application> Applications { get; set; }
        public DbSet<FoodTruck> FoodTrucks { get; set; }
        public DbSet<MenuItem> MenuItems { get; set; }
        public DbSet<Location> Locations { get; set; }
    }
}
