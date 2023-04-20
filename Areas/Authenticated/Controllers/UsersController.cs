using System.Security.Claims;
using FPT_BOOKSTORE.Data;
using FPT_BOOKSTORE.Models;
using FPT_BOOKSTORE.Utility.cs;
using FPT_BOOKSTORE.VM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FPT_BOOKSTORE.Controllers;

    [Area(Constraintt.AuthenticatedArea)]
    [Authorize(Roles = Constraintt.AdminRole)]

    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManger;

        // GET
        public UsersController(ApplicationDbContext db, UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _roleManger = roleManager;
        }
        
        public async Task<IActionResult> Index()
        {
            // taking current login user id
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            // exception itself admin
            var userList = _db.Users.Where(u => u.Id != claims.Value);

            foreach (var user in userList)
            {
                var userTemp = await _userManager.FindByIdAsync(user.Id);
                var roleTemp = await _userManager.GetRolesAsync(userTemp);
                user.Role = roleTemp.FirstOrDefault();
            }


            return View(userList.ToList());
        }


        // lock and unlock
        [HttpGet]
        public async Task<IActionResult> LockUnlock(string id)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var userNeedToLock = _db.Users.Where(u => u.Id == id).First();

            if (userNeedToLock.Id == claims.Value)
            {
                // hien ra loi ban dang khoa tai khoan cua chinh minh
            }

            if (userNeedToLock.LockoutEnd != null && userNeedToLock.LockoutEnd > DateTime.Now)
                userNeedToLock.LockoutEnd = DateTime.Now;
            else
                userNeedToLock.LockoutEnd = DateTime.Now.AddYears(1000);

            _db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        
        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();
            await _userManager.DeleteAsync(user);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string id)
        {
            var user = _db.Users.Find(id);

            if (user == null) return View();

            var confirmEmailVm = new ConfirmEmailVM()
            {
                Email = user.Email
            };

            return View(confirmEmailVm);
        }
        
        [HttpGet]
        public async Task<IActionResult> ResetPassword(string token, string email)
        {
            if (token == null || email == null) ModelState.AddModelError("", "Invalid password reset token");

            var resetPasswordViewModel = new ResetPasswordVM()
            {
                Email = email,
                Token = token
            };

            return View(resetPasswordViewModel);
        }
        
        [HttpPost]
        public async Task<IActionResult> ConfirmEmail(ConfirmEmailVM confirmEmailVm)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(confirmEmailVm.Email);
                if (user != null)
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                    return RedirectToAction("ResetPassword", "Users"
                        , new { token, email = user.Email });
                }
            }

            return View(confirmEmailVm);
        }
        
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM resetPasswordViewModel)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(resetPasswordViewModel.Email);
                if (user != null)
                {
                    var result = await _userManager.ResetPasswordAsync(user, resetPasswordViewModel.Token,
                        resetPasswordViewModel.Password);
                    if (result.Succeeded) return RedirectToAction(nameof(Index));
                }
            }

            return View(resetPasswordViewModel);
        }


        
        [HttpGet]
        public IActionResult EditUser(string id)
        {
            var adminStoreOwnerCustomer = _db.Users.Find(id);
            return View(adminStoreOwnerCustomer);
        }

        [HttpPost]
        public IActionResult EditUser(User user)
        {
            var adminStoreOwnerCustomer = _db.Users.Find(user.Id);
            if (adminStoreOwnerCustomer == null)
            {
                return NotFound("User is null");
            }

            adminStoreOwnerCustomer.FullName = user.FullName;
            adminStoreOwnerCustomer.PhoneNumber = user.PhoneNumber;
            adminStoreOwnerCustomer.HomeAddress = user.HomeAddress;
                
            _db.Users.Update(adminStoreOwnerCustomer);
            _db.SaveChanges();
            
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(string id)
        {
            var user = _db.Users.Find(id);
            var roletemp = await _userManager.GetRolesAsync(user);
            var role = roletemp.First();

            return RedirectToAction("EditUser", new { id });
        }
        
        // list all request categories to admin
        [Authorize(Roles = Constraintt.AdminRole)]
        [HttpGet]
        public async Task<IActionResult> Requests()
        {
            var requests = await _db.Categories.Where(_ => _.Status == Category.StatusCategory.Pending).ToListAsync();

            return View(requests);

        }
        
        [Authorize(Roles = Constraintt.AdminRole)]
        // this will be used for approve category
        [HttpGet] // not http post because it's just trigger the function not pushing st
        public async Task<IActionResult> ApproveRequest(int id)
        {
            var categoryToApprove = await _db.Categories.FindAsync(id);
            if (categoryToApprove == null)
                return NotFound("The request not found!");

            // approve it if there's one
            categoryToApprove.Status = Category.StatusCategory.Approve;
            await _db.SaveChangesAsync();
            Console.WriteLine("======================> Approve request successfully");

            return RedirectToAction("Requests");
        }
        
        
        [Authorize(Roles = Constraintt.AdminRole)]
        // this will be used for reject category
        [HttpGet] // not http post because it's just trigger the function not pushing st
        public async Task<IActionResult> RejectRequest(int id)
        {
            var categoryToReject = await _db.Categories.FindAsync(id);
            if (categoryToReject == null)
                return NotFound("The request not found!");

            // reject it if there's one
            categoryToReject.Status = Category.StatusCategory.Reject;
            // also you can choose which way to deal with this data this line above just change the status of request not (delete yet) 
            _db.Categories.Remove(categoryToReject); // this will delete 
            await _db.SaveChangesAsync();
            Console.WriteLine("======================> Reject request successfully");

            return RedirectToAction("Requests");
        }

    }