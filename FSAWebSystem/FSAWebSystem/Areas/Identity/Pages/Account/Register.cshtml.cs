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
using AspNetCoreHero.ToastNotification.Abstractions;

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
        private readonly IUserService _userService;
        private readonly INotyfService _notyfService;
        public RegisterModel(
            UserManager<FSAWebSystemUser> userManager,
            IUserStore<FSAWebSystemUser> userStore,
            SignInManager<FSAWebSystemUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            FSAWebSystemDbContext db, IBannerService bannerService, IRoleService roleService, IUserService userService, INotyfService notyfService)
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
            _userService = userService;
            _notyfService = notyfService;
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

        public async Task<IActionResult> OnPostAsync(string[] bannerIds, string roleId, string worklevelId, string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            await FillDropdowns(ViewData);

            if(string.IsNullOrEmpty(worklevelId))
            {
                ModelState.AddModelError(string.Empty, "Work Level must be filled!");
                _notyfService.Warning("Create user failed!");
                return Page();
            }


            if (ModelState.IsValid)
            {
                var savedUser = await _userService.GetUserByEmail(Input.Email);
                var savedUserLogin = await _userManager.FindByEmailAsync(Input.Email);

                if(savedUser == null && savedUserLogin == null)
                {
                    var userUnilever = await _userService.CreateUser(Input.Name, Input.Email, Input.Password, bannerIds, roleId, worklevelId, User.Identity.Name, _userStore, _emailStore);
                    if (userUnilever.Message != null)
                    {
                        foreach (var error in userUnilever.Message)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                            await FillDropdowns(ViewData);
                            return Page();
                        }
                    }

                    _notyfService.Success("User " + userUnilever.Name + " successfully added");
                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "User Already exist!");
                }
               
            }

                return Page();
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
            await _userService.FillWorkLevelDropdown(viewData);
        }
    }
}
