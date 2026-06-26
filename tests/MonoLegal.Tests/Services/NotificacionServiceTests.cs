using Moq;
using MonoLegal.Application.Services;
using MonoLegal.Domain.Entities;
using MonoLegal.Domain.Enums;
using MonoLegal.Domain.Interfaces;

namespace MonoLegal.Tests.Services
{
    public class NotificacionServiceTests
    {
        private readonly Mock<IFacturaRepository> _repoMock;
        private readonly Mock<IEmailService> _emailMock;
        private readonly NotificacionService _service;

        public NotificacionServiceTests()
        {
            _repoMock = new Mock<IFacturaRepository>();
            _emailMock = new Mock<IEmailService>();
            _service = new NotificacionService(_repoMock.Object, _emailMock.Object);
        }

        [Fact]
        public async Task ProcesarNotificaciones_FacturaEnPrimerRecordatorio_ActualizaASegundoYEnviaEmail()
        {
            var factura = new Factura
            {
                Id = "abc123",
                ClienteId = "cli001",
                Numero = "FAC-001",
                Monto = 1000,
                Estado = EstadoFactura.PrimerRecordatorio
            };
            var cliente = new Cliente { Id = "cli001", Nombre = "Test", Email = "test@mail.com" };

            _repoMock.Setup(r => r.GetFacturasPendientesAsync()).ReturnsAsync(new List<Factura> { factura });
            _repoMock.Setup(r => r.GetClienteByIdAsync("cli001")).ReturnsAsync(cliente);
            _emailMock.Setup(e => e.SendAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                      .Returns(Task.CompletedTask);
            _repoMock.Setup(r => r.UpdateEstadoAsync(It.IsAny<string>(), It.IsAny<string>()))
                     .Returns(Task.CompletedTask);

            var resultados = await _service.ProcesarNotificacionesAsync();

            Assert.Single(resultados);
            Assert.Equal(EstadoFactura.SegundoRecordatorio, resultados[0].EstadoNuevo);
            Assert.True(resultados[0].EmailEnviado);
            _repoMock.Verify(r => r.UpdateEstadoAsync("abc123", EstadoFactura.SegundoRecordatorio), Times.Once);
            _emailMock.Verify(e => e.SendAsync("test@mail.com", It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task ProcesarNotificaciones_FacturaEnSegundoRecordatorio_ActualizaADesactivadoYEnviaEmail()
        {
            var factura = new Factura
            {
                Id = "def456",
                ClienteId = "cli002",
                Numero = "FAC-002",
                Monto = 2000,
                Estado = EstadoFactura.SegundoRecordatorio
            };
            var cliente = new Cliente { Id = "cli002", Nombre = "Otro", Email = "otro@mail.com" };

            _repoMock.Setup(r => r.GetFacturasPendientesAsync()).ReturnsAsync(new List<Factura> { factura });
            _repoMock.Setup(r => r.GetClienteByIdAsync("cli002")).ReturnsAsync(cliente);
            _emailMock.Setup(e => e.SendAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                      .Returns(Task.CompletedTask);
            _repoMock.Setup(r => r.UpdateEstadoAsync(It.IsAny<string>(), It.IsAny<string>()))
                     .Returns(Task.CompletedTask);

            var resultados = await _service.ProcesarNotificacionesAsync();

            Assert.Equal(EstadoFactura.Desactivado, resultados[0].EstadoNuevo);
            Assert.True(resultados[0].EmailEnviado);
            _repoMock.Verify(r => r.UpdateEstadoAsync("def456", EstadoFactura.Desactivado), Times.Once);
        }

        [Fact]
        public async Task ProcesarNotificaciones_ClienteNoExiste_RetornaErrorSinEnviarEmail()
        {
            var factura = new Factura
            {
                Id = "ghi789",
                ClienteId = "noexiste",
                Estado = EstadoFactura.PrimerRecordatorio
            };

            _repoMock.Setup(r => r.GetFacturasPendientesAsync()).ReturnsAsync(new List<Factura> { factura });
            _repoMock.Setup(r => r.GetClienteByIdAsync("noexiste")).ReturnsAsync((Cliente?)null);

            var resultados = await _service.ProcesarNotificacionesAsync();

            Assert.Single(resultados);
            Assert.False(resultados[0].EmailEnviado);
            Assert.Equal("Cliente no encontrado", resultados[0].Error);
            _emailMock.Verify(e => e.SendAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task ProcesarNotificaciones_SinFacturasPendientes_RetornaListaVacia()
        {
            _repoMock.Setup(r => r.GetFacturasPendientesAsync()).ReturnsAsync(new List<Factura>());

            var resultados = await _service.ProcesarNotificacionesAsync();

            Assert.Empty(resultados);
            _emailMock.Verify(e => e.SendAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task ProcesarNotificaciones_ErrorAlEnviarEmail_MarcaEmailNoEnviado()
        {
            var factura = new Factura
            {
                Id = "jkl000",
                ClienteId = "cli003",
                Numero = "FAC-003",
                Estado = EstadoFactura.PrimerRecordatorio
            };
            var cliente = new Cliente { Id = "cli003", Nombre = "Falla", Email = "falla@mail.com" };

            _repoMock.Setup(r => r.GetFacturasPendientesAsync()).ReturnsAsync(new List<Factura> { factura });
            _repoMock.Setup(r => r.GetClienteByIdAsync("cli003")).ReturnsAsync(cliente);
            _emailMock.Setup(e => e.SendAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                      .ThrowsAsync(new Exception("SMTP connection failed"));

            var resultados = await _service.ProcesarNotificacionesAsync();

            Assert.False(resultados[0].EmailEnviado);
            Assert.Contains("SMTP", resultados[0].Error);
            _repoMock.Verify(r => r.UpdateEstadoAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }
    }
}
