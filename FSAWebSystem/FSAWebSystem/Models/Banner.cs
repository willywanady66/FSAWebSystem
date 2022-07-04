using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FSAWebSystem.Models
{
    public class Banner
    {
         public Guid Id { get; set; }


        [Required]
        [Display(Name = "Trade")]
        public string Trade{ get; set; }
        
        [Required]
        [Display(Name = "Banner Name")]
        public string BannerName { get; set; }

        [Required]
        [Display(Name = "Plant Name")]
        public string PlantName{ get; set; }

        [Required]
        [Display(Name = "Plant Code")]
        public string PlantCode{ get; set; }       
        public bool IsActive { get; set; }       
        public DateTime? CreatedAt{ get; set; }
        public string? CreatedBy{ get; set; }
        public DateTime? ModifiedAt{ get; set; }
        public string? ModifiedBy{ get; set; }

        public List<UserUnilever>? UserUnilevers { get; set; }
        
    }
}
