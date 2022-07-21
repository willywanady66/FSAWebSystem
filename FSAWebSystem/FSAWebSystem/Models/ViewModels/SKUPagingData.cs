namespace FSAWebSystem.Models.ViewModels
{
    public class SKUPagingData : PagingData
    {
        public SKUPagingData()
        {
            skus = new List<SKU>();
        }

        public List<SKU> skus { get; set; }
    }
}
