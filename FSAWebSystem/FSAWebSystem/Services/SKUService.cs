using FSAWebSystem.Models;
using FSAWebSystem.Models.Context;
using FSAWebSystem.Models.ViewModels;
using FSAWebSystem.Services.Interface;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
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

        public async Task<bool> IsDuplicate(string pcMap, Guid id)
        {
            return await _db.SKUs.AnyAsync(x => x.PCMap == pcMap && x.Id != id);
        }

        public async Task<SKU> GetSKUById(Guid id)
        {
            return await _db.SKUs.SingleOrDefaultAsync(x => x.Id == id);
        }

		public async Task<SKU> GetSKU(string pcMap)
        {
			return await _db.SKUs.SingleOrDefaultAsync(x => x.PCMap == pcMap);
        }

		public async Task<SKUPagingData> GetSKUPagination(DataTableParam param)
        {
            //var skus = _db.SKUs.Include(x => x.ProductCategory).AsQueryable();
            var skus = (from sku in _db.SKUs.Include(x => x.ProductCategory)
                        select new SKU
                        {
                            Id= sku.Id,
                            PCMap = sku.PCMap,
                            DescriptionMap = sku.DescriptionMap,
                            Category = sku.ProductCategory.CategoryProduct,
                            Status = sku.IsActive ? "Active" : "Non-Active"
                        });
			if (!string.IsNullOrEmpty(param.search.value))
			{
				var search = param.search.value.ToLower();
				skus = skus.Where(x => x.PCMap.ToLower().Contains(search) || x.DescriptionMap.ToLower().Contains(search) || x.Category.ToLower().Contains(search) || x.Status.ToLower().Contains(search));
			}

            if (param.order.Any())
            {
                var order = param.order[0];
                switch (order.column)
                {
                    case 0:
                        skus = order.dir == "desc" ? skus.OrderBy(x => x.Status).ThenBy(x => x.PCMap) : skus.OrderBy(x => x.Status).ThenByDescending(x => x.PCMap);
                        break;
                    case 1:
                        skus = order.dir == "desc" ? skus.OrderByDescending(x => x.DescriptionMap) : skus.OrderBy(x => x.DescriptionMap);
                        break;
                    case 2:
                        skus = order.dir == "desc" ? skus.OrderByDescending(x => x.Category) : skus.OrderBy(x => x.Category);
                        break;
                    case 3:
                        skus = order.dir == "desc" ? skus.OrderByDescending(x => x.Status) : skus.OrderBy(x => x.Status);
                        break;
                }
            }

            var totalCount = skus.Count();
            var listSKU = await skus.Skip(param.start).Take(param.length).ToListAsync();
            return new SKUPagingData
            {
                totalRecord = totalCount,
                skus = listSKU
            };
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

        public async Task<CategoryPagingData> GetCategoryPagination(DataTableParam param)
        {
            var categories = _db.ProductCategories.AsQueryable();
            if (!string.IsNullOrEmpty(param.search.value))
            {
                var search = param.search.value.ToLower();
                categories = categories.Where(x => x.CategoryProduct.ToLower().Contains(search));
            }

            if (param.order.Any())
            {
                var order = param.order[0];
                switch (order.column)
                {
                    case 1:
                        categories = order.dir == "desc" ? categories.OrderByDescending(x => x.CategoryProduct) : categories.OrderBy(x => x.CategoryProduct);
                        break;
                }
            }

            var totalCount = categories.Count();
            var listCategory = await categories.Skip(param.start).Take(param.length).ToListAsync();
            return new CategoryPagingData
            {
                totalRecord = totalCount,
                categories = listCategory
            };
        }

        public async Task FillSKUDropdown(ViewDataDictionary viewData)
        {
            var skus = GetAllProducts().Where(x => x.IsActive).ToList();
            List<SelectListItem> listSku = new List<SelectListItem>();
            listSku = skus.Select(x => new SelectListItem { Text = x.PCMap + " (" + x.DescriptionMap + " )", Value = x.Id.ToString() }).ToList();
            viewData["ListSku"] = listSku;
        }

        public async Task FillCategoryDropdown(ViewDataDictionary viewData)
        {
            var categories = GetAllProductCategories().ToList();
            List<SelectListItem> listCategory = new List<SelectListItem>();
            listCategory = categories.Select(x => new SelectListItem { Text = x.CategoryProduct, Value = x.Id.ToString() }).ToList();
            viewData["ListCategory"] = listCategory;
        }
    }
}
	 