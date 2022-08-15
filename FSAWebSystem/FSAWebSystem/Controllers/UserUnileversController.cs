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

        public UserUnileversController(FSAWebSystemDbContext context, IBannerService bannerService, IRoleService roleService, IUserService userService, UserManager<FSAWebSystemUser> userManager, INotyfService notyfService)
        {
            _context = context;
            _bannerService = bannerService;
            _roleService = roleService;
            _userService = userService;
            _userManager = userManager;
            _notyfService = notyfService;
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
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            var userUnilever = await _userService.GetUser((Guid)user.UserUnileverId);
            userUnilever.UserId = id;
            if (userUnilever == null)
            {
                return NotFound();
            }

            await FillDropdowns(ViewData);
            var listBanner = (List<SelectListItem>)ViewData["ListBanner"];
            var listRole = (List<SelectListItem>)ViewData["ListRole"];
            var listWorkLevel = (List<SelectListItem>)ViewData["ListWorkLevel"];
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

            var userRole = listRole.SingleOrDefault(x => Guid.Parse(x.Value) == userUnilever.RoleUnilever.RoleUnileverId);
            if (userRole != null) userRole.Selected = true;

            return View(userUnilever);
        }

        // POST: UserUnilevers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind ("Id,Name,Email,IsActive,UserId,CreatedAt,CreatedBy")] UserUnilever userUnilever, string[] bannerIds, string roleUnileverId, string workLevelId)
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
                try
                {
                    var user = await _userManager.FindByIdAsync(userUnilever.UserId);
					var savedUser = await _userService.GetUser((Guid)user.UserUnileverId);

					List<Guid> selectedBannerId = (from bannerId in bannerIds select Guid.Parse(bannerId)).ToList();
                    var selectedBanners = (_bannerService.GetAllBanner().ToList()).Where(x => selectedBannerId.Contains(x.Id)).ToList();
                    var selectedWorkLevel = _userService.GetAllWorkLevel().Single(x => x.Id == Guid.Parse(workLevelId)).Id;
        

                    user.UserName = userUnilever.Email;
                    user.NormalizedUserName = userUnilever.Email;
                    user.Email = userUnilever.Email;
                    user.NormalizedEmail = userUnilever.Email;
                    await _userManager.UpdateAsync(user);

                    var claims = await _userManager.GetClaimsAsync(user);

                    foreach(var claim in claims)
                    {
                        await _userManager.RemoveClaimAsync(user, claim);
                    }

                    savedUser.Banners = selectedBanners;
                    savedUser.Name = userUnilever.Name;
                    savedUser.Email = userUnilever.Email;
                    savedUser.RoleUnilever = await _roleService.GetRole(Guid.Parse(roleUnileverId));
                    savedUser.WLId = selectedWorkLevel;
                    savedUser.IsActive = userUnilever.IsActive;

                    foreach(var menu in savedUser.RoleUnilever.Menus)
                    {
                        await _userManager.AddClaimAsync(user, new Claim("Menu", menu.Name));
                    }
                  
                    await _userService.Update(savedUser, User.Identity.Name);
                    await _context.SaveChangesAsync();
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
                _notyfService.Success("User Saved");
                return RedirectToAction("Index", "Admin");
            }
            else
            {
                await FillDropdowns(ViewData);
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
        }
    }
}
