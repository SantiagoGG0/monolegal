using MonoLegal.Domain.Entities;

namespace MonoLegal.Domain.Interfaces
{
    public interface IFacturaRepository
    {
        Task<List<Factura>> GetFacturasPendientesAsync();
        Task<List<Factura>> GetFacturasByClienteIdAsync(string clienteId);
        Task<List<Factura>> GetAllAsync();
        Task UpdateEstadoAsync(string facturaId, string nuevoEstado);
        Task<Cliente?> GetClienteByIdAsync(string clienteId);
    }
}
