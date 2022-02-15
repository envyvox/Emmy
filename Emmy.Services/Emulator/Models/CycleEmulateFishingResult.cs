using System.Collections.Generic;

namespace Emmy.Services.Emulator.Models
{
    public class CycleEmulateFishingResult
    {
        public double AverageTotalSuccess { get; set; }
        public double AverageTotalFail { get; set; }
        public double AverageFinalCurrency { get; set; }
        public List<EmulateFishingResult> Results { get; set; }
    }
}