namespace Emmy.Services.Emulator.Models
{
    public class EmulateCasinoResult
    {
        public long BetCount { get; set; }
        public long BetAmount { get; set; }
        public long Lose { get; set; }
        public long WinX2 { get; set; }
        public long WinX4 { get; set; }
        public long WinX10 { get; set; }
        public long FinalCurrency { get; set; }
    }
}