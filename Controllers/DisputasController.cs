using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RpgApi.Data;
using RpgApi.Models;

namespace RpgApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DisputasController : ControllerBase
    {
        
        [HttpPost("Arma")]
        public async Task<IActionResult> AtaqueComArma(Disputa d)
        {
            
            Personagem atacante = await _context.Personagens
                .Include(p => p.Arma)
                .FirstOrDefaultAsync(p => p.Id == d.AtacanteId);

            Personagem oponente = await _context.Personagens
                .FirstOrDefaultAsync(p => p.Id == d.OponenteId);


            int dano = atacante.Arma.Dano + (new Random().Next(atacante.Forca));

            dano = dano - new Random().Next(oponente.Defesa);

            if(dano > 0)
                oponente.PontosVida = oponente.PontosVida - (int)dano;
            if(oponente.PontosVida <= 0)
                d.Narracao = $"{oponente.Nome} foi derrotado";

            _context.Personagens.Update(oponente);
            await _context.SaveChangesAsync();

            StringBuilder dados = new StringBuilder();
            dados.AppendFormat("Atacante: {0}", atacante.Nome);
            dados.AppendFormat("Oponente: {0}", oponente.Nome);
            dados.AppendFormat("Pontos de vida do atacante: {0}", atacante.PontosVida);
            dados.AppendFormat("Pontos de vida do oponente: {0}", oponente.PontosVida);
            dados.AppendFormat("Dano {0}", dano);

            d.Narracao += dados.ToString();

            return Ok(d);
        }

        
        [HttpPost("Habilidade")]
        public async Task<IActionResult> AtaqueComHabilidade(Disputa d)
        {
            Personagem atacante = await _context.Personagens
                .Include(p => p.PersonagemHabilidades).ThenInclude(ph => ph.Habilidade)
                .FirstOrDefaultAsync(p => p.Id == d.AtacanteId);

            Personagem oponente = await _context.Personagens
                .FirstOrDefaultAsync(p => p.Id == d.OponenteId);

            PersonagemHabilidade ph = await _context.PersonagemHabilidades
                .Include(p => p.Habilidade)
                .FirstOrDefaultAsync(phBusca => phBusca.HabilidadeId == d.HabilidadeId);

            if(ph == null)
                d.Narracao = $"{atacante.Nome} não possui esta habilidade.";
            else
            {
                int dano = ph.Habilidade.Dano + (new Random().Next(atacante.Inteligencia));
                dano = dano - new Random().Next(oponente.Defesa);

                if(dano > 0)
                    oponente.PontosVida = oponente.PontosVida - (int)dano;
                if(oponente.PontosVida <= 0)
                    d.Narracao += $"{oponente.Nome} foi derrotado";

                _context.Personagens.Update(oponente);
                await _context.SaveChangesAsync();

                StringBuilder dados = new StringBuilder();
                dados.AppendFormat(" Atacante: {0}", atacante.Nome);
                dados.AppendFormat(" Oponente: {0}", oponente.Nome);
                dados.AppendFormat(" Pontos de vida do atacante: {0}", atacante.PontosVida);
                dados.AppendFormat(" Pontos de vida do oponente: {0}", oponente.PontosVida);
                dados.AppendFormat(" Dano: {0}", dano);

                d.Narracao += dados.ToString();
            }
            return Ok(d);
        }


        [HttpPost("DisputaEmGrupo")]
        public async Task<IActionResult> DisputaEmGrupo(Disputa d)
        {
            
            //Busca no banco de dados os personagens juntamente com as armas e  habilidades
            List<Personagem> personagens =
                await _context.Personagens
                .Include(p => p.Arma)
                .Include(p => p.PersonagemHabilidades).ThenInclude(ph => ph.Habilidade)
                .Where(p => d.ListaIdPersonagens.Contains(p.Id)).ToListAsync();


            bool derrotado = false;
            while(!derrotado)
            {
                foreach(Personagem atacante in personagens)
                {
                    //Cria uma lista de oponentes excluindo o personagem que fará o ataque
                    List<Personagem> oponentes = personagens.Where(p => p.Id != atacante.Id).ToList();
                    //Faz um sorteio que escolherá o oponente entre a lista criada anteriormente
                    Personagem oponente = oponentes[new Random().Next(oponentes.Count)];
                    
                    int dano = 0;
                    string ataqueUsado = string.Empty;

                    bool ataqueUsaArma = new Random().Next(2) == 0;

                    if(ataqueUsaArma)
                    {
                        //Programação do ataque com arma
                        dano = atacante.Arma.Dano + (new Random().Next(atacante.Forca));
                        dano = dano - new Random().Next(oponente.Defesa);
                        ataqueUsado = atacante.Arma.Nome;

                        if(dano > 0)
                            oponente.PontosVida = oponente.PontosVida - (int)dano;  
                    }
                    else
                    {
                        //Programação do ataque com habilidade
                        int sorteioHabilidadeId = new Random().Next(atacante.PersonagemHabilidades.Count);
                        Habilidade habilidadeEscolhida = atacante.PersonagemHabilidades[sorteioHabilidadeId].Habilidade;
                        ataqueUsado = habilidadeEscolhida.Nome;

                        dano = habilidadeEscolhida.Dano + (new Random().Next(atacante.Inteligencia));
                        dano = dano - new Random().Next(oponente.Defesa);

                        if(dano > 0)
                            oponente.PontosVida = oponente.PontosVida - (int)dano;
                    }

                    string resultado = string.Format("{0} atacou {1} usando {2} com dano {3}.", atacante.Nome, oponente.Nome, ataqueUsado, dano);
                    d.Resultados.Add(resultado);

                    if(oponente.PontosVida <= 0)
                    {
                        derrotado = true;
                        atacante.Vitorias++;
                        oponente.Derrotas++;
                        d.Resultados.Add($"{oponente.Nome} foi derrotado.");
                        d.Resultados.Add($"{atacante.Nome} ganhou com {atacante.PontosVida} restantes.");
                        break;
                    }
                }

                //Fora do foreach: Recomposição dos pontos de vida, atualização do número de lutas.
                personagens.ForEach(p => 
                {
                    p.Disputas++;
                    p.PontosVida = 100;
                });  
            }
            _context.Personagens.UpdateRange(personagens);
            await _context.SaveChangesAsync();

            return Ok(d);  
        }
        
        private readonly DataContext _context;
        public DisputasController(DataContext context)
        {
            _context = context;
        }
    
    }
}

