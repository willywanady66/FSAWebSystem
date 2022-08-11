using System.ComponentModel.DataAnnotations.Schema;

namespace FSAWebSystem.Models
{
    public class ApprovalDetail
    {
        public Guid Id { get; set; }
        public Guid ApprovalId { get; set; }
        public Guid BannerId { get; set; }
        public Guid SKUId { get; set; }
        public decimal RatingRate { get; set; }
        public decimal MonthlyBucket { get; set; }
        public decimal CurrentBucket { get; set; }
        public decimal NextWeekBucket { get; set; }
        public decimal ValidBJ{ get; set; }
        public decimal RemFSA{ get; set; }
        public decimal ProposeAdditional { get; set; }



        [NotMapped]
        public string BannerName { get; set; }
        [NotMapped]
        public string PlantCode { get; set; }
        [NotMapped]
        public string PlantName { get; set; }
        [NotMapped]
        public string PCMap { get; set; }
        [NotMapped]
        public string DescriptionMap { get; set; }
    }
}
