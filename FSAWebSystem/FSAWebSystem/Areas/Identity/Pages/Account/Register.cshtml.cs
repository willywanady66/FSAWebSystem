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
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using FSAWebSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using FSAWebSystem.Areas.Identity.Data;
using FSAWebSystem.Models.Context;
using FSAWebSystem.Services.Interface;
using FSAWebSystem.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace FSAWebSystem.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<FSAWebSystemUser> _signInManager;
        private readonly UserManager<FSAWebSystemUser> _userManager;
        private readonly IUserStore<FSAWebSystemUser> _userStore;
        private readonly IUserEmailStore<FSAWebSystemUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly FSAWebSystemDbContext _db;
        private readonly IBannerService _bannerService;
        private readonly IRoleService _roleService;
        public RegisterModel(
            UserManager<FSAWebSystemUser> userManager,
            IUserStore<FSAWebSystemUser> userStore,
            SignInManager<FSAWebSystemUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            FSAWebSystemDbContext db, IBannerService bannerService, IRoleService roleService)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _db = db;
            _bannerService = bannerService;
            _roleService = roleService;

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

            [Required]
            [Display(Name = "Name")]
            public string Name { get; set; }
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

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


            public string Roles { get; set; }
            public List<Banner> Banners { get; set; }
            public List<SelectListItem> ListBanners { get; set; }
            [BindProperty]
            public string[] SelectedId { get; set; }
        }


        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
 
            await FillDropdowns(ViewData);
        }

        public async Task<IActionResult> OnPostAsync(string[] bannerIds, string roleId, string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = CreateUser();

                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                    var selectedBannerIds = bannerIds.Select(x => Guid.Parse(x)).ToList();
                    var selectedBanners = (_bannerService.GetAllBanner().ToList()).Where(x => selectedBannerIds.Contains(x.Id)).ToList();

                    var userUnilever = new UserUnilever
                    {
                        Name = Input.Name,
                        Email = Input.Email,
                        //Password = Input.Password,
                        Id = Guid.NewGuid(),
                        CreatedAt = DateTime.Now,
                        CreatedBy = User.Identity.Name,
                        RoleUnilever = await _roleService.GetRole(Guid.Parse(roleId)),
                        Banners = selectedBanners
                    };

                    _db.UsersUnilever.Add(userUnilever);
                    _db.SaveChanges();

                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            else
            {
                await FillDropdowns(ViewData);
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        private FSAWebSystemUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<FSAWebSystemUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(User)}'. " +
                    $"Ensure that '{nameof(User)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<FSAWebSystemUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<FSAWebSystemUser>)_userStore;
        }


        private async Task FillDropdowns(ViewDataDictionary viewData)
        {
            await _bannerService.FillBannerDropdown(viewData);
            await _roleService.FillRoleDropdown(viewData);
        }
    }
}
