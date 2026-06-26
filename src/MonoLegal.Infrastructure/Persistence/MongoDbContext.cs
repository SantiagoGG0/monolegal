using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MonoLegal.Domain.Entities;
using MonoLegal.Infrastructure.Configuration;

namespace MonoLegal.Infrastructure.Persistence
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _db;

        public MongoDbContext(IOptions<MongoDbSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            _db = client.GetDatabase(settings.Value.DatabaseName);
        }

        public IMongoCollection<Factura> Facturas => _db.GetCollection<Factura>("facturas");
        public IMongoCollection<Cliente> Clientes => _db.GetCollection<Cliente>("clientes");
    }
}
