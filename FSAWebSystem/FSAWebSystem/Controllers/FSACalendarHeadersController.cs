using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FSAWebSystem.Models;
using FSAWebSystem.Models.Context;
using System.Globalization;

namespace FSAWebSystem.Controllers
{
    public class FSACalendarHeadersController : Controller
    {
        private readonly FSAWebSystemDbContext _context;

        public FSACalendarHeadersController(FSAWebSystemDbContext context)
        {
            _context = context;
        }

        // GET: FSACalendarHeaders
        public async Task<IActionResult> Index()
        {
            return _context.FSACalendarHeader != null ?
                        View(await _context.FSACalendarHeader.ToListAsync()) :
                        Problem("Entity set 'FSAWebSystemDbContext.FSACalendarHeader'  is null.");
        }

        // GET: FSACalendarHeaders/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.FSACalendarHeader == null)
            {
                return NotFound();
            }

            var fSACalendarHeader = await _context.FSACalendarHeader
                .FirstOrDefaultAsync(m => m.Id == id);
            if (fSACalendarHeader == null)
            {
                return NotFound();
            }

            return View(fSACalendarHeader);
        }

        // GET: FSACalendarHeaders/Create
        public IActionResult Create()
        {
            List<SelectListItem> listMonth = GetListMonth();
            ViewData["ListMonth"] = listMonth;
            FSACalendarHeader fsaCalHeader = new FSACalendarHeader();
            fsaCalHeader.Month = DateTime.Now.Month;
            fsaCalHeader.FSACalendarDetails = new List<FSACalendarDetail>
            {
                new FSACalendarDetail{ Week = 1, StartDate = DateTime.Now, EndDate = DateTime.Now },
                new FSACalendarDetail{ Week = 2, StartDate = DateTime.Now, EndDate = DateTime.Now },
                new FSACalendarDetail{ Week = 3, StartDate = DateTime.Now, EndDate = DateTime.Now },
                new FSACalendarDetail{ Week = 4, StartDate = DateTime.Now, EndDate = DateTime.Now },
                new FSACalendarDetail{ Week = 5}
            };
            return View(fsaCalHeader);
        }

        // POST: FSACalendarHeaders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Month,Year, FSACalendarDetails")] FSACalendarHeader fSACalendarHeader, string month)
        {
            ModelState.Remove("Month");
            List<SelectListItem> listMonth = GetListMonth();
            ViewData["ListMonth"] = listMonth;
            if (ModelState.IsValid)
            {

                var allWeekHasValue = fSACalendarHeader.FSACalendarDetails.Where(x => x.Week != 5).All(x => x.StartDate.HasValue && x.EndDate.HasValue);
                var week5HasValue = fSACalendarHeader.FSACalendarDetails.Where(x => x.Week == 5).All(x => x.StartDate.HasValue && x.EndDate.HasValue);
                if(!allWeekHasValue)
                {
                    ModelState.AddModelError("", "Week 1 to Week 4 must be filled");
                    return View(fSACalendarHeader);
                }
               

                foreach(var detail in fSACalendarHeader.FSACalendarDetails)
                {
                    if (detail.Week == 5 && !(detail.StartDate.HasValue && detail.EndDate.HasValue))
                    {
                        continue;
                    }
                    else
                    {
                        detail.Id = Guid.NewGuid();
                    }

                }
                fSACalendarHeader.CreatedBy = User.Identity.Name;
                fSACalendarHeader.CreatedAt = DateTime.Now;
                fSACalendarHeader.Id = Guid.NewGuid();
                _context.Add(fSACalendarHeader);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            
            return View(fSACalendarHeader);
        }

        // GET: FSACalendarHeaders/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.FSACalendarHeader == null)
            {
                return NotFound();
            }

            var fSACalendarHeader = await _context.FSACalendarHeader.FindAsync(id);
            if (fSACalendarHeader == null)
            {
                return NotFound();
            }
            return View(fSACalendarHeader);
        }

        // POST: FSACalendarHeaders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Month,Year,CreatedAt,CreatedBy,ModifiedAt,ModifiedBy")] FSACalendarHeader fSACalendarHeader)
        {
            if (id != fSACalendarHeader.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(fSACalendarHeader);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FSACalendarHeaderExists(fSACalendarHeader.Id))
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
            return View(fSACalendarHeader);
        }

        // GET: FSACalendarHeaders/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.FSACalendarHeader == null)
            {
                return NotFound();
            }

            var fSACalendarHeader = await _context.FSACalendarHeader
                .FirstOrDefaultAsync(m => m.Id == id);
            if (fSACalendarHeader == null)
            {
                return NotFound();
            }

            return View(fSACalendarHeader);
        }

        // POST: FSACalendarHeaders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.FSACalendarHeader == null)
            {
                return Problem("Entity set 'FSAWebSystemDbContext.FSACalendarHeader'  is null.");
            }
            var fSACalendarHeader = await _context.FSACalendarHeader.FindAsync(id);
            if (fSACalendarHeader != null)
            {
                _context.FSACalendarHeader.Remove(fSACalendarHeader);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FSACalendarHeaderExists(Guid id)
        {
            return (_context.FSACalendarHeader?.Any(e => e.Id == id)).GetValueOrDefault();
        }


        public List<SelectListItem> GetListMonth()
        {

            var months = DateTimeFormatInfo
                       .InvariantInfo
                       .MonthNames
                       .Select((monthName, index) => new SelectListItem
                       {
                           Value = (index + 1).ToString(),
                           Text = monthName
                       });
            List<SelectListItem> listMonths = months.ToList();
            listMonths.Single(x => x.Value == DateTime.Now.Month.ToString()).Selected = true;
            return listMonths;
        }
    }
}
