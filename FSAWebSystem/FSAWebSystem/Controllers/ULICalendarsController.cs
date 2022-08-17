using AspNetCoreHero.ToastNotification.Abstractions;
using FSAWebSystem.Models;
using FSAWebSystem.Models.Context;
using FSAWebSystem.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        public async Task<IActionResult> Create(int month, int year)
        {
            ViewData["ListMonth"] = _calendarService.GetListMonth();
            ViewData["ListYear"] = _calendarService.GetListYear();
            ULICalendar uliCalendar = new ULICalendar();
            uliCalendar.Month = DateTime.Now.Month;
            uliCalendar.Year = DateTime.Now.Year;
            var startWeek = 1;

            if (month == 0)
            {
                if (uliCalendar.Month != 1)
                {
                    var prevCalendar = await _calendarService.GetULICalendar(uliCalendar.Month - 1, uliCalendar.Year);
                    if (prevCalendar != null)
                    {
                        startWeek = prevCalendar.ULICalendarDetails.OrderByDescending(x => x.Week).First().Week + 1;
                    }
                }
            }
            else
            {
       
                if (month != 1)
                {
                    var prevCalendar = await _calendarService.GetULICalendar(month - 1, year);
                    if (prevCalendar != null)
                    {
                        startWeek = prevCalendar.ULICalendarDetails.OrderByDescending(x => x.Week).First().Week + 1;
                    }
                }
            }
          
           
            uliCalendar.ULICalendarDetails = new List<ULICalendarDetail>
            {
                new ULICalendarDetail{ Week = startWeek++},
                new ULICalendarDetail{ Week = startWeek++},
                new ULICalendarDetail{ Week = startWeek++},
                new ULICalendarDetail{ Week = startWeek++},
                new ULICalendarDetail{ Week = startWeek++}
            };
            uliCalendar.ULICalendarDetails = uliCalendar.ULICalendarDetails.OrderBy(x => x.Week).ToList();
            return View(uliCalendar);
        }


        public async Task<IActionResult> Reload(int month, int year)
        {
            ViewData["ListMonth"] = _calendarService.GetListMonth();
            ViewData["ListYear"] = _calendarService.GetListYear();
            ULICalendar uliCalendar = new ULICalendar();
            uliCalendar.Month = DateTime.Now.Month;
            uliCalendar.Year = DateTime.Now.Year;
            var startWeek = 1;

            if (month == 0)
            {
                if (uliCalendar.Month != 1)
                {
                    var prevCalendar = await _calendarService.GetULICalendar(uliCalendar.Month - 1, uliCalendar.Year);
                    if (prevCalendar != null)
                    {
                        startWeek = prevCalendar.ULICalendarDetails.OrderByDescending(x => x.Week).First().Week + 1;
                    }
                }
            }
            else
            {

                if (month != 1)
                {
                    var prevCalendar = await _calendarService.GetULICalendar(month - 1, year);
                    if (prevCalendar != null)
                    {
                        startWeek = prevCalendar.ULICalendarDetails.OrderByDescending(x => x.Week).First().Week + 1;
                    }
                }
            }


            uliCalendar.ULICalendarDetails = new List<ULICalendarDetail>
            {
                new ULICalendarDetail{ Week = startWeek++},
                new ULICalendarDetail{ Week = startWeek++},
                new ULICalendarDetail{ Week = startWeek++},
                new ULICalendarDetail{ Week = startWeek++},
                new ULICalendarDetail{ Week = startWeek++}
            };
            return Ok(uliCalendar);
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
            ModelState.Remove("ULICalendarDetails");
            ModelState.Remove("ULICalendarDetails.ULICalendar");

            List<SelectListItem> listYears = ViewData["listYear"] as List<SelectListItem>;
            List<SelectListItem> listMonths = ViewData["listMonth"] as List<SelectListItem>;

            var selectedMonth = listMonths.SingleOrDefault(x => x.Selected);
            if(selectedMonth != null)
            {
                if(uLICalendar.Month.ToString() != selectedMonth.Value)
                {
                    selectedMonth.Selected = false;
                    var newSelected = listMonths.Single(x => x.Value == uLICalendar.Month.ToString());
                    newSelected.Selected = true;
                }
            }

            var selectedYear = listYears.SingleOrDefault(x => x.Selected);
            if (selectedYear != null)
            {
                if (uLICalendar.Month.ToString() != selectedYear.Value)
                {
                    selectedYear.Selected = false;
                    var newSelected = listYears.Single(x => x.Value == uLICalendar.Year.ToString());
                    newSelected.Selected = true;
                }
            }


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

                var allWeekHasValue = uLICalendarDetails.Where(x => x.Week != 0).All(x => x.StartDate.HasValue && x.EndDate.HasValue);
                if (!allWeekHasValue)
                {
                    var emptyWeek = uLICalendarDetails.Where(x => x.Week != 0 && (!x.StartDate.HasValue || !x.EndDate.HasValue)).Select(x => x.Week).ToList();
                    ModelState.AddModelError("", "Start Date and End Date must be filled on Week " + String.Join(", ", emptyWeek));
                    return View(uLICalendar);
                }

                var isStartDateLessEndDate = uLICalendarDetails.Where(x => (x.StartDate.HasValue && x.EndDate.HasValue)).Any(x => x.StartDate?.Date < x.EndDate?.Date);
                if (!isStartDateLessEndDate)
                {
                    ModelState.AddModelError("", "Start Date must be less than End Date");
                }

                foreach (var calendarDetail in uLICalendarDetails.Where(x => x.Week != 0))
                {
                    var startDateExistOnOtherWeek = uLICalendarDetails.Where(x => x.StartDate.HasValue && x.EndDate.HasValue).SingleOrDefault(x => calendarDetail.StartDate  >= x.StartDate.Value && calendarDetail.StartDate  <= x.EndDate.Value && x.Week != calendarDetail.Week);
                    var startDateExistOnOtherWeekInDb = _calendarService.GetULICalendarDetails().Where(x => x.StartDate.HasValue && x.EndDate.HasValue).SingleOrDefault(x => calendarDetail.StartDate >= x.StartDate.Value  && calendarDetail.StartDate <= x.EndDate.Value  && x.Week != calendarDetail.Week);
                    if(startDateExistOnOtherWeek != null)
                    {
                        ModelState.AddModelError("", "Start Date " + calendarDetail.StartDate.Value.ToString("dd/MM/yyyy") + " on Week: " + calendarDetail.Week + " already included on Week: " + startDateExistOnOtherWeek.Week + " Month: " + startDateExistOnOtherWeek.Month);
                    }
                    if(startDateExistOnOtherWeekInDb != null)
                    {
                        ModelState.AddModelError("", "Start Date " + calendarDetail.StartDate.Value.ToString("dd/MM/yyyy") + " on Week: " + calendarDetail.Week + " already included on Week: " + startDateExistOnOtherWeekInDb.Week + " Month: " + startDateExistOnOtherWeekInDb.Month);
                    }

          
                }

                var weeks = uLICalendarDetails.Where(x => x.Week != 0).Select(x => x.Week).ToList();
                var isWeekExist = _calendarService.GetULICalendarDetails().Where(x => weeks.Contains(x.Week) && x.Year == uLICalendar.Year).ToList();
                
                if (isWeekExist.Any())
                {
                    
                    ModelState.AddModelError("", "Week: " + string.Join(", ", isWeekExist.Select(x => x.Week)) + " already exist on Month: " + String.Join(", ", isWeekExist.DistinctBy(x => x.Month).Select(x => x.Month)) + " and Year : " + String.Join(", ", isWeekExist.DistinctBy(x => x.Year).Select(x => x.Year)));
                  
                }

                

                if(ModelState.ErrorCount > 0)
                {
                    return View(uLICalendar);
                }

                foreach(var calendar in uLICalendarDetails)
                {
                    calendar.Month = uLICalendar.Month;
                    calendar.Year = uLICalendar.Year;
                }

                uLICalendar.Id = Guid.NewGuid();
                uLICalendar.CreatedBy = User.Identity.Name;
                uLICalendar.CreatedAt = DateTime.Now;
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

            ViewData["ListMonth"] = _calendarService.GetListMonth();
            ViewData["ListYear"] = _calendarService.GetListYear();

            var months = (List<SelectListItem>)ViewData["ListMonth"];
            var years = (List<SelectListItem>)ViewData["ListYear"];
            var uLICalendar = await _calendarService.GetULICalendarById((Guid)id);
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
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Month,Year,CreatedAt,CreatedBy,ModifiedAt,ModifiedBy")] ULICalendar uLICalendar, List<ULICalendarDetail> uLICalendarDetails)
        {
            ViewData["ListMonth"] = _calendarService.GetListMonth();
            ViewData["ListYear"] = _calendarService.GetListYear();
            ModelState.Remove("ULICalendarDetails");
            ModelState.Remove("ULICalendarDetails.ULICalendar");

            List<SelectListItem> listYears = ViewData["listYear"] as List<SelectListItem>;
            List<SelectListItem> listMonths = ViewData["listMonth"] as List<SelectListItem>;

            var selectedMonth = listMonths.SingleOrDefault(x => x.Selected);
            if (selectedMonth != null)
            {
                if (uLICalendar.Month.ToString() != selectedMonth.Value)
                {
                    selectedMonth.Selected = false;
                    var newSelected = listMonths.Single(x => x.Value == uLICalendar.Month.ToString());
                    newSelected.Selected = true;
                }
            }

            var selectedYear = listYears.SingleOrDefault(x => x.Selected);
            if (selectedYear != null)
            {
                if (uLICalendar.Month.ToString() != selectedYear.Value)
                {
                    selectedYear.Selected = false;
                    var newSelected = listYears.Single(x => x.Value == uLICalendar.Year.ToString());
                    newSelected.Selected = true;
                }
            }
            uLICalendar.ULICalendarDetails = uLICalendarDetails;
            

            if (id != uLICalendar.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {

                var allWeekHasValue = uLICalendarDetails.Where(x => x.Week != 0).All(x => x.StartDate.HasValue && x.EndDate.HasValue);
                if (!allWeekHasValue)
                {
                    var emptyWeek = uLICalendarDetails.Where(x => x.Week != 0 && (!x.StartDate.HasValue || !x.EndDate.HasValue)).Select(x => x.Week).ToList();
                    ModelState.AddModelError("", "Start Date and End Date must be filled on Week " + String.Join(", ", emptyWeek));
                    return View(uLICalendar);
                }

                var isStartDateLessEndDate = uLICalendarDetails.Where(x => (x.StartDate.HasValue && x.EndDate.HasValue)).Any(x => x.StartDate?.Date < x.EndDate?.Date);
                if (!isStartDateLessEndDate)
                {
                    ModelState.AddModelError("", "Start Date must be less than End Date");
                }

                foreach (var calendarDetail in uLICalendarDetails.Where(x => x.Week != 0))
                {
                    var startDateExistOnOtherWeek = uLICalendarDetails.Where(x => x.StartDate.HasValue && x.EndDate.HasValue).SingleOrDefault(x => calendarDetail.StartDate >= x.StartDate.Value && calendarDetail.StartDate <= x.EndDate.Value && x.Week != calendarDetail.Week);
                    var startDateExistOnOtherWeekInDb = _calendarService.GetULICalendarDetails().Where(x => x.StartDate.HasValue && x.EndDate.HasValue && x.Id != calendarDetail.Id).SingleOrDefault(x => calendarDetail.StartDate >= x.StartDate.Value && calendarDetail.StartDate <= x.EndDate.Value && x.Week != calendarDetail.Week);
                    if (startDateExistOnOtherWeek != null)
                    {
                        ModelState.AddModelError("", "Start Date " + calendarDetail.StartDate.Value.ToString("dd/MM/yyyy") + " on Week: " + calendarDetail.Week + " already included on Week: " + startDateExistOnOtherWeek.Week + " Month: " + startDateExistOnOtherWeek.Month);
                    }
                    if (startDateExistOnOtherWeekInDb != null)
                    {
                        ModelState.AddModelError("", "Start Date " + calendarDetail.StartDate.Value.ToString("dd/MM/yyyy") + " on Week: " + calendarDetail.Week + " already included on Week: " + startDateExistOnOtherWeekInDb.Week + " Month: " + startDateExistOnOtherWeekInDb.Month);
                    }


                }

                var weeks = uLICalendarDetails.Where(x => x.Week != 0).Select(x => x.Week).ToList();
                var isWeekExist = _calendarService.GetULICalendarDetails().Where(x => weeks.Contains(x.Week) && x.Year == uLICalendar.Year && x.ULICalendarId != uLICalendar.Id).ToList();

                if (isWeekExist.Any())
                {

                    ModelState.AddModelError("", "Week: " + string.Join(", ", isWeekExist.Select(x => x.Week)) + " already exist on Month: " + String.Join(", ", isWeekExist.DistinctBy(x => x.Month).Select(x => x.Month)) + " and Year : " + String.Join(", ", isWeekExist.DistinctBy(x => x.Year).Select(x => x.Year)));

                }



                if (ModelState.ErrorCount > 0)
                {
                    return View(uLICalendar);
                }



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
