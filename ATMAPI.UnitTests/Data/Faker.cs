using Bogus;
using Domain.Enum;
using Domain.Entities;

namespace ATMAPI.UnitTests.Data
{
    public static class Faker
    {
        
        private static int userIdCounter = 1;  

        public static User GenerateUser()
        {
            var faker = new Faker<User>()
                .RuleFor(u => u.Id, f => userIdCounter++)  
                .RuleFor(u => u.AccountNumber, f => GetAccount())
                .RuleFor(u => u.Email, f => f.Internet.Email())
                .RuleFor(u => u.Password, f => f.Internet.Password())
                .RuleFor(u => u.Pin, f => f.Random.Int(1000, 9999))
                .RuleFor(u => u.FirstName, f => f.Name.FirstName())
                .RuleFor(u => u.LastName, f => f.Name.LastName())
                .RuleFor(u => u.Balance, f => (long)f.Finance.Amount(0, 1000000))
                .RuleFor(u => u.Role, f => Roles.User.ToString());

            return faker.Generate();
        }

        public static List<User> GenerateUsers(int amount = 8)
        {
            return new Faker<User>().Generate(amount);
        }

        public static Admin GenerateAdmin()
        {
            var faker = new Faker<Admin>()
                .RuleFor(a => a.Id, f => f.IndexFaker)
                .RuleFor(a => a.AccountNumber, f => GetAccount())
                .RuleFor(a => a.Email, f => f.Internet.Email())
                .RuleFor(a => a.Password, f => f.Internet.Password())
                .RuleFor(a => a.Pin, f => f.Random.Int(1000, 9999))
                .RuleFor(a => a.FirstName, f => f.Name.FirstName())
                .RuleFor(a => a.LastName, f => f.Name.LastName())
                .RuleFor(a => a.Balance, f => (long)f.Finance.Amount(0, 1000000))
                .RuleFor(a => a.Role, f => Roles.Admin.ToString());

            return faker.Generate();
        }

        public static List<Admin> GenerateAdmins(int amount = 8)
        {
            return new Faker<Admin>().Generate(amount);
        }

        private static long GetAccount()
        {
            // Generate a random 10-digit account number
            return new Bogus.Randomizer().Long(1000000000, 9999999999);
        }
    }
}
