using MonoLegal.Domain.Entities;
using MonoLegal.Domain.Enums;
using MonoLegal.Infrastructure.Persistence;
using MongoDB.Driver;

namespace MonoLegal.Infrastructure.Seed
{
    public static class DataSeeder
    {
        public static async Task SeedAsync(MongoDbContext context)
        {
            var count = await context.Clientes.CountDocumentsAsync(_ => true);
            if (count > 0) return;

            var clientes = new List<Cliente>
            {
                new() { Nombre = "Carlos Mendoza", Email = "carlos.mendoza@email.com", Telefono = "3001234567" },
                new() { Nombre = "Laura Quintero", Email = "laura.quintero@email.com", Telefono = "3107654321" },
                new() { Nombre = "Andrés Vargas", Email = "andres.vargas@email.com", Telefono = "3209876543" }
            };

            await context.Clientes.InsertManyAsync(clientes);

            var guardados = await context.Clientes.Find(_ => true).ToListAsync();

            var facturas = new List<Factura>
            {
                new()
                {
                    ClienteId = guardados[0].Id,
                    Numero = "FAC-2024-001",
                    Monto = 1500000,
                    FechaEmision = DateTime.UtcNow.AddMonths(-3),
                    FechaVencimiento = DateTime.UtcNow.AddMonths(-1),
                    Estado = EstadoFactura.PrimerRecordatorio,
                    Descripcion = "Honorarios asesoría legal enero 2024"
                },
                new()
                {
                    ClienteId = guardados[1].Id,
                    Numero = "FAC-2024-002",
                    Monto = 2800000,
                    FechaEmision = DateTime.UtcNow.AddMonths(-4),
                    FechaVencimiento = DateTime.UtcNow.AddMonths(-2),
                    Estado = EstadoFactura.SegundoRecordatorio,
                    Descripcion = "Representación proceso laboral"
                },
                new()
                {
                    ClienteId = guardados[2].Id,
                    Numero = "FAC-2024-003",
                    Monto = 950000,
                    FechaEmision = DateTime.UtcNow.AddMonths(-2),
                    FechaVencimiento = DateTime.UtcNow.AddDays(-15),
                    Estado = EstadoFactura.PrimerRecordatorio,
                    Descripcion = "Consultoría contratos comerciales"
                }
            };

            await context.Facturas.InsertManyAsync(facturas);
        }
    }
}
