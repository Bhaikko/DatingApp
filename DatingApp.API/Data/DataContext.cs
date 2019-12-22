using Microsoft.EntityFrameworkCore;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace DatingApp.API.Data
{
    // IdentityDbContext is added for DbContext along with Identity database support
    // All this done cuz we are using int instead of default string
    public class DataContext : IdentityDbContext<
        User, 
        Role, 
        int, 
        IdentityUserClaim<int>, 
        UserRole, 
        IdentityUserLogin<int>, 
        IdentityRoleClaim<int>, 
        IdentityUserToken<int>
    >
    {
        // : base is used to call the constructor of base class which this class derives from
        public DataContext(DbContextOptions<DataContext> options) : base (options) {}

        // defined to setup tables called 'Values'
        public DbSet<Value> Values { get; set; }   

        public DbSet<Photo> Photos { get; set; }

        public DbSet<Like> Likes { get; set; }
        
        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);  // required for identity

            // configutaion for many to many relation of user and userRoles based on Ids
            builder.Entity<UserRole>(userRole => {
                userRole.HasKey(ur => new {ur.UserId, ur.RoleId});

                userRole.HasOne(ur => ur.Role)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.RoleId)
                    .IsRequired();

                userRole.HasOne(ur => ur.User)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.UserId)
                    .IsRequired(); 
            });

            // this is done to create n : m relation table between Users to Users using Like table
            builder.Entity<Like>()
                .HasKey(k => new {k.LikerId, k.LikeeId});

            builder.Entity<Like>()
                .HasOne(u => u.Likee)
                .WithMany(u => u.Likers)
                .HasForeignKey(u => u.LikeeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Like>()
                .HasOne(u => u.Liker)
                .WithMany(u => u.Likees)
                .HasForeignKey(u => u.LikerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Message>()
                .HasOne(u => u.Sender)
                .WithMany(u => u.MessagesSent)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Message>()
                .HasOne(u => u.Recipient)
                .WithMany(u => u.MessagesRecieved)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Photo>()
                .HasQueryFilter(p => p.IsApproved);
        }



    }
}