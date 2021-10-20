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
            var nationalParkInDb = await _npRepo.GetAllAsync(SD.NationalParkAPIPath);

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
                var trailInDb = await _trailRepo.GetAsync(SD.TrailsAPIPath, id.GetValueOrDefault());
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(TrailVM trailRequest)
        {
            if (ModelState.IsValid)
            {
                if (trailRequest.Trail.Id == 0)
                {
                     await _trailRepo
                        .CreateAsync(SD.TrailsAPIPath, trailRequest.Trail);
                }
                else
                {
                     await _trailRepo
                        .UpdateAsync(SD.TrailsAPIPath + trailRequest.Trail.Id, trailRequest.Trail);
                }
                return RedirectToAction(nameof(Index));
            }

            return View(trailRequest);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteTrail (int id)
        {
            var trailIsDelete = await _trailRepo.DeleteAsync(SD.TrailsAPIPath, id);

            if (!trailIsDelete)
            {
                return Json(new { success = false, message = "Delete error!" });
            }

            return Json(new { success = true, message = "Delete Successful!" });
        }

        public async Task<IActionResult> GetAllTrails()
        {
            return Json(
                new { data = await _trailRepo.GetAllAsync(SD.TrailsAPIPath)});
        }
    }
}
