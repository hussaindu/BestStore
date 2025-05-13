using BestStore.Models;
using BestStore.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseSqlServer(connectionString);
});

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(
    options =>
    {
        options.Password.RequiredLength = 6;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireLowercase= false;

    }).AddEntityFrameworkStores<ApplicationDbContext>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

//create the role 
using(var scope = app.Services.CreateScope())
{
    var userManager =scope.ServiceProvider.GetService(typeof(UserManager<ApplicationUser>))
    as UserManager<ApplicationUser>;

    var roleManager=scope.ServiceProvider.GetService(typeof (RoleManager<IdentityRole>))
       as RoleManager<IdentityRole>;

    await DataBaseInitializer.SeedDataAsync(userManager, roleManager);
}



app.Run();
