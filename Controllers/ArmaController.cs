using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RpgApi.Data;
using RpgApi.Models;

namespace RpgApi.Controllers
{
    [Authorize]
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
            //Buscar personagem de acordo com a claim do token
            Personagem personagem = await _context.Personagens
                .FirstOrDefaultAsync(p => p.Id == novaArma.PersonagemId && 
                p.Usuario.Id == ObterUsuarioId());

            //Se não achar ninguém com o Id correspondente
            if(personagem == null)
                return BadRequest("Seu usuário não contém personagens com o Id do personagem informado");

            await _context.Armas.AddAsync(novaArma);
            await _context.SaveChangesAsync();

            List<Arma> armas = await _context.Armas.Where(a => a.PersonagemId == novaArma.PersonagemId).ToListAsync();

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


        [HttpGet("Sorteio")]
        public IActionResult Sorteio()
        {
            int numero = new Random().Next(10);

            return Ok(numero);
        }


        private readonly DataContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ArmaController(DataContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        private int ObterUsuarioId()
        {
            return int.Parse(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
        }
    
    }
}