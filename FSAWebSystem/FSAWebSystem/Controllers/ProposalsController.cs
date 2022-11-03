using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FSAWebSystem.Models;
using FSAWebSystem.Models.Context;
using FSAWebSystem.Services.Interface;
using FSAWebSystem.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using FSAWebSystem.Areas.Identity.Data;
using static FSAWebSystem.Models.ViewModels.ProposalViewModel;
using Microsoft.AspNetCore.Authorization;
using AspNetCoreHero.ToastNotification.Abstractions;
using FSAWebSystem.Models.Bucket;
using FSAWebSystem.Services;
using System.Text.Encodings.Web;
using System.Globalization;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;

namespace FSAWebSystem.Controllers
{
    public class ProposalsController : Controller
    {
        private readonly FSAWebSystemDbContext _db;
        private readonly IProposalService _proposalService;
        private readonly IBucketService _bucketService;
        private readonly ICalendarService _calendarService;
        private readonly IUserService _userService;
        private readonly INotyfService _notyfService;
        private readonly IApprovalService _approvalService;
        private readonly IBannerPlantService _bannerPlantService;
        private readonly IBannerService _bannerService;
        private readonly ISKUService _skuService;
        private readonly IEmailService _emailService;
        private readonly UserManager<FSAWebSystemUser> _userManager;

        public ProposalsController(FSAWebSystemDbContext db, IProposalService proposalService, IBucketService bucketService, ICalendarService calendarService, UserManager<FSAWebSystemUser> userManager, IUserService userService, INotyfService notyfService,
            IBannerPlantService bannerPlantService, IApprovalService approvalService, ISKUService skuService, IEmailService emailService, IBannerService bannerService)
        {
            _db = db;
            _proposalService = proposalService;
            _bucketService = bucketService;
            _calendarService = calendarService;
            _userManager = userManager;
            _userService = userService;
            _notyfService = notyfService;
            _approvalService = approvalService;
            _bannerPlantService = bannerPlantService;
            _skuService = skuService;
            _emailService = emailService;
            _bannerService = bannerService;
        }

        // GET: Proposals


        [Authorize(Policy = ("ProposalPage"))]
        public async Task<IActionResult> Index(string message)
        {
            ViewData["ListMonth"] = _calendarService.GetListMonth();
            ViewData["ListYear"] = _calendarService.GetListYear();
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> GetProposalPagination(DataTableParamProposal param)
        {
            var user = await _userManager.GetUserAsync(User);
            var userUnilever = await _userService.GetUser((Guid)user.UserUnileverId);
            List<Proposal> listProposal = new List<Proposal>();
            var listData = Json(new { });
            var data = new ProposalData();
            var currentDate = DateTime.Now;
            var week = 0;
            try
            {
                var fsaDetail = await _calendarService.GetCalendarDetail(currentDate.Date);

                if (fsaDetail != null)
                {
                    week = fsaDetail.Week;
                    if (Convert.ToInt32(param.month) != currentDate.Month || Convert.ToInt32(param.year) != currentDate.Year)
                    {
                        week = 1;
                    }
                    data = await _proposalService.GetProposalForView(Convert.ToInt32(param.month), Convert.ToInt32(param.year), week, param, userUnilever);

                }
                listData = Json(new
                {
                    draw = param.draw,
                    recordsTotal = data.totalRecord,
                    recordsFiltered = data.totalRecord,
                    data = data.proposals,
                    week = week
                });
            }
            catch (Exception ex)
            {
                listData = Json(new
                {
                    error = ex.Message
                });
            }
            return listData;

        }

        [HttpPost]
        public async Task<IActionResult> GetProposalHistoryPagination(DataTableParam param)
        {
            var user = await _userManager.GetUserAsync(User);
            var userUnilever = await _userService.GetUser((Guid)user.UserUnileverId);
            var listData = Json(new { });
            try
            {
                var listProposalHistory = _proposalService.GetProposalHistoryPagination(param, userUnilever, Convert.ToInt32(param.month), Convert.ToInt32(param.year));
                listData = Json(new
                {
                    draw = param.draw,
                    recordsTotal = listProposalHistory.totalRecord,
                    recordsFiltered = listProposalHistory.totalRecord,
                    data = listProposalHistory.proposalsHistory
                });

                return listData;
            }
            catch (Exception ex)
            {
                listData = Json(new
                {
                    error = ex.Message
                });
            }
            return listData;
        }

        [HttpPost]
        public async Task<IActionResult> GetMonthlyBucketHistoryPagination(DataTableParam param)
        {
            var user = await _userManager.GetUserAsync(User);
            var userUnilever = await _userService.GetUser((Guid)user.UserUnileverId);
            var listData = Json(new { });
            try
            {
                var listMonthlyBucketHistory = await _bucketService.GetMonthlyBucketHistoryPagination(param, userUnilever);
                listData = Json(new
                {
                    draw = param.draw,
                    recordsTotal = listMonthlyBucketHistory.totalRecord,
                    recordsFiltered = listMonthlyBucketHistory.totalRecord,
                    data = listMonthlyBucketHistory.monthlyBuckets
                });
            }
            catch (Exception ex)
            {
                listData = Json(new
                {
                    error = ex.Message
                });
            }
            return listData;
        }

        [HttpPost]
        public async Task<IActionResult> GetWeeklyBucketHistoryPagination(DataTableParam param)
        {
            var user = await _userManager.GetUserAsync(User);
            var userUnilever = await _userService.GetUser((Guid)user.UserUnileverId);
            var listData = Json(new { });
            try
            {
                var listWeeklyBucketHistory = await _bucketService.GetWeeklyBucketHistoryPagination(param, userUnilever);
                listData = Json(new
                {
                    draw = param.draw,
                    recordsTotal = listWeeklyBucketHistory.totalRecord,
                    recordsFiltered = listWeeklyBucketHistory.totalRecord,
                    data = listWeeklyBucketHistory.weeklyBucketHistories
                });
            }
            catch (Exception ex)
            {
                listData = Json(new
                {
                    error = ex.Message
                });
            }
            return listData;
        }

        [HttpPost]
        public async Task<IActionResult> SaveProposal(List<ProposalInput> proposals)
        {
            var currDate = DateTime.Now;
            var user = await _userManager.GetUserAsync(User);
            List<string> errorMessages = new List<string>();
            var message = string.Empty;
            var fsaDetail = await _calendarService.GetCalendarDetail(currDate.Date);
            ValidateProposalInput(proposals, errorMessages, fsaDetail);
            List<Proposal> listProposal = new List<Proposal>();
            List<Approval> listApproval = new List<Approval>();
            List<ProposalHistory> listProposalHistory = new List<ProposalHistory>();
            var listEmail = new List<EmailApproval>();
            if (!errorMessages.Any())
            {
                try
                {
                    var bannerPlants = _bannerPlantService.GetAllActiveBannerPlant();
                    var skus = _skuService.GetAllProducts().Where(x => x.IsActive);

                    foreach (var proposalInput in proposals.Where(x => (x.proposeAdditional > 0 || x.rephase > 0) && !x.isWaitingApproval))
                    {
                        var approval = new Approval();
                        var proposal = new Proposal();
                        var proposalHistories = new List<ProposalHistory>();
                        var proposalHistory = new ProposalHistory();
                        var approvalId = Guid.NewGuid();

                            if (proposalInput.rephase > 0)
                            {
                                approval = CreateApproval(approvalId, ProposalType.Rephase);

                                proposal = await CreateProposalRephase(proposalInput, fsaDetail, (Guid)user.UserUnileverId);
                                approval.Proposal = proposal;
                            }
                            else if (proposalInput.proposeAdditional > 0)
                            {
                                proposal = await CreateProposalProposeAdditional(proposalInput, fsaDetail, (Guid)user.UserUnileverId, bannerPlants, skus);
                                approval = CreateApproval(approvalId, proposal.Type.Value);
                                approval.Proposal = proposal;
                            }
                            proposalHistory = await CreateProposalHistory(approval, proposal, fsaDetail);
                            listProposal.Add(proposal);

                        listApproval.Add(approval);

                        listProposalHistory.Add(proposalHistory);
                    }
                    var baseUrl = Request.Scheme + "://" + Request.Host + Url.Action("Index", "Approvals");
                    var emails = await _approvalService.GenerateCombinedEmailProposal(listApproval, baseUrl, User.Identity.Name);
                    listEmail.AddRange(emails);

                    await _proposalService.SaveProposals(listProposal);
                    await _proposalService.SaveProposalHistories(listProposalHistory);
                    await _approvalService.SaveApprovals(listApproval);
                    await _db.SaveChangesAsync();
                    foreach (var email in listEmail)
                    {
                        await _emailService.SendEmailAsync(email.RecipientEmail, email.Subject, email.Body);
                    }

                    message = "Your Proposal has been submitted!";
                    _notyfService.Success(message);


                    return Ok(proposals);
                }
                catch (Exception ex)
                {
                    message = "Submit Proposal Failed";
                    _notyfService.Warning(message);
                    errorMessages.Add(ex.Message);
                    return Ok(Json(new { proposals, errorMessages }));
                }
            }
            else
            {
                message = "Submit Proposal Failed";
                _notyfService.Warning(message);
                return Ok(Json(new { proposals, errorMessages }));
            }

        }


        public async Task<Proposal> CreateProposalRephase(ProposalInput proposalInput, FSACalendarDetail fsaDetail, Guid userId)
        {
            var bannerPlants = _bannerPlantService.GetAllBannerPlant().Where(x => x.IsActive);
            var banners = _bannerService.GetAllBanner();
            var skus = _skuService.GetAllProducts().Where(x => x.IsActive);
            var proposal = new Proposal
            {
                Id = Guid.NewGuid(),
                Week = fsaDetail.Week,
                Year = fsaDetail.Year,
                Month = fsaDetail.Month,
                Banner = banners.Single(x => x.Id == Guid.Parse(proposalInput.bannerId)),
                Sku = skus.Single(x => x.Id == Guid.Parse(proposalInput.skuId)),
                IsWaitingApproval = true,
                Type = ProposalType.Rephase,
                SubmittedAt = DateTime.Now,
                SubmittedBy = userId,
                Remark = proposalInput.remark,
                Rephase = proposalInput.rephase,
                ProposeAdditional = proposalInput.proposeAdditional,
                CDM = proposalInput.CDM,
                KAM = proposalInput.KAM
            };

            var weeklyBuckets = (await _bucketService.GetWeeklyBucketsByBannerSKU(Guid.Parse(proposalInput.bannerId), Guid.Parse(proposalInput.skuId))).Where(x => x.Month == fsaDetail.Month && x.Year == fsaDetail.Year);
            var z = weeklyBuckets.ToList();
            foreach (var weeklyBucket in weeklyBuckets)
            {
                var bannerPlant = bannerPlants.Single(x => x.Id == weeklyBucket.BannerPlant.Id);
                var proposalDetail = new ProposalDetail
                {
                    Id = Guid.NewGuid(),
                    BannerPlant = bannerPlant,
                    PlantContribution = weeklyBucket.PlantContribution,
                    ActualRephase = (proposalInput.rephase * weeklyBucket.PlantContribution) / 100,
                    Proposal = proposal,
                    WeeklyBucketId = weeklyBucket.Id,
                    IsTarget = true
                };
                proposal.ProposalDetails.Add(proposalDetail);
            }

            return proposal;
        }
        //public async Task<ProposeAddtionalBucket> GetWeeklyBucketSource(IQueryable<BannerPlant> bannerPlants, IQueryable<SKU> skus, decimal proposeAdditional, Proposal proposal, int month, int year)
        //{
        //    var i = 0;
        //    var bannerPlantSourceIds = new List<Guid>();
        //    var weeklyBucketSource = new WeeklyBucket();
        //    var weeklyBucketTargets = (await _bucketService.GetWeeklyBucketsByBannerSKU(proposal.Banner.Id, proposal.Sku.Id)).Where(x => x.Month == month && x.Year == year);
        //    var proposeAdditionalBucket = new ProposeAddtionalBucket();
        //    proposeAdditionalBucket.WeeklyBucketTargets = weeklyBucketTargets.ToList();
        //    //var bPlants = bannerPlants.Where(x => x.IsActive && x.CDM == proposal.CDM && x.KAM == proposal.KAM && !weeklyBuckets.Select(y => y.BannerPlant.Id).Contains(x.Id) && x.Banner.Id == proposal.Banner.Id).ToList();
        //    var zz = 5;
        //    //var sku = await skus.SingleAsync(x => x.Id == weeklyBucket.SKUId);
        //    //var bannerPlant = await bannerPlants.SingleAsync(x => x.Id == weeklyBucket.BannerPlant.Id);
        //    var targetBannerPlants = weeklyBucketTargets.Select(y => y.BannerPlant.Id);
        //    var weeklyBuckets = _bucketService.GetWeeklyBuckets().Where(x => x.Month == month && x.Year == year);
        //    while (i != 3)
        //    {

        //        switch (i)
        //        {
        //            //REALLOCATE ACROSS KAM
        //            case 0:
        //                bannerPlantSourceIds = await bannerPlants.Where(x => x.IsActive && x.CDM == proposal.CDM && x.KAM == proposal.KAM && !targetBannerPlants.Contains(x.Id) && x.Banner.Id != proposal.Banner.Id).Select(x => x.Id).ToListAsync();

        //                proposal.Type = ProposalType.ReallocateAcrossKAM;

        //                //bucketTargetIds = await bannerPlants.Where(x => x.KAM == bannerPlant.KAM && x.CDM == bannerPlant.CDM).Select(x => x.Id).ToListAsync();
        //                //weeklyBucketTargets = _bucketService.GetWeeklyBuckets().Where(x => bucketTargetIds.Contains(x.BannerPlant.Id) && x.SKUId == sku.Id && x.Month == month && x.Year == year);
        //                //proposal.Type = ProposalType.ReallocateAcrossKAM;
        //                break;
        //            //REALLOCATE ACROSS CDM
        //            case 1:
        //                bannerPlantSourceIds = await bannerPlants.Where(x => x.IsActive && x.CDM == proposal.CDM && !targetBannerPlants.Contains(x.Id) && x.Banner.Id != proposal.Banner.Id).Select(x => x.Id).ToListAsync();

        //                proposal.Type = ProposalType.ReallocateAcrossCDM;
        //                break;
        //            //REALLOCATE ACROSS MT
        //            case 2:
        //                bannerPlantSourceIds = await bannerPlants.Where(x => x.IsActive && !targetBannerPlants.Contains(x.Id) && x.Banner.Id != proposal.Banner.Id).Select(x => x.Id).ToListAsync();
        //                proposal.Type = ProposalType.ReallocateAcrossMT;
        //                break;
        //            default:
        //                proposal.Type = ProposalType.ProposeAdditional;
        //                break;
        //        }

        //        if (bannerPlantSourceIds.Any())
        //        {
        //            var weeklyBucketSources = weeklyBuckets.AsEnumerable().Where(x => bannerPlantSourceIds.Contains(x.BannerPlant.Id) && x.SKUId == proposal.Sku.Id);

        //            var weeklyBucketSouceGroups = weeklyBucketSources.GroupBy(x => new { x.BannerPlant.CDM, x.BannerPlant.KAM, x.BannerPlant.Banner.Id, x.SKUId }).ToList();

        //            var groupedWeeklyBucketSource = new WeeklyBucket();
        //            var groupedWeeklyBucketSources = new List<WeeklyBucket>();
        //            foreach (var weeklyBucketSourceGroup in weeklyBucketSouceGroups)
        //            {
        //                groupedWeeklyBucketSource.CDM = weeklyBucketSourceGroup.Key.CDM;
        //                groupedWeeklyBucketSource.KAM = weeklyBucketSourceGroup.Key.KAM;
        //                groupedWeeklyBucketSource.Banner = weeklyBucketSourceGroup.First().Banner;
        //                groupedWeeklyBucketSource.MonthlyBucket = weeklyBucketSourceGroup.Sum(x => x.MonthlyBucket);
        //                groupedWeeklyBucketSource.RemFSA = weeklyBucketSourceGroup.Sum(x => x.RemFSA);
        //                groupedWeeklyBucketSources.Add(groupedWeeklyBucketSource);
        //            }
        //            if (!groupedWeeklyBucketSources.Any())
        //            {
        //                i++;
        //            }
        //            else
        //            {
        //                var weeklyBucketSourceByMonthly = groupedWeeklyBucketSources.Where(x => x.MonthlyBucket > proposeAdditional).OrderByDescending(x => x.MonthlyBucket).FirstOrDefault();
        //                var weeklyBucketSourceByRemFSA = groupedWeeklyBucketSources.Where(x => x.RemFSA > proposeAdditional).OrderByDescending(x => x.RemFSA).FirstOrDefault();
        //                if (weeklyBucketSourceByMonthly == null && weeklyBucketSourceByRemFSA == null)
        //                {
        //                    proposal.Type = ProposalType.ProposeAdditional;
        //                    i++;
        //                }
        //                else if (weeklyBucketSourceByMonthly != null && weeklyBucketSourceByRemFSA != null)
        //                {
        //                    if (weeklyBucketSourceByMonthly.MonthlyBucket > weeklyBucketSourceByRemFSA.RemFSA)
        //                    {
        //                        weeklyBucketSource = weeklyBucketSourceByMonthly;
        //                    }
        //                    else
        //                    {
        //                        weeklyBucketSource = weeklyBucketSourceByRemFSA;
        //                    }
        //                }
        //                else
        //                {
        //                    weeklyBucketSource = weeklyBucketSourceByMonthly != null ? weeklyBucketSourceByMonthly : weeklyBucketSourceByRemFSA;

        //                }
        //                proposeAdditionalBucket.GroupedBucket = weeklyBucketSource;
        //                proposeAdditionalBucket.WeeklyBucketSource = weeklyBucketSources.ToList();
        //                break;
        //            }

        //        }
        //        else
        //        {
        //            i++;
        //        }
        //    }
        //    return proposeAdditionalBucket;
        //}

        public async Task<Proposal> CreateProposalProposeAdditional(ProposalInput proposalInput, FSACalendarDetail fsaDetail, Guid userId, IQueryable<BannerPlant> bannerPlants, IQueryable<SKU> skus)
        {
            var banners = _bannerService.GetAllBanner();
            var proposeAdditional = proposalInput.proposeAdditional;
            var proposal = new Proposal();
            proposal.Id = Guid.NewGuid();
            proposal.Week = fsaDetail.Week;
            proposal.Year = fsaDetail.Year;
            proposal.Month = fsaDetail.Month;
            proposal.Banner = banners.Single(x => x.Id == Guid.Parse(proposalInput.bannerId));
            proposal.Sku = skus.Single(x => x.Id == Guid.Parse(proposalInput.skuId));
            proposal.Remark = proposalInput.remark;
            proposal.IsWaitingApproval = true;
            proposal.SubmittedAt = DateTime.Now;
            proposal.SubmittedBy = userId;
            proposal.CDM = proposalInput.CDM;
            proposal.KAM = proposalInput.KAM;
            proposal.ProposeAdditional = proposeAdditional;


            var proposeAdditionalBucket = new ProposeAddtionalBucket();

            proposeAdditionalBucket = await _bucketService.GetWeeklyBucketSource(bannerPlants, skus, proposeAdditional, proposal, fsaDetail.Month, fsaDetail.Year);

     
            foreach(var weeklyBucket in proposeAdditionalBucket.WeeklyBucketTargets)
            {
  
                var proposalDetail = new ProposalDetail();
                proposalDetail.Id = Guid.NewGuid();
                proposalDetail.Proposal = proposal;
                proposalDetail.MonthlyBucket = weeklyBucket.MonthlyBucket;
                proposalDetail.PlantContribution = weeklyBucket.PlantContribution;
                proposalDetail.ActualProposeAdditional = (proposalInput.proposeAdditional * weeklyBucket.PlantContribution) / 100;
                proposalDetail.BannerPlant = weeklyBucket.BannerPlant;
                proposalDetail.WeeklyBucketId = weeklyBucket.Id;
                proposalDetail.IsTarget = true;
                proposal.ProposalDetails.Add(proposalDetail);
            }

            foreach(var weeklyBucketSource in proposeAdditionalBucket.WeeklyBucketSource)
            {
                var proposalDetail = new ProposalDetail();
                proposalDetail.Id = Guid.NewGuid();
                proposalDetail.Proposal = proposal;
                proposalDetail.MonthlyBucket = weeklyBucketSource.MonthlyBucket;
                proposalDetail.PlantContribution = weeklyBucketSource.PlantContribution;
                proposalDetail.ActualProposeAdditional = (proposalInput.proposeAdditional * weeklyBucketSource.PlantContribution) / 100 * -1;
                proposalDetail.BannerPlant = weeklyBucketSource.BannerPlant;
                proposalDetail.WeeklyBucketId = weeklyBucketSource.Id;
                proposalDetail.IsTarget = false;
                proposal.ProposalDetails.Add(proposalDetail);
            }


            //var proposalDetailTarget = new ProposalDetail();

            //if (proposal.Type != ProposalType.ProposeAdditional)
            //{
            //    proposalDetailTarget.WeeklyBucketId = weeklyBucketTarget.Id;
            //    proposalDetailTarget.ProposeAdditional = -1 * proposeAdditional;
            //    proposalDetailTarget.ApprovalId = approvalId;
            //    listProposalDetail.Add(proposalDetailTarget);
            //}

            //proposalDetail.WeeklyBucketId = weeklyBucketId;
            //proposalDetail.ProposeAdditional = proposeAdditional;
            //proposalDetail.ApprovalId = approvalId;
            //listProposalDetail.Add(proposalDetail);




            return proposal;
        }

        public Approval CreateApproval(Guid id, ProposalType proposalType)
        {
            var approval = new Approval
            {
                Id = id,
                ApprovalStatus = ApprovalStatus.Pending,
                ProposalType = proposalType,
                Level = proposalType != ProposalType.ProposeAdditional ? 2 : 3,
                ApprovedBy = string.Empty
            };

            approval.ApproverWL = _approvalService.GetWLApprover(approval);

            return approval;
        }

        public async Task<ProposalHistory> CreateProposalHistory(Approval approval, Proposal proposal, FSACalendarDetail fsaDetail)
        {

            var propHistory = new ProposalHistory
            {
                Id = Guid.NewGuid(),
                ProposalId = proposal.Id,
                SKUId = proposal.Sku.Id,
                BannerId = proposal.Banner.Id,
                Week = fsaDetail.Week,
                Month = fsaDetail.Month,
                Year = fsaDetail.Year,
                Rephase = proposal.Rephase,
                ProposeAdditional = proposal.ProposeAdditional,
                Remark = proposal.Remark,
                SubmittedAt = proposal.SubmittedAt.ToString(),
                SubmittedBy = proposal.SubmittedBy.Value,
            };
            //foreach (var detail in proposal.ProposalDetails)
            //{
            //    var weeklyBucket = await _bucketService.GetWeeklyBucket(detail.WeeklyBucketId);
            //    Thread.CurrentThread.CurrentCulture = new CultureInfo("id-ID");
            //    var propHistory = new ProposalHistory
            //    {
            //        Id = Guid.NewGuid(),
            //        ApprovalId = approval.Id,
            //        SKUId = proposal.Sku.Id,
            //        BannerId = proposal.Banner.Id,
            //        Week = fsaDetail.Week,
            //        Month = fsaDetail.Month,
            //        Year = fsaDetail.Year,
            //        Rephase = detail.Rephase,
            //        ProposeAdditional = detail.ProposeAdditional,
            //        Remark = proposal.Remark,
            //        SubmittedAt = proposal.SubmittedAt.ToShortDateString(),
            //        SubmittedBy = proposal.SubmittedBy.Value,
            //    };
            //    listHistory.Add(propHistory);
            //}

            return propHistory;
        }

        public void ValidateProposalInput(List<ProposalInput> proposalInputs, List<string> errorMessages, FSACalendarDetail fSACalendarDetail)
        {
            var currDate = DateTime.Now;

            if (proposalInputs.Where(x => !x.isWaitingApproval).All(x => x.rephase == 0) && proposalInputs.Where(x => !x.isWaitingApproval).All(x => x.proposeAdditional == 0))
            {
                errorMessages.Add("No submitted proposal!");
            }

            foreach (var proposal in proposalInputs.Where(x => (!string.IsNullOrEmpty(x.remark) || x.proposeAdditional > 0 || x.rephase > 0)))
            {

                if (proposal.rephase > 0 && proposal.proposeAdditional > 0)
                {
                    errorMessages.Add(string.Format("Cannot request Rephase and Propose Additional together at the same time on Bucket Name: {0}, PlantName: {1} and PCMap: {2}", proposal.bannerName, proposal.plantName, proposal.pcMap));
                }
                if (proposal.rephase > 0 && ((currDate.DayOfWeek >= DayOfWeek.Thursday && currDate.TimeOfDay.Hours > 12) || currDate.DayOfWeek == DayOfWeek.Sunday))
                {
                    errorMessages.Add("Cannot request rephase after Thursday 12 AM, Please request again on Monday");
                }

                if (proposal.rephase > proposal.nextWeekBucket)
                {
                    errorMessages.Add(string.Format("Cannot request Rephase more than next week bucket value on Bucket Name: {0}, PlantName: {1} and PCMap: {2}", proposal.bannerName, proposal.plantName, proposal.pcMap));
                }

                if (proposal.proposeAdditional > 0)
                {
                    if (currDate.DayOfWeek >= DayOfWeek.Friday && currDate.DayOfWeek <= DayOfWeek.Sunday && fSACalendarDetail.Week == 4)
                    {
                        errorMessages.Add(string.Format("Cannot request Propose Additional after Friday on Week 4 "));
                    }
                }

                if (string.IsNullOrEmpty(proposal.remark))
                {
                    errorMessages.Add(string.Format("Please fill Remark on Bucket Name: {0}, PlantName: {1} and PCMap: {2}", proposal.bannerName, proposal.plantName, proposal.pcMap));
                }
            }
        }

        public async Task<ActionResult> DownloadProposalExcel(int month, int year)
        {
            var user = await _userManager.GetUserAsync(User);
            var userUnilever = await _userService.GetUser((Guid)user.UserUnileverId);
            userUnilever.WLName = (await _userService.GetAllWorkLevel().SingleAsync(x => x.Id == userUnilever.WLId)).WL;
            var data = _proposalService.GetProposalExcelData(month, year, userUnilever);
            var monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);
            var workbook = new HSSFWorkbook();
            ISheet worksheet = workbook.CreateSheet("Sheet1");
            var monthRow = worksheet.CreateRow(0).CreateCell(0);

            List<string> errorMessages = new List<string>();

            var style = workbook.CreateCellStyle();
            style.Alignment = HorizontalAlignment.Center;

            var columns = new List<string>
            {
                "Month",
                "KAM",
                "CDM",
                "BannerName",
                "PC Map",
                "Description Map",
                "Monthly Bucket",
                "Plant Contribution",
                "Banner Week 1",
                "Banner Week 2",
                "Banner Week 3",
                "Banner Week 4",
                "Banner Week 5",
                "Rating Rate",
                "Dispatch Consume",
                "Valid + BJ",
                "Remaining FSA",
            };

            CellRangeAddress range = new CellRangeAddress(0, 0, 0, columns.Count - 1);
            worksheet.AddMergedRegion(range);

            monthRow.CellStyle = style;
            monthRow.SetCellValue("Month:" + monthName + " - " + year.ToString());

            range = new CellRangeAddress(1, 1, 0, columns.Count - 1);
            worksheet.AddMergedRegion(range);
            var headerRow = worksheet.CreateRow(1).CreateCell(0);
            headerRow.CellStyle = style;
            headerRow.SetCellValue("Proposal");

            var row = worksheet.CreateRow(2);

            for (var i = 0; i < columns.Count; i++)
            {
                row.CreateCell(i).SetCellValue(columns[i]);
            }

            int x = 0;
            foreach (var item in data)
            {
                var i = 0;
                row = worksheet.CreateRow(x + 3);

                row.CreateCell(i).SetCellValue(item.Month);
                i++;
                row.CreateCell(i).SetCellValue(item.KAM);
                i++;
                row.CreateCell(i).SetCellValue(item.CDM);
                i++;
                row.CreateCell(i).SetCellValue(item.BannerName);
                i++;
                row.CreateCell(i).SetCellValue(item.PCMap);
                i++;
                row.CreateCell(i).SetCellValue(item.DescriptionMap);
                i++;
                row.CreateCell(i).SetCellValue((double)item.MonthlyBucket);
                i++;
                row.CreateCell(i).SetCellValue((double)item.PlantContribution);
                i++;
                row.CreateCell(i).SetCellValue((double)item.BucketWeek1);
                i++;
                row.CreateCell(i).SetCellValue((double)item.BucketWeek2);
                i++;
                row.CreateCell(i).SetCellValue((double)item.BucketWeek3);
                i++;
                row.CreateCell(i).SetCellValue((double)item.BucketWeek4);
                i++;
                row.CreateCell(i).SetCellValue((double)item.BucketWeek5);
                i++;
                row.CreateCell(i).SetCellValue((double)item.RatingRate);
                i++;
                row.CreateCell(i).SetCellValue((double)item.DispatchConsume);
                i++;
                row.CreateCell(i).SetCellValue((double)item.ValidBJ);
                i++;
                row.CreateCell(i).SetCellValue((double)item.RemFSA);
                i++;

                x++;
            }

            MemoryStream ms = new MemoryStream();
            workbook.Write(ms);
            ms.Position = 0;
            try
            {
                FileStreamResult file = File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Proposal - " + monthName + " - " + year.ToString() + ".xls");

                return file;
            }
            catch (Exception ex)
            {
                errorMessages.Add(ex.Message);
            }


            return Ok();
        }

    }
}


