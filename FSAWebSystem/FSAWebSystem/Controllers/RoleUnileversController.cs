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
using AspNetCoreHero.ToastNotification.Abstractions;

namespace FSAWebSystem.Controllers
{
    public class RoleUnileversController : Controller
    {
        private readonly FSAWebSystemDbContext _context;
        private readonly IRoleService _roleService;
        INotyfService _notyfService;
        public RoleUnileversController(FSAWebSystemDbContext context, IRoleService roleSerivce, INotyfService notyfService)
        {
            _context = context;
            _roleService = roleSerivce;
            _notyfService = notyfService;
        }

        // GET: RoleUnilevers
        public async Task<IActionResult> Index()
        {
              return _context.RoleUnilevers != null ? 
                          View(await _context.RoleUnilevers.ToListAsync()) :
                          Problem("Entity set 'FSAWebSystemDbContext.RoleUnilevers'  is null.");
        }

        // GET: RoleUnilevers/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.RoleUnilevers == null)
            {
                return NotFound();
            }

            var roleUnilever = await _context.RoleUnilevers
                .FirstOrDefaultAsync(m => m.RoleUnileverId == id);
            if (roleUnilever == null)
            {
                return NotFound();
            }

            return View(roleUnilever);
        }

        // GET: RoleUnilevers/Create
        public IActionResult Create()
        {
            var listMenu = _roleService.GetMenuDropdown();
            ViewData["ListMenu"] = listMenu;
            return View();
        }

        // POST: RoleUnilevers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RoleUnileverId,RoleName, Menus")] RoleUnilever roleUnilever, string[] menuIds)
        {
            ModelState.Remove("ModifiedAt");
            ModelState.Remove("ModifiedBy");
            ModelState.Remove("CreatedAt");
            ModelState.Remove("CreatedBy");
            if (ModelState.IsValid)
            {
                var savedRole = _roleService.GetRoleByName(roleUnilever.RoleName);
                if (savedRole != null)
                {
                    ModelState.AddModelError(string.Empty, "Role " + roleUnilever.RoleName + " already exist");
                }
                var savedMenus = _roleService.GetAllMenu();
                foreach(var menuId in menuIds)
                {
                    var savedMenu = savedMenus.Single(x => x.Id == Guid.Parse(menuId));
                    roleUnilever.Menus.Add(savedMenu);
                }
                roleUnilever.RoleUnileverId = Guid.NewGuid();
                roleUnilever.CreatedAt = DateTime.Now;
                roleUnilever.CreatedBy = User.Identity.Name;
                _context.Add(roleUnilever);
                await _context.SaveChangesAsync();
                _notyfService.Success("Role " + roleUnilever.RoleName + " successfully added");
                return RedirectToAction("Index", "Admin");
            }
            return View(roleUnilever);
        }

        // GET: RoleUnilevers/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            var listMenu = _roleService.GetMenuDropdown();
           

            if (id == null || _context.RoleUnilevers == null)
            {
                return NotFound();
            }

            var roleUnilever = await _roleService.GetRole((Guid)id);
            var roleMenus = roleUnilever.Menus.Select(x => x.Id);
            var selectedMenus = listMenu.Where(x => roleMenus.Contains(Guid.Parse(x.Value)));

            foreach(var selectedMenu in selectedMenus)
            {
                selectedMenu.Selected = true;
            }
            ViewData["ListMenu"] = listMenu;
            if (roleUnilever == null)
            {
                return NotFound();
            }
            return View(roleUnilever);
        }

        // POST: RoleUnilevers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("RoleUnileverId,RoleName")] RoleUnilever roleUnilever, string[] menuIds)
        {
            ModelState.Remove("ModifiedAt");
            ModelState.Remove("ModifiedBy");
            ModelState.Remove("CreatedAt");
            ModelState.Remove("CreatedBy");
            if (id != roleUnilever.RoleUnileverId)
            {
                return NotFound();
            }
            
            if (ModelState.IsValid)
            {
                try
                {
                    var savedRole = await _roleService.GetRoleByName(roleUnilever.RoleName);
                    if (savedRole != null)
                    {
                        if(savedRole.RoleUnileverId != id)
                        {
                            ModelState.AddModelError(string.Empty, "Role " + roleUnilever.RoleName + " already exist");
                        }
                    }

                    List<Guid> selectedMenuId = (from menuId in menuIds select Guid.Parse(menuId)).ToList();
                    var selectedMenu = (_roleService.GetAllMenu().ToList()).Where(x => selectedMenuId.Contains(x.Id)).ToList();

                    savedRole.RoleName = roleUnilever.RoleName;
                    savedRole.Menus = selectedMenu;
                    savedRole.ModifiedAt = DateTime.Now;
                    savedRole.ModifiedBy = User.Identity.Name;

                    await _roleService.Update(savedRole);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RoleUnileverExists(roleUnilever.RoleUnileverId))
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
            _notyfService.Success("Role " + roleUnilever.RoleName + " successfully saved");
            return RedirectToAction("Index", "Admin");
        }

        // GET: RoleUnilevers/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.RoleUnilevers == null)
            {
                return NotFound();
            }

            var roleUnilever = await _context.RoleUnilevers
                .FirstOrDefaultAsync(m => m.RoleUnileverId == id);
            if (roleUnilever == null)
            {
                return NotFound();
            }

            return View(roleUnilever);
        }

        // POST: RoleUnilevers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.RoleUnilevers == null)
            {
                return Problem("Entity set 'FSAWebSystemDbContext.RoleUnilevers'  is null.");
            }
            var roleUnilever = await _context.RoleUnilevers.FindAsync(id);
            if (roleUnilever != null)
            {
                _context.RoleUnilevers.Remove(roleUnilever);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RoleUnileverExists(Guid id)
        {
          return (_context.RoleUnilevers?.Any(e => e.RoleUnileverId == id)).GetValueOrDefault();
        }
    }
}
