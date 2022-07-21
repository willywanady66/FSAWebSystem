namespace FSAWebSystem.Models.ViewModels
{
    public class PagingData
    {
        public int draw { get; set; }
        public int totalRecord { get; set; }
        public int pageSize { get; set; }
        public int pageNo { get; set; }
    }
}
