using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using Microsoft.AspNetCore.Mvc;

namespace MagicVilla_VillaAPI.Controllers
{
    [Route("api/VillaAPI")]
    [ApiController]
    public class VillaAPIController : ControllerBase
    {
        
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<VillaDTO>> GetVillas()
        {
            return Ok(VillaStore.villaList);
        }

        /*
         * ActionResult<VillaDTO> especifica el tipo de objeto que va a regresar
         * otra forma es poner el tipo en ProducesResponseType, ejemplo:
         * [ProducesResponseType(200, Type = typeof(VillaDTO))]
         */
        [HttpGet("{id:int}", Name="GetVilla")]
        //[ProducesResponseType(200)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<VillaDTO> GetVilla(int id)
        {
            if (id == 0) { return BadRequest(); }

            var villa = VillaStore.villaList.FirstOrDefault(v => v.Id == id);

            if (villa == null) { NotFound(); }

            return Ok(villa);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<VillaDTO> PostVilla([FromBody] VillaDTO villaDTO)
        {
            if(villaDTO == null)
            {
                return BadRequest();
            }

            if (villaDTO.Id > 0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError );
            }

            villaDTO.Id = VillaStore.villaList.OrderByDescending(u => u.Id).FirstOrDefault().Id + 1;
            VillaStore.villaList.Add(villaDTO);

            //var villaBd = VillaStore.villaList.FirstOrDefault( v => v.Id == villaDTO.Id);

            //if (villaBd == null) {
            //    villaBd = new VillaDTO(); 
            //    villaBd.Id = villaDTO.Id;
            //    villaBd.Name = villaDTO.Name;
            
            //    VillaStore.villaList.Add(villaBd);
            //}
            
            //return Ok(villaDTO);

            //esto devuelve la ruta en la que se creo, invoca el metodo getvilla
            return CreatedAtRoute("GetVilla", new {id = villaDTO.Id}, villaDTO);
            //CreatedAtRoute devuelve el codigo 201
        }
    }
}
