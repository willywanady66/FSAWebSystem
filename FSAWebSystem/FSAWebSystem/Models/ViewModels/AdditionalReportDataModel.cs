using System.Composition.Convention;

namespace FSAWebSystem.Models.ViewModels
{
    public class AdditionalReportDataModel
    {

        public int noOfRephase { get; set; }
        public int noOfReallocate { get; set; }
        public int noOfPropose { get; set; }
        public IEnumerable<DailyRecordModel> DailyRecord { get; set; }

    }

    public class DailyRecordModel
    {
        public Guid WeeklyBucketId { get; set; }
        public decimal Rephase { get; set; }
        public decimal ProposeAdditional { get; set; }
        public ProposalType Type { get; set; }
        public DateTime SubmitDate { get; set; }
        public int Week { get; set; }
    }
}
