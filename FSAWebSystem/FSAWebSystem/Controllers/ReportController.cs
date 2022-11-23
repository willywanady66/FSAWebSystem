using FSAWebSystem.Models;
using FSAWebSystem.Models.ViewModels;
using FSAWebSystem.Services;
using FSAWebSystem.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FSAWebSystem.Controllers
{
    public class ReportController : Controller
    {
        private readonly ICalendarService _calendarService;
        private readonly IReportService _reportService;
        public ReportController(ICalendarService calendarService, IReportService reportService)
        {
            _calendarService = calendarService;
            _reportService = reportService;
        }
        public IActionResult Index()
        {
            ViewData["ListMonth"] = _calendarService.GetListMonth();
            ViewData["ListYear"] = _calendarService.GetListYear();
            return View();
        }



        [Authorize(Policy = ("ReportPage"))]
        [HttpPost]
        public async Task<IActionResult> GetDailyReports(DataTableParam param, int month, int year)
        {
            var reports = new List<Report>();
            var listData = Json(new { });
            try
            {
               var reportsData= await _reportService.GetDailyReports(param);
                listData = Json (new
                {
                    draw = param.draw,
                    recordsTotal = reportsData.totalRecord,
                    recordsFiltered = reportsData.totalRecord,
                    data = reportsData.reports
                });
            }
            catch (Exception ex)
            {
                listData = Json(new { error = ex.Message});
            }

            return listData;

      
        }


        [Authorize(Policy = ("ReportPage"))]
        [HttpPost]
        public async Task<IActionResult> GetWeeekyReports(DataTableParam param, int month, int year)
        {
            var reports = new List<Report>();
            var listData = Json(new { });
            try
            {
                var reportsData = await _reportService.GetWeeklyReports(param);
                listData = Json(new
                {
                    draw = param.draw,
                    recordsTotal = reportsData.totalRecord,
                    recordsFiltered = reportsData.totalRecord,
                    data = reportsData.reports
                });
            }
            catch (Exception ex)
            {
                listData = Json(new { error = ex.Message });
            }

            return listData;
        }
    }
}
