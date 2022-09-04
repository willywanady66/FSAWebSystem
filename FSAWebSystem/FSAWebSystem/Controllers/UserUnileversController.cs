using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FSAWebSystem.Models;
using FSAWebSystem.Models.Context;
using FSAWebSystem.Services.Interface;
using FSAWebSystem.Models.ViewModels;
using System.Dynamic;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using FSAWebSystem.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using AspNetCoreHero.ToastNotification.Abstractions;

namespace FSAWebSystem.Controllers
{
    public class UserUnileversController : Controller
    {
        private readonly FSAWebSystemDbContext _context;
        private readonly IBannerService _bannerService;
        private readonly IRoleService _roleService;
        private readonly IUserService _userService;
        private readonly UserManager<FSAWebSystemUser> _userManager;
        private readonly INotyfService _notyfService;
        private readonly ISKUService _skuService;

        public UserUnileversController(FSAWebSystemDbContext context, IBannerService bannerService, IRoleService roleService, IUserService userService, UserManager<FSAWebSystemUser> userManager, INotyfService notyfService, ISKUService skuService)
        {
            _context = context;
            _bannerService = bannerService;
            _roleService = roleService;
            _userService = userService;
            _userManager = userManager;
            _notyfService = notyfService;
            _skuService = skuService;
        }

        // GET: UserUnilevers/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.UsersUnilever == null)
            {
                return NotFound();
            }

            var userUnilever = await _context.UsersUnilever
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userUnilever == null)
            {
                return NotFound();
            }


            return View(userUnilever);
        }
        // GET: UserUnilevers/Edit/5
        public async Task<IActionResult> Edit(Guid id)
        {
            var userUnilever = await _userService.GetUser(id);
            var user = await _userManager.FindByEmailAsync(userUnilever.Email);
            
            userUnilever.UserId = user.Id;
            if (userUnilever == null)
            {
                return NotFound();
            }

            await FillDropdowns(ViewData);
            var listBanner = (List<SelectListItem>)ViewData["ListBanner"];
            var listRole = (List<SelectListItem>)ViewData["ListRole"];
            var listWorkLevel = (List<SelectListItem>)ViewData["ListWorkLevel"];
            var listSku = (List<SelectListItem>)ViewData["ListSku"];
            var listCategory = (List<SelectListItem>)ViewData["ListCategory"];
            var userBanner = userUnilever.Banners.Select(x => x.Id).ToList();

            var selectedBanner = listBanner.Where(x => userBanner.Contains(Guid.Parse(x.Value))).ToList();
            var selectedWorkLevel = listWorkLevel.SingleOrDefault(x => userUnilever.WLId == Guid.Parse(x.Value));
            if(selectedWorkLevel != null)
            {
                selectedWorkLevel.Selected = true;
            }
            foreach (var item in selectedBanner)
            {
                item.Selected = true;
            }

            var userSkuIds = userUnilever.SKUs.Select(x => x.Id).ToList();
            var selectedSku = listSku.Where(x => userSkuIds.Contains(Guid.Parse(x.Value))).ToList();
            foreach(var item in selectedSku)
            {
                item.Selected = true;
            }

            var userProdCategIds = userUnilever.ProductCategories.Select(x => x.Id).ToList();
            var selectedCateg = listCategory.Where(x => userProdCategIds.Contains(Guid.Parse(x.Value))).ToList();
            foreach (var item in selectedCateg)
            {
                item.Selected = true;
            }

            var userRole = listRole.SingleOrDefault(x => Guid.Parse(x.Value) == userUnilever.RoleUnilever.RoleUnileverId);
            if (userRole != null) userRole.Selected = true;

            return View(userUnilever);
        }

        // POST: UserUnilevers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind ("Id,Name,Email,IsActive,UserId,CreatedAt,CreatedBy")] UserUnilever userUnilever, string[] bannerIds, string roleUnileverId, string workLevelId, string[] skuIds, string[] categoryIds)
        {
            if (id != userUnilever.Id)
            {
                return NotFound();
            }
            ModelState.Remove("RoleUnilever");
            ModelState.Remove("UserId");
            ModelState.Remove("BannerName");
            ModelState.Remove("Password"); 
            ModelState.Remove("Role");
            ModelState.Remove("Message");
            ModelState.Remove("Status");
            ModelState.Remove("WLName");
            ModelState.Remove("WLId");
            ModelState.Remove("PlantCode");
            //ModelState.Remove("WorkLevelId");
            if (ModelState.IsValid)
            {
                await FillDropdowns(ViewData);
                try
                {
                    var user = await _userManager.FindByIdAsync(userUnilever.UserId);
                    var savedUser = await _userService.GetUser((Guid)user.UserUnileverId);

                    List<Guid> selectedBannerId = (from bannerId in bannerIds select Guid.Parse(bannerId)).ToList();
                    var selectedBanners = (_bannerService.GetAllBanner().ToList()).Where(x => selectedBannerId.Contains(x.Id)).ToList();
                    if ((savedUser == null || savedUser.Id == userUnilever.Id) && (user == null || user.Id == userUnilever.UserId))
                    {
                        var selectedWorkLevel = _userService.GetAllWorkLevel().Single(x => x.Id == Guid.Parse(workLevelId)).Id;

                        var selectedSkuIds = skuIds.Select(x => Guid.Parse(x)).ToList();
                        var selectedSKUs = (_skuService.GetAllProducts().ToList()).Where(x => selectedSkuIds.Contains(x.Id)).ToList();

                        var selectedCategoryIds = categoryIds.Select(x => Guid.Parse(x)).ToList();
                        var selectedCategories = (_skuService.GetAllProductCategories().ToList()).Where(x => selectedCategoryIds.Contains(x.Id)).ToList();

                        user.UserName = userUnilever.Email;
                        user.NormalizedUserName = userUnilever.Email;
                        user.Email = userUnilever.Email;
                        user.NormalizedEmail = userUnilever.Email;
                        await _userManager.UpdateAsync(user);

                        var claims = await _userManager.GetClaimsAsync(user);

                        foreach (var claim in claims)
                        {
                            await _userManager.RemoveClaimAsync(user, claim);
                        }

                        savedUser.SKUs = selectedSKUs;
                        savedUser.ProductCategories = selectedCategories;
                        savedUser.Banners = selectedBanners;
                        savedUser.Name = userUnilever.Name;
                        savedUser.Email = userUnilever.Email;
                        savedUser.RoleUnilever = await _roleService.GetRole(Guid.Parse(roleUnileverId));
                        savedUser.WLId = selectedWorkLevel;
                        savedUser.IsActive = userUnilever.IsActive;

                        foreach (var menu in savedUser.RoleUnilever.Menus)
                        {
                            await _userManager.AddClaimAsync(user, new Claim("Menu", menu.Name));
                        }

                        await _userService.Update(savedUser, User.Identity.Name);
                        await _context.SaveChangesAsync();

                        _notyfService.Success("User Saved");
                        return RedirectToAction("Index", "Admin");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "User Already exist!");
                        var listBanner = (List<SelectListItem>)ViewData["ListBanner"];
                        var userBanner = savedUser.Banners.Select(x => x.Id).ToList();

                        var selectedBanner = listBanner.Where(x => selectedBannerId.Contains(Guid.Parse(x.Value))).ToList();
                        foreach (var item in selectedBanner)
                        {
                            item.Selected = true;
                        }
                        userUnilever.Banners = selectedBanners;
                    }
                       
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserUnileverExists(userUnilever.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
    
   

            return View(userUnilever);
        }

        // GET: UserUnilevers/Delete/5
     

        private bool UserUnileverExists(Guid id)
        {
          return (_context.UsersUnilever?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        private async Task FillDropdowns(ViewDataDictionary viewData)
        {
            await _bannerService.FillBannerDropdown(viewData);
            await _roleService.FillRoleDropdown(viewData);
            await _userService.FillWorkLevelDropdown(viewData);
            await _skuService.FillSKUDropdown(viewData);
            await _skuService.FillCategoryDropdown(viewData);
        }
    }
}
