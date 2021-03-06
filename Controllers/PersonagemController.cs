using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using RpgApi.Models;
using System.Linq;
using RpgApi.Models.Enuns;
using RpgApi.Data;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace RpgApi.Controllers
{
    [Authorize(Roles="Jogador, Admin")]
    [ApiController]
    [Route("[controller]")]
    public class PersonagemController : ControllerBase
    {
       // private Personagem p = new Personagem();


        //Lista de objetos da Classe Personagem
        private static List<Personagem> personagens = new List<Personagem> {
            new Personagem() { Id = 1,}, //Frodo Cavaleiro
            new Personagem() { Id = 2, Nome= "Sam", PontosVida = 100, Forca = 15, Defesa = 25, Inteligencia = 30, Classe = ClasseEnum.Cavaleiro },
            new Personagem() { Id = 3, Nome= "Galadriel", PontosVida = 100, Forca = 18, Defesa = 21, Inteligencia = 35, Classe = ClasseEnum.Clerigo },
            new Personagem() { Id = 4, Nome= "Gandalf", PontosVida = 100, Forca = 19, Defesa = 18, Inteligencia = 37, Classe = ClasseEnum.Mago },
            new Personagem() { Id = 5, Nome= "Hobbit", PontosVida = 100, Forca = 20, Defesa = 17, Inteligencia = 31, Classe = ClasseEnum.Cavaleiro },
            new Personagem() { Id = 6, Nome= "Celeborn", PontosVida = 100, Forca = 21, Defesa = 13, Inteligencia = 34, Classe = ClasseEnum.Clerigo },
            new Personagem() { Id = 7, Nome= "Radgast", PontosVida = 100, Forca = 25, Defesa = 11, Inteligencia = 35, Classe = ClasseEnum.Mago },  
        };


        [HttpGet("GetByUser")]
        public async Task<IActionResult> GetByUserAsync()
        {
            int id = int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);

            List<Personagem> personagens = await _context.Personagens.Where(c => c.Usuario.Id == id).ToListAsync();
            return Ok(personagens); 
        }


        //Listar todos os personagens
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAsync()
        {
            List<Personagem> personagens = new List<Personagem>();

            if(ObterUsuarioPerfil() == "Admin")
            {
                personagens = await _context.Personagens.ToListAsync();
            }
            else
            {
                personagens = await _context.Personagens.Where(p => p.Usuario.Id == ObterUsuarioId()).ToListAsync();
            }

            
            return Ok(personagens);
        }

        //filtrar personagem pela array
        /*
        [HttpGet("{posicao}")]
        public IActionResult GetSingle(int posicao)
        {
            return Ok(personagens[posicao]);
        }
        */

        
        //filtrar personagem pela ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSingleAsync (int id)
        {
            Personagem p = await _context.Personagens
            .Include(p => p.Arma)
            .Include(p => p.Usuario)
            .Include(p => p.PersonagemHabilidades).ThenInclude(ps => ps.Habilidade)
            .FirstOrDefaultAsync(p => p.Id == id &&
                p.Usuario.Id == ObterUsuarioId());
            //FirstOrDefaultAsync(apelido => apelido.Id == id);
            return Ok(p); //pegar um personagem onde 'apelido.id' seja igual ao 'id'
        
        }
        
        /*
        //filtrar um personagem pela força
        [HttpGet("GetForca/{forca}")] //a rota tem de ser diferente senão ele não vai saber se é pra pegar a força ou id
        public IActionResult GetByForca (int forca)
        {

            List<Personagem> listaFinal = personagens.FindAll( f => f.Forca == forca);
            return Ok(listaFinal);
        
        }
        */
        /*
        //Remover um personagem pelo Id
        [HttpGet("RemoverById/{id}")]
        public IActionResult Remover(int id)
        {
            Personagem pRemover = personagens.Find(elementos => elementos.Id == id);

            personagens.Remove(pRemover);

            return Ok(personagens);
        }
        */

        //Adicionar personagem na lista
        [HttpPost]
        public async Task<IActionResult> AddPersonagensAsync(Personagem novoPersonagem)
        {
            if(novoPersonagem.Forca > 100)
                return BadRequest("Cara, vc não pode adicionar um personagem com força maior a 100.");

            novoPersonagem.Usuario = await _context.Usuarios.FirstOrDefaultAsync(uBusca => uBusca.Id == ObterUsuarioId());


            await _context.Personagens.AddAsync(novoPersonagem);
            await _context.SaveChangesAsync();
            List<Personagem> personagens = await _context.Personagens.ToListAsync();
            return Ok(personagens);
        }

        //Atualizar um personagem existente
        [HttpPut]
        public async Task<IActionResult> UpdatePersonagemAsync(Personagem p)
        {
            //salvar id do personagem
            p.Usuario = await _context.Usuarios.FirstOrDefaultAsync(uBusca => uBusca.Id == ObterUsuarioId());

            _context.Personagens.Update(p);
            await _context.SaveChangesAsync();

            return Ok(personagens);
        }

        //Deletar personagem pela ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            Personagem pRemover = await _context.Personagens.FirstOrDefaultAsync(p => p.Id == id);
            _context.Personagens.Remove(pRemover);
            await _context.SaveChangesAsync();
            List<Personagem> personagems = await _context.Personagens.ToListAsync();

            return Ok(personagens);
        }


        //Obter Role da claim para verificar se é Admin ou Jogador
        private string ObterUsuarioPerfil()
        {
            return _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role);
        }


        private int ObterUsuarioId()
        {
            return int.Parse(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
        }

        private readonly DataContext _context;

        private readonly  IHttpContextAccessor _httpContextAccessor;
    
        public PersonagemController(DataContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;

            _httpContextAccessor = httpContextAccessor;
        }

    }
}



