using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FSAWebSystem.Models;
using FSAWebSystem.Models.Context;

namespace FSAWebSystem.Controllers
{
    public class ApprovalDetailsController : Controller
    {
        private readonly FSAWebSystemDbContext _context;

        public ApprovalDetailsController(FSAWebSystemDbContext context)
        {
            _context = context;
        }

        // GET: ApprovalDetails
        public async Task<IActionResult> Index()
        {
              return _context.ApprovalDetail != null ? 
                          View(await _context.ApprovalDetail.ToListAsync()) :
                          Problem("Entity set 'FSAWebSystemDbContext.ApprovalDetail'  is null.");
        }

        // GET: ApprovalDetails/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.ApprovalDetail == null)
            {
                return NotFound();
            }

            var approvalDetail = await _context.ApprovalDetail
                .FirstOrDefaultAsync(m => m.Id == id);
            if (approvalDetail == null)
            {
                return NotFound();
            }

            return View(approvalDetail);
        }

        // GET: ApprovalDetails/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ApprovalDetails/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ApprovalId,BannerId,SKUId,MonthlyBucket,CurrentBucket,NextWeekBucket,ValidBJ,RemFSA")] ApprovalDetail approvalDetail)
        {
            if (ModelState.IsValid)
            {
                approvalDetail.Id = Guid.NewGuid();
                _context.Add(approvalDetail);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(approvalDetail);
        }

        // GET: ApprovalDetails/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.ApprovalDetail == null)
            {
                return NotFound();
            }

            var approvalDetail = await _context.ApprovalDetail.FindAsync(id);
            if (approvalDetail == null)
            {
                return NotFound();
            }
            return View(approvalDetail);
        }

        // POST: ApprovalDetails/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,ApprovalId,BannerId,SKUId,MonthlyBucket,CurrentBucket,NextWeekBucket,ValidBJ,RemFSA")] ApprovalDetail approvalDetail)
        {
            if (id != approvalDetail.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(approvalDetail);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ApprovalDetailExists(approvalDetail.Id))
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
            return View(approvalDetail);
        }

        // GET: ApprovalDetails/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.ApprovalDetail == null)
            {
                return NotFound();
            }

            var approvalDetail = await _context.ApprovalDetail
                .FirstOrDefaultAsync(m => m.Id == id);
            if (approvalDetail == null)
            {
                return NotFound();
            }

            return View(approvalDetail);
        }

        // POST: ApprovalDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.ApprovalDetail == null)
            {
                return Problem("Entity set 'FSAWebSystemDbContext.ApprovalDetail'  is null.");
            }
            var approvalDetail = await _context.ApprovalDetail.FindAsync(id);
            if (approvalDetail != null)
            {
                _context.ApprovalDetail.Remove(approvalDetail);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ApprovalDetailExists(Guid id)
        {
          return (_context.ApprovalDetail?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
