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
				skus = skus.Where(x => x.PCMap.ToLower().Contains(search.ToLower()) || x.DescriptionMap.ToLower().Contains(search.ToLower()) || x.Category.ToLower().Contains(search.ToLower()) || x.Status.ToLower().Contains(search.ToLower()));
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
                categories = categories.Where(x => x.CategoryProduct.ToLower().Contains(search.ToLower()));
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

        public async Task<AndromedaPagingData> GetAndromedaPagination(DataTableParam param)
        {
            //var skus = _db.SKUs.Include(x => x.ProductCategory).AsQueryable();
            var andromedas = _db.Andromedas.AsQueryable();
            if (!string.IsNullOrEmpty(param.search.value))
            {
                var search = param.search.value.ToLower();
                andromedas = andromedas.Where(x => x.PCMap.ToLower().Contains(search.ToLower()) || x.Description.ToLower().Contains(search.ToLower()));
            }

            if (param.order.Any())
            {
                var order = param.order[0];
                switch (order.column)
                {
                    case 0:
                        andromedas = order.dir == "desc" ? andromedas.OrderBy(x => x.PCMap) : andromedas.OrderBy(x => x.PCMap);
                        break;
                    case 1:
                        andromedas = order.dir == "desc" ? andromedas.OrderByDescending(x => x.Description) : andromedas.OrderBy(x => x.Description);
                        break;
                    case 2:
                        andromedas = order.dir == "desc" ? andromedas.OrderByDescending(x => x.UUStock) : andromedas.OrderBy(x => x.UUStock);
                        break;
                    case 3:
                        andromedas = order.dir == "desc" ? andromedas.OrderByDescending(x => x.ITThisWeek) : andromedas.OrderBy(x => x.ITThisWeek);
                        break;
                    case 4:
                        andromedas = order.dir == "desc" ? andromedas.OrderByDescending(x => x.WeekCover) : andromedas.OrderBy(x => x.WeekCover);
                        break;
                }
            }

            var totalCount = andromedas.Count();
            var listAndromeda = await andromedas.Skip(param.start).Take(param.length).ToListAsync();
            return new AndromedaPagingData
            {
                totalRecord = totalCount,
                andromedas = listAndromeda
            };
        }

        public async Task<BottomPricePagingData> GetBottomPricePagination(DataTableParam param)
        {
            var bottomPrices = _db.BottomPrices.AsQueryable();
            if (!string.IsNullOrEmpty(param.search.value))
            {
                var search = param.search.value.ToLower();
                bottomPrices = bottomPrices.Where(x => x.PCMap.ToLower().Contains(search.ToLower()) || x.Description.ToLower().Contains(search.ToLower()) || x.Remarks.ToLower().Contains(search.ToLower()));
            }

            if (param.order.Any())
            {
                var order = param.order[0];
                switch (order.column)
                {
                    case 0:
                        bottomPrices = order.dir == "desc" ? bottomPrices.OrderBy(x => x.PCMap) : bottomPrices.OrderBy(x => x.PCMap);
                        break;
                    case 1:
                        bottomPrices = order.dir == "desc" ? bottomPrices.OrderByDescending(x => x.Description) : bottomPrices.OrderBy(x => x.Description);
                        break;
                    case 2:
                        bottomPrices = order.dir == "desc" ? bottomPrices.OrderByDescending(x => x.AvgNormalPrice) : bottomPrices.OrderBy(x => x.AvgNormalPrice);
                        break;
                    case 3:
                        bottomPrices = order.dir == "desc" ? bottomPrices.OrderByDescending(x => x.AvgBottomPrice) : bottomPrices.OrderBy(x => x.AvgBottomPrice);
                        break;
                    case 4:
                        bottomPrices = order.dir == "desc" ? bottomPrices.OrderByDescending(x => x.AvgActualPrice) : bottomPrices.OrderBy(x => x.AvgActualPrice);
                        break;
                    case 5:
                        bottomPrices = order.dir == "desc" ? bottomPrices.OrderByDescending(x => x.MinActualPrice) : bottomPrices.OrderBy(x => x.MinActualPrice);
                        break;
                    case 6:
                        bottomPrices = order.dir == "desc" ? bottomPrices.OrderByDescending(x => x.Gap) : bottomPrices.OrderBy(x => x.Gap);
                        break;
                    case 7:
                        bottomPrices = order.dir == "desc" ? bottomPrices.OrderByDescending(x => x.Remarks) : bottomPrices.OrderBy(x => x.Remarks);
                        break;
                }
            }

            var totalCount = bottomPrices.Count();
            var listBottomPrice = await bottomPrices.Skip(param.start).Take(param.length).ToListAsync();
            return new BottomPricePagingData
            {
                totalRecord = totalCount,
                bottomPrices = listBottomPrice
            };
        }

        public async Task<ITrustPagingData> GetITrustPagination(DataTableParam param)
        {
            var iTrusts = _db.ITrusts.AsQueryable();
            if (!string.IsNullOrEmpty(param.search.value))
            {
                var search = param.search.value.ToLower();
                iTrusts = iTrusts.Where(x => x.PCMap.ToLower().Contains(search.ToLower()) || x.Description.ToLower().Contains(search.ToLower()));
            }

            if (param.order.Any())
            {
                var order = param.order[0];
                switch (order.column)
                {
                    case 0:
                        iTrusts = order.dir == "desc" ? iTrusts.OrderBy(x => x.PCMap) : iTrusts.OrderBy(x => x.PCMap);
                        break;
                    case 1:
                        iTrusts = order.dir == "desc" ? iTrusts.OrderByDescending(x => x.Description) : iTrusts.OrderBy(x => x.Description);
                        break;
                    case 2:
                        iTrusts = order.dir == "desc" ? iTrusts.OrderByDescending(x => x.SumIntransit) : iTrusts.OrderBy(x => x.SumIntransit);
                        break;
                    case 3:
                        iTrusts = order.dir == "desc" ? iTrusts.OrderByDescending(x => x.SumStock) : iTrusts.OrderBy(x => x.SumStock);
                        break;
                    case 4:
                        iTrusts = order.dir == "desc" ? iTrusts.OrderByDescending(x => x.SumFinalRpp) : iTrusts.OrderBy(x => x.SumFinalRpp);
                        break;
                    case 5:
                        iTrusts = order.dir == "desc" ? iTrusts.OrderByDescending(x => x.DistStock) : iTrusts.OrderBy(x => x.DistStock);
                        break;
                }
            }

            var totalCount = iTrusts.Count();
            var listITrust = await iTrusts.Skip(param.start).Take(param.length).ToListAsync();
            return new ITrustPagingData
            {
                totalRecord = totalCount,
                iTrusts = listITrust
            };
        }

    }
}
	 