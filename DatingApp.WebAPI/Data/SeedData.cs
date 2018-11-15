using System.Collections.Generic;
using DatingApp.WebAPI.Models;
using Newtonsoft.Json;

namespace DatingApp.WebAPI.Data
{
    public class SeedData
    {
        private readonly DataContext _dataContext;

        public SeedData(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public void SeedUsers()
        {
            var userData = System.IO.File.ReadAllText("Data/UserSeedData.json");
            var users = JsonConvert.DeserializeObject<List<User>>(userData);

            users.ForEach(user =>
            {
                (byte[] passwordHash, byte[] passwordSalt) = CreatePasswordHash("password");

                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
                user.UserName = user.UserName.ToLower();

                _dataContext.Users.Add(user);
            });

            _dataContext.SaveChanges();
        }

        private static (byte[], byte[]) CreatePasswordHash(string password)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                var passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                var passwordSalt = hmac.Key;

                return (passwordHash, passwordSalt);
            }
        }
    }
}
