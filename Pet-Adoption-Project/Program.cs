using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Pet_Adoption_Project.Data;
using PetAdoption.Interfaces;
using PetAdoption.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Add service interfaces with their implementations
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IPetService, PetService>();
builder.Services.AddScoped<IApplicationService, ApplicationService>();
builder.Services.AddScoped<IFoodTruckService, FoodTruckService>();
builder.Services.AddScoped<ILocationService, LocationService>();
builder.Services.AddScoped<IMenuItemService, MenuItemService>();

builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
    options.Password.RequireNonAlphanumeric = false; // Optional: adjust password settings
    options.Password.RequiredLength = 8; // Optional: set minimum password length
})
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Add session services
builder.Services.AddDistributedMemoryCache(); // Session cache
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set session timeout
    options.Cookie.HttpOnly = true; // Makes the session cookie more secure
    options.Cookie.IsEssential = true; // Essential for the app to function
});

builder.Services.AddControllersWithViews();

// Add Swagger for API documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseMigrationsEndPoint();

    // Enable Swagger in Development mode
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Pet Adoption API V1");
        c.RoutePrefix = "swagger"; // Swagger is now available at /swagger
    });
}

else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Enable session middleware here
app.UseSession(); // <-- Add this line to enable session functionality

app.UseAuthentication(); // Ensure authentication middleware is added
app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
