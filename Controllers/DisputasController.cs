using System;
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
                int dano = ph. Habilidade.Dano + (new Random().Next(atacante.Inteligencia));
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

        private readonly DataContext _context;
        public DisputasController(DataContext context)
        {
            _context = context;
        }

    }
}