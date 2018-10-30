using System.Threading.Tasks;
using DatingApp.WebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.WebAPI.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _dataContext;
        public AuthRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<User> Register(User user, string password)
        {
            (byte[] passwordHash, byte[] passwordSalt) = CreatePasswordHash(password);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            await _dataContext.Users.AddAsync(user);
            await _dataContext.SaveChangesAsync();

            return user;
        }

        public async Task<User> Login(string userName, string password)
        {
            var user = await _dataContext.Users.FirstOrDefaultAsync(u => u.UserName == userName);
            if (user == null)
                return null;

            if (!VerifyPassword(password, user.PasswordHash, user.PasswordSalt))
                return null;

            return user;
        }

        public async Task<bool> UserExists(string userName)
        {
            if (await _dataContext.Users.AnyAsync(u => u.UserName == userName))
                return true;

            return false;
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

        private bool VerifyPassword(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

                for (var i = 0; i <= computedHash.Length-1; i++)
                {
                    if (passwordHash[i] != computedHash[i])
                    {
                        return false;
                    }
                }
            }

            return true;
        }


    }
}
