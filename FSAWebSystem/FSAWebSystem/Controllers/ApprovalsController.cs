 using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FSAWebSystem.Models;
using FSAWebSystem.Models.Context;
using FSAWebSystem.Models.ViewModels;
using FSAWebSystem.Services.Interface;

namespace FSAWebSystem.Controllers
{
    public class ApprovalsController : Controller
    {
        private readonly FSAWebSystemDbContext _context;
        private readonly IApprovalService _approvalService;


        public ApprovalsController(FSAWebSystemDbContext context, IApprovalService approvalService)
        {
            _context = context;
            _approvalService = approvalService;
        }

        // GET: Approvals
        public async Task<IActionResult> Index()
        {
            return View();
        }

        // GET: Approvals/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.Approvals == null)
            {
                return NotFound();
            }

            var approval = await _context.Approvals
                .FirstOrDefaultAsync(m => m.Id == id);
            if (approval == null)
            {
                return NotFound();
            }

            return View(approval);
        }

        [HttpPost]
        public async Task<IActionResult> GetApprovalPagination(DataTableParam param)
        {
            var listData = Json(new { });
            try
            {
               
                var currentDate = DateTime.Now;
                var data = await _approvalService.GetApprovalPagination(param, currentDate.Month, currentDate.Year);
                listData = Json(new
                {
                    draw = param.draw,
                    recordsTotal = data.totalRecord,
                    recordsFiltered = data.totalRecord,
                    data = data.approvals
                });
            }
            catch (Exception ex)
            {

            }
            return listData;
        }


        // GET: Approvals/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.Approvals == null)
            {
                return NotFound();
            }

            var approval = await _context.Approvals.FindAsync(id);
            if (approval == null)
            {
                return NotFound();
            }
            return View(approval);
        }

        // POST: Approvals/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,ProposalId,SubmittedBy,SubmittedAt,ApprovalStatus,RejectionReason,ApprovedBy,ApprovedAt")] Approval approval)
        {
            if (id != approval.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(approval);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ApprovalExists(approval.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(approval);
        }

        // GET: Approvals/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.Approvals == null)
            {
                return NotFound();
            }

            var approval = await _context.Approvals
                .FirstOrDefaultAsync(m => m.Id == id);
            if (approval == null)
            {
                return NotFound();
            }

            return View(approval);
        }

        // POST: Approvals/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.Approvals == null)
            {
                return Problem("Entity set 'FSAWebSystemDbContext.Approvals'  is null.");
            }
            var approval = await _context.Approvals.FindAsync(id);
            if (approval != null)
            {
                _context.Approvals.Remove(approval);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ApprovalExists(Guid id)
        {
          return (_context.Approvals?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
