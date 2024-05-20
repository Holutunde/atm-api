using ATMAPI.Models;
using ATMAPI.Services;
using OfficeOpenXml;


namespace ATMAPI.Services
{
    public class RegisteredAccountsService : IRegisteredAccountsService
    {
        static readonly string ExcelFilePath = @"/Users/apple/Documents/Programming Coding/C#/ATMAPI/ATMAPI/ATMAPI/Database/Accountdetails.xlsx";

        public void AddAccount(Account account)
        {

            Console.WriteLine(account.AccountNumber);
            string directoryPath = Path.GetDirectoryName(ExcelFilePath);
            if (!Directory.Exists(directoryPath))
            {
                _ = Directory.CreateDirectory(directoryPath);
            }

            FileInfo fileInfo = new(ExcelFilePath);
            try
            {
                using ExcelPackage package = new(fileInfo);
                ExcelWorksheet worksheet = package.Workbook.Worksheets.FirstOrDefault() ?? package.Workbook.Worksheets.Add("Accounts");
                int rowCount = worksheet.Dimension?.Rows ?? 0;
                // In the AddAccount method
                worksheet.Cells[rowCount + 1, 1].Value = account.FirstName;
                worksheet.Cells[rowCount + 1, 2].Value = account.LastName;
                worksheet.Cells[rowCount + 1, 3].Value = Convert.ToInt64(account.AccountNumber);
                worksheet.Cells[rowCount + 1, 4].Value = Convert.ToInt32(account.Pin);
                worksheet.Cells[rowCount + 1, 5].Value = account.Balance;
                worksheet.Cells[rowCount + 1, 6].Value = account.OpeningDate.ToString("M/d/yyyy h:mm:ss tt");


                package.Save();
                Console.WriteLine("Account added successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving account: {ex.Message}");
            }
        }

        public void UpdateAccount(Account updatedAccount)
        {
            FileInfo fileInfo = new(ExcelFilePath);
            if (!fileInfo.Exists)
            {
                Console.WriteLine("Excel file does not exist.");
                return;
            }

            try
            {
                using ExcelPackage package = new ExcelPackage(fileInfo);
                ExcelWorksheet worksheet = package.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                {
                    Console.WriteLine("Accounts worksheet not found.");
                    return;
                }

                int rowCount = worksheet.Dimension?.Rows ?? 0;
                bool accountFound = false;

                for (int i = 2; i <= rowCount; i++) 
                {
                    if (worksheet.Cells[i, 3].Value?.ToString() == updatedAccount.AccountNumber.ToString())
                    {
                        worksheet.Cells[i, 1].Value = updatedAccount.FirstName;
                        worksheet.Cells[i, 2].Value = updatedAccount.LastName;
                        worksheet.Cells[i, 3].Value = updatedAccount.AccountNumber;
                        worksheet.Cells[i, 4].Value = updatedAccount.Pin;
                        worksheet.Cells[i, 5].Value = updatedAccount.Balance;
                        worksheet.Cells[i, 6].Value = updatedAccount.OpeningDate;

                        accountFound = true;
                        break;
                    }
                }

                if (!accountFound)
                {
                    Console.WriteLine("Account not found in the worksheet.");
                    return;
                }

                package.Save();
                Console.WriteLine("Account updated successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating account: {ex.Message}");
            }
        }

        // public void UpdateAccount(Account updatedAccount)
        // {
        //     FileInfo fileInfo = new FileInfo(ExcelFilePath);
        //     try
        //     {
        //         using ExcelPackage package = new ExcelPackage(fileInfo);
        //         ExcelWorksheet worksheet = package.Workbook.Worksheets.FirstOrDefault();
        //         if (worksheet == null)
        //         {
        //             Console.WriteLine("Accounts worksheet not found.");
        //             return;
        //         }

        //         int rowCount = worksheet.Dimension?.Rows ?? 0;

        //         int rowIndex = 0;
        //         for (int i = 1; i <= rowCount; i++)
        //         {
        //             if (worksheet.Cells[i, 2].Value?.ToString() == updatedAccount.AccountNumber.ToString())
        //             {
        //                 rowIndex = i;
        //                 break;
        //             }
        //         }

        //         if (rowIndex == 0)
        //         {
        //             Console.WriteLine("Account not found in the worksheet.");
        //             return;
        //         }

        //         worksheet.Cells[rowIndex, 1].Value = updatedAccount.FirstName;
        //         worksheet.Cells[rowIndex, 2].Value = updatedAccount.LastName;
        //         worksheet.Cells[rowIndex, 3].Value = updatedAccount.AccountNumber;
        //         worksheet.Cells[rowIndex, 4].Value = updatedAccount.Pin;
        //         worksheet.Cells[rowIndex, 5].Value = updatedAccount.Balance;
        //         worksheet.Cells[rowIndex, 6].Value = updatedAccount.OpeningDate;

        //         package.Save();
        //         Console.WriteLine("Account updated successfully.");
        //     }
        //     catch (Exception ex)
        //     {
        //         Console.WriteLine($"Error updating account: {ex.Message}");
        //     }
        // }

        public ICollection<Account> GetAccounts()
        {
            ICollection<Account> accounts = new List<Account>();

            FileInfo fileInfo = new FileInfo(ExcelFilePath);
            if (!fileInfo.Exists)
            {
                Console.WriteLine("Excel file does not exist.");
                return accounts;
            }

            try
            {
                using ExcelPackage package = new ExcelPackage(fileInfo);
                ExcelWorksheet worksheet = package.Workbook.Worksheets.FirstOrDefault();
                if (worksheet != null && worksheet.Dimension != null)
                {
                    int rowCount = worksheet.Dimension.Rows;

                    for (int row = 2; row <= rowCount; row++) // Assuming data starts from row 2
                    {
                        Account account = new Account
                        {
                            FirstName = worksheet.Cells[row, 1].Value?.ToString(),
                            LastName = worksheet.Cells[row, 2].Value?.ToString(),
                            AccountNumber = Convert.ToInt64(worksheet.Cells[row, 3].Value),
                            Pin = Convert.ToInt32(worksheet.Cells[row, 4].Value),
                            Balance = Convert.ToDouble(worksheet.Cells[row, 5].Value),
                            OpeningDate = Convert.ToDateTime(worksheet.Cells[row, 6].Value)
                        };
                        accounts.Add(account);
                    }
                }
                else
                {
                    Console.WriteLine("Worksheet not found in Excel file.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading Excel file: {ex.Message}");
            }

            return accounts;
        }

        public Account GetAccountByNumber(long accountNumber)
        {
            var accounts = GetAccounts();
            return accounts.FirstOrDefault(a => a.AccountNumber == accountNumber);
        }
    }


}





