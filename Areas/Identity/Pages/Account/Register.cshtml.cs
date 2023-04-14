// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using FPT_BOOKSTORE.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;

namespace FPT_BOOKSTORE.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserStore<IdentityUser> _userStore;
        private readonly IUserEmailStore<IdentityUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RegisterModel(
            UserManager<IdentityUser> userManager,
            IUserStore<IdentityUser> userStore,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender, 
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _userStore = userStore;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _roleManager = roleManager;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            
            [Required]
            [Display(Name = "Full Name")]
            public string FullName { get; set; }
            
            
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
            
            [Required]
            [Display(Name = "Your phone number")]
            public string PhoneNum { get; set; }
            
            // address and rolelist
            [Required]
            [Display(Name = "Your Address")]
            public string HomeAddress { get; set; }
            
            [Required] 
            public string Role { get; set; }
            
            // select list items for each role
            public IEnumerable<SelectListItem> SelectYourRole { get; set; }
        }


        public async Task OnGetAsync(string returnUrl = null)
        {
            // get roles len tren cung de no chay dau tien truoc khi return toi URL
            GetRoles();
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = new User()
                {
                    // o day phai dugn email lam mac dinh username vi theo requirement ko can user name ma ke thua thang 
                    // Idenitty user thi bac buoc phai co user name minh se auto gan username bang email
                    UserName = Input.Email,
                    Email = Input.Email,
                    FullName = Input.FullName,
                    EmailConfirmed = true,
                    HomeAddress = Input.HomeAddress,
                    PhoneNum = Input.PhoneNum,
                };

                // phan nay se validate role cho drop down list role neu no co gia tri la 3 roles nhu duoi thi se add vao usermanger cho moi role
                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    if (Input.Role == "Customer")
                    {
                        await _userManager.AddToRolesAsync(user, new[] { "Customer" });
                    }
                    
                    if (Input.Role == "StoreOwner")
                    {
                        await _userManager.AddToRolesAsync(user, new[] { "StoreOwner" });
                    }


                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(returnUrl);
                }
                
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // Also calls this getRoles method here for init role when user submit the resiger form
            GetRoles();
            // If we got this far, something failed, redisplay form
            return Page();
        }

        // private IdentityUser CreateUser()
        // {
        //     try
        //     {
        //         return Activator.CreateInstance<IdentityUser>();
        //     }
        //     catch
        //     {
        //         throw new InvalidOperationException($"Can't create an instance of '{nameof(IdentityUser)}'. " +
        //             $"Ensure that '{nameof(IdentityUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
        //             $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
        //     }
        // }

        // private IUserEmailStore<IdentityUser> GetEmailStore()
        // {
        //     if (!_userManager.SupportsUserEmail)
        //     {
        //         throw new NotSupportedException("The default UI requires a user store with email support.");
        //     }
        //     return (IUserEmailStore<IdentityUser>)_userStore;
        // }
        
        // get roles function
        private void GetRoles()
        {
            Input = new InputModel()
            {
                SelectYourRole = _roleManager.Roles.Where(x => x.Name != "Admin")
                    .Select(x => x.Name).Select(x => new SelectListItem()
                    {
                        Text = x,
                        Value = x
                    })
            };
        }
    }
}
