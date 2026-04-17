# Origami ASP.NET Web API

This project is the ASP.NET Web API layer for the origami recognition system.

It is responsible for:

- exposing PostgreSQL-backed origami data to a website
- organizing the API by feature modules instead of one flat service layer
- handling requests through MediatR queries/commands and handlers
- using EF Core for data access against PostgreSQL
- using EF Core reverse-engineering from the live database schema
- delegating ML inference to the existing Python recognition code

## Implemented Module Structure

- `Modules/Health`
- `Modules/Discovery`
- `Modules/Analytics`
- `Modules/Catalog`
- `Modules/Creators`
- `Modules/Resources`
- `Modules/Recognition`
- `Modules/Showcase`
- `Infrastructure/Persistence`
- `Infrastructure/Persistence/Scaffolded`
- `Infrastructure/Recognition`

## EF Core Database-First Note

The active EF model now comes from live-schema scaffolding:

- context: `Infrastructure/Persistence/Scaffolded/OrigamiScaffoldDbContext.cs`
- entities: `Infrastructure/Persistence/Scaffolded/Entities/*`

This replaced the earlier hand-written entity mapping so the API matches the actual PostgreSQL schema more closely.

Scaffold result notes:

- EF could not scaffold the expression index `uq_models_source_url_normalized`
- EF could not scaffold the expression index `uq_orc_models_src_name_diagram`

That is normal. Expression indexes must be maintained manually in SQL or explicit migrations.

## Current Endpoint Surface

- `GET /api/health`
- `GET /api/showcase/hero`
- `GET /api/discovery/overview`
- `GET /api/discovery/featured`
- `GET /api/analytics/landscape`
- `GET /api/analytics/creators`
- `GET /api/catalog/models`
- `GET /api/catalog/models/{modelId}`
- `GET /api/creators`
- `GET /api/creators/{creatorId}`
- `GET /api/resources/books`
- `GET /api/resources/diagrams`
- `GET /api/resources/articles`
- `GET /api/resources/calls`
- `POST /api/recognition/predict`

## Database Configuration

Do not hardcode the production password in source-controlled config.

Preferred local options:

1. Environment variable

```powershell
$env:ORIGAMI_DB_CONNECTION_STRING="Host=db-origami-nur-3364.c.aivencloud.com;Port=19924;Database=defaultdb;Username=avnadmin;Password=YOUR_PASSWORD;SslMode=Require"
```

2. Or set `ConnectionStrings__OrigamiDb`

```powershell
$env:ConnectionStrings__OrigamiDb="Host=db-origami-nur-3364.c.aivencloud.com;Port=19924;Database=defaultdb;Username=avnadmin;Password=YOUR_PASSWORD;SslMode=Require"
```

The project also supports `appsettings.json`, but environment variables are safer for secrets.

## Recognition Bridge

The API does not reimplement the TensorFlow model in .NET.

Instead:

- the website uploads an image to `POST /api/recognition/predict`
- ASP.NET stores it in a temporary file
- the API calls `ai/api_predict.py`
- the Python script uses the existing project inference code and returns JSON

That keeps the model logic in one place while still giving the website an ASP.NET Web API.

## Why These Endpoints Exist

They map directly to the project’s strongest user-facing capabilities:

- `showcase` provides a homepage-ready payload with signature models, ORC highlights, featured content, and recognition flavor
- `discovery` surfaces the overall scale and featured content of the platform
- `analytics` exposes the visual/statistical strengths already present in the recognition project
- `catalog` exposes the core origami model collection with filters and detail lookup
- `creators` exposes authorship, aliases, and creator portfolios
- `resources` exposes scraped learning content from CFC
- `recognition` exposes image-based origami prediction backed by the existing Python model

## Run

```powershell
cd origami_api
dotnet build
dotnet run
```

Swagger:

```text
http://localhost:5102/swagger
```

## Notes

- Build currently succeeds locally.
- The API expects Python plus the existing ML dependencies to be available for `/api/recognition/predict`.
- The API expects an actual trained model file to exist in `ai/origami_model.h5` or `ai/checkpoints/best_model.keras`.
