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
    public class PersonagemHabilidadesController : ControllerBase
    {
        
        [HttpPost]
        public async Task<IActionResult> AddPersonagemHabilidadeAsync(PersonagemHabilidade novoPersonagemHabilidade)
        {
            Personagem personagem = await _context.Personagens
            .Include(p => p.Arma)
            .Include(p => p.PersonagemHabilidades).ThenInclude(ps => ps.Habilidade)
            .FirstOrDefaultAsync(p => p.Id ==novoPersonagemHabilidade.PersonagemId &&
                p.Usuario.Id == ObterUsuarioId());

            if(personagem == null)
                return BadRequest("Personagem não encontrado para o usuário em questão");

            Habilidade habilidade = await _context.Habilidades
                                .FirstOrDefaultAsync(s => s.Id == novoPersonagemHabilidade.HabilidadeId);
            
            if(habilidade == null)
            {
                BadRequest("Habilidade não encontrada");
            }

            PersonagemHabilidade ph = new PersonagemHabilidade();
            ph.Personagem = personagem;
            ph.Habilidade = habilidade;

            await _context.PersonagemHabilidades.AddAsync(ph);
            await _context.SaveChangesAsync();

            return Ok(ph);
        }


        [HttpPost("DeletePersonagemHabilidade")]
        public IActionResult Delete(PersonagemHabilidade ph)
        {   
            PersonagemHabilidade phRemover = _context.PersonagemHabilidades            
            .FirstOrDefault(phBusca => phBusca.PersonagemId == ph.PersonagemId
                            && phBusca.HabilidadeId == ph.HabilidadeId);   
            
            if(phRemover == null)
                return BadRequest("Personagem ou habilidades não encontrados com os Id's informados");
                                                       
            _context.PersonagemHabilidades.Remove(phRemover);            
            _context.SaveChanges();            
           
            return Ok("Dados removidos com sucesso.");
        }

        //Pegar habilidades de um personagem de acordo com usuario
        [HttpGet("{personagemId}")]
        public async Task<IActionResult> GetHabilidadesPersonagem(int personagemId)
        {
            List<PersonagemHabilidade> phLista = new List<PersonagemHabilidade>();
        
            phLista = await _context.PersonagemHabilidades
            .Include(p => p.Personagem)
            .Include(p => p.Habilidade)
            .Where(p => p.Personagem.Usuario.Id == ObterUsuarioId()
                    && p.Personagem.Id == Personagem.Id).ToListAsync();
            
            return Ok(phLista);
        }

        //Lista todas as habilidades
        [HttpGet("GetHabilidades")]
        public async Task<IActionResult> GetHabilidades()
        {
            List<Habilidade> habilidades = new List<habilidade>();

            habilidades = await _context.Habilidades.ToListAsync();

            return Ok(habilidades);
        }


        private int ObterUsuarioId()
        {
            return int.Parse(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
        }

        private readonly DataContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public PersonagemHabilidadesController(DataContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }
    }
}