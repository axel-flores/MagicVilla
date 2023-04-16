using MagicVilla_VillaAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace MagicVilla_VillaAPI.Controllers
{
    [Route("api/VillaAPI")]
    [ApiController]
    public class VillaAPIController : ControllerBase
    {
        //public IActionResult Index()
        //{
        //    return View();
        //}
        [HttpGet]
        public IEnumerable<Villa> GetVillas()
        {
            return new List<Villa>
            {
                new Villa {Id = 1, Name = "villa pro"},
                new Villa { Id = 2, Name = "villa junior" }
            };
        }
    }
}
