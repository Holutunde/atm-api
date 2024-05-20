
namespace ATMAPI.Dto
{
    public class ChangePinDto
    {
        public long AccountNumber { get; set; }
        public int OldPin { get; set; }
        public int NewPin { get; set; }
    }
}