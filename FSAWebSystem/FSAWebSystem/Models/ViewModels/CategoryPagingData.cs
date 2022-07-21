namespace FSAWebSystem.Models.ViewModels
{
    public class CategoryPagingData : PagingData
    {
        public CategoryPagingData()
        {
            categories = new List<ProductCategory>();
        }

        public List<ProductCategory>  categories { get; set; }
    }
}
