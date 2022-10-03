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
    public class BannerPlantsController : Controller
    {
        private readonly FSAWebSystemDbContext _context;
        private readonly IBannerPlantService _bannerPlantService;

        public BannerPlantsController(FSAWebSystemDbContext context, IBannerPlantService bannerService)
        {
            _context = context;
            _bannerPlantService = bannerService;
         }


        // GET: Banners/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.BannerPlants == null)
            {
                return NotFound();
            }

            var banner = await _context.BannerPlants
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
        public async Task<IActionResult> Create([Bind("Id,Trade,BannerName,PlantName,PlantCode")] BannerPlant bannerPlant)
        {
            if (ModelState.IsValid)
            {
                var IsBannerPlantExist = await _bannerPlantService.IsBannerPlantExist(bannerPlant.Banner.BannerName);
                if(IsBannerPlantExist)
                {
                    ModelState.AddModelError("", "Banner Plant" + bannerPlant.Banner.BannerName + " already exist!");
                }
                else
                {
                    var savedBanner = await _bannerPlantService.SaveBannerPlant(bannerPlant, User.Identity.Name);
                    return RedirectToAction("Index", "Admin");
                }   
            }
            return View(bannerPlant);
            
        }

        // GET: Banners/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.BannerPlants == null)
            {
                return NotFound();
            }

            var banner = await _context.BannerPlants.FindAsync(id);
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
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Trade, CDM, KAM, BannerName,PlantName,PlantCode,IsActive")] BannerPlant bannerPlant)
        {
            if (id != bannerPlant.Id)
            {
                return NotFound();
            }
       
            if (ModelState.IsValid)
            {   
                try
                {
                    var savedBanner = await _bannerPlantService.GetBannerPlant(bannerPlant.Id);
                    if (await _bannerPlantService.IsBannerPlantUsed(bannerPlant.Banner.BannerName, bannerPlant.Plant.Id))
                    {
                        ModelState.AddModelError("", "Cannot Edit, Banner is registered on User/ Bucket");
                        return View(bannerPlant);
                    }
                    
                    savedBanner.Trade = bannerPlant.Trade;
                    savedBanner.Banner = bannerPlant.Banner;
                    //savedBanner.PlantName = banner.PlantName;
                    //savedBanner.PlantCode = banner.PlantCode;
                    savedBanner.Plant = bannerPlant.Plant;
                    savedBanner.KAM = bannerPlant.KAM;
                    savedBanner.CDM = bannerPlant.CDM;
                    savedBanner.IsActive = bannerPlant.IsActive;
                    await _bannerPlantService.UpdateBannerPlant(savedBanner, User.Identity.Name);
                    return RedirectToAction("Index", "Admin");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BannerExists(bannerPlant.Id))
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
                    return View(bannerPlant);
                }
            }
            return View(bannerPlant);
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
            if (_context.BannerPlants == null)
            {
                return Problem("Entity set 'FSAWebSystemDbContext.Banners'  is null.");
            }
            var banner = await _context.BannerPlants.FindAsync(id);
            if (banner != null)
            {
                _context.BannerPlants.Remove(banner);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BannerExists(Guid id)
        {
          return (_context.BannerPlants?.Any(e => e.Id == id)).GetValueOrDefault();
        }


        public async Task ValidateDeleteBanner(Guid bannerId)
        {
            var banner = await _bannerPlantService.GetBannerPlant(bannerId);


        }
    }
}
