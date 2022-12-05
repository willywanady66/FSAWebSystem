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

                var dt = await CreateFile(baseData, "Publish Day " + currDate.Day, currDate, dailyRecord);

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
                            MonthlyBucket = decimal.Round(monthlyBucket.RunningRate * (monthlyBucket.TCT / 100) * (monthlyBucket.MonthlyTarget / 100)),
                            Week1 = decimal.Round(monthlyBucket.RunningRate * (monthlyBucket.TCT / 100) * (monthlyBucket.MonthlyTarget / 100) * ((decimal)50 / (decimal)100)),
                            Week2 = decimal.Round(monthlyBucket.RunningRate * (monthlyBucket.TCT / 100) * (monthlyBucket.MonthlyTarget / 100) * ((decimal)50 / (decimal)100)),
                            ValidBJ = weeklyBucket.ValidBJ,
                            RemFSA = decimal.Round(monthlyBucket.RunningRate * (monthlyBucket.TCT / 100) * (monthlyBucket.MonthlyTarget / 100) - weeklyBucket.ValidBJ),

                        }).AsEnumerable();

            return data;
        }

        private async Task<string> CreateFile(IEnumerable<BaseReportModel> baseReportData, string title, DateTime currDate, IEnumerable<DailyRecordModel> dailyRecord)
        {
            int x = 0;
            try
            {
                var colToAddRephase = 0;
                var colToAddReallocate = 0;
                var colToAddPropose = 0;
                var month = currDate.ToString("MMM").ToUpper();
                var year = currDate.ToString("yy");

        

                var workbook = new HSSFWorkbook();
                ISheet worksheet = workbook.CreateSheet("v1.0");
                var titleRow = worksheet.CreateRow(0).CreateCell(0);

                var style = workbook.CreateCellStyle();
                style.Alignment = HorizontalAlignment.Center;
                var listCol = baseReportData.First().GetType().GetProperties().Select(x => x.Name).Where(x => x != "BannerPlantId" && x != "SKUId" && x != "WeeklyBucketId").ToList();

                var fsaCalDetail = await _calendarService.GetCalendarDetail(currDate);
                int week = fsaCalDetail.Week;

                var listDate = dailyRecord.Select(x => new { x.SubmitDate.Date, x.Week }).Distinct().OrderBy(x => x.Date).ToList();

                var t4st = dailyRecord.Where(x => x.Type == ProposalType.Rephase).Select(x => new { x.SubmitDate.Date, x.Week }).Distinct().ToList();
                var listDateRephase = dailyRecord.Where(x => x.Type == ProposalType.Rephase).Select(x => new { x.SubmitDate.Date, x.Week }).Distinct().OrderBy(x => x.Date).ToList();
                var listDatePropose = dailyRecord.Where(x => x.Type == ProposalType.ProposeAdditional).Select(x => new { x.SubmitDate.Date, x.Week }).Distinct().OrderBy(x => x.Date).ToList();
                var listDateReallocate = dailyRecord.Where(x => x.Type == ProposalType.ReallocateAcrossCDM || x.Type == ProposalType.ReallocateAcrossKAM || x.Type == ProposalType.ReallocateAcrossMT).Select(x => new { x.SubmitDate.Date, x.Week }).Distinct().OrderBy(x => x.Date).ToList();

                var weeklyBucketHistories = _db.WeeklyBucketHistories.Where(x => x.Month == currDate.Month && x.Year == currDate.Year).ToList();

                var groupDailyRecord = dailyRecord.GroupBy(x => new { x.WeeklyBucketId, x.SubmitDate.Date, x.Type, x.Week }).ToList();
                var recordRephase = groupDailyRecord.Where(x => x.Key.Type == ProposalType.Rephase);
                colToAddRephase = listDateRephase.Count;
                var recordReallocate = groupDailyRecord.Where(x => x.Key.Type == ProposalType.ReallocateAcrossCDM || x.Key.Type == ProposalType.ReallocateAcrossKAM || x.Key.Type == ProposalType.ReallocateAcrossMT);
                colToAddReallocate = listDateReallocate.Count;
                var recordPropose = groupDailyRecord.Where(x => x.Key.Type == ProposalType.ProposeAdditional);
                colToAddPropose = listDatePropose.Count;

                var listColToAdd = new List<int>
            {
                colToAddReallocate,
                colToAddPropose,
                colToAddRephase
            };

               
                //var bucketMaxVersion = bucketHistory != null ? bucketHistory.Version : 0;
                var bucketMaxVersion = listDate.Count;

                //var maxVersionWeeks = bucketHistories.GroupBy(x => new { x.Week }).Select(y => new { Version = y.Max(x => x.Version), Week = y.Key.Week }).ToList();

                var maxColToAdd = listColToAdd.Max();

                var indexAfterMonthlyBucket = 12;
                var indexAfterWeek1 = 13;
                var indexAfterWeek2 = 14;

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


                var maxWeek = listDate.Where(x => x.Week == 1).Count();
                var maxWeek2 = listDate.Where(x => x.Week == 2).Count();
                var maxWeek3 = listDate.Where(x => x.Week == 3).Count();
                var maxWeek4 = listDate.Where(x => x.Week == 4).Count();
                var maxWeek5 = listDate.Where(x => x.Week == 5).Count();
                var indexAfterWeek3 = 0;
                var indexAfterWeek4 = 0;
                var indexAfterWeek5 = 0;
                var indexAfterWeek = 0;
                for (var i = 1; i <= week; i++)
                {
                    if (i < 6)
                    {
                        if (i == 1)
                        {
                            for (var j = 1; j < maxWeek + 1; j++)
                            {
                                if (i == 1)
                                {
                                    listCol.Insert(indexAfterWeek1, "Week " + i + " v" + j);
                                    indexAfterWeek1++;
                                    indexAfterWeek2++;
                                    listCol.Insert(indexAfterWeek2, "Week " + (i + 1) + " v" + j);
                                    indexAfterWeek2++;
                                }
                            }
                        }
                        else if (i == 2)
                        {
                            for (var j = 0; j < maxWeek2; j++)
                            {
                                listCol.Insert(indexAfterWeek2, "Week " + (i) + " v" + (j + maxWeek + 1));
                                indexAfterWeek2++;
                            }

                            if (listDateRephase.Any(x => x.Week == 2))
                            {
                                indexAfterWeek3 = indexAfterWeek2;
                                
                                for (var j = 0; j < maxWeek2; j++)
                                {
                                    if (j == 0)
                                    {
                                        listCol.Insert(indexAfterWeek3, "Week " + (i + 1));
                                        indexAfterWeek3++;
                                    }
                                    listCol.Insert(indexAfterWeek3, "Week " + (i + 1) + " v" + (j + 1));
                                    indexAfterWeek3++;
                                }
                            }
                            indexAfterWeek = indexAfterWeek3;
                        }
                        else if (i == 3)
                        {

                            for (var j = 0; j < maxWeek3; j++)
                            {

                                listCol.Insert(indexAfterWeek3, "Week " + (i) + " v" + (j + maxWeek2 + 1));
                                indexAfterWeek3++;
                            }


                            if (listDateRephase.Any(x => x.Week == 3))
                            {
                                indexAfterWeek4 = indexAfterWeek3;
                                for (var j = 0; j < maxWeek3; j++)
                                {
                                    if (j == 0)
                                    {
                                        listCol.Insert(indexAfterWeek4, "Week " + (i + 1));
                                        indexAfterWeek4++;
                                    }
                                    listCol.Insert(indexAfterWeek4, "Week " + (i + 1) + " v" + (j + 1));
                                    indexAfterWeek4++;
                                }
                            }
                            indexAfterWeek = indexAfterWeek4;
                        }
                        else if (i == 4)
                        {
                            for (var j = 0; j < maxWeek4; j++)
                            {

                                listCol.Insert(indexAfterWeek4, "Week " + (i) + " v" + (j + maxWeek3 + 1));
                                indexAfterWeek4++;
                            }

                            indexAfterWeek5 = indexAfterWeek4;
                            if (listDateRephase.Any(x => x.Week == 4))
                            {
                                
                                for (var j = 0; j < maxWeek4; j++)
                                {
                                    if (j == 0)
                                    {
                                        listCol.Insert(indexAfterWeek5, "Week " + (i + 1));
                                        indexAfterWeek5++;
                                    }
                                    listCol.Insert(indexAfterWeek5, "Week " + (i + 1) + " v" + (j + 1));
                                    indexAfterWeek5++;
                                }
                            }
                            indexAfterWeek = indexAfterWeek4;
                        }
                    }
                }


                if (listDateRephase.Any(x => x.Week == 2) || week == 3)
                {
                    listCol.Insert(indexAfterWeek, "Dispatch Wk1");
                    indexAfterWeek++;
                    listCol.Insert(indexAfterWeek, "Rem Wk1");
                    indexAfterWeek++;
                }



                if (listDateRephase.Any(x => x.Week == 3) || week == 4)
                {
                    listCol.Insert(indexAfterWeek, "Dispatch Wk2");
                    indexAfterWeek++;
                    listCol.Insert(indexAfterWeek, "Rem Wk2");
                    indexAfterWeek++;
                }


                if (listDateRephase.Any(x => x.Week == 4) || week == 5)
                {
                    listCol.Insert(indexAfterWeek, "Dispatch Wk3");
                    indexAfterWeek++;
                    listCol.Insert(indexAfterWeek, "Rem Wk3");
                    indexAfterWeek++;
                }


                //for (var i = 2; i <= week; i++)
                //{
                //    if (listDateRephase.Any(x => x.Week == i) || i > 2)
                //    {
                //        listCol.Insert(indexAfterWeek5, "Dispatch Wk" + (i - 1));
                //        indexAfterWeek5++;
                //        listCol.Insert(indexAfterWeek5, "Rem Wk" + (i - 1));
                //        indexAfterWeek5++;
                //    }

                //    //else if(listDateRephase.Any(x => x.Week == 3) || i == 4) 
                //    //{

                //    //        listCol.Insert(indexAfterWeek4, "Dispatch Wk" + (i - 1));
                //    //        indexAfterWeek4++;
                //    //        listCol.Insert(indexAfterWeek4, "Rem Wk" + (i - 1));
                //    //        indexAfterWeek4++;

                //    //}


                //}



                CellRangeAddress range = new CellRangeAddress(0, 0, 0, listCol.Count - 1);
                worksheet.AddMergedRegion(range);

                titleRow.CellStyle = style;
                titleRow.SetCellValue(title);




                var percentStyle = workbook.CreateCellStyle();
                percentStyle.DataFormat = workbook.CreateDataFormat().GetFormat("0.00%");

                var accountingStyle = workbook.CreateCellStyle();
                accountingStyle.DataFormat = workbook.CreateDataFormat().GetFormat("_(* #,##0_);_(* (#,##0);_(* \"-\"_);_(@_)");


                foreach (var data in baseReportData.OrderBy(x => x.BannerName.ToString()))
                {
                    if (x == 2330)
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
                    row.GetCell(i).CellStyle = accountingStyle;
                    i++;


                    var rephaseDataThisRow = Enumerable.Repeat(new ReportRephaseData(), colToAddRephase).ToList();
                    var reallocateDataThisRowTarget = Enumerable.Repeat(new ReportProposeData(), colToAddReallocate).ToList();
                    var reallocateDataThisRowSource = Enumerable.Repeat(new ReportProposeData(), colToAddReallocate).ToList();
                    var proposeDataThisRow = Enumerable.Repeat(new ReportProposeData(), colToAddPropose).ToList();



                    var rephaseDatas = recordRephase.Where(x => x.Key.WeeklyBucketId == weeklyBucketId).ToList();
                    //var rephaseDatas = recordRephase.SingleOrDefault(x => x.Key.WeeklyBucketId == weeklyBucketId).AsEnumerable();

                    if (rephaseDatas.Any())
                    {
                        //foreach(var date in listDate)
                        for (var j = 0; j < colToAddRephase; j++)
                        {
                            decimal totalRephase = decimal.Zero;
                            var date = listDateRephase.ElementAt(j);
                            var groupRephaseDate = rephaseDatas.SingleOrDefault(x => x.Key.Date == date.Date).AsEnumerable();
                            if (groupRephaseDate != null)
                            {
                                totalRephase = groupRephaseDate.Sum(x => x.Rephase);
                            }
                            rephaseDataThisRow[j] = new ReportRephaseData { Rephase = totalRephase, Week = date.Week };

                        }

                    }


                    var proposeDatas = recordPropose.Where(x => x.Key.WeeklyBucketId == weeklyBucketId).ToList();
                    if (proposeDatas.Any())
                    {
                        for (var j = 0; j < colToAddPropose; j++)
                        {
                            decimal totalPropose = decimal.Zero;
                            var date = listDatePropose.ElementAt(j);
                            var groupProposeData = proposeDatas.SingleOrDefault(x => x.Key.Date == date.Date).AsEnumerable();
                            if (groupProposeData != null)
                            {
                                totalPropose = groupProposeData.Sum(x => x.ProposeAdditional);

                            }
                            proposeDataThisRow[j] = new ReportProposeData { Propose = totalPropose, Week = date.Week };
                        }
                    }




                    var reallocateDatas = recordReallocate.Where(x => x.Key.WeeklyBucketId == weeklyBucketId).ToList();

                    if (reallocateDatas.Any())
                    {
                        for (var j = 0; j < colToAddReallocate; j++)
                        {
                            decimal totalReallocSource = decimal.Zero;
                            decimal totalReallocTarget = decimal.Zero;
                            var date = listDateReallocate.ElementAt(j);
                            var groupReallocateData = reallocateDatas.SingleOrDefault(x => x.Key.Date == date.Date).AsEnumerable();

                            if (groupReallocateData != null)
                            {
                                totalReallocSource += groupReallocateData.Where(x => x.ProposeAdditional < 0).Sum(x => x.ProposeAdditional);
                                totalReallocTarget += groupReallocateData.Where(x => x.ProposeAdditional > 0).Sum(x => x.ProposeAdditional);
                            }
                            reallocateDataThisRowSource[j] = new ReportProposeData { Propose = totalReallocSource, Week = date.Week };
                            reallocateDataThisRowTarget[j] = new ReportProposeData { Propose = totalReallocTarget, Week = date.Week }; ;


                        }
                    }




                    //for (var j = 1; j < maxVersion + 1; j++)
                    //{
                    //    var monthlyBucketData = bucketHistory.SingleOrDefault(x => x.WeeklyBucketId == weeklyBucketId && x.Version == j) != null ? bucketHistory.Single(x => x.WeeklyBucketId == weeklyBucketId && x.Version == j).MonthlyBucket : decimal.Zero;
                    //    row.CreateCell(i).SetCellValue((double)monthlyBucketData);
                    //    i++;
                    //}

                    var updatedMonthlyBucket = monthlyBucket;
                    for (var j = 0; j < listDate.Count; j++)
                    {
                        var date = listDate.ElementAt(j);
                        var indexReallocate = listDateReallocate.IndexOf(date);

                        if(indexReallocate >= 0)
                        {
                            updatedMonthlyBucket += reallocateDataThisRowSource[indexReallocate].Propose;
                            updatedMonthlyBucket += reallocateDataThisRowTarget[indexReallocate].Propose;
                        }
            

                        //if (indexReallocate > 0)
                        //{
                        //    if (indexReallocate < reallocateDataThisRowSource.Count)
                        //    {
                        //        updatedMonthlyBucket += reallocateDataThisRowSource[j].Propose;
                        //    }
                        //    else
                        //    {
                        //        updatedMonthlyBucket += 0;
                        //    }

                        //    if (j >= 0 && j < reallocateDataThisRowTarget.Count)
                        //    {
                        //        updatedMonthlyBucket += reallocateDataThisRowTarget[j].Propose;
                        //    }
                        //    else
                        //    {
                        //        updatedMonthlyBucket += 0;
                        //    }
                        //}

                        var indexPropose = listDatePropose.IndexOf(date);

                        if(indexPropose >= 0)
                        {
                            updatedMonthlyBucket += proposeDataThisRow[indexPropose].Propose;
                        }    
             
                        //if (listDatePropose.Contains(date))
                        //{
                        //    if (j >= 0 && j < proposeDataThisRow.Count)
                        //    {
                        //        updatedMonthlyBucket += proposeDataThisRow[j].Propose;
                        //    }
                        //    else
                        //    {
                        //        updatedMonthlyBucket += 0;
                        //    }
                        //}

                        row.CreateCell(i).SetCellValue((double)updatedMonthlyBucket);
                        row.GetCell(i).CellStyle = accountingStyle;
                        i++;
                    }

                    for (var j = 0; j < colToAddReallocate; j++)
                    {
                        row.CreateCell(i).SetCellValue((double)reallocateDataThisRowTarget.ElementAt(j).Propose);
                        row.GetCell(i).CellStyle = accountingStyle;
                        i++;
                        row.CreateCell(i).SetCellValue((double)reallocateDataThisRowSource.ElementAt(j).Propose);
                        row.GetCell(i).CellStyle = accountingStyle;
                        i++;
                    }



                    //Rephase
                    for (var j = 0; j < colToAddRephase; j++)
                    {
                        row.CreateCell(i).SetCellValue((double)rephaseDataThisRow.ElementAt(j).Rephase);
                        row.GetCell(i).CellStyle = accountingStyle;
                        i++;
                    }


                    //Propose Additional
                    for (var j = 0; j < colToAddPropose; j++)
                    {
                        row.CreateCell(i).SetCellValue((double)proposeDataThisRow.ElementAt(j).Propose);
                        row.GetCell(i).CellStyle = accountingStyle;
                        i++;
                    }



                    //Week 1
                    var week1 = data.Week1;
                    var week1Val = week1;
                    row.CreateCell(i).SetCellValue((double)week1);
                    row.GetCell(i).CellStyle = accountingStyle;
                    i++;

                    //Week 1 v
                    for (var j = 0; j < maxWeek; j++)
                    {
                        if (j >= 0 && j < rephaseDataThisRow.Count)
                        {
                            week1Val += rephaseDataThisRow[j].Rephase;
                        }

                        row.CreateCell(i).SetCellValue((double)week1Val);
                        row.GetCell(i).CellStyle = accountingStyle;
                        i++;
                    }

                    //Week2
                    var week2 = data.Week2;
                    var week2Val = week2;
                    row.CreateCell(i).SetCellValue((double)week2);
                    row.GetCell(i).CellStyle = accountingStyle;
                    i++;

                    //Week 2 v
                    for (var j = 0; j < maxWeek; j++)
                    {
                        if (j >= 0 && j < rephaseDataThisRow.Where(x => x.Week == 1).Count())
                        {
                            var thisRephase = rephaseDataThisRow[j];

                            week2Val -= rephaseDataThisRow[j].Rephase;
                        }

                        row.CreateCell(i).SetCellValue((double)week2Val);
                        row.GetCell(i).CellStyle = accountingStyle;
                        i++;
                    }

                    for (var j = 0; j < maxWeek2; j++)
                    {
                        var rephaseThisWeek = rephaseDataThisRow.Where(x => x.Week == 2).ToList();
                        if (j >= 0 && j < rephaseThisWeek.Count)
                        {
                            var thisRephase = rephaseThisWeek[j];

                            week2Val += thisRephase.Rephase;
                        }


                        if (listDatePropose.Contains(listDate.Where(x => x.Week == 2).ElementAt(j)))
                        {
                            if (j >= 0 && j < proposeDataThisRow.Count)
                            {
                                week2Val += proposeDataThisRow[j].Propose;
                            }
                        }

                        if (listDateReallocate.Contains(listDate.Where(x => x.Week == 2).ElementAt(j)))
                        {
                            if (j >= 0 && j < reallocateDataThisRowTarget.Count)
                            {
                                week2Val += reallocateDataThisRowTarget[j].Propose;
                            }
                            if (j >= 0 && j < reallocateDataThisRowSource.Count)
                            {
                                week2Val += reallocateDataThisRowSource[j].Propose;
                            }
                        }

                
                        row.CreateCell(i).SetCellValue((double)week2Val);
                        row.GetCell(i).CellStyle = accountingStyle;
                        i++;
                    }


                    var dispatchWk1 = decimal.Zero;
                    var remWk1 = decimal.Zero;
                    var week3Val = decimal.Zero;
                    for (var j = 0; j < maxWeek2; j++)
                    {
                        var weeklyBucketWk2 = weeklyBucketHistories.FirstOrDefault(x => x.BannerPlantId == data.BannerPlantId && x.SKUId == data.SKUId && x.Week == 2);
                        dispatchWk1 = weeklyBucketWk2 != null ? weeklyBucketWk2.DispatchConsume : decimal.Zero;
                        remWk1 = week1Val - dispatchWk1;

                        if (listDateRephase.Any(x => x.Week == 2) || week == 3)
                        {
                            if (j == 0)
                            {

                                //Week 3
                                week3Val = remWk1;
                                row.CreateCell(i).SetCellValue((double)week3Val);
                                row.GetCell(i).CellStyle = accountingStyle;
                                i++;
                            }

                            var rephaseThisWeek = rephaseDataThisRow.Where(x => x.Week == 2).ToList();

                            if (j >= 0 && j < rephaseThisWeek.Count)
                            {
                                week3Val -= rephaseThisWeek[j].Rephase;
                            }

                            row.CreateCell(i).SetCellValue((double)week3Val);
                            row.GetCell(i).CellStyle = accountingStyle;
                            i++;
                        }
                    }

                    var dispatchWk2 = decimal.Zero;
                    var remWk2 = decimal.Zero;
                    var week4Val = decimal.Zero;

                    for (var j = 0; j < maxWeek3; j++)
                    {
                        var rephaseThisWeek = rephaseDataThisRow.Where(x => x.Week == 3).ToList();
                        if (j >= 0 && j < rephaseThisWeek.Count)
                        {
                            var thisRephase = rephaseThisWeek[j];

                            week3Val += thisRephase.Rephase;
                        }


                        if (listDatePropose.Contains(listDate.Where(x => x.Week == 3).ElementAt(j)))
                        {
                            if (j >= 0 && j < proposeDataThisRow.Count)
                            {
                                week3Val += proposeDataThisRow[j].Propose;
                            }
                        }



                        if (listDateReallocate.Contains(listDate.Where(x => x.Week == 3).ElementAt(j)))
                        {
                            if (j >= 0 && j < reallocateDataThisRowTarget.Count)
                            {
                                week3Val += reallocateDataThisRowTarget[j].Propose;
                            }
                            if (j >= 0 && j < reallocateDataThisRowSource.Count)
                            {
                                week3Val += reallocateDataThisRowSource[j].Propose;
                            }
                        }
                        row.CreateCell(i).SetCellValue((double)week3Val);
                        row.GetCell(i).CellStyle = accountingStyle;
                        i++;

                    }

                    for (var j = 0; j < maxWeek3; j++)
                    {
                        var weeklyBucketWk3 = weeklyBucketHistories.FirstOrDefault(x => x.BannerPlantId == data.BannerPlantId && x.SKUId == data.SKUId && x.Week == 2);
                        dispatchWk2 = weeklyBucketWk3 != null ? weeklyBucketWk3.DispatchConsume : decimal.Zero;
                        remWk2 = week2Val - dispatchWk2;

                        if (listDateRephase.Any(x => x.Week == 3) || week == 4)
                        {
                            if (j == 0)
                            {

                                //Week 4
                                week4Val = remWk2;
                                row.CreateCell(i).SetCellValue((double)week4Val);
                                row.GetCell(i).CellStyle = accountingStyle;
                                i++;
                            }

                            var rephaseThisWeek = rephaseDataThisRow.Where(x => x.Week == 3).ToList();

                            if (j >= 0 && j < rephaseThisWeek.Count)
                            {
                                week4Val -= rephaseThisWeek[j].Rephase;
                            }

                            row.CreateCell(i).SetCellValue((double)week4Val);
                            row.GetCell(i).CellStyle = accountingStyle;
                            i++;
                        }
                    }


                    var dispatchWk3 = decimal.Zero;
                    var remWk3 = decimal.Zero;
                    var week5Val = decimal.Zero;

                    for (var j = 0; j < maxWeek4; j++)
                    {
                        var rephaseThisWeek = rephaseDataThisRow.Where(x => x.Week == 4).ToList();
                        if (j >= 0 && j < rephaseThisWeek.Count)
                        {
                            var thisRephase = rephaseThisWeek[j];

                            week4Val += thisRephase.Rephase;
                        }


                        if (listDatePropose.Contains(listDate.Where(x => x.Week == 4).ElementAt(j)))
                        {
                            if (j >= 0 && j < proposeDataThisRow.Count)
                            {
                                week4Val += proposeDataThisRow[j].Propose;
                            }
                        }



                        if (listDateReallocate.Contains(listDate.Where(x => x.Week == 4).ElementAt(j)))
                        {
                            if (j >= 0 && j < reallocateDataThisRowTarget.Count)
                            {
                                week4Val += reallocateDataThisRowTarget[j].Propose;
                            }
                            if (j >= 0 && j < reallocateDataThisRowSource.Count)
                            {
                                week4Val += reallocateDataThisRowSource[j].Propose;
                            }
                        }
                        row.CreateCell(i).SetCellValue((double)week4Val);
                        row.GetCell(i).CellStyle = accountingStyle;
                        i++;

                    }

                    for (var j = 0; j < maxWeek4; j++)
                    {
                        var weeklyBucketWk4 = weeklyBucketHistories.FirstOrDefault(x => x.BannerPlantId == data.BannerPlantId && x.SKUId == data.SKUId && x.Week == 2);
                        dispatchWk3 = weeklyBucketWk4 != null ? weeklyBucketWk4.DispatchConsume : decimal.Zero;
                        remWk3 = week3Val - dispatchWk3;

                        if (listDateRephase.Any(x => x.Week == 4) || week == 5)
                        {
                            if (j == 0)
                            {

                                //Week 4
                                week5Val = remWk3; 
                                row.CreateCell(i).SetCellValue((double)week5Val);
                                row.GetCell(i).CellStyle = accountingStyle;
                                i++;
                            }

                            var rephaseThisWeek = rephaseDataThisRow.Where(x => x.Week == 4).ToList();

                            if (j >= 0 && j < rephaseThisWeek.Count)
                            {
                                week5Val -= rephaseThisWeek[j].Rephase;
                            }

                            row.CreateCell(i).SetCellValue((double)week5Val);
                            row.GetCell(i).CellStyle = accountingStyle;
                            i++;
                        }
                    }

                    var dispatch = decimal.Zero;
                    var remWk = decimal.Zero;

                    if (listDateRephase.Any(x => x.Week == 2) || week == 3)
                    {
                        dispatch = dispatchWk1;
                        remWk = remWk1;

                        row.CreateCell(i).SetCellValue((double)dispatch);
                        row.GetCell(i).CellStyle = accountingStyle;
                        i++;

                        row.CreateCell(i).SetCellValue((double)remWk);
                        row.GetCell(i).CellStyle = accountingStyle;
                        i++;
                    }

                    dispatch = decimal.Zero;
                    if (listDateRephase.Any(x => x.Week == 3) || week == 4)
                    {
                        dispatch = dispatchWk2;
                        remWk = remWk2;

                        row.CreateCell(i).SetCellValue((double)dispatch);
                        row.GetCell(i).CellStyle = accountingStyle;
                        i++;

                        row.CreateCell(i).SetCellValue((double)remWk);
                        row.GetCell(i).CellStyle = accountingStyle;
                        i++;
                    }

                    dispatch = decimal.Zero;
                    if (listDateRephase.Any(x => x.Week == 4) || week == 5)
                    {
                        dispatch = dispatchWk3;
                        remWk = remWk3;

                        row.CreateCell(i).SetCellValue((double)dispatch);
                        row.GetCell(i).CellStyle = accountingStyle;
                        i++;

                        row.CreateCell(i).SetCellValue((double)remWk);
                        row.GetCell(i).CellStyle = accountingStyle;
                        i++;
                    }

                    var validBJ = data.ValidBJ;
                    row.CreateCell(i).SetCellValue((double)validBJ);
                    row.GetCell(i).CellStyle = accountingStyle;
                    i++;

                    var remFSA = updatedMonthlyBucket - validBJ;
                    row.CreateCell(i).SetCellValue((double)remFSA);
                    row.GetCell(i).CellStyle = accountingStyle;
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
                    colRow.CreateCell(i).SetCellValue(listCol[i]);
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

                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }

                var outputPath = Path.Combine(folder, "FSA_" + day + "_" + currDate.Month + "_" + currDate.Year + ".xls");



                using (FileStream outputStream = new FileStream(outputPath, FileMode.Create))
                {
                    ms.Position = 0;
                    ms.CopyTo(outputStream);
                }


                //var reportWeekVersion = reportWeekVersions.SingleOrDefault(x => x.Week == week);
                //if (reportWeekVersion != null)
                //{
                //    reportWeekVersion.MaxVersion += 1;
                //}
                //else
                //{
                //    var newReportWeekVersion = new ReportWeekVersion();
                //    newReportWeekVersion.Id = Guid.NewGuid();
                //    newReportWeekVersion.Month = currDate.Month;
                //    newReportWeekVersion.Week = week;
                //    newReportWeekVersion.Year = currDate.Year;
                //    newReportWeekVersion.MaxVersion = 1;
                //    _db.ReportWeekVersions.Add(newReportWeekVersion);
                //}


                //if (bucketHistory != null)
                //{
                //    bucketHistory.Version += 1;
                //}
                //else
                //{
                //    var bucketHist = new MonthlyBucketHistory();
                //    bucketHist.Id = Guid.NewGuid();
                //    bucketHist.Version = 1;
                //    bucketHist.Month = currDate.Month;
                //    bucketHist.Year = currDate.Year;

                //    _db.MonthlyBucketHistories.Add(bucketHist);
                //}


                //_db.SaveChanges();
                var msg = "File Generated";
                return msg;
                //FileStreamResult file = File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Daily Report.xls");
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
