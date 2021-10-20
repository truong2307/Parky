using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ParkyAPI.Data;
using ParkyAPI.Models;
using ParkyAPI.Models.Dtos;
using ParkyAPI.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParkyAPI.Controllers
{
    //[Route("api/Trail")]
    [Route("api/v{version:apiVersion}/trails")]
    [ApiController]
    //[ApiExplorerSettings(GroupName = "TrailsAPI")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public class TrailController : Controller
    {
        private readonly ITrailRepository _trailRepo;
        private readonly IMapper _mapper;
        public TrailController(ITrailRepository trailRepo, IMapper mapper)
        {
            _trailRepo = trailRepo;
            _mapper = mapper;
        }
        /// <summary>
        /// Get all trail
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<TrailDto>))]
        public IActionResult GetTrails()
        {
            var trailInDb = _trailRepo.GetTrails();

            List<TrailDto> trailsDto = new List<TrailDto>();

            foreach (var trailDto in trailInDb)
            {
                trailsDto.Add(_mapper.Map<TrailDto>(trailDto));
            }

            return Ok(trailsDto);
        }
        /// <summary>
        /// Get trail by trail id
        /// </summary>
        /// <param name="trailId">the id of trail</param>
        /// <returns></returns>
        [HttpGet("{trailId}", Name = "GetTrail")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TrailDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public IActionResult GetTrail (int trailId)
        {
            var trailInDn = _trailRepo.GetTrail(trailId);
            if (trailInDn == null)
            {
                return BadRequest(ModelState);
            }
            var trailDto = _mapper.Map<TrailDto>(trailInDn);

            return Ok(trailDto);
        }

        /// <summary>
        /// Get trails with national park Id
        /// </summary>
        /// <param name="nationalParkId">national park id request</param>
        /// <returns></returns>
        [HttpGet("[action]/{nationalParkId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TrailDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public IActionResult GetTrailsInNationalPark(int nationalParkId)
        {
            var trailInDb = _trailRepo.GetTrailsIsInNationalPark(nationalParkId);
            if (trailInDb == null)
            {
                return BadRequest(ModelState);
            }

            var trailsDto = new List<TrailDto>();
            foreach (var trail in trailInDb)
            {
                trailsDto.Add(_mapper.Map<TrailDto>(trail));
            }
            
            return Ok(trailsDto);
        }

        /// <summary>
        /// Create new trail
        /// </summary>
        /// <param name="trailNew">object trailnew request</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(TrailDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult CreateTrail([FromBody] TrailCreateDto trailNew)
        {
            if (trailNew == null)
            {
                return BadRequest(ModelState);
            }

            if (_trailRepo.TrailExists(trailNew.Name))
            {
                ModelState
                    .AddModelError("", "Trail Exists");
                return StatusCode(404, ModelState);
            }

            var trailToDb = _mapper.Map<Trail>(trailNew);
            if (!_trailRepo.CreateTrail(trailToDb))
            {
                ModelState
                    .AddModelError("", $"Some thing wrong when create the record {trailToDb.Name}");
                return StatusCode(500, ModelState);
            }
            return CreatedAtRoute("GetTrail", new { trailId = trailToDb.Id }, trailToDb);
        }

        [HttpPatch("{trailId}", Name = "UpdateTrail")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdateTrail (int trailId,[FromBody] TrailUpdateDto trailUpdate)
        {
            if (trailUpdate == null || trailId != trailUpdate.Id)
            {
                return BadRequest(ModelState);
            }

            var trailToDb = _mapper.Map<Trail>(trailUpdate);
            if (!_trailRepo.UpdateTrail(trailToDb))
            {
                ModelState
                    .AddModelError("", $"Some thing wrong when update the record {trailToDb.Name}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        [HttpDelete("{trailId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult DeleteTrail(int trailId)
        {
            if (!_trailRepo.TrailExists(trailId))
            {
                return NotFound(ModelState);
            }

            var trailInDb = _trailRepo.GetTrail(trailId);
            if (!_trailRepo.DeleteTrail(trailId))
            {
                ModelState
                    .AddModelError("", $"Some thing wrong when delete the record {trailInDb.Name}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }


    }
}
