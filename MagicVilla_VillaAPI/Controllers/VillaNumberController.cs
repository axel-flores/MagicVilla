using AutoMapper;
using Azure;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Runtime.CompilerServices;

namespace MagicVilla_VillaAPI.Controllers
{
    [Route("api/VillaNumberAPI")]
    [ApiController]
    public class VillaNumberController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IVillaNumberRepository _dbVillaNumber;
        private readonly IVillaRepository _dbVilla;
        protected APIResponse _apiResponse;

        public VillaNumberController(IVillaNumberRepository villaNumberRepository, IMapper mapper, IVillaRepository dbVilla)
        {
            _dbVillaNumber = villaNumberRepository;
            _dbVilla = dbVilla;
            _mapper = mapper;
            this._apiResponse = new APIResponse();
            _dbVilla = dbVilla;
        }

        [HttpGet()]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetVillaNumbers()
        {
            IEnumerable<VillaNumber> villaNumbers = await _dbVillaNumber.GetAllAsync(includeProperties: "Villa");
            _apiResponse.Result = _mapper.Map<List<VillaNumberDTO>>(villaNumbers);
            _apiResponse.StatusCode = HttpStatusCode.OK;
            return Ok(_apiResponse);
        }

        [HttpGet("{id:int}", Name = "GetVillaNumber")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> GetVillaNumber(int id)
        {
            if (id == 0)
            {
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_apiResponse);
            }

            var villaNumber = await _dbVillaNumber.GetAsync(v => v.VillaNum == id);

            if (villaNumber == null) { NotFound(); }

            _apiResponse.Result = _mapper.Map<VillaNumberDTO>(villaNumber);
            _apiResponse.StatusCode = HttpStatusCode.OK;

            return (_apiResponse);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> PostVillaNumber([FromBody] VillaNumberCreateDTO createDTO)
        {
            if (await _dbVillaNumber.GetAsync(v => v.VillaNum == createDTO.VillaNum) != null)
            {
                ModelState.AddModelError("ErrorMessages", "Numero de villa existente");
                return BadRequest(ModelState);
            }

            if (await _dbVilla.GetAsync(r => r.Id == createDTO.VillaID) == null)
            {
                ModelState.AddModelError("ErrorMessages", "Id de villa invalido");
                return BadRequest(ModelState);
            }

            if (createDTO == null)
            {
                return BadRequest(createDTO);
            }

            VillaNumber villaNumber = _mapper.Map<VillaNumber>(createDTO);

            await _dbVillaNumber.CreateAsync(villaNumber);

            _apiResponse.Result = _mapper.Map<VillaNumberDTO>(villaNumber);
            _apiResponse.StatusCode = HttpStatusCode.Created;

            return CreatedAtRoute("GetVilla", new { id = villaNumber.VillaNum }, _apiResponse);
        }

        [HttpDelete("{id:int}", Name = "DeleteVillaNumber")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> DeleteVillaNumber(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }
            var villaNumber = await _dbVillaNumber.GetAsync(u => u.VillaNum == id);
            if (villaNumber == null)
            {
                return NotFound();
            }

            await _dbVillaNumber.RemoveAsync(villaNumber);

            _apiResponse.StatusCode = HttpStatusCode.NoContent;
            _apiResponse.IsSuccess = true;
            return Ok(_apiResponse);
        }

        [HttpPut("{id:int}", Name = "UpdateVillaNumber")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> UpdateVillaNumber(int id, [FromBody] VillaNumberUpdateDTO updateDTO)
        {
            try
            {
                if (updateDTO == null || id != updateDTO.VillaNum)
                {
                    return BadRequest();
                }
                if (await _dbVilla.GetAsync(u => u.Id == updateDTO.VillaID) == null)
                {
                    ModelState.AddModelError("ErrorMessages", "Villa ID is Invalid!");
                    return BadRequest(ModelState);
                }
                VillaNumber model = _mapper.Map<VillaNumber>(updateDTO);

                await _dbVillaNumber.UpdateAsync(model);
                _apiResponse.StatusCode = HttpStatusCode.NoContent;
                _apiResponse.IsSuccess = true;
                return Ok(_apiResponse);
            }
            catch (Exception ex)
            {
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages
                     = new List<string>() { ex.ToString() };
            }
            return _apiResponse;
        }
    }
}
