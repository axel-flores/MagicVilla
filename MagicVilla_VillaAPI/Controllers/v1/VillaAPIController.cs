using AutoMapper;
using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Logging;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using MagicVilla_VillaAPI.Repository;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace MagicVilla_VillaAPI.Controllers.v1
{
    [Route("api/v{version:apiVersion}/VillaAPI")]
    [ApiController]
    [ApiVersion("1.0")]
    public class VillaAPIController : ControllerBase
    {

        //private readonly ILogger<VillaAPIController> _logger;

        //custom logger
        //private readonly ILogging _logger;

        //public VillaAPIController(ILogger<VillaAPIController> logger)

        //logger v2
        //public VillaAPIController(ILogging logger)
        //{
        //    _logger = logger;
        //}



        private readonly IMapper _mapper;
        private readonly IVillaRepository _dbVilla;
        protected APIResponse _apiResponse;

        public VillaAPIController(IVillaRepository dbVilla, IMapper mapper)
        {
            _mapper = mapper;
            _dbVilla = dbVilla;
            _apiResponse = new();
        }


        [HttpGet]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetVillas()
        {
            //_logger.LogInformation("Obteniendo todas las villas");
            //_logger.Log("Obteniendo todas las villas","");

            //usando autommaper
            IEnumerable<Villa> villas = await _dbVilla.GetAllAsync();
            _apiResponse.Result = _mapper.Map<List<VillaDTO>>(villas);
            _apiResponse.StatusCode = HttpStatusCode.OK;

            return Ok(_apiResponse);
        }

        /*
         * ActionResult<VillaDTO> especifica el tipo de objeto que va a regresar
         * otra forma es poner el tipo en ProducesResponseType, ejemplo:
         * [ProducesResponseType(200, Type = typeof(VillaDTO))]
         */
        [HttpGet("{id:int}", Name = "GetVilla")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        //[ProducesResponseType(200)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> GetVilla(int id)
        {
            if (id == 0)
            {
                //_logger.Log("Error al obtener la villa con el id "+id, "error");
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_apiResponse);
            }

            //var villa = VillaStore.villaList.FirstOrDefault(v => v.Id == id);
            var villa = await _dbVilla.GetAsync(v => v.Id == id);

            if (villa == null) { NotFound(); }

            _apiResponse.Result = _mapper.Map<VillaDTO>(villa);
            _apiResponse.StatusCode = HttpStatusCode.OK;

            return Ok(_apiResponse);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> PostVilla([FromBody] VillaCreateDTO createDTO)
        {


            if (await _dbVilla.GetAsync(v => v.Name.ToLower() == createDTO.Name.ToLower()) != null)
            {
                ModelState.AddModelError("ErrorMessages", "Villa already exists!");
                return BadRequest(ModelState);
            }

            if (createDTO == null)
            {
                return BadRequest(createDTO);
            }

            Villa villa = _mapper.Map<Villa>(createDTO);

            await _dbVilla.CreateAsync(villa);

            _apiResponse.Result = _mapper.Map<VillaDTO>(villa);
            _apiResponse.StatusCode = HttpStatusCode.Created;
            //return Ok(_apiResponse);

            //esto devuelve la ruta en la que se creo, invoca el metodo getvilla
            return CreatedAtRoute("GetVilla", new { id = villa.Id }, _apiResponse);
            //CreatedAtRoute devuelve el codigo 201
        }

        [HttpDelete("{id:int}", Name = "DeleteVilla")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> DeleteVilla(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }

            var villa = await _dbVilla.GetAsync(u => u.Id == id);
            if (villa == null)
            {
                return NotFound();
            }

            await _dbVilla.RemoveAsync(villa);

            _apiResponse.StatusCode = HttpStatusCode.NoContent;
            _apiResponse.IsSuccess = true;
            return Ok(_apiResponse);
        }

        [Authorize(Roles = "admin")]
        [HttpPut("{id:int}", Name = "UpdateVilla")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult<APIResponse>> UpdateVilla(int id, [FromBody] VillaUpdateDTO updateDTO)
        {
            if (updateDTO == null || id != updateDTO.Id)
            {
                return BadRequest();
            }

            Villa model = _mapper.Map<Villa>(updateDTO);

            await _dbVilla.UpdateAsync(model);
            _apiResponse.StatusCode = HttpStatusCode.NoContent;
            _apiResponse.IsSuccess = true;
            return Ok(_apiResponse);
        }


        //para mayor referencia jsonpatch.com
        [HttpPatch("{id:int}", Name = "UpdatedPartialVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdatedPartialVilla(int id, JsonPatchDocument<VillaUpdateDTO> patchDTO)
        {
            if (patchDTO == null || id == 0)
            {
                return BadRequest();
            }

            var villa = await _dbVilla.GetAsync(v => v.Id == id, tracked: false);

            VillaUpdateDTO villaDTO = _mapper.Map<VillaUpdateDTO>(villa);

            if (villa == null)
            {
                return BadRequest();
            }

            patchDTO.ApplyTo(villaDTO, ModelState);

            Villa model = _mapper.Map<Villa>(villaDTO);

            await _dbVilla.UpdateAsync(model);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return NoContent();
        }



    }
}
