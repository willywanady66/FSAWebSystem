using FSAWebSystem.Models;
using FSAWebSystem.Models.Context;
using FSAWebSystem.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace FSAWebSystem.Services
{
	public class SKUService : ISKUService
	{
		private FSAWebSystemDbContext _db;

		public SKUService(FSAWebSystemDbContext db)
		{
			_db = db;
		}


		public IQueryable<SKU> GetAllProducts()
		{
			return _db.SKUs;
		}

		public async Task<SKU> GetSKU(string pcMap)
        {
			return await _db.SKUs.SingleOrDefaultAsync(x => x.PCMap == pcMap);
        }


		public  IQueryable<ProductCategory> GetAllProductCategories()
		{
			return  _db.ProductCategories;
		}

        public async Task SaveProductCategories(List<ProductCategory> listProducCategory)
        {
			await _db.AddRangeAsync(listProducCategory);
        }

        public async Task SaveSKUs(List<SKU> listSKUs)
        {
			await _db.AddRangeAsync(listSKUs);
        }

       
    }
}
	 