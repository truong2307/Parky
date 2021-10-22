using ParkyWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParkyWeb.Repository.IRepository
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User> Login(string url, UserRequest userRequest);
        Task<bool> Register(string url, UserRequest userRequest);
    }
}
