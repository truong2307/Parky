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
    [Route("api/[controller]")]
    [ApiController]
    public class NationalParksController : Controller
    {
        private readonly INationalParkRepository _npRepo;
        private readonly IMapper _mapper;
        public NationalParksController(INationalParkRepository npRepo
            ,IMapper mapper)

        {
            _npRepo = npRepo;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetNationalParks()
        {
            var nationalParkList = _npRepo.GetNationalParks();
            var npListDto = new List<NationalParkDto>();

            foreach (var nationalPark in nationalParkList)
            {
                npListDto.Add(_mapper.Map<NationalParkDto>(nationalPark));
            }
            return Ok(npListDto);
        }

        [HttpGet("{nationalParkId}", Name = "GetNationalPark")]
        public IActionResult GetNationalPark(int nationalParkId)
        {
            var nationalParkInDb = _npRepo.GetNationalPark(nationalParkId);
            if (nationalParkInDb == null)
            {
                return NotFound();
            }
            var nationalParkDto = _mapper.Map<NationalParkDto>(nationalParkInDb);
            return Ok(nationalParkDto);
        }

        [HttpPost]
        public IActionResult CreateNationalPark([FromBody] NationalParkDto nationalParkNew)
        {
            if (nationalParkNew == null)
            {
                return BadRequest(ModelState);
            }

            if (_npRepo.NationalParkExists(nationalParkNew.Name))
            {
                ModelState.AddModelError("", "National Park Exists!");
                return StatusCode(404, ModelState);
            }

            var nationParkToDb = _mapper.Map<NationalPark>(nationalParkNew);
            if (!_npRepo.CreateNationalPark(nationParkToDb))
            {
                ModelState
                    .AddModelError("", $"Something went wrong when saving the record {nationParkToDb.Name} ");
                return StatusCode(500, ModelState);
            }

            return CreatedAtRoute("GetNationalPark"
                , new { nationalParkId = nationParkToDb.Id}, nationParkToDb);
        }
    }
}
