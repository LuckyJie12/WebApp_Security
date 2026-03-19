using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebAPP.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();


builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddIdentity<IdentityUser, IdentityRole>(option =>
{
    option.Password.RequiredLength = 8;
    option.Password.RequireLowercase=true;
    option.Password.RequireUppercase=true;

    option.Lockout.MaxFailedAccessAttempts = 5;
    option.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
    option.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.ConfigureApplicationCookie(option =>
{
    option.LoginPath = "/Account/Login";
    option.AccessDeniedPath = "/Account/AccessDenied";
    option.ExpireTimeSpan = TimeSpan.FromMinutes(60);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
