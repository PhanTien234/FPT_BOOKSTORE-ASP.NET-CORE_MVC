using FPT_BOOKSTORE.AutoCreateDB;
using FPT_BOOKSTORE.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("lylyConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); //The time cookie available
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
// add automatic create db service here
builder.Services.AddScoped<IAutoCreateDb, CreateDb>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}



app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
app.UseSession();

// inject or execute it
using (var scope = app.Services.CreateScope())
{
    var dbCreate = scope.ServiceProvider.GetRequiredService<IAutoCreateDb>();
    dbCreate.CreateDB();
}

app.MapControllerRoute(
    name: "default",
    "{area=UnAuthenticated}/{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
