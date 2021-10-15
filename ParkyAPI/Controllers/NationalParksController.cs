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
    [ApiExplorerSettings(GroupName = "NationalParkAPI")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
        /// <summary>
        /// Get list national park
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK,Type = typeof(List<NationalParkDto>))]
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
        /// <summary>
        /// Get nationalpark with id
        /// </summary>
        /// <param name="nationalParkId"> The id of the national park</param>
        /// <returns></returns>
        [HttpGet("{nationalParkId}", Name = "GetNationalPark")]
        [ProducesResponseType(StatusCodes.Status200OK, Type =typeof(NationalParkDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
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
        /// <summary>
        /// Create National Park
        /// </summary>
        /// <param name="nationalParkNew"> Obj request of the national park</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type =typeof(NationalParkDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

            var nationalParkToDb = _mapper.Map<NationalPark>(nationalParkNew);
            if (!_npRepo.CreateNationalPark(nationalParkToDb))
            {
                ModelState
                    .AddModelError("", $"Something went wrong when saving the record {nationalParkToDb.Name} ");
                return StatusCode(500, ModelState);
            }

            return CreatedAtRoute("GetNationalPark"
                , new { nationalParkId = nationalParkToDb.Id}, nationalParkToDb);
        }

        [HttpPatch("{nationalParkId}", Name = "UpdateNationalPark")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdateNationalPark(int nationalParkId
            , [FromBody] NationalParkDto nationalParkUpdate)
        {
            if (nationalParkUpdate == null || nationalParkUpdate.Id != nationalParkId)
            {
                return BadRequest(ModelState);
            }

            var nationalParkToDB = _mapper.Map<NationalPark>(nationalParkUpdate);
            if (!_npRepo.UpdateNationalPark(nationalParkToDB))
            {
                ModelState
                    .AddModelError("", $"Something went wrong when update the record {nationalParkToDB.Name} ");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        [HttpDelete("{nationalParkId}", Name = "DeleteNationalPark")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult DeleteNationalPark(int nationalParkId)
        {
            if (!_npRepo.NationalParkExists(nationalParkId))
            {
                return BadRequest();
            }

            var nationalParkInDb = _npRepo.GetNationalPark(nationalParkId);
            if (!_npRepo.DeleteNationalPark(nationalParkInDb))
            {
                ModelState
                    .AddModelError("", $"Something went wrong when delete the record {nationalParkInDb.Name} ");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    }
}
