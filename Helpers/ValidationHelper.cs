namespace ATMAPI.Helpers
{
    public static class ValidationHelper
    {
        public static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsValidPassword(string password)
        {
            if (password.Length < 7)
            {
                return false;
            }

            bool hasNumber = false;
            bool hasSpecialChar = false;
            foreach (var c in password)
            {
                if (char.IsDigit(c))
                {
                    hasNumber = true;
                }
                if (!char.IsLetterOrDigit(c))
                {
                    hasSpecialChar = true;
                }
            }

            return hasNumber && hasSpecialChar;
        }
    }
}
