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

namespace FSAWebSystem.Controllers
{
    public class BannersController : Controller
    {
        private readonly FSAWebSystemDbContext _context;
        private readonly IBannerService _bannerService;

        public BannersController(FSAWebSystemDbContext context, IBannerService bannerService)
        {
            _context = context;
            _bannerService = bannerService;
         }


        // GET: Banners/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.Banners == null)
            {
                return NotFound();
            }

            var banner = await _context.Banners
                .FirstOrDefaultAsync(m => m.Id == id);
            if (banner == null)
            {
                return NotFound();
            }

            return View(banner);
        }

        // GET: Banners/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Banners/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Trade,BannerName,PlantName,PlantCode")] Banner banner)
        {
            if (ModelState.IsValid)
            {
                var isBannerExist = await _bannerService.IsBannerExist(banner.BannerName);
                if(isBannerExist)
                {
                    ModelState.AddModelError("", "Banner " + banner.BannerName + " already exist!");
                }
                else
                {
                    var savedBanner = await _bannerService.SaveBanner(banner, User.Identity.Name);
                    return RedirectToAction("Index", "Admin");
                }   
            }
            return View(banner);
            
        }

        // GET: Banners/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.Banners == null)
            {
                return NotFound();
            }

            var banner = await _context.Banners.FindAsync(id);
            if (banner == null)
            {
                return NotFound();
            }
            return View(banner);
        }

        // POST: Banners/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Trade,BannerName,PlantName,PlantCode,IsActive")] Banner banner)
        {
            if (id != banner.Id)
            {
                return NotFound();
            }
       
            if (ModelState.IsValid)
            {   
                try
                {
                    var savedBanner = await _bannerService.GetBanner(banner.Id);
                    if (await _bannerService.IsBannerUsed(banner.BannerName))
                    {
                        ModelState.AddModelError("", "Cannot Edit, Banner is selected on User");
                        return View(banner);
                    }
                    
                    savedBanner.Trade = banner.Trade;
                    savedBanner.BannerName = banner.BannerName;
                    savedBanner.PlantName = banner.PlantName;
                    savedBanner.PlantCode = banner.PlantCode;
                    await _bannerService.UpdateBanner(savedBanner, User.Identity.Name);
                    return RedirectToAction("Index", "Admin");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BannerExists(banner.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                    return View(banner);
                }
            }
            return View(banner);
        }

        // GET: Banners/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            await ValidateDeleteBanner((Guid)id);
            return RedirectToAction("Index", "Admin");
        }

        // POST: Banners/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.Banners == null)
            {
                return Problem("Entity set 'FSAWebSystemDbContext.Banners'  is null.");
            }
            var banner = await _context.Banners.FindAsync(id);
            if (banner != null)
            {
                _context.Banners.Remove(banner);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BannerExists(Guid id)
        {
          return (_context.Banners?.Any(e => e.Id == id)).GetValueOrDefault();
        }


        public async Task ValidateDeleteBanner(Guid bannerId)
        {
            var banner = await _bannerService.GetBanner(bannerId);


        }
    }
}
