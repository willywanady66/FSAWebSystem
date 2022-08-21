using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FSAWebSystem.Models
{
    public class SKU
    {
      
        public Guid Id { get; set; }

        [Required]
        [Display(Name = "PC Map")]
        public string PCMap { get; set; }

        [Required]
        [Display(Name = "Description Map")]
        public string DescriptionMap{ get; set; }

        [Required]
        [Display(Name = "Product Category")]
        public ProductCategory ProductCategory { get; set; }
        
        public bool IsActive{ get; set; }

        public DateTime? CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public string? ModifiedBy{ get; set; }

        public Guid FSADocumentId { get; set; }
        public List<UserUnilever>? UserUnilevers { get; set; }
        //[NotMapped] public ProductCategory ProductCategory { get; set; }
        [NotMapped]
        public string Category { get; set; }
        [NotMapped]
        public string Status { get; set; }
    }
}
