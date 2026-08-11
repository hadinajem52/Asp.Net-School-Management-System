using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using razor_exercise.Data;
using razor_exercise.Middleware;
using razor_exercise.Models;
using razor_exercise.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages(options =>
{
    // Every Razor Page requires a signed-in user unless it is explicitly allowed below.
    options.Conventions.AuthorizeFolder("/");
    options.Conventions.AllowAnonymousToPage("/Account/Login");
    options.Conventions.AllowAnonymousToPage("/Account/Register");
    options.Conventions.AllowAnonymousToPage("/Error");
});
builder.Services.AddServerSideBlazor();



string connectionString = builder.Configuration.GetConnectionString("SchoolDatabase")
    ?? throw new InvalidOperationException("The SchoolDatabase connection string is missing.");




// registering the SchoolDbContext with the dependency injection container, 
builder.Services.AddDbContext<SchoolDbContext>(options =>

    options.UseNpgsql(connectionString)
    
    );




// registering the SchoolDataService with the DI container as a scoped service
builder.Services.AddScoped<SchoolDataService>();
builder.Services.AddScoped<AccountApprovalService>();



// Register Identity's account, password, role, and cookie services.
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
    })
    // register service that stores users and roles in the SchoolDbContext database
    .AddEntityFrameworkStores<SchoolDbContext>()
    // register service that generates tokens for password reset, email confirmation...
    .AddDefaultTokenProviders();




// Configure the application cookie settings in the login and access denied paths for the application.
builder.Services.ConfigureApplicationCookie(options =>
{
    // when unauthenticated users try to access a page that requires authentication, they will be redirected to the login page.
    options.LoginPath = "/Account/Login";

    // when authenticated users try to access a page that they are not authorized to access, they will be redirected to the access denied page.
    options.AccessDeniedPath = "/Account/AccessDenied";
});


builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
});






var app = builder.Build();

// Create the initial Identity roles and local administrator account when the application starts.
using (var scope = app.Services.CreateScope())
{
    await IdentityDataSeeder.SeedAsync(scope.ServiceProvider);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseMiddleware<RequestLoggingMiddleware>();

// Read the Identity authentication cookie and set HttpContext.User for this request.
app.UseAuthentication();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();
app.MapBlazorHub();

app.Run();
