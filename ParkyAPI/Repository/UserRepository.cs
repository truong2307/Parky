using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ParkyAPI.Data;
using ParkyAPI.Models;
using ParkyAPI.Models.Dtos;
using ParkyAPI.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ParkyAPI.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly IConfiguration _configuration;
        public UserRepository(ApplicationDbContext db, IConfiguration configuration)
        {
            _db = db;
            _configuration = configuration;
        }

        public async Task<User> Login(string userName, string password)
        {
            if (!await UserExists(userName))
            {
                return null;
            }

            var userInDb = await _db.Users.FirstOrDefaultAsync(c => c.UserName.ToLower() == userName.ToLower());

            if (VerifyPassword(password, userInDb.PasswordHash, userInDb.PasswordSalt))
            {
                userInDb.Token = CreateToken(userInDb);
                return userInDb;
            }

            return null;
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

            if (!await _db.Users.AnyAsync(c => c.Role == "Admin"))
            {
                user.Role = "Admin";
                await _db.Users.AddAsync(user);
            }
            else
            {
                user.Role = "User";
                await _db.Users.AddAsync(user);
            }
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

        private bool VerifyPassword (string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using(var hmac = new HMACSHA512(passwordSalt))
            {
                var computerHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < passwordHash.Length; i++)
                {
                    if (computerHash[i] != passwordHash[i])
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using(var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        private string CreateToken (User user)
        {
            var claim = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));

            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescription = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claim),
                Expires = DateTime.Now.AddDays(2),
                SigningCredentials = cred
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescription);

            return tokenHandler.WriteToken(token);
        }

    }
}
