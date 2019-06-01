using API.DatingApp.Models;
using Microsoft.EntityFrameworkCore;

namespace API.DatingApp.Data
{
    public class DataContext : DbContext
    {
        public DataContext (DbContextOptions<DataContext> options) : base (options) {}
        public DbSet<Value> Value {get; set; }
        public DbSet<User> Users {get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating (ModelBuilder builder)
        {
            builder.Entity<Like>()
                .HasKey(key => new { key.LikerId, key.LikeeId });

            builder.Entity<Like>()
                .HasOne(user => user.Likee)
                .WithMany(user => user.Likers)
                .HasForeignKey(user => user.LikeeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Like>()
                .HasOne(user => user.Liker)
                .WithMany(user => user.Likees)
                .HasForeignKey(user => user.LikerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Message>()
                .HasOne(u => u.Sender)
                .WithMany(m => m.MessagesSend)
                .OnDelete(DeleteBehavior.Restrict);
            
            builder.Entity<Message>()
                .HasOne(u => u.Recipient)
                .WithMany(m => m.MessagesRecived)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}