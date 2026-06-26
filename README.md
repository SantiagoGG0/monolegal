# MonoLegal - Sistema de Notificaciones de Facturas

Sistema que conecta con MongoDB, detecta facturas en estado `primerrecordatorio` o `segundorecordatorio`, envía emails automáticos a los clientes y actualiza los estados en la base de datos.

## Arquitectura

Clean Architecture con 4 capas:

```
src/
├── MonoLegal.Domain/          → Entidades, enums, interfaces
├── MonoLegal.Application/     → Servicios de negocio, DTOs
├── MonoLegal.Infrastructure/  → MongoDB, MailKit (email), seed
└── MonoLegal.API/             → Controllers, Program.cs, DI

tests/
└── MonoLegal.Tests/           → xUnit + Moq

frontend/
└── monolegal-front/           → Angular 17 standalone
```

**Principios aplicados:** SOLID, Dependency Injection, Repository Pattern, Clean Code.

## Flujo de negocio

1. Al iniciar la API, se insertan 3 clientes con facturas de prueba (si la DB está vacía).
2. `POST /api/notificaciones/procesar`:
   - Busca facturas con estado `primerrecordatorio` → envía email → actualiza a `segundorecordatorio`
   - Busca facturas con estado `segundorecordatorio` → envía email → actualiza a `desactivado`
3. `GET /api/facturas/resumen` → retorna todos los clientes con sus facturas agrupadas.

## Requisitos

- .NET 8 SDK
- MongoDB corriendo en `localhost:27017` (o MongoDB Atlas)
- Node.js 18+ con Angular CLI: `npm install -g @angular/cli`

## Configuración

Editar `src/MonoLegal.API/appsettings.json`:

```json
{
  "MongoDb": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "monolegal"
  },
  "Email": {
    "Host": "smtp.gmail.com",
    "Port": 587,
    "Username": "tu-email@gmail.com",
    "Password": "tu-app-password-de-gmail",
    "FromName": "MonoLegal"
  }
}
```

> Para Gmail: activar autenticación de 2 pasos y generar una **App Password** en la configuración de cuenta.

## Ejecutar Backend

```bash
cd src/MonoLegal.API
dotnet run
```

- API: `http://localhost:5000`
- Swagger UI: `http://localhost:5000/swagger`

## Ejecutar Frontend

```bash
cd frontend/monolegal-front
npm install
ng serve
```

App en `http://localhost:4200`.

## Ejecutar Tests

```bash
cd tests/MonoLegal.Tests
dotnet test
```

5 pruebas unitarias sobre `NotificacionService` usando Moq.

## Endpoints

| Método | Ruta | Descripción |
|--------|------|-------------|
| `GET` | `/api/facturas/resumen` | Resumen de facturas agrupadas por cliente |
| `POST` | `/api/notificaciones/procesar` | Procesa recordatorios y envía emails |
