using ATMAPI.Models;


namespace ATMAPI.Authentication {

}
public class Authentication
{
    private List<User> Users;
    private bool AuthenticateUserUser = true;
    public long InputtedAccountNumber { get; set; }

    public Authentication(List<User> Users)
    {
        this.Users = Users;
    }


    private bool AuthenticateUser(long inputAccountNumber, int inputPin)
    {
        var User = Users.Find(acc => acc.AccountNumber == inputAccountNumber && acc.Pin == inputPin);
        return User != null;
    }

    public User? StartAuthentication()
    {
        while (AuthenticateUserUser)
        {
            Console.Write("Enter your 10 digit User number: ");
            if (!long.TryParse(Console.ReadLine(), out long inputAccountNumber))
            {
                Messages.EnterValidAccountNumber();
                continue;
            }

            if (inputAccountNumber.ToString().Length != 10)
            {
                Console.WriteLine("User number must be 10 digits");
                continue;
            }


            var User = Users.Find(acc => acc.AccountNumber == inputAccountNumber);
            if (User == null)
            {
                 Messages.UserNotFound();
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

                if (AuthenticateUser(inputAccountNumber, inputPin))
                {
                    Console.WriteLine("Correct pin entered.");
                    return User;
                }

                Console.WriteLine("Incorrect pin!!! Confirm and enter correct pin.");
            }

            Console.WriteLine("Max attempts reached. User suspended, visit the bank for further inquiries.");
            AuthenticateUserUser = false;
        }

        return null;
    }

}
