using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RpgApi.Data;
using RpgApi.Models;

namespace RpgApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ArmaController : ControllerBase
    {
        private Arma a = new Arma();
                
        private static List<Arma> armas = new List<Arma>
        {
            new Arma() {Id = 1, Nome = "The Master Sword", Dano = 8000}
        };

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(armas);
        }


        //Listar todas as armas
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAsync()
        {
            List<Arma> armas = await _context.Armas.ToListAsync();

            return Ok(armas);
        }

        //Filtrar arma pela ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSingleAsync(int id)
        {

            Arma a = await _context.Armas.FirstOrDefaultAsync(arma => arma.Id == id);
            
            return Ok(a);
        }


        //Adicionar uma arma 
        [HttpPost]
        public async Task<IActionResult> AddArmaAsync(Arma novaArma)
        {
            
            await _context.Armas.AddAsync(novaArma);
            await _context.SaveChangesAsync();

            List<Arma> armas = await _context.Armas.ToListAsync();

            return Ok(armas);
        }


        //Atualizar uma arma

        [HttpPut]
        public async Task<IActionResult> UpdateArmaAsync(Arma a)
        {
            _context.Armas.Update(a);
            await _context.SaveChangesAsync();

            return Ok(armas);
        }


        //Deletar arma
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync (int id)
        {
            Arma armaRemover = await _context.Armas.FirstOrDefaultAsync(arma => arma.Id  == id);
            _context.Armas.Remove(armaRemover);
            await _context.SaveChangesAsync();

            List<Arma> armas = await _context.Armas.ToListAsync();

            return Ok(armas);
        } 

        private readonly DataContext _context;

        public ArmaController(DataContext context)
        {
            _context = context;
        }
    
}
}