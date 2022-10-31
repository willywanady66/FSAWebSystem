using Microsoft.EntityFrameworkCore;

namespace FSAWebSystem.Models
{
    [Index(nameof(BannerName))]
    public class Banner
    {
        public Guid Id { get; set; }
        public string Trade { get; set; }
        public string BannerName { get; set; }
    }
}
