namespace MonoLegal.Application.DTOs
{
    public class NotificacionResultDto
    {
        public string FacturaId { get; set; } = string.Empty;
        public string ClienteEmail { get; set; } = string.Empty;
        public string EstadoAnterior { get; set; } = string.Empty;
        public string EstadoNuevo { get; set; } = string.Empty;
        public bool EmailEnviado { get; set; }
        public string? Error { get; set; }
    }
}
