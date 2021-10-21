using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ParkyAPI.Models;
using ParkyAPI.Models.Dtos;
using ParkyAPI.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParkyAPI.Controllers
{
    [Route("api/v{version:apiVersion}/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepo;
        private readonly IMapper _mapper;

        public UsersController(IUserRepository userRepo, IMapper mapper)
        {
            _userRepo = userRepo;
            _mapper = mapper;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login (UserLoginRegisDto userRequest)
        {
            var userToLogin = await _userRepo.Login(userRequest.UserName, userRequest.Password);
            if (userToLogin == null)
            {
                return BadRequest(new { message = "UserName or password incorrect!" });
            }
            return Ok(_mapper.Map<UserDto>(userToLogin));
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register (UserLoginRegisDto userRequest)
        {
            var userToRegister = await _userRepo
                .Register(new User {UserName = userRequest.UserName }, userRequest.Password);

            if (userToRegister == null)
            {
                return BadRequest(new { message = "Username already exists"});
            }

            return Ok();
        }

    }
}
