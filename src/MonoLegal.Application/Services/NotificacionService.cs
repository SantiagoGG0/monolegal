using MonoLegal.Application.DTOs;
using MonoLegal.Application.Interfaces;
using MonoLegal.Domain.Enums;
using MonoLegal.Domain.Interfaces;

namespace MonoLegal.Application.Services
{
    public class NotificacionService : INotificacionService
    {
        private readonly IFacturaRepository _facturaRepo;
        private readonly IEmailService _emailService;

        public NotificacionService(IFacturaRepository facturaRepo, IEmailService emailService)
        {
            _facturaRepo = facturaRepo;
            _emailService = emailService;
        }

        public async Task<List<NotificacionResultDto>> ProcesarNotificacionesAsync()
        {
            var resultados = new List<NotificacionResultDto>();
            var facturas = await _facturaRepo.GetFacturasPendientesAsync();

            foreach (var factura in facturas)
            {
                var resultado = new NotificacionResultDto
                {
                    FacturaId = factura.Id,
                    EstadoAnterior = factura.Estado
                };

                try
                {
                    var cliente = await _facturaRepo.GetClienteByIdAsync(factura.ClienteId);
                    if (cliente == null)
                    {
                        resultado.Error = "Cliente no encontrado";
                        resultados.Add(resultado);
                        continue;
                    }

                    resultado.ClienteEmail = cliente.Email;

                    string nuevoEstado;
                    string asunto;
                    string cuerpo;

                    if (factura.Estado == EstadoFactura.PrimerRecordatorio)
                    {
                        nuevoEstado = EstadoFactura.SegundoRecordatorio;
                        asunto = $"Segundo recordatorio de pago - Factura {factura.Numero}";
                        cuerpo = ConstruirCuerpoEmail(
                            cliente.Nombre,
                            factura.Numero,
                            factura.Monto,
                            "Su factura ha pasado a segundo recordatorio de pago. " +
                            "Por favor regularice su situación para evitar la desactivación de su cuenta."
                        );
                    }
                    else
                    {
                        nuevoEstado = EstadoFactura.Desactivado;
                        asunto = $"Cuenta desactivada - Factura {factura.Numero}";
                        cuerpo = ConstruirCuerpoEmail(
                            cliente.Nombre,
                            factura.Numero,
                            factura.Monto,
                            "Lamentamos informarle que su cuenta ha sido desactivada por falta de pago. " +
                            "Para reactivarla comuníquese con nuestro equipo."
                        );
                    }

                    await _emailService.SendAsync(cliente.Email, asunto, cuerpo);
                    await _facturaRepo.UpdateEstadoAsync(factura.Id, nuevoEstado);

                    resultado.EstadoNuevo = nuevoEstado;
                    resultado.EmailEnviado = true;
                }
                catch (Exception ex)
                {
                    resultado.Error = ex.Message;
                    resultado.EmailEnviado = false;
                }

                resultados.Add(resultado);
            }

            return resultados;
        }

        public async Task<List<ClienteResumenDto>> GetResumenClientesAsync()
        {
            var facturas = await _facturaRepo.GetAllAsync();
            var grupos = facturas.GroupBy(f => f.ClienteId);
            var resumenes = new List<ClienteResumenDto>();

            foreach (var grupo in grupos)
            {
                var cliente = await _facturaRepo.GetClienteByIdAsync(grupo.Key);
                if (cliente == null) continue;

                var listaFacturas = grupo.Select(f => new FacturaDto
                {
                    Id = f.Id,
                    ClienteId = f.ClienteId,
                    ClienteNombre = cliente.Nombre,
                    ClienteEmail = cliente.Email,
                    Numero = f.Numero,
                    Monto = f.Monto,
                    FechaEmision = f.FechaEmision,
                    FechaVencimiento = f.FechaVencimiento,
                    Estado = f.Estado,
                    Descripcion = f.Descripcion
                }).ToList();

                resumenes.Add(new ClienteResumenDto
                {
                    ClienteId = cliente.Id,
                    ClienteNombre = cliente.Nombre,
                    ClienteEmail = cliente.Email,
                    TotalFacturas = listaFacturas.Count,
                    MontoTotal = listaFacturas.Sum(f => f.Monto),
                    Facturas = listaFacturas
                });
            }

            return resumenes;
        }

        private static string ConstruirCuerpoEmail(string nombre, string numeroFactura, decimal monto, string mensaje)
        {
            return $@"Estimado/a {nombre},

{mensaje}

Detalle de la factura:
- Número: {numeroFactura}
- Monto pendiente: ${monto:N2}

Para consultas o pagos comuníquese con nuestro equipo de soporte.

Atentamente,
MonoLegal";
        }
    }
}
