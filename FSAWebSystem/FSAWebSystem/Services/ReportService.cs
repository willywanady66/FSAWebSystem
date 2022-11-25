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
using System.Diagnostics.Metrics;
using System.Dynamic;
using System.Linq;
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

            if (currDate.Day == 1)
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

            CreateFile(data, "Publish Beginning of Month", 1);
        }


        public async Task<object> GenerateDailyReportData(DateTime currDate)
        {
            var zz = new List<string>();


            try
            {
                var fsaCalDetail = await _calendarService.GetCalendarDetail(currDate);

                var baseData = GetBaseReportData(currDate.Month, currDate.Year);

                var approvedProposal = _db.Approvals.Include(x => x.Proposal).Where(x => x.ApprovalStatus == ApprovalStatus.Approved && x.Proposal.SubmittedAt.Date == currDate.Date);

                var approvedProposalIds = approvedProposal.Select(x => x.Proposal.Id);

                var dailyRecord = (from proposalDetail in _db.ProposalDetails.Include(x => x.BannerPlant).Include(x => x.Proposal)
                                   join weeklyBucket in _db.WeeklyBuckets on proposalDetail.WeeklyBucketId equals weeklyBucket.Id
                                   where approvedProposalIds.Contains(proposalDetail.Proposal.Id) && proposalDetail.Proposal.SubmittedAt.Date == currDate.Date
                                   select new DailyRecordModel
                                   {
                                       WeeklyBucketId = weeklyBucket.Id,
                                       Rephase = proposalDetail.ActualRephase,
                                       ProposeAdditional = proposalDetail.ActualProposeAdditional,
                                       SubmitDate = proposalDetail.Proposal.SubmittedAt,
                                       Type = proposalDetail.Proposal.Type.Value
                                   }).AsEnumerable();

                if (fsaCalDetail.Week == 1)
                {
                    dailyRecord = dailyRecord.Where(x => x.Type == ProposalType.Rephase);
                }

                var obj = new DailyRecordModel();

                CreateFile(baseData, "Publish Day 1", fsaCalDetail.Week, dailyRecord);

                return obj;
            }

            catch (Exception ex)
            {
                return zz;
            }
        }

        private IEnumerable<BaseReportModel> GetBaseReportData(int month, int year)
        {
            var data = (from weeklyBucket in _db.WeeklyBuckets.Include(x => x.BannerPlant).Include(x => x.BannerPlant.Banner).Include(x => x.BannerPlant.Plant).Where(x => x.Month == month && x.Year == year)
                        join monthlyBucket in _db.MonthlyBuckets.Include(x => x.BannerPlant) on new { BannerPlantId = weeklyBucket.BannerPlant.Id, SKUId = weeklyBucket.SKUId } equals new { BannerPlantId = monthlyBucket.BannerPlant.Id, SKUId = monthlyBucket.SKUId }
                        join sku in _db.SKUs.Include(x => x.ProductCategory) on weeklyBucket.SKUId equals sku.Id
                        select new BaseReportModel
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

        private void CreateFile(IEnumerable<BaseReportModel> baseReportData, string title, int week, IEnumerable<DailyRecordModel>? dailyRecord = null)
        {
            //var currDate = DateTime.Now;
            var currDate = new DateTime(2022, 11, 16);
            var colToAddRephase = 0;
            var colToAddReallocate = 0;
            var colToAddPropose = 0;
            var month = currDate.ToString("MMM").ToUpper();
            var year = currDate.ToString("yy");
            var outputPath = Path.Combine(Directory.GetCurrentDirectory(), "ReportExcel", "Daily Report.xls");
            var listHistoryMonthly = new List<MonthlyBucketHistory>();

            var workbook = new HSSFWorkbook();
            ISheet worksheet = workbook.CreateSheet("v1.0");
            var titleRow = worksheet.CreateRow(0).CreateCell(0);

            var style = workbook.CreateCellStyle();
            style.Alignment = HorizontalAlignment.Center;
            var listCol = baseReportData.First().GetType().GetProperties().Select(x => x.Name).ToList();

            var groupDailyRecord = dailyRecord.GroupBy(x => new { x.WeeklyBucketId, x.SubmitDate.Date, x.Type }).ToList();
            var recordRephase = groupDailyRecord.Where(x => x.Key.Type == ProposalType.Rephase);
            colToAddRephase = recordRephase.Any() ? recordRephase.Max(x => x.Count()) : 0;
            var recordReallocate = groupDailyRecord.Where(x => x.Key.Type == ProposalType.ReallocateAcrossCDM || x.Key.Type == ProposalType.ReallocateAcrossKAM || x.Key.Type == ProposalType.ReallocateAcrossMT);
            colToAddReallocate = recordReallocate.Any() ? recordReallocate.Max(x => x.Count()) : 0;
            var recordPropose = groupDailyRecord.Where(x => x.Key.Type == ProposalType.ProposeAdditional);
            colToAddPropose = recordPropose.Any() ? recordPropose.Max(x => x.Count()) : 0;

            var listColToAdd = new List<int>
            {
                colToAddReallocate,
                colToAddPropose,
                colToAddRephase
            };

            var bucketHistory = _db.MonthlyBucketHistories.Where(x => x.Week == week && x.Year == currDate.Year).ToList();

            var maxVersion = 0;
            if (bucketHistory.Any()){
                maxVersion = bucketHistory.Select(x => x.Version).Max();
            }

            var maxColToAdd = listColToAdd.Max();

            var indexAfterMonthlyBucket = 13;
            var indexAfterWeek1 = 14;
            var indexAfterWeek2 = 15;

            for (var i = 1; i <= maxVersion+1; i++)
            {
                listCol.Insert(indexAfterMonthlyBucket, string.Format("{0}'{1} v1.{2}", month, year, (i)));
                indexAfterMonthlyBucket++;
                indexAfterWeek1++;
                indexAfterWeek2++;
            }

            for (var i = 0; i < colToAddRephase; i++)
            {
                listCol.Insert(indexAfterMonthlyBucket, "Rephase " + (i + 1) + "(+/-)");
                indexAfterMonthlyBucket++;
                indexAfterWeek1++;
                indexAfterWeek2++;
            }

            for (var i = 0; i < colToAddReallocate; i++)
            {

                listCol.Insert(indexAfterMonthlyBucket, "Reallocate " + (i + 1) + "(+)");
                indexAfterMonthlyBucket++;
                indexAfterWeek1++;
                indexAfterWeek2++;
                listCol.Insert(indexAfterMonthlyBucket, "Reallocate " + (i + 1) + "(-)");
                indexAfterMonthlyBucket++;
                indexAfterWeek1++;
                indexAfterWeek2++;
            }

            for (var i = 0; i < colToAddPropose; i++)
            {
                listCol.Insert(indexAfterMonthlyBucket, "Additional " + (i + 1) + "(+)");
                indexAfterMonthlyBucket++;
                indexAfterWeek1++;
                indexAfterWeek2++;
            }


            listCol.Insert(indexAfterWeek1, "Week " + week + " v1");
            indexAfterWeek1++;
            listCol.Insert(indexAfterWeek2, "Week " + (week + 1) + " v1");
            indexAfterWeek2++;


            CellRangeAddress range = new CellRangeAddress(0, 0, 0, listCol.Count - 1);
            worksheet.AddMergedRegion(range);

            titleRow.CellStyle = style;
            titleRow.SetCellValue(title);




            var percentStyle = workbook.CreateCellStyle();
            percentStyle.DataFormat = workbook.CreateDataFormat().GetFormat("0.00%");

            var accountingStyle = workbook.CreateCellStyle();
            accountingStyle.DataFormat = workbook.CreateDataFormat().GetFormat("_(* #,##0.00_);_(* (#,##0.00);_(* \"-\"??_);_(@_)");
            int x = 0;
            try
            {
                foreach (var data in baseReportData.OrderBy(x => x.BannerName.ToString()))
                {
                    var i = 0;
                    var row = worksheet.CreateRow(x + 2);
                    var weeklyBucketId = data.WeeklyBucketId;

                    row.CreateCell(i).SetCellValue(data.BannerName);
                    i++;

                    row.CreateCell(i).SetCellValue(data.PCMap);
                    i++;

                    row.CreateCell(i).SetCellValue(data.Description);
                    i++;

                    row.CreateCell(i).SetCellValue(data.Category);
                    i++;

                    row.CreateCell(i).SetCellValue(data.PlantCode);
                    i++;

                    row.CreateCell(i).SetCellValue(data.PlantName);
                    i++;

                    row.CreateCell(i).SetCellValue((double)data.Price);
                    row.GetCell(i).CellStyle = accountingStyle;
                    i++;

                    row.CreateCell(i).SetCellValue(Convert.ToDouble(data.PlantContribution / 100));
                    row.GetCell(i).CellStyle = percentStyle;
                    i++;

                    row.CreateCell(i).SetCellValue(Convert.ToDouble(data.RR));
                    i++;

                    row.CreateCell(i).SetCellValue(Convert.ToDouble(data.TCT / 100));
                    row.GetCell(i).CellStyle = percentStyle;
                    i++;

                    row.CreateCell(i).SetCellValue(Convert.ToDouble(data.Target) / 100);
                    row.GetCell(i).CellStyle = percentStyle;
                    i++;

                    var monthlyBucket = data.MonthlyBucket;
                    row.CreateCell(i).SetCellValue((double)monthlyBucket);
                    i++;


                    var rephaseDataThisRow = Enumerable.Repeat(decimal.Zero, colToAddRephase).ToList();
                    var reallocateDataThisRowTarget = Enumerable.Repeat(decimal.Zero, colToAddReallocate).ToList();
                    var reallocateDataThisRowSource = Enumerable.Repeat(decimal.Zero, colToAddReallocate).ToList();
                    var proposeDataThisRow = Enumerable.Repeat(decimal.Zero, colToAddPropose).ToList();
                    if (recordRephase.Any())
                    {
                        var rephaseDatas = recordRephase.SingleOrDefault(x => x.Key.WeeklyBucketId == weeklyBucketId).AsEnumerable();

                        if (rephaseDatas != null)
                        {
                            rephaseDatas = rephaseDatas.OrderBy(x => x.SubmitDate).AsEnumerable();
                            for (var j = 0; j < colToAddRephase; j++)
                            {

                                if (j < rephaseDatas.Count())
                                {
                                    rephaseDataThisRow[j] = rephaseDatas.ElementAt(j).Rephase;
                                }
                            }
                        }
                    }

                    if (recordPropose.Any())
                    {
                        var proposeDatas = recordPropose.SingleOrDefault(x => x.Key.WeeklyBucketId == weeklyBucketId).AsEnumerable();

                        if (proposeDatas != null)
                        {
                            proposeDatas = proposeDatas.OrderBy(x => x.SubmitDate).AsEnumerable();
                            for (var j = 0; j < colToAddRephase; j++)
                            {

                                if (j < proposeDatas.Count())
                                {
                                    proposeDataThisRow[j] = proposeDatas.ElementAt(j).ProposeAdditional;
                                }
                            }
                        }
                    }


                    if (recordReallocate.Any())
                    {
                        var reallocateDatas = recordReallocate.SingleOrDefault(x => x.Key.WeeklyBucketId == weeklyBucketId).AsEnumerable();

                        if (reallocateDatas != null)
                        {
                            reallocateDatas = reallocateDatas.OrderBy(x => x.SubmitDate).AsEnumerable();
                            for (var j = 0; j < colToAddReallocate; j++)
                            {

                                if (j < reallocateDatas.Count())
                                {
                                    if (reallocateDatas.ElementAt(j).ProposeAdditional < 0)
                                    {
                                        reallocateDataThisRowSource[j] = reallocateDatas.ElementAt(j).ProposeAdditional;
                                    }
                                    else
                                    {
                                        reallocateDataThisRowTarget[j] = reallocateDatas.ElementAt(j).ProposeAdditional;
                                    }
                                }
                            }
                        }
                    }

                    for(var j = 1; j <= maxVersion; j++)
                    {
                        var monthlyBucketData = bucketHistory.SingleOrDefault(x => x.WeeklyBucketId == weeklyBucketId && x.Version == j) != null ? bucketHistory.Single(x => x.WeeklyBucketId == weeklyBucketId && x.Version == j).MonthlyBucket : decimal.Zero;
                        row.CreateCell(i).SetCellValue((double)monthlyBucketData);
                        i++;
                    }

                    var updatedMonthlyBucket = decimal.Zero;
                    for (var j = 0; j < maxColToAdd; j++)
                    {
                        updatedMonthlyBucket = monthlyBucket + reallocateDataThisRowSource[j] + reallocateDataThisRowTarget[j];
                        row.CreateCell(i).SetCellValue((double)updatedMonthlyBucket);
                        i++;
                    }


                    //Rephase
                    for (var j = 0; j < colToAddRephase; j++)
                    {
                        row.CreateCell(i).SetCellValue((double)rephaseDataThisRow.ElementAt(j));
                        i++;
                    }

                    for (var j = 0; j < colToAddReallocate; j++)
                    {
                        row.CreateCell(i).SetCellValue((double)reallocateDataThisRowTarget.ElementAt(j));
                        i++;
                        row.CreateCell(i).SetCellValue((double)reallocateDataThisRowSource.ElementAt(j));
                        i++;
                    }

                    //Propose Additional
                    for (var j = 0; j < colToAddPropose; j++)
                    {
                        row.CreateCell(i).SetCellValue((double)proposeDataThisRow.ElementAt(j));
                        i++;
                    }




                    var week1 = Convert.ToDouble(data.Week1);
                    var week1Val = week1;
                    row.CreateCell(i).SetCellValue(week1);
                    i++;

                    week1Val += (double)rephaseDataThisRow.Sum(x => x) + (double)reallocateDataThisRowTarget.Sum(x => x) + (double)reallocateDataThisRowSource.Sum(x => x);
                    row.CreateCell(i).SetCellValue(week1Val);
                    i++;



                    var week2 = Convert.ToDouble(data.Week2);
                    var week2Val = week2;
                    row.CreateCell(i).SetCellValue(week2);
                    i++;


                    week2Val -= (double)rephaseDataThisRow.Sum(x => x);
                    row.CreateCell(i).SetCellValue(week2Val);
                    i++;

                    var validBJ = data.ValidBJ;
                    row.CreateCell(i).SetCellValue((double)validBJ);
                    i++;

                    var remFSA = monthlyBucket - validBJ;
                    row.CreateCell(i).SetCellValue((double)remFSA);
                    row.GetCell(i).CellStyle = accountingStyle;
                    i++;
                    x++;

                    var historyMonthly = new MonthlyBucketHistory();
                    historyMonthly.Id = Guid.NewGuid();
                    historyMonthly.MonthlyBucket = updatedMonthlyBucket;
                    historyMonthly.WeeklyBucketId = weeklyBucketId;
                    historyMonthly.Version = currDate.Day;
                    historyMonthly.Month = currDate.Month;
                    historyMonthly.Year = currDate.Year;
                    historyMonthly.Week = week;
                    listHistoryMonthly.Add(historyMonthly);
                }

                var colRow = worksheet.CreateRow(1);
                for (var i = 1; i < listCol.Count; i++)
                {
                    if (listCol[i] == "Target")
                    {
                        listCol[i] = "Target " + month + "%";
                    }
                    else if (listCol[i] == "Week1")
                    {
                        listCol[i] = "Week" + week;
                    }
                    else if (listCol[i] == "Week2")
                    {
                        listCol[i] = "Week " + (week + 1);
                    }
                    else if (listCol[i] == "MonthlyBucket")
                    {
                        listCol[i] = string.Format("{0}'{1} v1.0", month, year);
                    }
                    else if (listCol[i] == "TCT")
                    {
                        listCol[i] += "%";
                    }
                    else if (listCol[i] == "RemFSA")
                    {
                        listCol[i] = "REM FSA";
                    }
                    colRow.CreateCell(i - 1).SetCellValue(listCol[i]);
                    worksheet.AutoSizeColumn(i);
                }

                MemoryStream ms = new MemoryStream();
                workbook.Write(ms);

                using (FileStream outputStream = new FileStream(outputPath, FileMode.Create))
                {
                    ms.Position = 0;
                    ms.CopyTo(outputStream);
                }

                _db.MonthlyBucketHistories.AddRange(listHistoryMonthly);
                _db.SaveChanges();
                //FileStreamResult file = File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Daily Report.xls");
            }
            catch (Exception ex)
            {

            }
        }


        private void GenerateHistoryReport(IEnumerable<object> datas)
        {

        }

    }
}
