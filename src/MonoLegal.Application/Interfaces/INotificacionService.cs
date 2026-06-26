using MonoLegal.Application.DTOs;

namespace MonoLegal.Application.Interfaces
{
    public interface INotificacionService
    {
        Task<List<NotificacionResultDto>> ProcesarNotificacionesAsync();
        Task<List<ClienteResumenDto>> GetResumenClientesAsync();
    }
}
