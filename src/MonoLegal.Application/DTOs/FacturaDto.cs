namespace MonoLegal.Application.DTOs
{
    public class FacturaDto
    {
        public string Id { get; set; } = string.Empty;
        public string ClienteId { get; set; } = string.Empty;
        public string ClienteNombre { get; set; } = string.Empty;
        public string ClienteEmail { get; set; } = string.Empty;
        public string Numero { get; set; } = string.Empty;
        public decimal Monto { get; set; }
        public DateTime FechaEmision { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public string Estado { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
    }
}
