using ParkyAPI.Models;
using ParkyAPI.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParkyAPI.Repository.IRepository
{
    public interface IUserRepository
    {
        Task<bool> UserExists(string userName);
        Task<User> Login(string userName, string password);
        Task<User> Register(User user, string password);
    }
}
