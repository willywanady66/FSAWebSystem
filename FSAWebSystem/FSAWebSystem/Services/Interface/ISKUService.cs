using FSAWebSystem.Models;
using FSAWebSystem.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace FSAWebSystem.Services.Interface
{
	public interface ISKUService
	{
		public IQueryable<SKU> GetAllProducts();
		public Task<SKUPagingData> GetSKUPagination(DataTableParam param);
			
		public Task<SKU> GetSKU(string pcMap);

		public IQueryable<ProductCategory> GetAllProductCategories();
		public Task<CategoryPagingData> GetCategoryPagination(DataTableParam param);

		public Task SaveProductCategories(List<ProductCategory> listProductCategory);

		public Task SaveSKUs(List<SKU> listSKUs);
		public Task<bool> IsDuplicate(string pcMap, Guid id);
		public Task FillSKUDropdown(ViewDataDictionary viewData);
		public Task FillCategoryDropdown(ViewDataDictionary viewData);
	}
}
