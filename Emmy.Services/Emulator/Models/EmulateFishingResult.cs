namespace Emmy.Services.Emulator.Models
{
    public class EmulateFishingResult
    {
        public long TotalSuccessCount { get; set; }
        public long TotalFailCount { get; set; }
        public long CommonFishSuccess { get; set; }
        public long CommonFishFail { get; set; }
        public long RareFishSuccess { get; set; }
        public long RareFishFail { get; set; }
        public long EpicFishSuccess { get; set; }
        public long EpicFishFail { get; set; }
        public long MythicalFishSuccess { get; set; }
        public long MythicalFishFail { get; set; }
        public long LegendaryFishSuccess { get; set; }
        public long LegendaryFishFail { get; set; }
        public long FinalCurrency { get; set; }
    }
}