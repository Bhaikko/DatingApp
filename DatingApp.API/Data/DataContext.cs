using Microsoft.EntityFrameworkCore;
using DatingApp.API.Models;

namespace DatingApp.API.Data
{
    public class DataContext : DbContext
    {
        // : base is used to call the constructor of base class which this class derives from
        public DataContext(DbContextOptions<DataContext> options) : base (options) {}

        // defined to setup tables called 'Values'
        public DbSet<Value> Values { get; set; }   
    }
}