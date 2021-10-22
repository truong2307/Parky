using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ParkyWeb.Models;
using ParkyWeb.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ParkyWeb.Controllers
{
    [Authorize(Roles ="Admin")]
    public class NationalParksController : Controller
    {
        private readonly INationalParkRepository _npRepo;

        public NationalParksController(INationalParkRepository npRepo) => _npRepo = npRepo;

        public IActionResult Index()
        {
            return View(new NationalPark() { });
        }

        public async Task<IActionResult> Upsert(int? id)
        {
            var nationalPark = new NationalPark();
            if (id == null)
            {
                return View(nationalPark);
            }

            nationalPark = await _npRepo.GetAsync(SD.NationalParkAPIPath
                , id.GetValueOrDefault(), HttpContext.Session.GetString("JWTToken"));
            if (nationalPark == null)
            {
                return NotFound();
            }

            return View(nationalPark);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(NationalPark nationalParkRequest)
        {
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                if (files.Count > 0)
                {
                    byte[] p1 = null;
                    using (var fs1 = files[0].OpenReadStream())
                    {
                        using (var ms1 = new MemoryStream())
                        {
                            fs1.CopyTo(ms1);
                            p1 = ms1.ToArray();
                        }    
                    }
                    nationalParkRequest.Picture = p1;
                }
                else
                {
                    var nationalParkInDb = await _npRepo
                        .GetAsync(SD.NationalParkAPIPath
                            , nationalParkRequest.Id, HttpContext.Session.GetString("JWTToken"));
                    nationalParkRequest.Picture = nationalParkInDb.Picture;
                }
                if (nationalParkRequest.Id ==0)
                {
                    await _npRepo
                        .CreateAsync(SD.NationalParkAPIPath
                            , nationalParkRequest, HttpContext.Session.GetString("JWTToken"));
                }
                else
                {
                    await _npRepo
                        .UpdateAsync(SD.NationalParkAPIPath+nationalParkRequest.Id
                            , nationalParkRequest, HttpContext.Session.GetString("JWTToken"));
                }

                return RedirectToAction(nameof(Index));
            }
            else
            {
                return View(nationalParkRequest);
            }
        }

        public async Task<IActionResult> GetAllNationalPark()
        {
            return Json(new {
                data = await _npRepo.GetAllAsync(SD.NationalParkAPIPath
                    , HttpContext.Session.GetString("JWTToken"))
            });
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteNationalPark(int id)
        {
            bool nationalParkIsDelete = await _npRepo.DeleteAsync(SD.NationalParkAPIPath
                , id, HttpContext.Session.GetString("JWTToken"));

            if (!nationalParkIsDelete)
            {
                return Json(new { success = false, message = "Delete error!" });
            }
            return Json(new { success = true, message = "Delete successful!" });
        }
    }
}
