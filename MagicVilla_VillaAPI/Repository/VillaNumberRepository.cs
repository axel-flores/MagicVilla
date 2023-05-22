using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Repository.IRepository;

namespace MagicVilla_VillaAPI.Repository
{
    public class VillaNumberRepository : Repository<VillaNumber>, IVillaNumberRepository
    {
        private readonly MagicVillaDbContext _db;

        public VillaNumberRepository(MagicVillaDbContext db): base(db)
        {
            _db = db;
        }

        public async Task<VillaNumber> UpdateAsync(VillaNumber villaNumber)
        {
            villaNumber.UpdatedDate = DateTime.Now;
            _db.VillaNumbers.Update(villaNumber); 
            await _db.SaveChangesAsync();
            return villaNumber;
        }

        
    }
}
