public class Messages
{
    public static void CheckBalanceMessage(double balance)
    {
        Console.WriteLine($"Your current balance is: {balance}");
    }

    public static void EnterPostiveAmount()
    {
        Console.WriteLine("Invalid amount. Please enter a positive amount: ");
    }

    public static void EnterValidPostivePin()
    {
        Console.WriteLine("Invalid input. Please enter a valid 4 digit PIN: ");
    }

    public static void EnterValidAccountNumber()
    {
        Console.WriteLine("Invalid input. Please enter a numeric digits. ");
       
    }

    public static void InvalidChoice()
    {
        Console.WriteLine("Invalid choice. Please enter a number from 1 to 6.");
    }

    public static void InsufficientBalance()
    {
        Console.WriteLine("Insufficient balance");
    }

    public static void TransferSuccessful(double? amount, string? senderName, string? receiverName, double senderBalance)
    {
        Console.WriteLine($"{amount} naira transferred successfully from {senderName}'s User to {receiverName}'s User.");
        Console.WriteLine($"Your new balance is: {senderBalance}");
    }
   public static void AccoutCreatedSuccessfully(long AccountNumber){

        Console.WriteLine("User created successfully!");
        Console.WriteLine($"Your User number is: {AccountNumber}");
    }
    public static void WithdrawSuccessful(double amount, double userBalance)
    {
        Console.WriteLine($"{amount} withdrawn successfully. Your new balance is: {userBalance}");
    }
    public static void DepositSuccessful(double amount, double userBalance)
    {
        Console.WriteLine($"{amount} naira deposited successfully. Your new balance is: {userBalance}");
    }
    public static void UpdatedPinSuccessful(string? userName)
    {
        Console.WriteLine($"Pin updated successfully for User {userName}.");
    }

    public static void UserNotFound()
    {
        Console.WriteLine("User not found.");
    }
}
