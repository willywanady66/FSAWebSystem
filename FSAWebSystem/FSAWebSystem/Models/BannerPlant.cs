﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FSAWebSystem.Models
{
    public class BannerPlant
    {
        public Guid Id { get; set; }


        //[Required]
        //[Display(Name = "Trade")]
        //public string Trade { get; set; }

        [Required]
        [Display(Name = "CDM")]
        public string CDM { get; set; }

        [Required]
        [Display(Name = "KAM")]
        public string KAM { get; set; }

        public Banner Banner { get; set; }
        //[Required]
        //[Display(Name = "Banner Name")]
        //public string BannerName { get; set; }

        //[Required]
        //[Display(Name = "Plant Name")]
        //public string PlantName{ get; set; }

        //[Required]
        //[Display(Name = "Plant Code")]
        //public string PlantCode{ get; set; }       
        public bool IsActive { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public string? ModifiedBy { get; set; }

        public List<UserUnilever>? UserUnilevers { get; set; }

        public Plant Plant { get; set; }

        public Guid FSADocumentId { get; set; }

        //[NotMapped]
        //public string BannerName { get; set; }
        [NotMapped]
        public string PlantName { get; set; }
        [NotMapped]
        public string PlantCode { get; set; }
        [NotMapped]
        public string BannerName { get; set; }
        [NotMapped]
        public string Trade { get; set; }
    }
}