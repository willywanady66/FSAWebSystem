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
    public class WorkLevelsController : Controller
    {
        private readonly FSAWebSystemDbContext _context;
        private readonly IUserService _userService;
        private readonly INotyfService _notyfService;

        public WorkLevelsController(FSAWebSystemDbContext context, IUserService userService, INotyfService notyfService)
        {
            _context = context;
            _userService = userService;
            _notyfService = notyfService;
        }

        // GET: WorkLevels/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.WorkLevels == null)
            {
                return NotFound();
            }

            var workLevel = await _context.WorkLevels
                .FirstOrDefaultAsync(m => m.Id == id);
            if (workLevel == null)
            {
                return NotFound();
            }

            return View(workLevel);
        }

        // GET: WorkLevels/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: WorkLevels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("WL, IsActive")] WorkLevel workLevel)
        {
            ModelState.Remove("CreatedBy");
            List<WorkLevel> workLevels = new List<WorkLevel>();
            if (ModelState.IsValid)
            {
                var savedWLs = _userService.GetAllWorkLevel();
                if(savedWLs.Any(x => x.WL == workLevel.WL))
                {
                    ModelState.AddModelError(string.Empty, "Work Level already exist");
                }
                else
                {
                    workLevel.Id = Guid.NewGuid();
                    workLevel.CreatedAt = DateTime.Now;
                    workLevel.CreatedBy = User.Identity.Name;
                    workLevels.Add(workLevel);
                    _userService.SaveWorkLevels(workLevels);
                    await _context.SaveChangesAsync();
                    _notyfService.Success("Work Level saved");
                    return RedirectToAction("Index", "Admin");
                }

            }
            return View(workLevel);
        }

        // GET: WorkLevels/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.WorkLevels == null)
            {
                return NotFound();
            }

            var workLevel = await _context.WorkLevels.FindAsync(id);
            if (workLevel == null)
            {
                return NotFound();
            }
            return View(workLevel);
        }

        // POST: WorkLevels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,WL,IsActive, CreatedBy, CreatedAt")] WorkLevel workLevel)
        {

            ModelState.Remove("CreatedAt");
            ModelState.Remove("CreatedBy");
            ModelState.Remove("Status");
            if (id != workLevel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
             
                var usedWL = _context.UsersUnilever.Any(x => x.WLId == workLevel.Id);
                if(usedWL)
                {
                    ModelState.AddModelError(string.Empty, "Cannot edit on used Work Level");
                    return View(workLevel);
                }
                try
                {
                    workLevel.ModifiedAt = DateTime.Now;
                    workLevel.ModifiedBy = User.Identity.Name;
                    _context.Update(workLevel);
                    _notyfService.Success("Work Level Updated!");
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WorkLevelExists(workLevel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index", "Admin");
            }
            return View(workLevel);
        }

        // GET: WorkLevels/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.WorkLevels == null)
            {
                return NotFound();
            }

            var workLevel = await _context.WorkLevels
                .FirstOrDefaultAsync(m => m.Id == id);
            if (workLevel == null)
            {
                return NotFound();
            }

            return View(workLevel);
        }

        // POST: WorkLevels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.WorkLevels == null)
            {
                return Problem("Entity set 'FSAWebSystemDbContext.WorkLevels'  is null.");
            }
            var workLevel = await _context.WorkLevels.FindAsync(id);
            if (workLevel != null)
            {
                _context.WorkLevels.Remove(workLevel);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WorkLevelExists(Guid id)
        {
          return (_context.WorkLevels?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
