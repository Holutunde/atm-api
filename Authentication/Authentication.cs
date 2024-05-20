using ATMAPI.Models;


namespace ATMAPI.Authentication {

}
public class Authentication
{
    private List<Account> accounts;
    private bool AuthenticateUserAccount = true;
    public long InputtedAccountNumber { get; set; }

    public Authentication(List<Account> accounts)
    {
        this.accounts = accounts;
    }


    private bool AuthenticateAccount(long inputAccountNumber, int inputPin)
    {
        var account = accounts.Find(acc => acc.AccountNumber == inputAccountNumber && acc.Pin == inputPin);
        return account != null;
    }

    public Account? StartAuthentication()
    {
        while (AuthenticateUserAccount)
        {
            Console.Write("Enter your 10 digit account number: ");
            if (!long.TryParse(Console.ReadLine(), out long inputAccountNumber))
            {
                Messages.EnterValidAccountNumber();
                continue;
            }

            if (inputAccountNumber.ToString().Length != 10)
            {
                Console.WriteLine("Account number must be 10 digits");
                continue;
            }


            var account = accounts.Find(acc => acc.AccountNumber == inputAccountNumber);
            if (account == null)
            {
                 Messages.AccountNotFound();
                continue;
            }

            InputtedAccountNumber = inputAccountNumber;

            for (int attempts = 0; attempts < 4; attempts++)
            {
                Console.Write("Enter your 4 digit pin: ");
                if (!int.TryParse(Console.ReadLine(), out int inputPin))
                {
                    Console.WriteLine("Invalid input format. Please enter a numeric pin.");
                    continue;
                }

                if (AuthenticateAccount(inputAccountNumber, inputPin))
                {
                    Console.WriteLine("Correct pin entered.");
                    return account;
                }

                Console.WriteLine("Incorrect pin!!! Confirm and enter correct pin.");
            }

            Console.WriteLine("Max attempts reached. Account suspended, visit the bank for further inquiries.");
            AuthenticateUserAccount = false;
        }

        return null;
    }

}
