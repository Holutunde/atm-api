using System.Security.Cryptography; // Ensure you have this namespace imported  
using System.Text;  

namespace Application.Users;  

public static class UserPinHasher  
{  
   
        public static string HashPin(int pin)  
        {  
            // Convert the integer pin to a string to hash it  
            var pinString = pin.ToString();  
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(pinString));  
            StringBuilder builder = new();  
            foreach (byte b in bytes)  
            {  
                builder.Append(b.ToString("x2"));  
            }  
            return builder.ToString();  
        }  

        public static bool VerifyPin(int enteredPin, string storedHashedPin)  
        {  
            var hashedPin = HashPin(enteredPin);  
            return hashedPin.Equals(storedHashedPin);  
        }  
     
}