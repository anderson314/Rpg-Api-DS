using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using RpgApi.Models;
using System.Linq;
using RpgApi.Models.Enuns;


namespace RpgApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PersonagemController : ControllerBase
    {
       // private Personagem p = new Personagem();

        //Lista de números
        List<int> nomeListaNumerosInteiros = new List<int>();

        List<Personagem> listaPersonagens = new List<Personagem>();

        //Lista de objetos da Classe Personagem
        private static List<Personagem> personagens = new List<Personagem> {
            new Personagem() {Id = 1,}, //Frodo Cavaleiro
            new Personagem() { Id = 2, Nome= "Sam", PontosVida = 100, Forca = 15, Defesa = 25, Inteligencia = 30, Classe = ClasseEnum.Cavaleiro },
            new Personagem() { Id = 3, Nome= "Galadriel", PontosVida = 100, Forca = 18, Defesa = 21, Inteligencia = 35, Classe = ClasseEnum.Clerigo },
            new Personagem() { Id = 4, Nome= "Gandalf", PontosVida = 100, Forca = 19, Defesa = 18, Inteligencia = 37, Classe = ClasseEnum.Mago },
            new Personagem() { Id = 5, Nome= "Hobbit", PontosVida = 100, Forca = 20, Defesa = 17, Inteligencia = 31, Classe = ClasseEnum.Cavaleiro },
            new Personagem() { Id = 6, Nome= "Celeborn", PontosVida = 100, Forca = 21, Defesa = 13, Inteligencia = 34, Classe = ClasseEnum.Clerigo },
            new Personagem() { Id = 7, Nome= "Radgast", PontosVida = 100, Forca = 25, Defesa = 11, Inteligencia = 35, Classe = ClasseEnum.Mago },  
        };

        [HttpGet("GetAll")]
        public IActionResult Get()
        {
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
        public IActionResult GetSingle (int id)
        {

            return Ok(personagens.FirstOrDefault(apelido => apelido.Id == id)); //pegar um personagem onde 'apelido.id' seja igual ao 'id'
        
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

        //Remover um personagem pelo Id
        [HttpGet("RemoverById/{id}")]
        public IActionResult Remover(int id)
        {
            Personagem pRemover = personagens.Find(elementos => elementos.Id == id);

            personagens.Remove(pRemover);

            return Ok(personagens);
        }
        

        //Adicionar personagem na lista
        [HttpPost]
        public IActionResult AddPersonagens(Personagem novoPersonagem)
        {
            if(novoPersonagem.Forca > 100)
                return NotFound("Cara, vc não pode adicionar um personagem com força maior a 100.");

            personagens.Add(novoPersonagem);
            return Ok(personagens);
        }


        [HttpPut]
        public IActionResult UpdatePersonagem(Personagem p)
        {
            Personagem personagemAlterado = personagens.Find(pers => pers.Id == p.Id);
            personagemAlterado.Nome = p.Nome;
            personagemAlterado.PontosVida = p.PontosVida;
            personagemAlterado.Forca = p.Forca;
            personagemAlterado.Defesa = p.Defesa;
            personagemAlterado.Inteligencia = p.Inteligencia;
            personagemAlterado.Classe = p.Classe;

            return Ok(personagens);
        }


        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            personagens.RemoveAll(pers => pers.Id == id);

            return Ok(personagens);
        }
    }
}



