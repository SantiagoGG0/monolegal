namespace MonoLegal.Application.DTOs
{
    public class ClienteResumenDto
    {
        public string ClienteId { get; set; } = string.Empty;
        public string ClienteNombre { get; set; } = string.Empty;
        public string ClienteEmail { get; set; } = string.Empty;
        public int TotalFacturas { get; set; }
        public decimal MontoTotal { get; set; }
        public List<FacturaDto> Facturas { get; set; } = new();
    }
}
