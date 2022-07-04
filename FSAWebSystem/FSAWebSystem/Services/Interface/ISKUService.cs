using FSAWebSystem.Models;

namespace FSAWebSystem.Services.Interface
{
	public interface ISKUService
	{
		public IQueryable<SKU> GetAllProducts();


		public IQueryable<ProductCategory> GetAllProductCategories();

		public Task SaveProductCategories(List<ProductCategory> listProductCategory);

		public Task SaveSKUs(List<SKU> listSKUs);
	}
}
