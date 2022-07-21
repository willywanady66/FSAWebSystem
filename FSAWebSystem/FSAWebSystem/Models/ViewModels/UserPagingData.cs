namespace FSAWebSystem.Models.ViewModels
{
    public class UserPagingData : PagingData
    {
        public UserPagingData()
        {
            userUnilevers = new List<UserUnilever>();
        }

        public List<UserUnilever> userUnilevers { get; set; }
    }
}
