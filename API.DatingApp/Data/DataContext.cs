using API.DatingApp.Models;
using Microsoft.EntityFrameworkCore;

namespace API.DatingApp.Data
{
    public class DataContext : DbContext
    {
        public DataContext (DbContextOptions<DataContext> options) : base (options) {}
        public DbSet<Value> Value {get; set; }
        
    }
}