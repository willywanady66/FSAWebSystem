using AspNetCoreHero.ToastNotification.Abstractions;
using FSAWebSystem.Models;
using FSAWebSystem.Models.Context;
using FSAWebSystem.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FSAWebSystem.Controllers
{
    public class ULICalendarsController : Controller
    {
        private readonly FSAWebSystemDbContext _context;
        private readonly ICalendarService _calendarService;
        private readonly INotyfService _notyfService;

        public ULICalendarsController(FSAWebSystemDbContext context, ICalendarService calendarService, INotyfService notyfService)
        {
            _context = context;
            _calendarService = calendarService;
            _notyfService = notyfService;
        }

        // GET: ULICalendars
        public async Task<IActionResult> Index()
        {
            return _context.ULICalendars != null ?
                        View(await _context.ULICalendars.ToListAsync()) :
                        Problem("Entity set 'FSAWebSystemDbContext.ULICalendars'  is null.");
        }

        // GET: ULICalendars/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.ULICalendars == null)
            {
                return NotFound();
            }

            var uLICalendar = await _context.ULICalendars
                .FirstOrDefaultAsync(m => m.Id == id);
            if (uLICalendar == null)
            {
                return NotFound();
            }

            return View(uLICalendar);
        }

        // GET: ULICalendars/Create
        public IActionResult Create()
        {
            ViewData["ListMonth"] = _calendarService.GetListMonth();
            ViewData["ListYear"] = _calendarService.GetListYear();
            ULICalendar uliCalendar = new ULICalendar();
            uliCalendar.Month = DateTime.Now.Month;
            uliCalendar.Year = DateTime.Now.Year;
            uliCalendar.ULICalendarDetails = new List<ULICalendarDetail>
            {
                new ULICalendarDetail{ Week = 1, StartDate = DateTime.Now, EndDate = DateTime.Now },
                new ULICalendarDetail{ Week = 2, StartDate = DateTime.Now, EndDate = DateTime.Now },
                new ULICalendarDetail{ Week = 3, StartDate = DateTime.Now, EndDate = DateTime.Now },
                new ULICalendarDetail{ Week = 4, StartDate = DateTime.Now, EndDate = DateTime.Now },
                new ULICalendarDetail{ Week = 5}
            };
            return View(uliCalendar);
        }

        // POST: ULICalendars/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Month,Year")] ULICalendar uLICalendar, List<ULICalendarDetail> uLICalendarDetails)
        {
            ViewData["ListMonth"] = _calendarService.GetListMonth();
            ViewData["ListYear"] = _calendarService.GetListYear();
            uLICalendar.ULICalendarDetails = uLICalendarDetails;
            var savedCalendarThisMonth = await _calendarService.GetULICalendar(uLICalendar.Month, uLICalendar.Year);
            if (savedCalendarThisMonth != null)
            {
                ModelState.AddModelError("", "ULI Calendar for Month: " + uLICalendar.Month + " and Year: " + uLICalendar.Month + " already exist");

                _notyfService.Error("Crate calendar failed");
                return View(uLICalendar);
            }
            if (ModelState.IsValid)
            {
                uLICalendar.Id = Guid.NewGuid();
                _context.Add(uLICalendar);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(uLICalendar);
        }

        // GET: ULICalendars/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.ULICalendars == null)
            {
                return NotFound();
            }

            var uLICalendar = await _context.ULICalendars.FindAsync(id);
            if (uLICalendar == null)
            {
                return NotFound();
            }
            return View(uLICalendar);
        }

        // POST: ULICalendars/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Year,CreatedAt,CreatedBy,ModifiedAt,ModifiedBy")] ULICalendar uLICalendar)
        {
            if (id != uLICalendar.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(uLICalendar);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ULICalendarExists(uLICalendar.Id))
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
            return View(uLICalendar);
        }

        // GET: ULICalendars/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.ULICalendars == null)
            {
                return NotFound();
            }

            var uLICalendar = await _context.ULICalendars
                .FirstOrDefaultAsync(m => m.Id == id);
            if (uLICalendar == null)
            {
                return NotFound();
            }

            return View(uLICalendar);
        }

        // POST: ULICalendars/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.ULICalendars == null)
            {
                return Problem("Entity set 'FSAWebSystemDbContext.ULICalendars'  is null.");
            }
            var uLICalendar = await _context.ULICalendars.FindAsync(id);
            if (uLICalendar != null)
            {
                _context.ULICalendars.Remove(uLICalendar);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ULICalendarExists(Guid id)
        {
            return (_context.ULICalendars?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
