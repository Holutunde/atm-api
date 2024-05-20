namespace ATMAPI.Dto
{
    public class TransferDto
    {
        public long SourceAccountNumber { get; set; }
        public long TargetAccountNumber { get; set; }
        public double Amount { get; set; }
    }
}