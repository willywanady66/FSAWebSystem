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
            var userUnilever = await _userService.GetUser(Guid.Parse(user.Id));
            List<Proposal> listProposal = new List<Proposal>();
            var listData = Json(new { });
            var data = new ProposalData();
            var currentDate = DateTime.Now;

            try
            {
                var fsaDetail = await _calendarService.GetCalendarDetail(currentDate.Date);

                if (fsaDetail != null)
                {
                    var week = fsaDetail.Week;
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
                    data = data.proposals
                });
            }
            catch (Exception ex)
            {

            }




            return listData;

        }

        //[HttpPost]
        //public async Task<IActionResult> GetProposalReallocatePagination(DataTableParamProposal param)
        //{
        //    var user = await _userManager.GetUserAsync(User);
        //    var userUnilever = await _userService.GetUser(Guid.Parse(user.Id));
        //    List<Proposal> listProposal = new List<Proposal>();
        //    var listData = Json(new { });
        //    var data = new ProposalData();
        //    var currentDate = DateTime.Now;

        //    var weeklyBucketBanners = await _bucketService.GetWeeklyBucketBanners();
        //    var userBanners = await _bannerService.GetUserBanners(userUnilever.Id);
        //    var dropdownBanner = userBanners.Where(x => weeklyBucketBanners.Contains(x.Id)).Select(x => new SelectListItem { Text = x.BannerName + " (" + x.PlantName + ")" + " (" + x.PlantCode + ')', Value = x.Id.ToString() }).ToList();

        //    try
        //    {
        //        var fsaDetail = await _calendarService.GetCalendarDetail(currentDate.Date);
        //        if (fsaDetail != null)
        //        {
        //            var week = fsaDetail.Week;
        //            if (Convert.ToInt32(param.month) != currentDate.Month || Convert.ToInt32(param.year) != currentDate.Year)
        //            {
        //                week = 1;
        //            }
        //            data = await _proposalService.GetProposalReallocateForView(Convert.ToInt32(param.month), Convert.ToInt32(param.year), week, param, userUnilever.Id);
        //        }

        //        listData = Json(new
        //        {

        //            draw = param.draw,
        //            recordsTotal = data.totalRecord,
        //            recordsFiltered = data.totalRecord,
        //            data = data.proposals,
        //            dropdownBanner = dropdownBanner
        //        });
        //    }
        //    catch
        //    {

        //    }
        //    return listData;
        //}

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
            ValidateProposalInput(proposals, errorMessages);
            List<Proposal> listProposal = new List<Proposal>();
            List<Approval> listApproval = new List<Approval>();
            List<ProposalHistory> listProposalHistory = new List<ProposalHistory>();
            if (!errorMessages.Any() && !proposals.Any(x => string.IsNullOrEmpty(x.weeklyBucketId)))
            {
                try
                {
                    var banners = _bannerService.GetAllActiveBanner();
                    var skus = _skuService.GetAllProducts().Where(x => x.IsActive);
                    var fsaDetail = await _calendarService.GetCalendarDetail(currDate.Date);
                    var savedProposals = _proposalService.GetPendingProposals(fsaDetail, (Guid)user.UserUnileverId);
                    var savedApprovals = _approvalService.GetPendingApprovals();
                    foreach (var proposalInput in proposals.Where(x => (x.proposeAdditional > 0 || x.rephase > 0) && !x.isWaitingApproval))
                    {
                        var approval = new Approval();
                        var proposal = new Proposal();
                        var proposalHistory = new ProposalHistory();
                        if (Guid.Parse(proposalInput.id) == Guid.Empty)
                        {
                            if (proposalInput.rephase > 0)
                            {
                                approval = CreateApproval(ProposalType.Rephase);

                                proposal = CreateProposalRephase(proposalInput, fsaDetail, (Guid)user.UserUnileverId);
                            } else if (proposalInput.proposeAdditional > 0)
                            {
                                approval = CreateApproval(ProposalType.ProposeAdditional);
                                approval.ApprovalDetails = await CreateApprovalDetail(proposalInput.proposeAdditional, approval.Id, Guid.Parse(proposalInput.weeklyBucketId), fsaDetail, banners, skus);
                                proposal = CreateProposaProposeAdditional(proposalInput, fsaDetail, (Guid)user.UserUnileverId);
                            }
                            proposalHistory = CrateProposalHistory(approval.Id, proposal.Id, fsaDetail);
                        }
                        else
                        {

                            var savedProposal = savedProposals.Single(x => x.Id == Guid.Parse(proposalInput.id));

                            approval = CreateApproval(savedProposal.Type.Value);

                            
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

                            savedProposal.Remark = proposalInput.remark;
                            savedProposal.IsWaitingApproval = true;
                            savedProposal.SubmittedAt = DateTime.Now;

                            proposalHistory = CrateProposalHistory(approval.Id, proposal.Id, fsaDetail);
                        }

                        listApproval.Add(approval);
                        listProposal.Add(proposal);
                        listProposalHistory.Add(proposalHistory);
                    }
                    await _proposalService.SaveProposals(listProposal);
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


        public Proposal CreateProposalRephase(ProposalInput proposalInput, FSACalendarDetail fsaDetail, Guid userId)
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
                //ApprovalId = approval.Id,
                Type = ProposalType.Rephase,
                SubmittedAt = DateTime.Now,
                SubmittedBy = userId
            };


            return proposal;
        }

        public Proposal CreateProposaProposeAdditional(ProposalInput proposalInput, FSACalendarDetail fsaDetail, Guid userId)
        {
            var proposal = new Proposal
            {
                Id = Guid.NewGuid(),
                Week = fsaDetail.Week,
                Year = fsaDetail.Year,
                Month = fsaDetail.Month,
                WeeklyBucketId = Guid.Parse(proposalInput.weeklyBucketId),
                ProposeAdditional = proposalInput.proposeAdditional,
                Remark = proposalInput.remark,
                IsWaitingApproval = true,
                //ApprovalId = approval.Id,
                Type = ProposalType.ProposeAdditional,
                SubmittedAt = DateTime.Now,
                SubmittedBy = userId
            };


            return proposal;
        }


        public Approval CreateApproval(ProposalType proposalType)
        {
            var approval = new Approval
            {
                Id = Guid.NewGuid(),
                ApprovalStatus = ApprovalStatus.Pending,
                ProposalType = proposalType
            };

            return approval;
        }

        public async Task<List<ApprovalDetail>> CreateApprovalDetail(decimal proposeAdditional, Guid approvalId, Guid weeklyBucketId, FSACalendarDetail fsaDetail, IQueryable<Banner> banners, IQueryable<SKU> skus)
        {
            var listApprovalDetail = new List<ApprovalDetail>();
            var approvalDetail = new ApprovalDetail();
  

            var weeklyBucket = await _bucketService.GetWeeklyBucket(weeklyBucketId);
            var sku = await skus.SingleAsync(x => x.Id == weeklyBucket.SKUId);
            var banner = await banners.SingleAsync(x => x.Id == weeklyBucket.BannerId);

            var bucketTargetIds = await banners.Where(x => x.KAM == banner.KAM).Select(x => x.Id).ToListAsync();

            var weeklyBucketTargets = _bucketService.GetWeeklyBuckets().Where(x => bucketTargetIds.Contains(x.BannerId) && x.SKUId == sku.Id);

            var weeklyBucketTarget = new WeeklyBucket();
            var weeklyBucketTarget1 = await weeklyBucketTargets.Where(x => x.MonthlyBucket > proposeAdditional).OrderByDescending(x => x.MonthlyBucket).FirstOrDefaultAsync();
            var weeklyBucketTarget2 = await weeklyBucketTargets.Where(x => x.RemFSA > proposeAdditional).OrderByDescending(x => x.RemFSA).FirstOrDefaultAsync();
            if(weeklyBucketTarget1 != null && weeklyBucketTarget2 != null)
            {
                bucketTargetIds = await banners.Where(x => x.CDM == banner.CDM).Select(x => x.Id).ToListAsync();
                weeklyBucketTargets = _bucketService.GetWeeklyBuckets().Where(x => bucketTargetIds.Contains(x.BannerId) && x.SKUId == sku.Id);
                weeklyBucketTarget1 = await weeklyBucketTargets.Where(x => x.RemFSA > proposeAdditional).OrderByDescending(x => x.RemFSA).FirstOrDefaultAsync();
                if(weeklyBucketTarget1 != null)
                {
                    var bannerTarget = await banners.SingleAsync(x => x.Id == weeklyBucketTarget1.BannerId);
                    var approvalDetail1 = new ApprovalDetail();
                    approvalDetail1.MonthlyBucket = weeklyBucketTarget1.MonthlyBucket;
                    approvalDetail1.RatingRate = weeklyBucketTarget1.RatingRate;
                    approvalDetail1.ValidBJ = weeklyBucketTarget1.ValidBJ;
                    approvalDetail1.RemFSA = weeklyBucketTarget1.RemFSA;
                    approvalDetail1.CurrentBucket = Convert.ToDecimal(weeklyBucket.GetType().GetProperty("BucketWeek" + fsaDetail.Week.ToString()).GetValue(weeklyBucket, null));
                    approvalDetail1.NextWeekBucket = Convert.ToDecimal(weeklyBucket.GetType().GetProperty("BucketWeek" + (fsaDetail.Week + 1).ToString()).GetValue(weeklyBucket, null));
                    approvalDetail1.RemFSA = weeklyBucket.RemFSA;
                    approvalDetail1.BannerName = bannerTarget.BannerName;
                    approvalDetail1.PlantName = bannerTarget.PlantName;
                    approvalDetail1.PlantCode = bannerTarget.PlantCode;
                    approvalDetail1.PCMap = sku.PCMap;
                    approvalDetail1.DescriptionMap = sku.DescriptionMap;
                    approvalDetail1.ProposeAdditional = -1 * proposeAdditional;
                }    
            }   

            else
            {
                //if(weeklyBucket.)
            }


            approvalDetail.MonthlyBucket = weeklyBucket.MonthlyBucket;
            approvalDetail.RatingRate = weeklyBucket.RatingRate;
            approvalDetail.ValidBJ = weeklyBucket.ValidBJ;
            approvalDetail.RemFSA = weeklyBucket.RemFSA;
            approvalDetail.CurrentBucket = Convert.ToDecimal(weeklyBucket.GetType().GetProperty("BucketWeek" + fsaDetail.Week.ToString()).GetValue(weeklyBucket, null));
            approvalDetail.NextWeekBucket = Convert.ToDecimal(weeklyBucket.GetType().GetProperty("BucketWeek" + (fsaDetail.Week + 1).ToString()).GetValue(weeklyBucket, null));
            approvalDetail.PCMap = sku.PCMap;
            approvalDetail.DescriptionMap = sku.DescriptionMap;
            approvalDetail.BannerName = banner.BannerName;
            approvalDetail.PlantName = banner.PlantName;
            approvalDetail.PlantCode = banner.PlantCode;
           

            listApprovalDetail.Add(approvalDetail);
            return listApprovalDetail;
        }

        public ProposalHistory CrateProposalHistory(Guid approvalId, Guid proposalId, FSACalendarDetail fsaDetail)
        {
            var propHistory = new ProposalHistory
            {
                Id = Guid.NewGuid(),
                Week = fsaDetail.Week,
                Month = fsaDetail.Month,
                Year = fsaDetail.Year,
                ApprovalId = approvalId,
                ProposalId = proposalId
            };
            return propHistory;
        }


        //public async Task<IActionResult> SaveProposalReallocate(List<ProposalInput> proposals)
        //{
        //    var currDate = DateTime.Now;
        //    var user = await _userManager.GetUserAsync(User);
        //    List<string> errorMessages = new List<string>();
        //    var message = string.Empty;
        //    ValidateProposalReallcoateInput(proposals, errorMessages);
        //    if (!errorMessages.Any())
        //    {
        //        try
        //        {


        //        }
        //        catch(Exception ex)
        //        {

        //        }
        //        List<Proposal> listProposal = new List<Proposal>();
        //        List<Approval> listApproval = new List<Approval>();
        //        var fsaDetail = await _calendarService.GetCalendarDetail(currDate.Date);
        //        var savedProposals = _proposalService.GetPendingProposals(fsaDetail, (Guid)user.UserUnileverId).Where(x => x.Type == ProposalType.Reallocate);
        //        var savedApprovals = _approvalService.GetPendingApprovals();
        //        foreach (var proposalInput in proposals.Where(x => x.reallocate > 0 && !x.isWaitingApproval))
        //        {
        //            if (Guid.Parse(proposalInput.id) == Guid.Empty)
        //            {
        //                var approval = new Approval
        //                {
        //                    Id = Guid.NewGuid(),
        //                    ApprovalStatus = ApprovalStatus.Pending,
        //                    SubmittedAt = DateTime.Now,
        //                    SubmittedBy = (Guid)user.UserUnileverId
        //                };

        //                var proposal = new Proposal
        //                {
        //                    Id = Guid.NewGuid(),
        //                    Week = fsaDetail.Week,
        //                    Year = fsaDetail.Year,
        //                    Month = fsaDetail.Month,
        //                    WeeklyBucketId = Guid.Parse(proposalInput.weeklyBucketId),
        //                    BannerTargetId = Guid.Parse(proposalInput.bannerTargetId),
        //                    Reallocate = proposalInput.reallocate,
        //                    IsWaitingApproval = true,
        //                    ApprovalId = approval.Id,
        //                    Type = ProposalType.Reallocate
        //                };
        //                approval.ProposalId = proposal.Id;
        //                approval.ProposalType = proposal.Type.Value;
        //                listApproval.Add(approval);
        //                listProposal.Add(proposal);
        //            }

        //        }
        //        await _proposalService.SaveProposals(listProposal);
        //        await _approvalService.SaveApprovals(listApproval);
        //        try
        //        {
        //            await _db.SaveChangesAsync();

        //            message = "Your Proposal has been submitted!";
        //            _notyfService.Success(message);
        //            return Ok(proposals);
        //        }
        //        catch (Exception ex)
        //        {
                   
        //            message = "Submit Proposal Failed";
        //            _notyfService.Warning(message);
        //        }
        //    }

        //    else
        //    {
        //        message = "Submit Proposal Failed";
        //        _notyfService.Warning(message);

        //    }
        //    return BadRequest(Json(new { proposals, errorMessages }));
        //}


        public void ValidateProposalInput(List<ProposalInput> proposalInputs, List<string> errorMessages)
        {
            var currDate = DateTime.Now;
          
            foreach (var proposal in proposalInputs.Where(x => (!string.IsNullOrEmpty(x.remark) || x.proposeAdditional > 0 || x.rephase > 0)))
            {
                if(proposal.rephase > 0 && (currDate.DayOfWeek >= DayOfWeek.Thursday && currDate.TimeOfDay.Hours > 12) || currDate.DayOfWeek == DayOfWeek.Sunday)
				{
                    errorMessages.Add("Cannot request rephase after Thursday 12 AM, Please request again on Monday");
				}

                if (proposal.rephase > proposal.nextWeekBucket)
                {
                    errorMessages.Add(string.Format("Cannot request rephase more than next week bucket value on Bucket Name: {0}, PlantName: {1} and PCMap: {2}", proposal.bannerName, proposal.plantName, proposal.pcMap));
                }
            }
        }

     //   public void ValidateProposalReallcoateInput(List<ProposalInput> proposalInputs, List<string> errorMessages)
     //   {
     //       {
     //           foreach (var proposal in proposalInputs.Where(x => x.reallocate > 0))
     //           {
     //               if(string.IsNullOrEmpty(proposal.bannerTargetId))
     //               {
     //                   errorMessages.Add(string.Format("Banner target must be filled on Banner: {0} {1}", proposal.bannerName, proposal.plantName));
     //               }

     //               if(proposal.currentBucket < proposal.reallocate)
					//{
     //                   errorMessages.Add(string.Format("Cannot request reallocation more than current bucket on Banner: {0} {1}", proposal.bannerName, proposal.plantName));
					//}
     //           }
     //       }
     //   }
    }
}


