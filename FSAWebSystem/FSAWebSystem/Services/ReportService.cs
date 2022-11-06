using FSAWebSystem.Models;
using FSAWebSystem.Models.Bucket;
using FSAWebSystem.Models.Context;
using FSAWebSystem.Models.ViewModels;
using FSAWebSystem.Services.Interface;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NPOI.HSSF.UserModel;
using NPOI.POIFS.Crypt.Dsig;
using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using System.Dynamic;
using System.Threading.Tasks.Dataflow;

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

		public async Task GenerateFirstReportOfMonth(int month, int year)
		{
			dynamic obj = new ExpandoObject();

            var data = GetBaseReportData(month, year);
           
            CreateFile(data, "Publish Beginning of Month");
        }


        public async Task<object> GenerateDailyReportData(DateTime currDate)
        {
            var zz = new List<string>();
         
         
            try
            {
                var fsaCalDetail = await _calendarService.GetCalendarDetail(currDate);

                var baseData = GetBaseReportData(currDate.Month, currDate.Year);

                var approvedProposal = _db.Approvals.Include(x => x.Proposal).Where(x => x.ApprovalStatus == ApprovalStatus.Approved);

                var approvedProposalIds = approvedProposal.Where(x => x.Proposal.Type == ProposalType.Rephase).Select(x => x.Proposal.Id);

                var dailyRecord = (from proposalDetail in _db.ProposalDetails.Include(x => x.BannerPlant).Include(x => x.Proposal)
                                   join weeklyBucket in _db.WeeklyBuckets on proposalDetail.WeeklyBucketId equals weeklyBucket.Id
                                   where approvedProposalIds.Contains(proposalDetail.Proposal.Id ) && proposalDetail.Proposal.SubmittedAt.Date == currDate.Date
                                   select new DailyRecordModel
                                   {
                                       WeeklyBucketId = weeklyBucket.Id,
                                       Rephase = proposalDetail.ActualRephase,
                                       ProposeAdditional = proposalDetail.ActualProposeAdditional,
                                       SubmitDate = proposalDetail.Proposal.SubmittedAt,
                                       Type = proposalDetail.Proposal.Type.Value
                                   }).AsEnumerable();

                if(fsaCalDetail.Week == 1)
                {
                    dailyRecord = dailyRecord.Where(x => x.Type == ProposalType.Rephase);
                }

                //var group = dailyRecord.GroupBy(x => new { x.WeeklyBucketId, x.SubmitDate.Date, x.Type }).ToList();
                //var recordRephase = group.Where(x => x.Key.Type == ProposalType.Rephase);
                //colToAddRephase = recordRephase.Any() ? recordRephase.Max(x => x.Count()) : 0;
                //var recordReallocate = group.Where(x => x.Key.Type == ProposalType.ReallocateAcrossCDM || x.Key.Type == ProposalType.ReallocateAcrossKAM || x.Key.Type == ProposalType.ReallocateAcrossMT);
                //colToAddReallocate = recordReallocate.Any() ? recordReallocate.Max(x => x.Count()) : 0;
                //var recordPropose = group.Where(x => x.Key.Type == ProposalType.ProposeAdditional);
                //colToAddPropose = recordPropose.Any() ? recordPropose.Max(x => x.Count()) : 0;
                var obj = new DailyRecordModel();

                CreateFile(baseData, "Publish Day 1", dailyRecord);

                return obj;
            }
             
            catch (Exception ex)
            {
                return zz;
            }

    

        
        }

        private IEnumerable<object> GetBaseReportData(int month, int year)
        {
            var data = (from weeklyBucket in _db.WeeklyBuckets.Include(x => x.BannerPlant).Include(x => x.BannerPlant.Banner).Include(x => x.BannerPlant.Plant).Where(x => x.Month == month && x.Year == year)
                        join monthlyBucket in _db.MonthlyBuckets.Include(x => x.BannerPlant) on new { BannerPlantId = weeklyBucket.BannerPlant.Id, SKUId = weeklyBucket.SKUId } equals new { BannerPlantId = monthlyBucket.BannerPlant.Id, SKUId = monthlyBucket.SKUId }
                        join sku in _db.SKUs.Include(x => x.ProductCategory) on weeklyBucket.SKUId equals sku.Id
                        select new
                        {
                            WeeklyBucketId = weeklyBucket.Id,
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

            return data;
        }

		private void CreateFile(IEnumerable<object> datas, string title, IEnumerable<DailyRecordModel>? dailyRecord = null)
		{
            var currDate = DateTime.Now;
            var colToAddRephase = 0;
            var colToAddReallocate = 0;
            var colToAddPropose = 0;

            var outputPath = Path.Combine(Directory.GetCurrentDirectory(), "ReportExcel", "Daily Report.xls");
            var workbook = new HSSFWorkbook();
            ISheet worksheet = workbook.CreateSheet("v1.0");
            var titleRow = worksheet.CreateRow(0).CreateCell(0);

            var style = workbook.CreateCellStyle();
            style.Alignment = HorizontalAlignment.Center;
            var listCol = datas.First().GetType().GetProperties().Select(x => x.Name).ToList();

            var groupDailyRecord = dailyRecord.GroupBy(x => new { x.WeeklyBucketId, x.SubmitDate.Date, x.Type }).ToList();
            var recordRephase = groupDailyRecord.Where(x => x.Key.Type == ProposalType.Rephase);
            colToAddRephase = recordRephase.Any() ? recordRephase.Max(x => x.Count()) : 0;
            var recordReallocate = groupDailyRecord.Where(x => x.Key.Type == ProposalType.ReallocateAcrossCDM || x.Key.Type == ProposalType.ReallocateAcrossKAM || x.Key.Type == ProposalType.ReallocateAcrossMT);
            colToAddReallocate = recordReallocate.Any() ? recordReallocate.Max(x => x.Count()) : 0;
            var recordPropose = groupDailyRecord.Where(x => x.Key.Type == ProposalType.ProposeAdditional);
            colToAddPropose = recordPropose.Any() ? recordPropose.Max(x => x.Count()) : 0;


            var indexAfterMonthlyBucket = 13;
            var indexAfterWeek1 = 14;
            var indexAfterWeek2 = 15;
            for (var i = 0; i < colToAddRephase; i++)
            {
                listCol.Insert(indexAfterMonthlyBucket, "Rephase " + (i + 1) + "(+/-)");
                indexAfterMonthlyBucket++;
                indexAfterWeek1++;
                indexAfterWeek2++;
                listCol.Insert(indexAfterWeek1, "Week 1 v" + (i + 1));
                indexAfterWeek1++;
                indexAfterWeek2++;
                listCol.Insert(indexAfterWeek2, "Week 2 v" + (i + 1));
                indexAfterWeek2++;
            }


            CellRangeAddress range = new CellRangeAddress(0, 0, 0, listCol.Count - 1);
            worksheet.AddMergedRegion(range);

            titleRow.CellStyle = style;
            titleRow.SetCellValue(title);

            var month = currDate.ToString("MMM").ToUpper();
            var year = currDate.ToString("yy");

            var percentStyle = workbook.CreateCellStyle();
            percentStyle.DataFormat = workbook.CreateDataFormat().GetFormat("0.00%");

            var accountingStyle = workbook.CreateCellStyle();
            accountingStyle.DataFormat = workbook.CreateDataFormat().GetFormat("_(* #,##0.00_);_(* (#,##0.00);_(* \"-\"??_);_(@_)");
            int x = 0;
            try{ 
                foreach (var data in datas.OrderBy(x => x.GetType().GetProperty("BannerName").GetValue(x, null).ToString()))
                {
                    var i = 0;
                    var row = worksheet.CreateRow(x + 2);

                    var item = data.GetType();

                    var weeklyBucketId = Guid.Parse(item.GetProperty("WeeklyBucketId").GetValue(data, null).ToString());





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
                    row.GetCell(i).CellStyle = accountingStyle;
                    i++;
                    row.CreateCell(i).SetCellValue(Convert.ToDouble(item.GetProperty("PlantContribution").GetValue(data, null)) / 100);
                    row.GetCell(i).CellStyle = percentStyle;
                    i++;
                    row.CreateCell(i).SetCellValue(Convert.ToDouble(item.GetProperty("RR").GetValue(data, null)));
                    i++;
                    row.CreateCell(i).SetCellValue(Convert.ToDouble(item.GetProperty("TCT").GetValue(data, null)) / 100);
                    row.GetCell(i).CellStyle = percentStyle;
                    i++;
                    row.CreateCell(i).SetCellValue(Convert.ToDouble(item.GetProperty("Target").GetValue(data, null)) / 100);
                    row.GetCell(i).CellStyle = percentStyle;
                    i++;
                    var monthlyBucket = Convert.ToDouble(item.GetProperty("MonthlyBucket").GetValue(data, null));
                    row.CreateCell(i).SetCellValue(monthlyBucket);
                    i++;
                    var rephase = decimal.Zero;
                    var rephaseDataThisRow = Enumerable.Repeat(decimal.Zero, colToAddRephase).ToList();
                    if (recordRephase.Any())
                    {
                        var rephaseDatas = recordRephase.SingleOrDefault(x => x.Key.WeeklyBucketId == weeklyBucketId).AsEnumerable();

                        if (rephaseDatas != null)
                        {
                            rephaseDatas = rephaseDatas.OrderBy(x => x.SubmitDate).AsEnumerable();
                            for (var j = 0; j < colToAddRephase; j++)
                            {

                                if(j < rephaseDatas.Count())
                                {
                                    rephaseDataThisRow[j] = rephaseDatas.ElementAt(j).Rephase;
                                }
                            }
                        }
                    }
                    for (var j = 0; j < colToAddRephase; j++)
                    {
                        row.CreateCell(i).SetCellValue((double)rephaseDataThisRow.ElementAt(j));
                        i++;
                    }



                    var week1 = Convert.ToDouble(item.GetProperty("Week1").GetValue(data, null));
                    var week1Val = week1;
                    row.CreateCell(i).SetCellValue(week1);
                    i++;
                    for (var j = 0; j < colToAddRephase; j++)
                    {
                        week1Val += (double)rephaseDataThisRow.ElementAt(j);
                        row.CreateCell(i).SetCellValue(week1Val);
                        i++;
                    }
                    var week2 = Convert.ToDouble(item.GetProperty("Week2").GetValue(data, null));
                    var week2Val = week2;
                    row.CreateCell(i).SetCellValue(week2);
                    i++;
                    for (var j = 0; j < colToAddRephase; j++)
                    {
                        week2Val -= (double)rephaseDataThisRow.ElementAt(j);
                        row.CreateCell(i).SetCellValue(week2Val);
                        i++;
                    }

                    var validBJ = Convert.ToDouble(item.GetProperty("ValidBJ").GetValue(data, null));
                    row.CreateCell(i).SetCellValue(validBJ);
                    i++;
                    var remFSA = monthlyBucket - validBJ;
                    row.CreateCell(i).SetCellValue(remFSA);
                    row.GetCell(i).CellStyle = accountingStyle;
                    i++;
                    x++;
                }

                var colRow = worksheet.CreateRow(1);
                for (var i = 1; i < listCol.Count; i++)
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
                    colRow.CreateCell(i-1).SetCellValue(listCol[i]);
                    worksheet.AutoSizeColumn(i);
                }

                MemoryStream ms = new MemoryStream();
                workbook.Write(ms);

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
