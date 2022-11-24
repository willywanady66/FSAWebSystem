using FSAWebSystem.Models;
using FSAWebSystem.Models.Bucket;
using FSAWebSystem.Models.Context;
using FSAWebSystem.Models.ViewModels;
using FSAWebSystem.Services.Interface;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NPOI.HSSF.UserModel;
using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using System.Dynamic;

namespace FSAWebSystem.Services
{
	public class ReportService : IReportService
	{
		public FSAWebSystemDbContext _db;
		private readonly ICalendarService _calendarService;

		public ReportService(FSAWebSystemDbContext db, ICalendarService calendarService)
		{
			_db = db;
			_calendarService = calendarService;
		}

		public async Task<ReportPagingData> GetDailyReports(DataTableParam param)
		{
			var reports = _db.Reports.Where(x => x.Type == ReportType.Daily);
			var listReports = await reports.Skip(param.start).Take(param.length).ToListAsync();
			return new ReportPagingData
			{
				totalRecord = reports.Count(),
				reports = reports.ToList()
			};
		}

		public async Task<ReportPagingData> GetWeeklyReports(DataTableParam param)
		{
            var reports = _db.Reports.Where(x => x.Type == ReportType.Weekly);
            var listReports = await reports.Skip(param.start).Take(param.length).ToListAsync();
            return new ReportPagingData
            {
                totalRecord = reports.Count(),
                reports = reports.ToList()
            };
        }

		public async Task GenerateDailyReport()
		{
			var currDate = DateTime.Now;
			var listColumn = new List<string>
			{
				"PC MAP",
				"Description",
				"Category",
				"Plant Code",
				"Plant",
				"Price",
				"Plant Contribution",
				"RR",
				"TCT%",
				"Target",
				"WK 1",
				"WK 2",
				"Valid + BJ",
				"REM FSA"
			};

			if(currDate.Day == 1)
			{

			}
		}

        public async Task GenerateWeeklyReport()
        {
            var currDate = DateTime.Now;
            var listColumn = new List<string>
            {
                "PC MAP",
                "Description",
                "Category",
                "Plant Code",
                "Plant",
                "Price",
                "Plant Contribution",
                "RR",
                "TCT%",
                "Target"
            };
        }

		public async Task<IEnumerable<object>> GenerateFirstReportOfMonth(int month, int year)
		{
			dynamic obj = new ExpandoObject();
			var data = (from weeklyBucket in _db.WeeklyBuckets.Include(x => x.BannerPlant).Include(x => x.BannerPlant.Banner).Include(x => x.BannerPlant.Plant).Where(x => x.Month == month && x.Year == year)
						join monthlyBucket in _db.MonthlyBuckets.Include(x => x.BannerPlant) on new { BannerPlantId = weeklyBucket.BannerPlant.Id, SKUId = weeklyBucket.SKUId } equals new { BannerPlantId = monthlyBucket.BannerPlant.Id, SKUId = monthlyBucket.SKUId }
						join sku in _db.SKUs.Include(x => x.ProductCategory) on weeklyBucket.SKUId equals sku.Id
						select new
						{
							BannerName = weeklyBucket.BannerPlant.Banner.BannerName,
							PCMap = sku.PCMap,
							Description = sku.DescriptionMap,
							Category = sku.ProductCategory.CategoryProduct,
							PlantCode = weeklyBucket.BannerPlant.Plant.PlantCode,
							PlantName = weeklyBucket.BannerPlant.Plant.PlantName,
							Price = monthlyBucket.Price,
							PlantContribution = weeklyBucket.PlantContribution,
							RR = weeklyBucket.RatingRate,
							TCT = monthlyBucket.TCT,
							Target = monthlyBucket.MonthlyTarget,
							MonthlyBucket = monthlyBucket.RatingRate * (monthlyBucket.TCT / 100) * (monthlyBucket.MonthlyTarget / 100),
							Week1 = monthlyBucket.RatingRate * (monthlyBucket.TCT / 100) * (monthlyBucket.MonthlyTarget / 100) * ((decimal)50 / (decimal)100),
							Week2 = monthlyBucket.RatingRate * (monthlyBucket.TCT / 100) * (monthlyBucket.MonthlyTarget / 100) * ((decimal)50 / (decimal)100),
							ValidBJ = weeklyBucket.ValidBJ,
							RemFSA = monthlyBucket.RatingRate * (monthlyBucket.TCT / 100) * (monthlyBucket.MonthlyTarget / 100) - weeklyBucket.ValidBJ
						}).AsEnumerable();



			CreateFile(data, "Publish Beginning of Month");


            return data;
        }


		private void CreateFile(IEnumerable<object> datas, string title)
		{
            var currDate = DateTime.Now;
            var outputPath = Path.Combine(Directory.GetCurrentDirectory(), "ReportExcel", "Daily Report.xls");
            var workbook = new HSSFWorkbook();
            ISheet worksheet = workbook.CreateSheet("v1.0");
            var titleRow = worksheet.CreateRow(0).CreateCell(0);

            var style = workbook.CreateCellStyle();
            style.Alignment = HorizontalAlignment.Center;
            var listCol = datas.First().GetType().GetProperties().Select(x => x.Name).ToList();

            CellRangeAddress range = new CellRangeAddress(0, 0, 0, listCol.Count - 1);
            worksheet.AddMergedRegion(range);

            titleRow.CellStyle = style;
            titleRow.SetCellValue(title);

            var month = currDate.ToString("MMM").ToUpper();
            var year = currDate.ToString("yy");

            int x = 0;
            foreach (var data in datas)
            {
                var i = 0;
                var row = worksheet.CreateRow(x + 2);

                var item = data.GetType();

                row.CreateCell(i).SetCellValue(item.GetProperty("BannerName").GetValue(data, null).ToString());
                i++;
                row.CreateCell(i).SetCellValue(item.GetProperty("PCMap").GetValue(data, null).ToString());
                i++;
                row.CreateCell(i).SetCellValue(item.GetProperty("Description").GetValue(data, null).ToString());
                i++;
                row.CreateCell(i).SetCellValue(item.GetProperty("Category").GetValue(data, null).ToString());
                i++;
                row.CreateCell(i).SetCellValue(item.GetProperty("PlantCode").GetValue(data, null).ToString());
                i++;
                row.CreateCell(i).SetCellValue(item.GetProperty("PlantName").GetValue(data, null).ToString());
                i++;
                row.CreateCell(i).SetCellValue(Convert.ToDouble(item.GetProperty("Price").GetValue(data, null)));
                i++;
                row.CreateCell(i).SetCellValue(Convert.ToDouble(item.GetProperty("PlantContribution").GetValue(data, null))/100);
                row.GetCell(i).CellStyle.DataFormat = workbook.CreateDataFormat().GetFormat("0.00%");
                i++;
                row.CreateCell(i).SetCellValue(Convert.ToDouble(item.GetProperty("RR").GetValue(data, null)));  
                i++;
                row.CreateCell(i).SetCellValue(Convert.ToDouble(item.GetProperty("TCT").GetValue(data, null))/100);
                row.GetCell(i).CellStyle.DataFormat = workbook.CreateDataFormat().GetFormat("0.00%");
                i++;
                row.CreateCell(i).SetCellValue(Convert.ToDouble(item.GetProperty("Target").GetValue(data, null)) / 100);
                row.GetCell(i).CellStyle.DataFormat = workbook.CreateDataFormat().GetFormat("0.00%");
                i++;
                row.CreateCell(i).SetCellValue(Convert.ToDouble(item.GetProperty("MonthlyBucket").GetValue(data, null)));
                i++;
                row.CreateCell(i).SetCellValue(Convert.ToDouble(item.GetProperty("Week1").GetValue(data, null)));
                i++;
                row.CreateCell(i).SetCellValue(Convert.ToDouble(item.GetProperty("Week2").GetValue(data, null)));
                i++;
                row.CreateCell(i).SetCellValue(Convert.ToDouble(item.GetProperty("ValidBJ").GetValue(data, null)));
                i++;
                row.CreateCell(i).SetCellValue(Convert.ToDouble(item.GetProperty("RemFSA").GetValue(data, null)));
                row.GetCell(i).CellStyle.DataFormat = workbook.CreateDataFormat().GetFormat("_(* #,##0.00_);_(* (#,##0.00);_(* \"-\"??_);_(@_)");
                i++;
                x++;
            }

            var colRow = worksheet.CreateRow(1);
            for (var i = 0; i < listCol.Count; i++)
            {
                if (listCol[i] == "Target")
                {
                    listCol[i] = "Target " + month + "%";
                }
                else if (listCol[i] == "MonthlyBucket")
                {
                    listCol[i] = string.Format("{0}'{1}", month, year);
                }
                else if (listCol[i] == "TCT")
                {
                    listCol[i] += "%";
                }
                else if (listCol[i] == "RemFSA")
                {
                    listCol[i] = "REM FSA";
                }
                colRow.CreateCell(i).SetCellValue(listCol[i]);
                worksheet.AutoSizeColumn(i);
            }

            MemoryStream ms = new MemoryStream();
            workbook.Write(ms);

            try
            {
                using (FileStream outputStream = new FileStream(outputPath, FileMode.Create))
                {
                    ms.Position = 0;
                    ms.CopyTo(outputStream);
                }
                //FileStreamResult file = File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Daily Report.xls");
            }
            catch (Exception ex)
            {

            }
        }

    }
}
