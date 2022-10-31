﻿using System.ComponentModel.DataAnnotations.Schema;

namespace FSAWebSystem.Models
{
    public class ProposalDetail
    {

        public Guid Id { get; set; }
        public BannerPlant? BannerPlant { get; set; }
        public Guid WeeklyBucketId { get; set; }
        public decimal ActualRephase { get; set; }
        public decimal ActualProposeAdditional { get; set; }
        public decimal PlantContribution { get; set; }
        public Proposal Proposal { get; set; }
        public bool IsTarget { get; set; }

        [NotMapped]
        public string BannerName { get; set; }
        [NotMapped]
        public string CDM { get; set; }
        [NotMapped]
        public string KAM { get; set; }
        [NotMapped]
        public string PlantCode { get; set; }
        [NotMapped]
        public string PlantName { get; set; }
        [NotMapped]
        public string PCMap { get; set; }
        [NotMapped]
        public string DescriptionMap { get; set; }

        [NotMapped]
        public decimal RatingRate { get; set; }
        [NotMapped]
        public decimal MonthlyBucket { get; set; }
        [NotMapped]
        public decimal CurrentBucket { get; set; }
        [NotMapped]
        public decimal NextBucket { get; set; }
        [NotMapped]
        public decimal ValidBJ { get; set; }
        [NotMapped]
        public decimal RemFSA { get; set; }
    }
}
