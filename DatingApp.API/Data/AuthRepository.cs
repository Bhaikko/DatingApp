using System.Threading.Tasks;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class AuthRepository : IAuthRepository
    {
        // This is responsible for querying our database via Entity Framework
        // This repository will be then used in Controller
        private readonly DataContext _context;

        public AuthRepository(DataContext context)
        {
            this._context = context;
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt) 
        {
            // the salt passed is used to create hash
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++) {
                    if (computedHash[i] != passwordHash[i]) {
                        return false;
                    }
                }
            }
            return true;

        }

        public async Task<User> Login(string username, string password)
        {
            var user = await this._context.Users.Include(p => p.Photos).FirstOrDefaultAsync(x => x.UserName == username);
            if (user == null) {
                return null;
            }

            // if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt)) {
            //     return null;
            // } 

            return user;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt) 
        {
            // using() is used when the object is no longer required after the statement such that it is disposed of automatically. The object must be inherited from IDisposable
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        public async Task<User> Register(User user, string password)
        {
            byte[] passwordHash, passwordSalt;

            // out is used to pass arguements by reference
            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            // user.PasswordHash = passwordHash;
            // user.PasswordSalt = passwordSalt;

            await this._context.Users.AddAsync(user);
            await this._context.SaveChangesAsync();

            return user;
        }

        public async Task<bool> UserExists(string username)
        {
            if (await this._context.Users.AnyAsync(x => x.UserName == username)) {
                return true;
            }

            return false;
        }
    }
}