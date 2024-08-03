namespace Application.User;  

public static class UserGenerateAccountNumber  
{  
    private static readonly Random _random = new Random();  

    public static long GenerateAccountNumber()  
    {  
        return (long)(_random.NextDouble() * 9000000000L) + 1000000000L;  
    }  
}