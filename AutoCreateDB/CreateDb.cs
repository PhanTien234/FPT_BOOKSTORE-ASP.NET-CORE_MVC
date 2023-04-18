using FPT_BOOKSTORE.Data;
using FPT_BOOKSTORE.Models;
using FPT_BOOKSTORE.Utility.cs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FPT_BOOKSTORE.AutoCreateDB;

public class CreateDb : IAutoCreateDb
{
    private readonly ApplicationDbContext _db;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<IdentityUser> _userManager;

    public CreateDb(ApplicationDbContext db, UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        _db = db;
        _userManager = userManager;
        _roleManager = roleManager;
    }
    
    public void CreateDB()
    {
        // checking database, if not migration then migrate
        // cai nay dung de auto update database (luu y ko auto migrations db, ma phai migrations trc moi duoc)
            try
            {
                if (_db.Database.GetPendingMigrations().Any()) 
                {
                    _db.Database.Migrate();
                    Console.WriteLine("Migrations applied successfully.");
                }
                else
                {
                    Console.WriteLine("No pending migrations context.");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error applying migrations: " + e.Message);
                throw;
            }


            // checking in table Role, if yes then return, if not deploy the codes after these conditions ( phan nay chi check roles)
            if (_db.Roles.Any(r => r.Name == Constraintt.AdminRole)) return;
            if (_db.Roles.Any(r => r.Name == Constraintt.StoreOwnerRole)) return;
            if (_db.Roles.Any(r => r.Name == Constraintt.CustomerRole)) return;

            // this will deploy if there no have any role yet ( add cai role vao role manger)
            _roleManager.CreateAsync(new IdentityRole(Constraintt.AdminRole)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(Constraintt.StoreOwnerRole)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(Constraintt.CustomerRole)).GetAwaiter().GetResult();

            // create user admin ( cai nay no se tao san mot thang user admin moi khi ma ung dung khoi chay)
            _userManager.CreateAsync(new User()
            {
                UserName = "admin@gmail.com",
                Email = "fpt@gmail.com",
                FullName = "Admin",
                PhoneNumber = "1234566",
                HomeAddress = "Admin123",
                EmailConfirmed = true,
            }, "Tien123@").GetAwaiter().GetResult();


            // finding the user which is just have created (tao admin object)
            var admin = _db.Users.FirstOrDefault(a => a.Email == "admin@gmail.com");

            // add that user (admin) to admin role ( sau do add role cho admin roi tao ra no)
            _userManager.AddToRoleAsync(admin, Constraintt.AdminRole).GetAwaiter().GetResult();
    }
}