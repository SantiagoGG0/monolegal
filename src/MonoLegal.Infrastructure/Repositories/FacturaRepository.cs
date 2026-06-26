using MongoDB.Driver;
using MonoLegal.Domain.Entities;
using MonoLegal.Domain.Enums;
using MonoLegal.Domain.Interfaces;
using MonoLegal.Infrastructure.Persistence;

namespace MonoLegal.Infrastructure.Repositories
{
    public class FacturaRepository : IFacturaRepository
    {
        private readonly MongoDbContext _context;

        public FacturaRepository(MongoDbContext context)
        {
            _context = context;
        }

        public async Task<List<Factura>> GetFacturasPendientesAsync()
        {
            var filter = Builders<Factura>.Filter.In(
                f => f.Estado,
                new[] { EstadoFactura.PrimerRecordatorio, EstadoFactura.SegundoRecordatorio }
            );
            return await _context.Facturas.Find(filter).ToListAsync();
        }

        public async Task<List<Factura>> GetFacturasByClienteIdAsync(string clienteId)
        {
            var filter = Builders<Factura>.Filter.Eq(f => f.ClienteId, clienteId);
            return await _context.Facturas.Find(filter).ToListAsync();
        }

        public async Task<List<Factura>> GetAllAsync()
        {
            return await _context.Facturas.Find(_ => true).ToListAsync();
        }

        public async Task UpdateEstadoAsync(string facturaId, string nuevoEstado)
        {
            var filter = Builders<Factura>.Filter.Eq(f => f.Id, facturaId);
            var update = Builders<Factura>.Update.Set(f => f.Estado, nuevoEstado);
            await _context.Facturas.UpdateOneAsync(filter, update);
        }

        public async Task<Cliente?> GetClienteByIdAsync(string clienteId)
        {
            var filter = Builders<Cliente>.Filter.Eq(c => c.Id, clienteId);
            return await _context.Clientes.Find(filter).FirstOrDefaultAsync();
        }
    }
}
