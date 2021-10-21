using Microsoft.EntityFrameworkCore;
using ParkyAPI.Data;
using ParkyAPI.Models;
using ParkyAPI.Models.Dtos;
using ParkyAPI.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ParkyAPI.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _db;
        public UserRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<User> Login(string userName, string password)
        {
            throw new NotImplementedException();
        }

        public async Task<User> Register(User user, string password)
        {
            if (await UserExists(user.UserName))
            {
                return null;
            }
            CreatePasswordHash(password,out byte[] PasswordHash,out byte[] PasswordSalt);

            user.PasswordHash = PasswordHash;
            user.PasswordSalt = PasswordSalt;

            await _db.Users.AddAsync(user);
            await _db.SaveChangesAsync();

            return user;
        }

        public async Task<bool> UserExists(string userName)
        {
            var userIsInDb = await _db.Users.AnyAsync(c => c.UserName.ToLower() == userName.ToLower());
            if (!userIsInDb)
            {
                return false;
            }
            return true;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using(var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }
    }
}
