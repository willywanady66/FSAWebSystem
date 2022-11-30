using FSAWebSystem.Models;
using FSAWebSystem.Models.ViewModels;
using FSAWebSystem.Services;
using FSAWebSystem.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NuGet.Protocol;
using Org.BouncyCastle.Asn1.Mozilla;

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


        [Authorize(Policy = ("ReportPage"))]
        [HttpPost]
        public async Task<IActionResult> DownloadReport(Guid reportId)
        {
            return Ok();
        }
        
        public async Task<IActionResult> GetBegOfMonth()
        {
            await _reportService.GenerateFirstReportOfMonth(12, 2022);
            return Ok();
        }
        public async Task<IActionResult> GetDailyReportData()
        {
            var currDate = DateTime.Now;
            //var currDate = new DateTime(2022, 11, 15);
            //var datas = await _reportService.GenerateFirstReportOfMonth(currDate.Month, currDate.Year);

            var dt = await _reportService.GenerateDailyReportData(currDate);

            //var workbook = new HSSFWorkbook();
            //ISheet worksheet = workbook.CreateSheet("v1.0");
            //var title = worksheet.CreateRow(0).CreateCell(0);

            //var style = workbook.CreateCellStyle();
            //style.Alignment = HorizontalAlignment.Center;


            //var listCol = datas.First().GetType().GetProperties().Select(x => x.Name).ToList();

            //CellRangeAddress range = new CellRangeAddress(0, 0, 0, listCol.Count - 1);
            //worksheet.AddMergedRegion(range);

            //title.CellStyle = style;
            //title.SetCellValue("Publish Beginning of Month");



            //var month = currDate.ToString("MMM").ToUpper();
            //var year = currDate.ToString("yy");

            //int x = 0;
            //foreach(var data in datas)
            //{
            //    var i = 0;
            //    var row = worksheet.CreateRow(x + 2);

            //    var item = data.GetType();

            //    row.CreateCell(i).SetCellValue(item.GetProperty("BannerName").GetValue(data, null).ToString());
            //    i++;
            //    row.CreateCell(i).SetCellValue(item.GetProperty("PCMap").GetValue(data, null).ToString());
            //    i++;
            //    row.CreateCell(i).SetCellValue(item.GetProperty("Description").GetValue(data, null).ToString());
            //    i++;
            //    row.CreateCell(i).SetCellValue(item.GetProperty("Category").GetValue(data, null).ToString());
            //    i++;
            //    row.CreateCell(i).SetCellValue(item.GetProperty("PlantCode").GetValue(data, null).ToString());
            //    i++;
            //    row.CreateCell(i).SetCellValue(item.GetProperty("PlantName").GetValue(data, null).ToString());
            //    i++;
            //    row.CreateCell(i).SetCellValue(Convert.ToDouble(item.GetProperty("Price").GetValue(data, null)));
            //    i++;
            //    row.CreateCell(i).SetCellValue(Convert.ToDouble(item.GetProperty("PlantContribution").GetValue(data, null)));
            //    i++;
            //    row.CreateCell(i).SetCellValue(Convert.ToDouble(item.GetProperty("RR").GetValue(data, null)));
            //    i++;
            //    row.CreateCell(i).SetCellValue(Convert.ToDouble(item.GetProperty("TCT").GetValue(data, null)));
            //    i++;
            //    row.CreateCell(i).SetCellValue(Convert.ToDouble(item.GetProperty("Target").GetValue(data, null)));
            //    i++;
            //    row.CreateCell(i).SetCellValue(Convert.ToDouble(item.GetProperty("MonthlyBucket").GetValue(data, null)));
            //    i++;
            //    row.CreateCell(i).SetCellValue(Convert.ToDouble(item.GetProperty("Week1").GetValue(data, null)));
            //    i++;
            //    row.CreateCell(i).SetCellValue(Convert.ToDouble(item.GetProperty("Week2").GetValue(data, null)));
            //    i++;
            //    row.CreateCell(i).SetCellValue(Convert.ToDouble(item.GetProperty("ValidBJ").GetValue(data, null)));
            //    i++;
            //    row.CreateCell(i).SetCellValue(Convert.ToDouble(item.GetProperty("RemFSA").GetValue(data, null)));
            //    row.Cells[i].CellStyle.DataFormat = workbook.CreateDataFormat().GetFormat("_(* #,##0.00_);_(* (#,##0.00);_(* \"-\"??_);_(@_)");
            //    i++;
            //    x++;
            //}

            //var colRow = worksheet.CreateRow(1);
            //for (var i = 0; i < listCol.Count; i++)
            //{
            //    if (listCol[i] == "Target")
            //    {
            //        listCol[i] = "Target " + month + "%";
            //    }
            //    else if (listCol[i] == "MonthlyBucket")
            //    {
            //        listCol[i] = string.Format("{0}'{1}", month, year);
            //    }
            //    else if (listCol[i] == "TCT")
            //    {
            //        listCol[i] += "%";
            //    }
            //    else if (listCol[i] == "RemFSA")
            //    {
            //        listCol[i] = "REM FSA";
            //    }
            //    colRow.CreateCell(i).SetCellValue(listCol[i]);
            //    worksheet.AutoSizeColumn(i);
            //}

            //MemoryStream ms = new MemoryStream();
            //workbook.Write(ms);
            //ms.Position = 0;
            try
            {
                //FileStreamResult file = File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Daily Report.xls");
                //return file;
            }
            catch (Exception ex)
            {
                //return Ok();
            }
            return Ok();
        }
    }
        
}
