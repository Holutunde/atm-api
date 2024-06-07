using ATMAPI.Models;
using ATMAPI.Services;
using OfficeOpenXml;


namespace ATMAPI.Services
{
    public class RegisteredUsersService : IRegisteredUsersService
    {
        static readonly string ExcelFilePath = @"/Users/apple/Documents/Programming Coding/C#/ATMAPI/ATMAPI/ATMAPI/Database/Userdetails.xlsx";

        public void AddUser(User User)
        {

            Console.WriteLine(User.AccountNumber);
            string directoryPath = Path.GetDirectoryName(ExcelFilePath);
            if (!Directory.Exists(directoryPath))
            {
                _ = Directory.CreateDirectory(directoryPath);
            }

            FileInfo fileInfo = new(ExcelFilePath);
            try
            {
                using ExcelPackage package = new(fileInfo);
                ExcelWorksheet worksheet = package.Workbook.Worksheets.FirstOrDefault() ?? package.Workbook.Worksheets.Add("Users");
                int rowCount = worksheet.Dimension?.Rows ?? 0;
                // In the AddUser method
                worksheet.Cells[rowCount + 1, 1].Value = User.FirstName;
                worksheet.Cells[rowCount + 1, 2].Value = User.LastName;
                worksheet.Cells[rowCount + 1, 3].Value = Convert.ToInt64(User.AccountNumber);
                worksheet.Cells[rowCount + 1, 4].Value = Convert.ToInt32(User.Pin);
                worksheet.Cells[rowCount + 1, 5].Value = User.Balance;
                worksheet.Cells[rowCount + 1, 6].Value = User.OpeningDate.ToString("M/d/yyyy h:mm:ss tt");
                worksheet.Cells[rowCount + 1, 7].Value = User.Role; 


                package.Save();
                Console.WriteLine("User added successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving User: {ex.Message}");
            }
        }
        public void AddAdmin(Admin User)
        {

            Console.WriteLine(User.AccountNumber);
            string directoryPath = Path.GetDirectoryName(ExcelFilePath);
            if (!Directory.Exists(directoryPath))
            {
                _ = Directory.CreateDirectory(directoryPath);
            }

            FileInfo fileInfo = new(ExcelFilePath);
            try
            {
                using ExcelPackage package = new(fileInfo);
                ExcelWorksheet worksheet = package.Workbook.Worksheets.FirstOrDefault() ?? package.Workbook.Worksheets.Add("Users");
                int rowCount = worksheet.Dimension?.Rows ?? 0;
                // In the AddUser method
                worksheet.Cells[rowCount + 1, 1].Value = User.FirstName;
                worksheet.Cells[rowCount + 1, 2].Value = User.LastName;
                worksheet.Cells[rowCount + 1, 3].Value = Convert.ToInt64(User.AccountNumber);
                worksheet.Cells[rowCount + 1, 4].Value = Convert.ToInt32(User.Pin);
                worksheet.Cells[rowCount + 1, 5].Value = User.Balance;
                worksheet.Cells[rowCount + 1, 6].Value = User.OpeningDate.ToString("M/d/yyyy h:mm:ss tt");
                worksheet.Cells[rowCount + 1, 7].Value = User.Role; 


                package.Save();
                Console.WriteLine("User added successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving User: {ex.Message}");
            }
        }

        public void UpdateUser(User updatedUser)
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
                    Console.WriteLine("Users worksheet not found.");
                    return;
                }

                int rowCount = worksheet.Dimension?.Rows ?? 0;
                bool UserFound = false;

                for (int i = 2; i <= rowCount; i++) 
                {
                    if (worksheet.Cells[i, 3].Value?.ToString() == updatedUser.AccountNumber.ToString())
                    {
                        worksheet.Cells[i, 1].Value = updatedUser.FirstName;
                        worksheet.Cells[i, 2].Value = updatedUser.LastName;
                        worksheet.Cells[i, 3].Value = updatedUser.AccountNumber;
                        worksheet.Cells[i, 4].Value = updatedUser.Pin;
                        worksheet.Cells[i, 5].Value = updatedUser.Balance;
                        worksheet.Cells[i, 6].Value = updatedUser.OpeningDate;
                        worksheet.Cells[i, 7].Value = updatedUser.Role;

                        UserFound = true;
                        break;
                    }
                }

                if (!UserFound)
                {
                    Console.WriteLine("User not found in the worksheet.");
                    return;
                }

                package.Save();
                Console.WriteLine("User updated successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating User: {ex.Message}");
            }
        }


        public ICollection<User> GetUsers()
        {
            ICollection<User> Users = new List<User>();

            FileInfo fileInfo = new(ExcelFilePath);
            if (!fileInfo.Exists)
            {
                Console.WriteLine("Excel file does not exist.");
                return Users;
            }

            try
            {
                using ExcelPackage package = new(fileInfo);
                ExcelWorksheet worksheet = package.Workbook.Worksheets.FirstOrDefault();
                if (worksheet != null && worksheet.Dimension != null)
                {
                    int rowCount = worksheet.Dimension.Rows;

                    for (int row = 2; row <= rowCount; row++) // Assuming data starts from row 2
                    {
                        var openingDateCellValue = worksheet.Cells[row, 6].Value?.ToString();
                        if (!DateTime.TryParse(openingDateCellValue, out DateTime openingDate))
                        {
                            Console.WriteLine($"Error parsing date: {openingDateCellValue}");
                            continue;
                        }

                        User User = new User
                        {
                            FirstName = worksheet.Cells[row, 1].Value?.ToString(),
                            LastName = worksheet.Cells[row, 2].Value?.ToString(),
                            AccountNumber = Convert.ToInt64(worksheet.Cells[row, 3].Value),
                            Pin = Convert.ToInt32(worksheet.Cells[row, 4].Value),
                            Balance = Convert.ToDouble(worksheet.Cells[row, 5].Value),
                            OpeningDate = openingDate,
                            Role = worksheet.Cells[row, 7].Value?.ToString() 
                        };
                        Users.Add(User);
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

            return Users;
        }

        public User GetUserByNumber(long AccountNumber)
        {
            var Users = GetUsers();
            return Users.FirstOrDefault(a => a.AccountNumber == AccountNumber);
        }

        public void DeleteUser(long AccountNumber)
        {
            FileInfo fileInfo = new(ExcelFilePath);
            if (!fileInfo.Exists)
            {
                Console.WriteLine("Excel file does not exist.");
                return;
            }

            try
            {
                using ExcelPackage package = new(fileInfo);
                ExcelWorksheet worksheet = package.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                {
                    Console.WriteLine("Users worksheet not found.");
                    return;
                }

                int rowCount = worksheet.Dimension?.Rows ?? 0;
                bool UserFound = false;

                for (int i = 2; i <= rowCount; i++)
                {
                    if (worksheet.Cells[i, 3].Value?.ToString() == AccountNumber.ToString())
                    {
                        worksheet.DeleteRow(i);
                        UserFound = true;
                        break;
                    }
                }

                if (!UserFound)
                {
                    Console.WriteLine("User not found in the worksheet.");
                    return;
                }

                package.Save();
                Console.WriteLine("User deleted successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting User: {ex.Message}");
            }
        }
    }


}





