using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ParkyWeb.Models;
using ParkyWeb.Models.ViewModel;
using ParkyWeb.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParkyWeb.Controllers
{
    [Authorize(Roles = "Admin")]
    public class TrailsController : Controller
    {
        private readonly ITrailRepository _trailRepo;
        private readonly INationalParkRepository _npRepo;

        public TrailsController(ITrailRepository trailRepo, INationalParkRepository npRepo)
        {
            _trailRepo = trailRepo;
            _npRepo = npRepo;
        }

        public IActionResult Index()
        {
            return View(new Trail {});
        }

        public async Task<IActionResult> Upsert(int? id)
        {
            var nationalParkInDb = await _npRepo
                .GetAllAsync(SD.NationalParkAPIPath, HttpContext.Session.GetString("JWTToken"));

            if (id == null)
            {
                var trailViewModel = new TrailVM()
                {
                    Trail = new Trail(),
                    NationalParkList = nationalParkInDb.Select(i => new SelectListItem()
                    {
                        Value = i.Id.ToString(),
                        Text = i.Name
                    })
                };
                return View(trailViewModel);
            }
            else
            {
                var trailInDb = await _trailRepo
                    .GetAsync(SD.TrailsAPIPath
                    , id.GetValueOrDefault(), HttpContext.Session.GetString("JWTToken"));
                if (trailInDb == null)
                {
                    return NotFound();
                }
                else
                {
                    var trailViewModel = new TrailVM()
                    {
                        Trail = trailInDb,
                        NationalParkList = nationalParkInDb.Select(i => new SelectListItem()
                        {
                            Value = i.Id.ToString(),
                            Text = i.Name
                        })
                    };
                    return View(trailViewModel);
                }
            }
        }  
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(TrailVM trailRequest)
        {
            if (ModelState.IsValid)
            {
                if (trailRequest.Trail.Id == 0)
                {
                    await _trailRepo
                          .CreateAsync(SD.TrailsAPIPath
                          , trailRequest.Trail, HttpContext.Session.GetString("JWTToken"));
                }
                else
                {
                     await _trailRepo
                        .UpdateAsync(SD.TrailsAPIPath + trailRequest.Trail.Id
                        , trailRequest.Trail, HttpContext.Session.GetString("JWTToken"));
                }
                return RedirectToAction(nameof(Index));
            }
            else
            {
                var nationalParkInDb = await _npRepo.GetAllAsync(SD.NationalParkAPIPath
                    , HttpContext.Session.GetString("JWTToken"));
                var trailViewModel = new TrailVM()
                {
                    Trail = trailRequest.Trail,
                    NationalParkList = nationalParkInDb.Select(i => new SelectListItem()
                    {
                        Value = i.Id.ToString(),
                        Text = i.Name
                    })
                };
                return View(trailViewModel);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteTrail (int id)
        {
            var trailIsDelete = await _trailRepo.DeleteAsync(SD.TrailsAPIPath, id
                , HttpContext.Session.GetString("JWTToken"));

            if (!trailIsDelete)
            {
                return Json(new { success = false, message = "Delete error!" });
            }

            return Json(new { success = true, message = "Delete Successful!" });
        }

        public async Task<IActionResult> GetAllTrails()
        {
            return Json(
                new { data = await _trailRepo.GetAllAsync(SD.TrailsAPIPath
                , HttpContext.Session.GetString("JWTToken")) });
        }
    }
}
