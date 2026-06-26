using Microsoft.AspNetCore.Mvc;
using MonoLegal.Application.Interfaces;

namespace MonoLegal.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificacionesController : ControllerBase
    {
        private readonly INotificacionService _notificacionService;

        public NotificacionesController(INotificacionService notificacionService)
        {
            _notificacionService = notificacionService;
        }

        [HttpPost("procesar")]
        public async Task<IActionResult> ProcesarNotificaciones()
        {
            var resultados = await _notificacionService.ProcesarNotificacionesAsync();
            return Ok(resultados);
        }
    }
}
