using AutoMapper;
using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Logging;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using MagicVilla_VillaAPI.Repository;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla_VillaAPI.Controllers
{
    [Route("api/VillaAPI")]
    [ApiController]
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
        public VillaAPIController(IVillaRepository dbVilla, IMapper mapper)
        {
            _mapper = mapper;
            _dbVilla = dbVilla;
        }


        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<VillaDTO>>> GetVillas()
        {
            //_logger.LogInformation("Obteniendo todas las villas");
            //_logger.Log("Obteniendo todas las villas","");

            //usando autommaper
            IEnumerable<Villa> villas = await _dbVilla.GetAllAsync();
            return Ok(_mapper.Map<List<VillaDTO>>(villas));
        }

        /*
         * ActionResult<VillaDTO> especifica el tipo de objeto que va a regresar
         * otra forma es poner el tipo en ProducesResponseType, ejemplo:
         * [ProducesResponseType(200, Type = typeof(VillaDTO))]
         */
        [HttpGet("{id:int}", Name = "GetVilla")]
        //[ProducesResponseType(200)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<VillaDTO>> GetVilla(int id)
        {
            if (id == 0) {
                //_logger.Log("Error al obtener la villa con el id "+id, "error");
                return BadRequest(); }

            //var villa = VillaStore.villaList.FirstOrDefault(v => v.Id == id);
            var villa = await _dbVilla.GetAsync(v => v.Id == id);

            if (villa == null) { NotFound(); }

            return Ok(_mapper.Map<VillaDTO>(villa));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<VillaDTO>> PostVilla([FromBody] VillaCreateDTO createDTO)
        {
           

            if (await _dbVilla.GetAsync(v => v.Name.ToLower() == createDTO.Name.ToLower()) != null)
            {
                ModelState.AddModelError("CustomError", "Villa already exists!");
                return BadRequest(ModelState);
            }

            if (createDTO == null)
            {
                return BadRequest(createDTO);
            }

            Villa model = _mapper.Map<Villa>(createDTO);

            await _dbVilla.CreateAsync(model);

            //esto devuelve la ruta en la que se creo, invoca el metodo getvilla
            return CreatedAtRoute("GetVilla", new { id = model.Id }, model);
            //CreatedAtRoute devuelve el codigo 201
        }

        [HttpDelete("{id:int}", Name = "DeleteVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> DeleteVilla(int id)
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
            return NoContent();
        }

        [HttpPut("{id:int}", Name = "UpdateVilla")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> UpdateVilla(int id, [FromBody] VillaUpdateDTO updateDTO)
        {
            if (updateDTO == null || id != updateDTO.Id)
            {
                return BadRequest();
            }

            Villa model = _mapper.Map<Villa>(updateDTO);

            await _dbVilla.UpdateAsync(model);

            return NoContent();
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

            var villa = await _dbVilla.GetAsync(v => v.Id == id,tracked:false);

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
