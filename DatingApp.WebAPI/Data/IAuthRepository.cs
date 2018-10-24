using System.Threading.Tasks;
using DatingApp.WebAPI.Models;

namespace DatingApp.WebAPI.Data
{
    internal interface IAuthRepository
    {
        Task<User> Register(User user, string password);
        Task<User> Login(string userName, string password);
        Task<bool> UserExists(string userName);
    }
}
