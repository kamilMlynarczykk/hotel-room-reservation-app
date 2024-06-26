using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using HotelReservationApp.Data;
using HotelReservationApp.Controllers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<HotelReservationAppContext>()
    .AddDefaultTokenProviders();

builder.Services.AddMemoryCache();
builder.Services.AddSession();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie();

builder.Services.AddDbContext<HotelReservationAppContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("HotelReservationAppContext") ?? throw new InvalidOperationException("Connection string 'HotelReservationAppContext' not found.")));

builder.Services.AddHostedService<ExpiredReservationBackgroundService>();
builder.Services.AddScoped<ReservationHistoriesController>();

builder.Services.AddHostedService<ReservationCleanupService>();
builder.Services.AddScoped<ReservationHistoriesController>();
var app = builder.Build();

if (args.Length == 1 && args[0].ToLower().Equals("seeddata"))
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;
    try
    {
        Seed.SeedData(services);
        await Seed.SeedUsersAndRolesAsync(app);
        Seed.SeedReservationHistory(services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred seeding the DB.");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Rooms}/{action=Index}/{id?}");

app.Run();
