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
        private readonly IBannerService _bannerService;
        private readonly ISKUService _skuService;
        private readonly UserManager<FSAWebSystemUser> _userManager;

        public ProposalsController(FSAWebSystemDbContext db, IProposalService proposalService, IBucketService bucketService, ICalendarService calendarService, UserManager<FSAWebSystemUser> userManager, IUserService userService, INotyfService notyfService,
            IBannerService bannerService, IApprovalService approvalService, ISKUService skuService)
        {
            _db = db;
            _proposalService = proposalService;
            _bucketService = bucketService;
            _calendarService = calendarService;
            _userManager = userManager;
            _userService = userService;
            _notyfService = notyfService;
            _approvalService = approvalService;
            _bannerService = bannerService;
            _skuService = skuService;
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
                    data = await _proposalService.GetProposalForView(Convert.ToInt32(param.month), Convert.ToInt32(param.year), week, param, userUnilever.Id);

                }
                //if (param.proposalInputs != null)
                //{
                //    var weeklyBucketIds = data.proposals.Select(x => x.WeeklyBucketId).ToList();
                //    var proposalForUpdate = param.proposalInputs.Where(x => (!string.IsNullOrEmpty(x.remark) || x.proposeAdditional > 0 || x.rephase > 0) && weeklyBucketIds.Contains(Guid.Parse(x.weeklyBucketId))).ToList();
                //    foreach (var proposalInput in proposalForUpdate)
                //    {
                //        var proposal = data.proposals.SingleOrDefault(x => x.WeeklyBucketId == Guid.Parse(proposalInput.weeklyBucketId) && x.Type == );
                //        proposal.Rephase = proposalInput.rephase;
                //        proposal.ProposeAdditional = proposalInput.proposeAdditional;
                //        proposal.Remark = proposalInput.remark;
                //    }
                //}
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

            }
            return listData;

        }

        [HttpPost]
        public async Task<IActionResult> GetProposalHistoryPagination(DataTableParam param)
        {
            var user = await _userManager.GetUserAsync(User);
            var listData = Json(new { });
            try
            {
                var listProposalHistory = _proposalService.GetProposalHistoryPagination(param, (Guid)user.UserUnileverId, Convert.ToInt32(param.month), Convert.ToInt32(param.year));
                listData = Json(new
                {
                    draw = param.draw,
                    recordsTotal = listProposalHistory.totalRecord,
                    recordsFiltered = listProposalHistory.totalRecord,
                    data = listProposalHistory.proposalsHistory
                });
            }
            catch (Exception ex)
            {

            }
            return listData;
        }

        [HttpPost]
        public async Task<IActionResult> GetMonthlyBucketHistoryPagination(DataTableParam param)
        {
            var user = await _userManager.GetUserAsync(User);
            var listData = Json(new { });
            try
            {
                var listMonthlyBucketHistory = await _bucketService.GetMonthlyBucketHistoryPagination(param, (Guid)user.UserUnileverId);
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

            }
            return listData;
        }

        [HttpPost]
        public async Task<IActionResult> GetWeeklyBucketHistoryPagination(DataTableParam param)
        {
            var user = await _userManager.GetUserAsync(User);
            var listData = Json(new { });
            try
            {
                var listWeeklyBucketHistory = await _bucketService.GetWeeklyBucketHistoryPagination(param, (Guid)user.UserUnileverId);
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
            if (!errorMessages.Any() && !proposals.Any(x => string.IsNullOrEmpty(x.weeklyBucketId)))
            {
                try
                {
                    var banners = _bannerService.GetAllActiveBanner();
                    var skus = _skuService.GetAllProducts().Where(x => x.IsActive);
                    
                    var savedProposals = _proposalService.GetPendingProposals(fsaDetail, (Guid)user.UserUnileverId);
                    var savedApprovals = _approvalService.GetPendingApprovals();
                    foreach (var proposalInput in proposals.Where(x => (x.proposeAdditional > 0 || x.rephase > 0) && !x.isWaitingApproval))
                    {
                        var approval = new Approval();
                        var proposal = new Proposal();
                        var proposalHistory = new ProposalHistory();
                        var approvalId = Guid.NewGuid();
                        if (Guid.Parse(proposalInput.id) == Guid.Empty)
                        {
                            if (proposalInput.rephase > 0)
                            {
                                approval = CreateApproval(approvalId, ProposalType.Rephase);

                                proposal = CreateProposalRephase(proposalInput, fsaDetail, (Guid)user.UserUnileverId, approval.Id);
                            }
                            else if (proposalInput.proposeAdditional > 0)
                            {
                                //approval = await CreateApprovalProposeAdditional(proposalInput.proposeAdditional, approval.Id, Guid.Parse(proposalInput.weeklyBucketId), fsaDetail, banners, skus);
                                proposal = await CreateProposalProposeAdditional(proposalInput, fsaDetail, (Guid)user.UserUnileverId, approvalId, banners, skus);
                                approval = CreateApproval(approvalId, proposal.Type.Value);
                            }
                            proposalHistory = CrateProposalHistory(approval, proposal, fsaDetail);
                            listProposal.Add(proposal);
                            await _proposalService.SaveProposals(listProposal);
                        }
                        else
                        {
                            var listProposalDetail = new List<ProposalDetail>();
                            var proposalDetail = new ProposalDetail();
                            var savedProposal = savedProposals.Single(x => x.Id == Guid.Parse(proposalInput.id));

                            approval = CreateApproval(approvalId, savedProposal.Type.Value);


                            if (proposalInput.rephase > 0)
                            {
                                savedProposal.Rephase = proposalInput.rephase;
                                savedProposal.Type = ProposalType.Rephase;
                            }

                            if (proposalInput.proposeAdditional > 0)
                            {
                                savedProposal.ProposeAdditional = proposalInput.proposeAdditional;
                                savedProposal.Type = ProposalType.ProposeAdditional;
                            }

                            var weeklyBucketTarget = await ValidateProposeAdditional(banners, skus, proposalInput.proposeAdditional, savedProposal.WeeklyBucketId, savedProposal);

                            var proposalDetailTarget = new ProposalDetail();
                            if (savedProposal.Type != ProposalType.ProposeAdditional)
                            {
                                proposalDetailTarget.WeeklyBucketId = weeklyBucketTarget.Id;
                                proposalDetailTarget.ProposeAdditional = -1 * proposalInput.proposeAdditional;
                                proposalDetailTarget.ApprovalId = approvalId;
                                listProposalDetail.Add(proposalDetailTarget);
                            }

                            proposalDetail.WeeklyBucketId = savedProposal.WeeklyBucketId;
                            proposalDetail.ProposeAdditional = proposalInput.proposeAdditional;
                            proposalDetail.ApprovalId = approvalId;
                            listProposalDetail.Add(proposalDetail);

                            savedProposal.ProposalDetails = listProposalDetail;
                            savedProposal.Remark = proposalInput.remark;
                            savedProposal.IsWaitingApproval = true;
                            savedProposal.SubmittedAt = DateTime.Now;
                            savedProposal.ApprovalId = approvalId;

                            proposalHistory = CrateProposalHistory(approval, savedProposal, fsaDetail);
                        }

                        listApproval.Add(approval);

                        listProposalHistory.Add(proposalHistory);
                    }

                    await _proposalService.SaveProposalHistories(listProposalHistory);
                    await _approvalService.SaveApprovals(listApproval);

                    await _db.SaveChangesAsync();

                    message = "Your Proposal has been submitted!";
                    _notyfService.Success(message);
                    return Ok(proposals);
                }
                catch (Exception ex)
                {
                    message = "Submit Proposal Failed";
                    _notyfService.Warning(message);
                    errorMessages.Add(ex.Message);
                }
            }
            else
            {
                message = "Submit Proposal Failed";
                _notyfService.Warning(message);
            }
            return BadRequest(Json(new { proposals, errorMessages }));
        }


        public Proposal CreateProposalRephase(ProposalInput proposalInput, FSACalendarDetail fsaDetail, Guid userId, Guid approvalId)
        {
            var proposal = new Proposal
            {
                Id = Guid.NewGuid(),
                Week = fsaDetail.Week,
                Year = fsaDetail.Year,
                Month = fsaDetail.Month,
                WeeklyBucketId = Guid.Parse(proposalInput.weeklyBucketId),
                Rephase = proposalInput.rephase,
                Remark = proposalInput.remark,
                IsWaitingApproval = true,
                ApprovalId = approvalId,
                Type = ProposalType.Rephase,
                SubmittedAt = DateTime.Now,
                SubmittedBy = userId
            };


            return proposal;
        }
        public async Task<WeeklyBucket> ValidateProposeAdditional(IQueryable<Banner> banners, IQueryable<SKU> skus, decimal proposeAdditional, Guid weeklyBucketId, Proposal proposal)
        {
            var i = 0;
            var bucketTargetIds = new List<Guid>();
            var weeklyBucketTarget = new WeeklyBucket();
            var weeklyBucket = await _bucketService.GetWeeklyBucket(weeklyBucketId);
            var sku = await skus.SingleAsync(x => x.Id == weeklyBucket.SKUId);
            var banner = await banners.SingleAsync(x => x.Id == weeklyBucket.BannerId);

            while (i != 3)
            {
                var weeklyBucketTargets = _bucketService.GetWeeklyBuckets();
                switch (i)
                {
                    //REALLOCATE ACROSS KAM
                    case 0:
                        bucketTargetIds = await banners.Where(x => x.KAM == banner.KAM && x.CDM == banner.CDM).Select(x => x.Id).ToListAsync();
                        weeklyBucketTargets = _bucketService.GetWeeklyBuckets().Where(x => bucketTargetIds.Contains(x.BannerId) && x.SKUId == sku.Id);
                        proposal.Type = ProposalType.ReallocateAcrossKAM;
                        break;
                    //REALLOCATE ACROSS CDM
                    case 1:
                        bucketTargetIds = await banners.Where(x => x.CDM == banner.CDM).Select(x => x.Id).ToListAsync();
                        weeklyBucketTargets = _bucketService.GetWeeklyBuckets().Where(x => bucketTargetIds.Contains(x.BannerId) && x.SKUId == sku.Id);
                        proposal.Type = ProposalType.ReallocateAcrossCDM;
                        break;
                    //REALLOCATE ACROSS MT
                    case 2:
                        weeklyBucketTargets = _bucketService.GetWeeklyBuckets().Where(x => x.SKUId == sku.Id);
                        proposal.Type = ProposalType.ReallocateAcrossCDM;
                        break;
                    default:
                        proposal.Type = ProposalType.ProposeAdditional;
                        break;
                }

                var weeklyBucketTargetByMonthly = await weeklyBucketTargets.Where(x => x.MonthlyBucket > proposeAdditional && x.Id != weeklyBucket.Id).OrderByDescending(x => x.MonthlyBucket).FirstOrDefaultAsync();
                var weeklyBucketTargetByRemFSA = await weeklyBucketTargets.Where(x => x.RemFSA > proposeAdditional && x.Id != weeklyBucket.Id).OrderByDescending(x => x.RemFSA).FirstOrDefaultAsync();
                if (weeklyBucketTargetByMonthly == null && weeklyBucketTargetByRemFSA == null)
                {
                    i++;
                }
                else if (weeklyBucketTargetByMonthly != null && weeklyBucketTargetByRemFSA != null)
                {
                    if (weeklyBucketTargetByMonthly.MonthlyBucket > weeklyBucketTargetByRemFSA.RemFSA)
                    {
                        weeklyBucketTarget = weeklyBucketTargetByMonthly;
                        break;
                    }
                    else
                    {
                        weeklyBucketTarget = weeklyBucketTargetByRemFSA;
                        break;
                    }
                }
                else
                {
                    weeklyBucketTarget = weeklyBucketTargetByMonthly != null ? weeklyBucketTargetByMonthly : weeklyBucketTargetByRemFSA;
                    break;
                }
            }

            return weeklyBucketTarget;
        }

        public async Task<Proposal> CreateProposalProposeAdditional(ProposalInput proposalInput, FSACalendarDetail fsaDetail, Guid userId, Guid approvalId, IQueryable<Banner> banners, IQueryable<SKU> skus)
        {

            var listProposalDetail = new List<ProposalDetail>();
            var proposalDetail = new ProposalDetail();
            var proposeAdditional = proposalInput.proposeAdditional;
            var weeklyBucketId = Guid.Parse(proposalInput.weeklyBucketId);
            var proposal = new Proposal();

            var weeklyBucketTarget = new WeeklyBucket();

            weeklyBucketTarget = await ValidateProposeAdditional(banners, skus, proposeAdditional, weeklyBucketId, proposal);

            var proposalDetailTarget = new ProposalDetail();
            if (proposal.Type != ProposalType.ProposeAdditional)
            {
                proposalDetailTarget.WeeklyBucketId = weeklyBucketTarget.Id;
                proposalDetailTarget.ProposeAdditional = -1 * proposeAdditional;
                proposalDetailTarget.ApprovalId = approvalId;
                listProposalDetail.Add(proposalDetailTarget);
            }

            proposalDetail.WeeklyBucketId = weeklyBucketId;
            proposalDetail.ProposeAdditional = proposeAdditional;
            proposalDetail.ApprovalId = approvalId;
            listProposalDetail.Add(proposalDetail);


            proposal.Id = Guid.NewGuid();
            proposal.Week = fsaDetail.Week;
            proposal.Year = fsaDetail.Year;
            proposal.Month = fsaDetail.Month;
            proposal.WeeklyBucketId = Guid.Parse(proposalInput.weeklyBucketId);
            proposal.ProposeAdditional = proposalInput.proposeAdditional;
            proposal.Remark = proposalInput.remark;
            proposal.IsWaitingApproval = true;
            proposal.ApprovalId = approvalId;
            proposal.SubmittedAt = DateTime.Now;
            proposal.SubmittedBy = userId;
            proposal.ProposalDetails = listProposalDetail;


            return proposal;
        }

        //public async Task<List<ProposalDetail>> CreateProposalProposeAdditionalDetails(decimal proposeAdditional, Guid approvalId, Guid weeklyBucketId, FSACalendarDetail fsaDetail, IQueryable<Banner> banners, IQueryable<SKU> skus)
        //{

        //    approval.ApprovalDetails = list
        //}


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

            return approval;
        }

        public ProposalHistory CrateProposalHistory(Approval approval, Proposal proposal, FSACalendarDetail fsaDetail)
        {
            var propHistory = new ProposalHistory
            {
                Id = Guid.NewGuid(),
                Week = fsaDetail.Week,
                Month = fsaDetail.Month,
                Year = fsaDetail.Year,
                ApprovalId = approval.Id,
                ProposalId = proposal.Id,
                Rephase = proposal.Rephase,
                ProposeAdditional = proposal.ProposeAdditional,
                Remark = proposal.Remark,
                SubmittedAt = proposal.SubmittedAt.ToString("dd/MM/yyyy"),
                SubmittedBy = proposal.SubmittedBy
            };
            return propHistory;
        }

        public void ValidateProposalInput(List<ProposalInput> proposalInputs, List<string> errorMessages, FSACalendarDetail fSACalendarDetail)
        {
            var currDate = DateTime.Now;

            foreach (var proposal in proposalInputs.Where(x => (!string.IsNullOrEmpty(x.remark) || x.proposeAdditional > 0 || x.rephase > 0)))
            {
                if (proposal.rephase > 0 && ((currDate.DayOfWeek >= DayOfWeek.Thursday && currDate.TimeOfDay.Hours > 12) || currDate.DayOfWeek == DayOfWeek.Sunday))
                {
                    errorMessages.Add("Cannot request rephase after Thursday 12 AM, Please request again on Monday");
                }

                if (proposal.rephase > proposal.nextWeekBucket)
                {
                    errorMessages.Add(string.Format("Cannot request Rephase more than next week bucket value on Bucket Name: {0}, PlantName: {1} and PCMap: {2}", proposal.bannerName, proposal.plantName, proposal.pcMap));
                }

                if(proposal.proposeAdditional > 0)
                {
                    if(currDate.DayOfWeek >= DayOfWeek.Friday && currDate.DayOfWeek <= DayOfWeek.Sunday && fSACalendarDetail.Week == 4)
                    {
                        errorMessages.Add(string.Format("Cannot request Propose Additional on Week 4 after Friday"));
                    }
                }
            }
        }


    }
}


