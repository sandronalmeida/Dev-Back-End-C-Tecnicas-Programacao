namespace Projeto_Final.Domain.Entities
{
    public class Dispositivo
    {
        public string? MacAddress { get; set; }
        public string? EnderecoIP { get; set; }
        public string? Nome { get; set; }
        public string? Fabricante { get; set; }
        public double? ConsumoBanda { get; set; }
        public DateTime? DataConexao { get; set; }
    }
}
