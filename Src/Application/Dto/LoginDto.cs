namespace Application.Dto
{
    public class AtmLoginDto
    {
        public long AccountNumber { get; set; }
        public int Pin { get; set; }

    }


    public class OnlineLoginDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}