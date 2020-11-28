namespace RpgApi.Models
{
    public class Disputa
    {
        public int AtacanteId { get; set; }
        public int OponenteId { get; set; }
        public string Narracao { get; set; }
        public int HabilidadeId { get; set; }
    }
}