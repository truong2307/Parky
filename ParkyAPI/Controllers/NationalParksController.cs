using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        [HttpGet("{nationalParkId}")]
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
    }
}
