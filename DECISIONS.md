# DECISIONS.md

## Architecture & Structure

A pragmatic layered monolith was chosen intentionally — the scope is not big to justify microservices, Clean Architecture or any other architecture. 
A single API project with folder conventions (`Controllers`, `Services`, `Domain`, `Persistence`, `Models`) keeps things navigable without overhead. 
Flexibility is achieved through clean interface boundaries rather than architectural patterns — if the random number provider changes or the database switches, only the implementation changes.

## Key Technical Choices

PostgreSQL with EF Core handles persistence. Game rounds are identified by `Guid.CreateVersion7()` (sequential GUIDs — no database round-trip, no index fragmentation). 
Enums are stored as integers for efficiency.
Migrations run automatically on startup so Docker Compose is a single-command setup. Automatic migrations are intended to make testing process easier.

The Polly resilience pipeline (retry with exponential backoff, circuit breaker, timeout) is configured at HTTP client registration rather than inside `RandomNumberService` 
— keeping the service focused on a single responsibility.

FluentValidation runs at the controller boundary. The service assumes valid input. ASP.NET Core's built-in model validation is suppressed so all error responses go through the same `ErrorResponse` structure. 
Unexpected errors (external service failures, unhandled exceptions) are caught by global exception middleware and mapped to consistent error codes defined in a static `ErrorMessages` class.

## Intentional Omissions

Authentication, API versioning, Unit of Work, and MediatR were all considered and deliberately skipped — none add value at this scope. 
HTTPS redirection is omitted as it is an infrastructure concern handled at the reverse proxy level. 

## API Documentation

Scalar is used over Swagger UI for its cleaner interface and built-in HTTP client. XML documentation comments are enabled and picked up automatically by the OpenAPI pipeline.
Exposed in all environments because this is a coding challenge and API docs can be useful.

## Testing

Integration tests cover the full request pipeline using Testcontainers (real PostgreSQL) and WireMock.Net (external random number service). 
In-memory database was avoided — it does not enforce constraints and behaves differently from real PostgreSQL.

Unit tests cover `GameRules` — the pure domain logic with no dependencies, `Game Service` - core logic with mocked dependencies using NSubstitute.

## AI Usage

Claude and Gemini were used as a collaborative tool - analyze different trade-offs, improve codes, give different insight, generate tests and docs...
All of their responses were check and updated if it was needed.