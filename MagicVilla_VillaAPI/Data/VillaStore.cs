using MagicVilla_VillaAPI.Models.Dto;

namespace MagicVilla_VillaAPI.Data
{
    public static class VillaStore
    {
        public static List<VillaDTO> villaList = new List<VillaDTO>
            {
                new VillaDTO {Id = 1, Name = "villa pro", Occupancy = 4, sqft = 20},
                new VillaDTO { Id = 2, Name = "villa junior", Occupancy=1, sqft = 10},
                new VillaDTO { Id = 2, Name = "villa doble", Occupancy=2, sqft = 15}
            };
    }
}
