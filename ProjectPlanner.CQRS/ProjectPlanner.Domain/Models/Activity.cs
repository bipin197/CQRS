namespace ProjectPlanner.Domain.Models
{
    public class Activity
    {
        public string ProjectId { get; set; } = string.Empty;
        public string ActivityId { get; set; } = string.Empty;
        public string ActivityStatus { get; set; } = string.Empty;
        public string WbsCode { get; set; } = string.Empty;
        public string ActivityName { get; set; } = string.Empty;
        public string CodeLifeCycle { get; set; } = string.Empty;
        public string CodeTrade { get; set; } = string.Empty;
        public decimal Performance { get; set; }
        public decimal Schedule { get; set; }
        public decimal BudgetedValue { get; set; }
        public decimal EarnedValue { get; set; }
        public decimal PlannedValue { get; set; }
        public int OriginalDuration { get; set; }
        public double PlannedStart { get; set; }
        public double PlannedFinish { get; set; }
        public double? ActualStart { get; set; }
        public double? ActualFinish { get; set; }
        public double Start { get; set; }
        public double Finish { get; set; }
    }
}