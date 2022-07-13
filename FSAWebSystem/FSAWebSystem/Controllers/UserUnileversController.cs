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

namespace FSAWebSystem.Controllers
{
    public class UserUnileversController : Controller
    {
        private readonly FSAWebSystemDbContext _context;
        private readonly IBannerService _bannerService;
        private readonly IRoleService _roleService;
        private readonly IUserService _userService;

        public UserUnileversController(FSAWebSystemDbContext context, IBannerService bannerService, IRoleService roleService, IUserService userService)
        {
            _context = context;
            _bannerService = bannerService;
            _roleService = roleService;
            _userService = userService;
        }

        // GET: UserUnilevers
        public async Task<IActionResult> Index()
        {
              return _context.UsersUnilever != null ? 
                          View(await _context.UsersUnilever.ToListAsync()) :
                          Problem("Entity set 'FSAWebSystemDbContext.UsersUnilever'  is null.");
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

        // GET: UserUnilevers/Create
        public async Task<IActionResult> Create()
        {
            var banners = _bannerService.GetAllBanner().ToList();

            MultiDropDownListViewModel listBanner = new MultiDropDownListViewModel();
            listBanner.ItemList = banners.Select(x => new SelectListItem { Text = x.BannerName, Value = x.Id.ToString() }).ToList();
            //foreach (var banner in banners)
            //{
            //    listBanner.ItemList.Add(new SelectListItem { Text = banner.BannerName, Value = banner.Id.ToString() });
            //}
            ViewBag.Banners = listBanner.ItemList;
            return View();
        }

        // POST: UserUnilevers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Email,Password,CreatedAt,CreatedBy,ModifiedAt,ModifiedBy")] UserUnilever userUnilever)
        {
            if (ModelState.IsValid)
            {
                userUnilever.Id = Guid.NewGuid();
                _context.Add(userUnilever);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(userUnilever);
        }

        // GET: UserUnilevers/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.UsersUnilever == null)
            {
                return NotFound();
            }

            var userUnilever = await _userService.GetUser((Guid)id);
            if (userUnilever == null)
            {
                return NotFound();
            }

            await FillDropdowns(ViewData);
            var listBanner = (List<SelectListItem>)ViewData["ListBanner"];
            var listRole = (List<SelectListItem>)ViewData["ListRole"];
            var userBanner = userUnilever.Banners.Select(x => x.Id).ToList();

            var selectedBanner = listBanner.Where(x => userBanner.Contains(Guid.Parse(x.Value))).ToList();

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
        public async Task<IActionResult> Edit(Guid id, [Bind ("Id,Name,Email,CreatedAt,CreatedBy")] UserUnilever userUnilever, string[] bannerIds, string roleUnileverId)
        {
            if (id != userUnilever.Id)
            {
                return NotFound();
            }
            ModelState.Remove("RoleUnilever");
            ModelState.Remove("BannerName");
            ModelState.Remove("Password");
            ModelState.Remove("Role");
            if (ModelState.IsValid)
            {
                try
                {
                    var savedUser = await _userService.GetUser(userUnilever.Id);
                    List<Guid> selectedBannerId = (from bannerId in bannerIds select Guid.Parse(bannerId)).ToList();
                    var selectedBanners = (_bannerService.GetAllBanner().ToList()).Where(x => selectedBannerId.Contains(x.Id)).ToList();
             
                    savedUser.Banners = selectedBanners;
                    savedUser.Name = userUnilever.Name;
                    savedUser.Email = userUnilever.Email;
                    savedUser.RoleUnilever = await _roleService.GetRole(Guid.Parse(roleUnileverId));
                    savedUser.IsActive = userUnilever.IsActive;
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
                return RedirectToAction(nameof(Index));
            }
            else
            {
                await FillDropdowns(ViewData);
            }
            return View(userUnilever);
        }

        // GET: UserUnilevers/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
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

        // POST: UserUnilevers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.UsersUnilever == null)
            {
                return Problem("Entity set 'FSAWebSystemDbContext.UsersUnilever'  is null.");
            }
            var userUnilever = await _context.UsersUnilever.FindAsync(id);
            if (userUnilever != null)
            {
                _context.UsersUnilever.Remove(userUnilever);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserUnileverExists(Guid id)
        {
          return (_context.UsersUnilever?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        private async Task FillDropdowns(ViewDataDictionary viewData)
        {
            await _bannerService.FillBannerDropdown(viewData);
            await _roleService.FillRoleDropdown(viewData);
        }

    }
}
