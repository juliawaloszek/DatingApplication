using System.Collections.Generic;
using System.Threading.Tasks;
using API.DatingApp.Models;

namespace API.DatingApp.Data
{
    public interface IDatingRepository
    {
         void Add<T>(T entity) where T: class;
         void Delete<T>(T entity) where T: class;
         Task<bool> SaveAllAsync();
         Task<IEnumerable<User>> GetUsers();
         Task<User> GetUser(int id);
         Task <Photo> GetPhoto(int id);
         
    }
}