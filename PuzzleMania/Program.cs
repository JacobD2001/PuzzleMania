using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PuzzleMania.Areas.Identity.Data;
using PuzzleMania.Data;
using PuzzleMania.Helpers;
using PuzzleMania.Interfaces;
using PuzzleMania.Repositories;
using PuzzleMania.Services;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("PuzzleManiaContextConnection") ?? throw new InvalidOperationException("Connection string 'PuzzleManiaContextConnection' not found.");

//adding db context
builder.Services.AddDbContext<PuzzleManiaContext>(options => options.UseSqlServer(connectionString));

//adding identity
builder.Services.AddDefaultIdentity<PuzzleManiaUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<PuzzleManiaContext>();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));


//add services for DI
builder.Services.AddScoped<ITeamRepository, TeamRepository>();
builder.Services.AddScoped<IProfileRepository, ProfileRepository>();
builder.Services.AddScoped<IPhotoService, PhotoService>();
builder.Services.AddScoped<IRiddleRepository, RiddleRepository>();


var app = builder.Build();

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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
