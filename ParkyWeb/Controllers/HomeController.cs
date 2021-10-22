using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ParkyWeb.Models;
using ParkyWeb.Models.ViewModel;
using ParkyWeb.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ParkyWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly INationalParkRepository _npRepo;
        private readonly IUserRepository _userRepo;
        private readonly ITrailRepository _trailRepo;

        public HomeController(ILogger<HomeController> logger
            ,INationalParkRepository npRepo
            ,ITrailRepository trailRepo
            ,IUserRepository userRepo)
        {
            _logger = logger;
            _trailRepo = trailRepo;
            _npRepo = npRepo;
            _userRepo = userRepo;
        }

        public async Task<IActionResult> Index()
        {
            var nationalParkAndTrailVM = new NationalParkAndTrailVM()
            {
                TrailList = await _trailRepo
                    .GetAllAsync(SD.TrailsAPIPath, HttpContext.Session.GetString("JWTToken")),
                NationalParkList = await _npRepo
                    .GetAllAsync(SD.NationalParkAPIPath, HttpContext.Session.GetString("JWTToken"))
            };
            return View(nationalParkAndTrailVM);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        public IActionResult Login()
        {
            var user = new UserRequest();
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(UserRequest userRequest)
        {
            var userResponse = await _userRepo.Login(SD.UserAPIPath + "login/", userRequest);

            if (userResponse != null)
            {
                HttpContext.Session.SetString("JWTToken", userResponse.Token);
                return RedirectToAction(nameof(Index));
            }

            return View();
        }

        public IActionResult Register()
        {
            var user = new UserRequest();
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(UserRequest userRequest)
        {
            var userRegisterSuccess = await _userRepo.Register(SD.UserAPIPath + "Register/", userRequest);

            if (userRegisterSuccess)
            {
                return RedirectToAction(nameof(Login));
            }

            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.SetString("JWTToken", "");
            return RedirectToAction(nameof(Index));
        }
    }
}
