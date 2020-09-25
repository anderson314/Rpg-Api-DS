using Microsoft.EntityFrameworkCore;
using RpgApi.Models;

namespace RpgApi.Data
{
    public class DataContext :  DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        public DbSet<Personagem> Personagens { get; set; }
    }
}