# RPSSL Game API

A REST API implementation of **Rock, Paper, Scissors, Lizard, Spock** — the extended variant made popular by The Big Bang Theory.

Game rules: [http://www.samkass.com/theories/RPSSL.html](http://www.samkass.com/theories/RPSSL.html)

Built with **.NET 10**, **PostgreSQL**, and **Docker**.

---

## Prerequisites

- [Docker](https://www.docker.com/products/docker-desktop) with Docker Compose
- [.NET 10 SDK](https://dotnet.microsoft.com/download) — only required for running tests locally
- [Git](https://git-scm.com)

---

## Getting Started

### 1. Clone the repository

```bash
git clone https://github.com/bzlatkovic/RPSSLGame.git
cd RPSSLGame
```

### 2. Create the `.env` file

Create a `.env` file in the repository root with the following content:

```env
DB_USERNAME=YourUsernameHere
DB_PASSWORD=YourPasswordHere
```

> **Note:** The `.env` file is included in the repository for reviewer convenience. In a production environment it would be excluded from version control and secrets managed via a secrets manager.

---

## Running the App

Start the API and PostgreSQL database with a single command:

```bash
docker compose up --build
```

Docker Compose will:
- Build the API image
- Start a PostgreSQL 16 container
- Wait for PostgreSQL to be healthy before starting the API
- Apply database migrations automatically on startup

### Verify the app is running

Open Scalar API documentation in your browser:

```
http://localhost:8080/api
```

If Scalar loads, the app is running correctly.

### Test against the provided UI

Paste the API root URL into the test UI at [https://codechallenge.boohma.com](https://codechallenge.boohma.com):

```
http://localhost:8080
```

---

## API Endpoints

| Method | Path | Description |
|--------|------|-------------|
| `GET` | `/choices` | Returns all five available choices |
| `GET` | `/choice` | Returns a randomly generated choice |
| `POST` | `/play` | Play a round against the computer |
| `GET` | `/stats` | Returns aggregated game statistics |

Full documentation is available at `http://localhost:8080/api`.

---

## Running Tests

Docker must be running — integration tests use Testcontainers to spin up a real PostgreSQL instance.

```bash
dotnet test
```

### What is tested

- **Integration tests** — full request pipeline including HTTP, validation, business logic, and database persistence via a real PostgreSQL container
- **Unit tests** 
  - `GameRules` - pure game logic covering all win, lose, and tie combinations across all five choices
  - `Game Service` - logic with mocked dependencies using NSubstitute

---

## Project Structure

```
RPSSLGame.Api/
  Controllers/        # HTTP endpoints
  Domain/             # Core game logic, enums (Choice, GameResult, GameRules)
  Extensions/         # IServiceCollection and WebApplication extension methods
  Models/             # Request/response DTOs and external API models
  Persistence/        # EF Core DbContext, entities, view models, migrations, repositories
  Services/           # Business logic (GameService, RandomNumberService)
  Validators/         # FluentValidation request validators
  Constants/          # Error messages, rate limiting policy names
  Program.cs

RPSSLGame.Tests/
  IntegrationTests/   # GameController integration tests
  UnitTests/          # GameRules, GameService unit tests
  Common/             # Shared test fixtures (GameApiFactory, WireMock setup)
```

---

## Stopping the App

```bash
docker compose down
```

To also remove the database volume:

```bash
docker compose down -v
```