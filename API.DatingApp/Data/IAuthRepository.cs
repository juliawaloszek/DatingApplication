using System.Threading.Tasks;
using API.DatingApp.Models;

namespace API.DatingApp.Data
{
    public interface IAuthRepository
    {
         Task <User> Register(User user, string password);
         Task <User> Login(string username, string password);
         Task <bool> UserExists(string username);
    }
}