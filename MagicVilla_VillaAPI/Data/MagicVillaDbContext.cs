using MagicVilla_VillaAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla_VillaAPI.Data
{
    public class MagicVillaDbContext : DbContext
    {
        public MagicVillaDbContext(DbContextOptions<MagicVillaDbContext> options) : base(options) { }

        public DbSet<Villa> Villas { get; set; }


    }
}
