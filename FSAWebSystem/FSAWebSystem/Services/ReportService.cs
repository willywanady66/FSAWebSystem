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

        public async Task<string> GenerateFirstReportOfMonth(int month, int year)
        {
            dynamic obj = new ExpandoObject();

            var data = GetBaseReportData(month, year);
            var dailyRecords = new List<DailyRecordModel>().AsEnumerable();
            var dt = await CreateFile(data, "Publish Beginning of Month", new DateTime(2022, 12, 01), dailyRecords);
            return dt;
        }


        public async Task<object> GenerateDailyReportData(DateTime currDate)
        {
            var zz = new List<string>();


            try
            {
                var fsaCalDetail = await _calendarService.GetCalendarDetail(currDate);

                var baseData = GetBaseReportData(currDate.Month, currDate.Year);
                var startDate = new DateTime(currDate.Year, currDate.Month, 1);
                var approvedProposal = _db.Approvals.Include(x => x.Proposal).Where(x => x.ApprovalStatus == ApprovalStatus.Approved && x.Proposal.SubmittedAt.Date <= currDate.Date && x.Proposal.SubmittedAt.Date >= startDate.Date);

                var approvedProposalIds = approvedProposal.Select(x => x.Proposal.Id);



                var dailyRecord = (from proposalDetail in _db.ProposalDetails.Include(x => x.BannerPlant).Include(x => x.Proposal).Where(y => y.Proposal.SubmittedAt.Date >= startDate.Date && y.Proposal.SubmittedAt.Date <= currDate.Date)
                                   join weeklyBucket in _db.WeeklyBuckets on proposalDetail.WeeklyBucketId equals weeklyBucket.Id
                                   where approvedProposalIds.Contains(proposalDetail.Proposal.Id)
                                   select new DailyRecordModel
                                   {
                                       WeeklyBucketId = weeklyBucket.Id,
                                       Rephase = proposalDetail.ActualRephase,
                                       ProposeAdditional = proposalDetail.ActualProposeAdditional,
                                       SubmitDate = proposalDetail.Proposal.SubmittedAt,
                                       Type = proposalDetail.Proposal.Type.Value,
                                       Week = proposalDetail.Proposal.Week
                                   }).AsEnumerable();

                if (fsaCalDetail.Week == 1)
                {
                    dailyRecord = dailyRecord.Where(x => x.Type == ProposalType.Rephase);
                }

                var obj = new DailyRecordModel();

                var zza = _db.ProposalDetails.Include(x => x.BannerPlant).Include(x => x.Proposal).Where(y => y.Proposal.SubmittedAt.Date >= startDate.Date && y.Proposal.SubmittedAt.Date <= currDate.Date).ToList();

                var dt = await CreateFile(baseData, "Publish Day 1", currDate, dailyRecord);

                return dt;
            }

            catch (Exception ex)
            {
                return zz;
            }
        }

        private IEnumerable<BaseReportModel> GetBaseReportData(int month, int year)
        {
            var data = (from weeklyBucket in _db.WeeklyBuckets.Include(x => x.BannerPlant).Include(x => x.BannerPlant.Banner).Include(x => x.BannerPlant.Plant).Where(x => x.Month == month && x.Year == year)
                        join monthlyBucket in _db.MonthlyBuckets.Include(x => x.BannerPlant) on new { BannerPlantId = weeklyBucket.BannerPlant.Id, SKUId = weeklyBucket.SKUId, Month = weeklyBucket.Month, Year = weeklyBucket.Year } equals new { BannerPlantId = monthlyBucket.BannerPlant.Id, SKUId = monthlyBucket.SKUId, Month = monthlyBucket.Month, Year = monthlyBucket.Year }
                        join sku in _db.SKUs.Include(x => x.ProductCategory) on weeklyBucket.SKUId equals sku.Id
                        select new BaseReportModel
                        {
                            WeeklyBucketId = weeklyBucket.Id,
                            BannerPlantId = weeklyBucket.BannerPlant.Id,
                            BannerName = weeklyBucket.BannerPlant.Banner.BannerName,
                            SKUId = sku.Id,
                            PCMap = sku.PCMap,
                            Description = sku.DescriptionMap,
                            Category = sku.ProductCategory.CategoryProduct,
                            PlantCode = weeklyBucket.BannerPlant.Plant.PlantCode,
                            PlantName = weeklyBucket.BannerPlant.Plant.PlantName,
                            Price = monthlyBucket.Price,
                            PlantContribution = weeklyBucket.PlantContribution,
                            RR = weeklyBucket.RunningRate,
                            TCT = monthlyBucket.TCT,
                            Target = monthlyBucket.MonthlyTarget,
                            MonthlyBucket = monthlyBucket.RunningRate * (monthlyBucket.TCT / 100) * (monthlyBucket.MonthlyTarget / 100),
                            Week1 = monthlyBucket.RunningRate * (monthlyBucket.TCT / 100) * (monthlyBucket.MonthlyTarget / 100) * ((decimal)50 / (decimal)100),
                            Week2 = monthlyBucket.RunningRate * (monthlyBucket.TCT / 100) * (monthlyBucket.MonthlyTarget / 100) * ((decimal)50 / (decimal)100),
                            ValidBJ = weeklyBucket.ValidBJ,
                            RemFSA = monthlyBucket.RunningRate * (monthlyBucket.TCT / 100) * (monthlyBucket.MonthlyTarget / 100) - weeklyBucket.ValidBJ,

                        }).AsEnumerable();

            return data;
        }

        private async Task<string> CreateFile(IEnumerable<BaseReportModel> baseReportData, string title, DateTime currDate, IEnumerable<DailyRecordModel> dailyRecord)
        {
            var colToAddRephase = 0;
            var colToAddReallocate = 0;
            var colToAddPropose = 0;
            var month = currDate.ToString("MMM").ToUpper();
            var year = currDate.ToString("yy");

            var listHistoryMonthly = new List<MonthlyBucketHistory>();

            var workbook = new HSSFWorkbook();
            ISheet worksheet = workbook.CreateSheet("v1.0");
            var titleRow = worksheet.CreateRow(0).CreateCell(0);

            var style = workbook.CreateCellStyle();
            style.Alignment = HorizontalAlignment.Center;
            var listCol = baseReportData.First().GetType().GetProperties().Select(x => x.Name).Where(x => x != "BannerPlantId" && x != "SKUId").ToList();

            var fsaCalDetail = await _calendarService.GetCalendarDetail(currDate);
            int week = fsaCalDetail.Week;

            var listDate = dailyRecord.Select(x => x.SubmitDate.Date).Distinct().OrderBy(x => x).ToList();

            var listDatePropose = dailyRecord.Where(x => x.Type == ProposalType.ProposeAdditional).Select(x => x.SubmitDate.Date).Distinct().OrderBy(x => x).ToList();
            var listDateReallocate = dailyRecord.Where(x => x.Type == ProposalType.ReallocateAcrossCDM || x.Type == ProposalType.ReallocateAcrossKAM || x.Type == ProposalType.ReallocateAcrossMT).Select(x => x.SubmitDate.Date).Distinct().OrderBy(x => x).ToList();

            var weeklyBucketHistories = _db.WeeklyBucketHistories.Where(x => x.Month == currDate.Month && x.Year == currDate.Year).ToList();

            var groupDailyRecord = dailyRecord.GroupBy(x => new { x.WeeklyBucketId, x.SubmitDate.Date, x.Type, x.Week }).ToList();
            var recordRephase = groupDailyRecord.Where(x => x.Key.Type == ProposalType.Rephase);
            colToAddRephase = recordRephase.Select(x => x.Key.Date).Distinct().Count();
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

            var bucketHistory = _db.MonthlyBucketHistories.SingleOrDefault(x => x.Month == currDate.Month && x.Year == currDate.Year);
            var bucketMaxVersion = bucketHistory != null ? bucketHistory.Version : 0;
            var reportWeekVersions = _db.ReportWeekVersions.Where(x => x.Month == currDate.Month && x.Year == currDate.Year).ToList();

            //var maxVersionWeeks = bucketHistories.GroupBy(x => new { x.Week }).Select(y => new { Version = y.Max(x => x.Version), Week = y.Key.Week }).ToList();

            var maxColToAdd = listColToAdd.Max();

            var indexAfterMonthlyBucket = 13;
            var indexAfterWeek1 = 14;
            var indexAfterWeek2 = 15;

            for (var i = 1; i < bucketMaxVersion + 1; i++)
            {
                listCol.Insert(indexAfterMonthlyBucket, string.Format("{0}'{1} v1.{2}", month, year, (i)));
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

            for (var i = 0; i < colToAddRephase; i++)
            {
                listCol.Insert(indexAfterMonthlyBucket, "Rephase " + (i + 1) + "(+/-)");
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


            var maxWeek = 0;
            var maxWeek2 = 0;
            for (var i = 1; i <= week; i++)
            {
                if (i < 6)
                {
                    if (i == 1)
                    {
                        maxWeek = reportWeekVersions.SingleOrDefault(x => x.Week == 1) != null ? reportWeekVersions.SingleOrDefault(x => x.Week == 1).MaxVersion : 0;

                        for (var j = 1; j < maxWeek + 1; j++)
                        {

                            if (i == 1)
                            {
                                listCol.Insert(indexAfterWeek1, "Week " + i + " v" + j);
                                indexAfterWeek1++;
                                indexAfterWeek2++;
                                listCol.Insert(indexAfterWeek2, "Week " + (i+1) + " v" + j);
                                indexAfterWeek2++;
                            }
                            else
                            {
                                
                            }
                        }
                    }
                    else if(i == 2)
                    {
                        maxWeek2 = maxWeek + (reportWeekVersions.SingleOrDefault(x => x.Week == 2) != null ? reportWeekVersions.SingleOrDefault(x => x.Week == 2).MaxVersion : 0);
                        for (var j = maxWeek; j < maxWeek2; j++)
                        {

                                listCol.Insert(indexAfterWeek2, "Week " + (i + 1) + " v" + j);
                                indexAfterWeek2++;

                        }
                    }
                }
            }

            for (var i = 2; i <= week; i++)
            {
                listCol.Insert(indexAfterWeek2, "Dispatch Wk" + (i - 1));
                indexAfterWeek2++;
                listCol.Insert(indexAfterWeek2, "Rem Wk" + (i - 1));
                indexAfterWeek2++;
            }
            //for (var i = 1; i < maxVersion + 1; i++)
            //{
            //    listCol.Insert(indexAfterWeek1, "Week " + week + " v" + i);
            //    indexAfterWeek1++;
            //    indexAfterWeek2++;
            //    listCol.Insert(indexAfterWeek2, "Week " + (week + 1) + " v" + i);
            //    indexAfterWeek2++;
            //}    



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
                    if(x == 2236)
                    {
                        var zzzz = 5;
                    }
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
                    decimal totalRephase = decimal.Zero;
                    if (recordRephase.Any())
                    {
                        var rephaseDatas = recordRephase.Where(x => x.Key.WeeklyBucketId == weeklyBucketId).ToList();
                        //var rephaseDatas = recordRephase.SingleOrDefault(x => x.Key.WeeklyBucketId == weeklyBucketId).AsEnumerable();

                        if (rephaseDatas.Any())
                        {


                            //foreach(var date in listDate)
                            for (var j = 0; j < colToAddRephase; j++)
                            {
                                var groupRephaseDate = rephaseDatas.SingleOrDefault(x => x.Key.Date == listDate.ElementAt(j)).AsEnumerable();
                                if(groupRephaseDate != null)
                                {
                                    totalRephase = groupRephaseDate.Sum(x => x.Rephase);
                                }
                                rephaseDataThisRow[j] = totalRephase;
                            }

                        }
                    }

                    if (recordPropose.Any())
                    {
                        var proposeDatas = recordPropose.SingleOrDefault(x => x.Key.WeeklyBucketId == weeklyBucketId).AsEnumerable();

                        if (proposeDatas != null)
                        {
                            var groupProposeData = proposeDatas.OrderBy(x => x.SubmitDate).GroupBy(x => x.SubmitDate.Date).AsEnumerable();
                            //proposeDatas = proposeDatas.OrderBy(x => x.SubmitDate).AsEnumerable();
                            for (var j = 0; j < colToAddPropose; j++)
                            {

                                if (j < proposeDatas.Count())
                                {
                                    proposeDataThisRow[j] = groupProposeData.Where(x => x.Key == listDatePropose.ElementAt(j)).Sum(y => y.Sum(z => z.ProposeAdditional));
                                }
                            }
                        }
                    }


                    if (recordReallocate.Any())
                    {
                        var reallocateDatas = recordReallocate.SingleOrDefault(x => x.Key.WeeklyBucketId == weeklyBucketId).AsEnumerable();

                        if (reallocateDatas != null)
                        {
                            var groupReallocateData = reallocateDatas.OrderBy(x => x.SubmitDate).GroupBy(x => x.SubmitDate.Date).AsEnumerable();
                            //reallocateDatas = reallocateDatas.OrderBy(x => x.SubmitDate).AsEnumerable();
                            for (var j = 0; j < colToAddReallocate; j++)
                            {

                                if (j < reallocateDatas.Count())
                                {
                                    var proposes = groupReallocateData.Where(x => x.Key == listDateReallocate.ElementAt(j)).AsEnumerable();

                                    foreach(var propose in proposes)
                                    {
                                        foreach(var proposeAdd in propose)
                                        {
                                            if(proposeAdd.ProposeAdditional > 0)
                                            {
                                                reallocateDataThisRowTarget[j] += proposeAdd.ProposeAdditional;
                                            }
                                            else
                                            {
                                                reallocateDataThisRowSource[j] += proposeAdd.ProposeAdditional;
                                            }
                                        }
                                    }
                                   
                                    
                                }
                            }
                        }
                    }

                    //for (var j = 1; j < maxVersion + 1; j++)
                    //{
                    //    var monthlyBucketData = bucketHistory.SingleOrDefault(x => x.WeeklyBucketId == weeklyBucketId && x.Version == j) != null ? bucketHistory.Single(x => x.WeeklyBucketId == weeklyBucketId && x.Version == j).MonthlyBucket : decimal.Zero;
                    //    row.CreateCell(i).SetCellValue((double)monthlyBucketData);
                    //    i++;
                    //}

                    var updatedMonthlyBucket = monthlyBucket;
                    for (var j = 0; j < bucketMaxVersion; j++)
                    {
                        if(listDateReallocate.Contains(listDate.ElementAt(j)))
                        {
                            for(var k = 0; k < colToAddReallocate; k++)
                            {
                                if (k >= 0 && k < reallocateDataThisRowSource.Count)
                                {
                                    updatedMonthlyBucket += reallocateDataThisRowSource[k];
                                }
                                else
                                {
                                    updatedMonthlyBucket += 0;
                                }

                                if (k >= 0 && k < reallocateDataThisRowTarget.Count)
                                {
                                    updatedMonthlyBucket += reallocateDataThisRowTarget[k];
                                }
                                else
                                {
                                    updatedMonthlyBucket += 0;
                                }
                            }
                            
                        }

                        if (listDatePropose.Contains(listDate.ElementAt(j)))
                        {
                            if (j >= 0 && j < proposeDataThisRow.Count)
                            {
                                updatedMonthlyBucket += proposeDataThisRow[j];
                            }
                            else
                            {
                                updatedMonthlyBucket += 0;
                            }
                        }

                        row.CreateCell(i).SetCellValue((double)updatedMonthlyBucket);
                        i++;
                    }

                    for (var j = 0; j < colToAddReallocate; j++)
                    {
                        row.CreateCell(i).SetCellValue((double)reallocateDataThisRowTarget.ElementAt(j));
                        i++;
                        row.CreateCell(i).SetCellValue((double)reallocateDataThisRowSource.ElementAt(j));
                        i++;
                    }



                    //Rephase
                    for (var j = 0; j < colToAddRephase; j++)
                    {
                        row.CreateCell(i).SetCellValue((double)rephaseDataThisRow.ElementAt(j));
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

                    var week1Version = reportWeekVersions.SingleOrDefault(x => x.Week == 1) != null ? reportWeekVersions.SingleOrDefault(x => x.Week == 1).MaxVersion : 0;
                    for (var j = 0; j < week1Version; j++)
                    {
                        if (j >= 0 && j < rephaseDataThisRow.Count)
                        {
                            week1Val += (double)rephaseDataThisRow[j];
                        }

                        row.CreateCell(i).SetCellValue(week1Val);
                        i++;
                    }

      
                    var week2 = Convert.ToDouble(data.Week2);
                    var week2Val = week2;
                    row.CreateCell(i).SetCellValue(week2);
                    i++;

                    var week2Version = reportWeekVersions.SingleOrDefault(x => x.Week == 2) != null ? reportWeekVersions.SingleOrDefault(x => x.Week == 2).MaxVersion : 0;
                    week2Version += week1Version;
                    for (var j = 0; j < week2Version; j++)
                    {
                        if (j >= 0 && j < rephaseDataThisRow.Count)
                        {
                            week2Val -= (double)rephaseDataThisRow[j];

                        }

                        if (j >= 0 && j < reallocateDataThisRowSource.Count)
                        {
                            week2Val += (double)reallocateDataThisRowSource[j];

                        }

                        if (listDateReallocate.Contains(listDate.ElementAt(j)))
                        {
                            for (var k = 0; k < colToAddReallocate; k++)
                            {
                                week2Val += (double)reallocateDataThisRowTarget[k] + (double)reallocateDataThisRowSource[k];
                            }
                        }


                        row.CreateCell(i).SetCellValue(week2Val);
                        i++;
                    }

                    for (var j = 2; j <= week; j++)
                    {
                        //var dispatch = weeklyBucketHistories.Single(x => x.BannerPlantId == data.BannerPlantId && x.SKUId == data.SKUId).DispatchConsume;
                        var dispatch = decimal.Zero;
                        row.CreateCell(i).SetCellValue((double)dispatch);
                        i++;
                        var remWk = decimal.Zero;
                        row.CreateCell(i).SetCellValue((double)remWk);
                    }

                    var validBJ = data.ValidBJ;
                    row.CreateCell(i).SetCellValue((double)validBJ);
                    i++;

                    var remFSA = updatedMonthlyBucket - validBJ;
                    row.CreateCell(i).SetCellValue((double)remFSA);
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

                var day = currDate.Day;
                if (day == 1 && bucketMaxVersion == 0)
                {
                    day = 0;
                }

                var folder = Path.Combine(Directory.GetCurrentDirectory(), "ReportExcel");

                if(!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }

                var outputPath = Path.Combine(folder, "FSA_" + day + "_" + currDate.Month + "_" + currDate.Year + ".xls");

                

                using (FileStream outputStream = new FileStream(outputPath, FileMode.Create))
                {
                    ms.Position = 0;
                    ms.CopyTo(outputStream);
                }


                var reportWeekVersion = reportWeekVersions.SingleOrDefault(x => x.Week == week);
                if(reportWeekVersion != null)
                {
                    reportWeekVersion.MaxVersion += 1;
                }
                else
                {
                    var newReportWeekVersion = new ReportWeekVersion();
                    newReportWeekVersion.Id = Guid.NewGuid();
                    newReportWeekVersion.Month = currDate.Month;
                    newReportWeekVersion.Week = week;
                    newReportWeekVersion.Year = currDate.Year;
                    newReportWeekVersion.MaxVersion = 1;
                    _db.ReportWeekVersions.Add(newReportWeekVersion);
                }

                
                if(bucketHistory != null)
                {
                    bucketHistory.Version += 1;
                }
                else
                {
                    var bucketHist = new MonthlyBucketHistory();
                    bucketHist.Id = Guid.NewGuid();
                    bucketHist.Version = 1;
                    bucketHist.Month = currDate.Month;
                    bucketHist.Year = currDate.Year;

                    _db.MonthlyBucketHistories.Add(bucketHist);
                }


                _db.SaveChanges();
                var msg = "File Generated";
                return msg;
                //FileStreamResult file = File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Daily Report.xls");
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }


        private void GenerateHistoryReport(IEnumerable<object> datas)
        {

        }

    }
}
