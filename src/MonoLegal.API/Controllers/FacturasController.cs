using Microsoft.AspNetCore.Mvc;
using MonoLegal.Application.Interfaces;

namespace MonoLegal.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FacturasController : ControllerBase
    {
        private readonly INotificacionService _notificacionService;

        public FacturasController(INotificacionService notificacionService)
        {
            _notificacionService = notificacionService;
        }

        [HttpGet("resumen")]
        public async Task<IActionResult> GetResumen()
        {
            var resumen = await _notificacionService.GetResumenClientesAsync();
            return Ok(resumen);
        }
    }
}
