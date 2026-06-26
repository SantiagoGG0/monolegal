using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MonoLegal.Domain.Entities
{
    public class Factura
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonRepresentation(BsonType.ObjectId)]
        public string ClienteId { get; set; } = string.Empty;

        public string Numero { get; set; } = string.Empty;
        public decimal Monto { get; set; }
        public DateTime FechaEmision { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public string Estado { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
    }
}
