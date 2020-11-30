using System.Collections.Generic;

namespace RpgApi.Models
{
    public class Disputa
    {
        public int AtacanteId { get; set; }
        public int OponenteId { get; set; }
        public string Narracao { get; set; }
        public int HabilidadeId { get; set; }
        public List<int> ListaIdPersonagens { get; set; }
        public List<string> Resultados { get; set; } = new List<string>();
    }
}