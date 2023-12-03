using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ContactManager.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using ContactManager.Authorization;

// Create the WebApplication instance
var builder = WebApplication.CreateBuilder(args);

// Retrieve the connection string from the configuration file
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Add the ApplicationDbContext to the services with SQL Server as the database provider
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Add the developer exception page to the services
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Configure the default identity system
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
    options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>() // Add support for roles
    .AddEntityFrameworkStores<ApplicationDbContext>(); // Use ApplicationDbContext for storing user and role data

// Register Razor Pages as part of the application's services
builder.Services.AddRazorPages();

// Configure authorization policies
builder.Services.AddAuthorization(options =>
{
    // Fallback policy: Requires any user accessing a protected resource to be authenticated
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

// Register custom authorization handlers for the application
builder.Services.AddScoped<IAuthorizationHandler, ContactIsOwnerAuthorizationHandler>();
builder.Services.AddSingleton<IAuthorizationHandler, ContactAdministratorsAuthorizationHandler>();
builder.Services.AddSingleton<IAuthorizationHandler, ContactManagerAuthorizationHandler>();

// Build the WebApplication instance
var app = builder.Build();

// Apply any pending database migrations and initialize seed data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    context.Database.Migrate();

    // Retrieve test user passwords from the configuration
    var testAdminPw = builder.Configuration.GetValue<string>("SeedAdminPW");
    var testManagerPw = builder.Configuration.GetValue<string>("SeedManagerPW");

    // Initialize seed data with test users
    await SeedData.Initialize(services, testAdminPw: testAdminPw, testManagerPw: testManagerPw);
}

// Configure middleware for handling HTTP redirections to HTTPS and serving static files
app.UseHttpsRedirection();
app.UseStaticFiles();

// Configure middleware for routing incoming requests to appropriate endpoints
app.UseRouting();

// Configure middleware for handling user authentication
app.UseAuthentication();

// Configure middleware for handling user authorization
app.UseAuthorization();

// Configure endpoints for the application
app.UseEndpoints(endpoints =>
{
    // Map Razor Pages to the application's endpoints
    endpoints.MapRazorPages();

    // Define a default controller route for handling requests when no specific route matches
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});

// Map Razor Pages to the application's endpoints
app.MapRazorPages();

// Start the application, making it ready to listen for incoming HTTP requests
app.Run();
