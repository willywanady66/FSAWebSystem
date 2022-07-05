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
using FSAWebSystem.Services.Interface;

namespace FSAWebSystem.Controllers
{
    public class FSACalendarHeadersController : Controller
    {
        private ICalendarService _calendarService;
        private readonly FSAWebSystemDbContext _db;

        public FSACalendarHeadersController(FSAWebSystemDbContext context, ICalendarService calendarService)
        {
            _db = context;
            _calendarService = calendarService;
        }

        // GET: FSACalendarHeaders
        public async Task<IActionResult> Index()
        {
            return _db.FSACalendarHeader != null ?
                        View(await _db.FSACalendarHeader.ToListAsync()) :
                        Problem("Entity set 'FSAWebSystemDbContext.FSACalendarHeader'  is null.");
        }

        // GET: FSACalendarHeaders/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _db.FSACalendarHeader == null)
            {
                return NotFound();
            }

            var fSACalendarHeader = await _db.FSACalendarHeader
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
            ViewData["ListMonth"] = _calendarService.GetListMonth();
            ViewData["ListYear"] = _calendarService.GetListYear();
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
        public async Task<IActionResult> Create([Bind("Id,Month,Year, FSACalendarDetails")] FSACalendarHeader fSACalendarHeader, string month, string year)
        {
            ModelState.Remove("Month");
      
            ViewData["ListMonth"] = _calendarService.GetListMonth();
            ViewData["ListYear"] = _calendarService.GetListYear();
            if (ModelState.IsValid)
            {

                var allWeekHasValue = fSACalendarHeader.FSACalendarDetails.Where(x => x.Week != 5).All(x => x.StartDate.HasValue && x.EndDate.HasValue);
                var week5HasValue = fSACalendarHeader.FSACalendarDetails.Where(x => x.Week == 5).All(x => x.StartDate.HasValue && x.EndDate.HasValue);
                if(!allWeekHasValue)
                {
                    ModelState.AddModelError("", "Week 1 to Week 4 must be filled");
                    return View(fSACalendarHeader);
                }


                var isStartDateLessEndDate = fSACalendarHeader.FSACalendarDetails.Where(x => (x.StartDate.HasValue && x.EndDate.HasValue)).Any(x => x.StartDate?.Date < x.EndDate?.Date);
                if(!isStartDateLessEndDate)
				{
					ModelState.AddModelError("", "Start Date must be less than End Date");
                    return View(fSACalendarHeader);
				}

				foreach (var detail in fSACalendarHeader.FSACalendarDetails.Where(x => x.StartDate.HasValue && x.EndDate.HasValue))
                {
                   
                        detail.Month = Convert.ToInt32(month);
                        detail.Year = Convert.ToInt32(year);
                        detail.Id = Guid.NewGuid();
                    
                }

                fSACalendarHeader.FSACalendarDetails = fSACalendarHeader.FSACalendarDetails.Where(x => x.Id != Guid.Empty).ToList();

                fSACalendarHeader.CreatedBy = User.Identity.Name;
                fSACalendarHeader.CreatedAt = DateTime.Now;
                fSACalendarHeader.Id = Guid.NewGuid();
                _calendarService.AddCalendar(fSACalendarHeader);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index", "Admin");
            }


            return RedirectToAction("Index", "Admin");
        }

        // GET: FSACalendarHeaders/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _db.FSACalendarHeader == null)
            {
                return NotFound();
            }

            var fSACalendarHeader = await _db.FSACalendarHeader.FindAsync(id);
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
                    _db.Update(fSACalendarHeader);
                    await _db.SaveChangesAsync();
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
            if (id == null || _db.FSACalendarHeader == null)
            {
                return NotFound();
            }

            var fSACalendarHeader = await _db.FSACalendarHeader
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
            if (_db.FSACalendarHeader == null)
            {
                return Problem("Entity set 'FSAWebSystemDbContext.FSACalendarHeader'  is null.");
            }
            var fSACalendarHeader = await _db.FSACalendarHeader.FindAsync(id);
            if (fSACalendarHeader != null)
            {
                _db.FSACalendarHeader.Remove(fSACalendarHeader);
            }

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FSACalendarHeaderExists(Guid id)
        {
            return (_db.FSACalendarHeader?.Any(e => e.Id == id)).GetValueOrDefault();
        }


        
    }
}
