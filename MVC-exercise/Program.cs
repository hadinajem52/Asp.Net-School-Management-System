using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MVC_exercise.Data;
using MVC_exercise.Models;
using MVC_exercise.Services;

var builder = WebApplication.CreateBuilder(args);



// Add services to the container.
builder.Services.AddControllersWithViews();


builder.Services.AddDbContext<SchoolDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("SchoolDatabase")));
builder.Services.AddScoped<SchoolDataService>();


builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
        options.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<SchoolDbContext>()
    .AddDefaultTokenProviders();



builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});



var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    await IdentityDataSeeder.SeedAsync(scope.ServiceProvider);
    await SchoolDataSeeder.SeedAsync(scope.ServiceProvider.GetRequiredService<SchoolDbContext>());
}



// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}



// Middleware: participates in the request pipeline
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();


// Endpoint mapping: describes which code handles matching requests
app.MapStaticAssets();

// this is the default route for the application.
//  It maps incoming requests to the appropriate controller and action method based on the URL pattern.
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
