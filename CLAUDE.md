# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Atlas is a knowledge extraction system with two main components:

1. **Offline Indexing Pipeline** - Batch processes Wikipedia articles to extract and index semantic relationships into an internal knowledge graph
2. **Query API** (Future) - GraphQL API for querying the indexed knowledge base

The system uses a .NET 9.0 backend with a separate Python-based NLP service (Narrator) for relationship extraction via gRPC.

### Important Architecture Notes

- **Everything in Atlas.Indexer is for offline batch processing** - This is NOT exposed via API endpoints. The indexing pipeline will run periodically (e.g., daily) to process Wikipedia content and populate the knowledge graph
- **Atlas.Console is a testing/debugging tool only** - Used to test individual components during development, not for production use
- **Future API will use GraphQL** - Query endpoints will be added later in Atlas.Api to search the indexed knowledge base

## Development Commands

### Python Narrator Service

The Narrator service requires Python 3.11.11 and uses Make for build automation:

```bash
cd src/Atlas.NarratorService

# Setup: Install dependencies including spaCy and en_core_web_lg model
make setup

# Generate: Create gRPC stub files from proto definitions
make generate

# Run: Start the gRPC server on port 50051
make run

# Or use the combined command
make build  # Runs setup + generate
python server.py
```

### .NET Projects

```bash
# Build all projects in the solution
dotnet build

# Run tests (xUnit)
dotnet test

# Run the CLI application
dotnet run --project src/Atlas.Console
```

### Container Deployment

```bash
# Build the Narrator service container (build context must be project root)
podman build -f containers/Narrator.Dockerfile -t narrator-service .

# Run the container
podman run -p 50051:50051 narrator-service
```

## Architecture

### Project Structure

**Atlas.Clients** - Shared external service clients:
- `Wiki/WikiService.cs` - Wikipedia API client (renamed from WikiApiService)
- `Wiki/Models/` - Wikipedia API response models
- `Wiki/Extensions/` - HttpClient extensions for query string building
- `Wiki/Exceptions/` - WikiApiException for API errors
- `Narrator/GrpcNarratorService.cs` - gRPC client wrapper for the NLP service
- `Narrator/Models/` - DocumentRequest, Phrase, RelationshipResponse
- Generated protobuf code in `obj/Debug/net9.0/` (namespace: `Atlas.Clients.Generated`)

**Atlas.Indexer** - Offline indexing pipeline (batch processing only):
- `Models/Document.cs` - Record with `Raw`, `Parsed`, and `Annotations` properties
- `Models/Annotations/` - Abstract `Annotation` base class with concrete types: LinkAnnotation, SectionAnnotation, AnnotationType
- `Models/IStageHandler.cs` - Interface for future pipeline stage handlers
- `Parsing/WikiHtmlParser.cs` - HTML parsing using handler chain pattern
- `Parsing/HtmlHandlers/` - LinkHandler, SectionHandler, TextHandler, WikiHandlerFactory
- `Extensions/` - IElementExtensions (IsTag, HasClass), StringExtensions (NormalizeWhiteSpace)
- `Narration/BatchedNarrator.cs` - Orchestrates relationship extraction
- `Narration/NarratedDocument.cs` - Document with extracted relationships
- `Exceptions/` - Domain-specific exceptions

**Atlas.Infrastructure** - Data access layer:
- Will contain graph database storage interfaces and implementations
- Shared between Indexer (write) and future API (read)

**Atlas.NarratorService** - Python gRPC NLP service:
- `server.py` - Entry point, starts gRPC server on port 50051
- `services/grpc/narrator_service.py` - Implements `GetRelationships()` using spaCy dependency parsing
- Uses spaCy's `en_core_web_lg` model for NLP analysis
- Processes `DocumentRequest` (text + target phrases) and streams `RelationshipResponse` objects

**Atlas.Console** - Testing/debugging CLI application:
- **Not for production use** - Only for testing components during development
- Uses CommandLine parser with verb-based commands
- Commands: `parse-page`, `get-page`, `page-content`, `narrate`
- Services in `Services/` include PageContentService, OutputService, etc.

**Atlas.Api** - Future GraphQL query API:
- Currently minimal ASP.NET Core setup
- Will expose GraphQL endpoints to query the indexed knowledge graph

### Indexing Pipeline Data Flow (Offline Batch Processing)

1. **Fetch**: Wikipedia API → retrieve article HTML via `WikiService`
2. **Parse**: `WikiHtmlParser` → parse HTML to extract text and create annotations (links, sections)
3. **Extract**: `BatchedNarrator` → coordinates relationship extraction
4. **NLP**: `GrpcNarratorService` → sends `DocumentRequest` to Python Narrator service via gRPC
5. **Analysis**: `NarratorService` → uses spaCy dependency parsing to identify semantic relationships
6. **Stream**: Returns stream of `RelationshipResponse` (Action, Prep, DetailedAction, Target)
7. **Persist**: Store extracted relationships in graph database (Atlas.Infrastructure)

This pipeline runs as a scheduled batch job, NOT as real-time API requests.

### Protocol Buffers

The `proto/` directory is a **git submodule** containing gRPC service definitions:
- `Narrator/Narrator.proto` - Main service definition
- `Narrator/DocumentRequest.proto` - Input message
- `Narrator/RelationshipResponse.proto` - Output message

After updating proto files, regenerate code:
- C#: Build will auto-generate (via MSBuild integration in `Atlas.Clients`)
- Python: Run `make generate` in `src/Atlas.NarratorService`
- **Important**: Proto files use `option csharp_namespace = "Atlas.Clients.Generated"`

### Key Patterns

- **Pipeline Pattern** (Future): `IStageHandler` interface will support composable pipeline stages for indexing
- **Handler Chain**: HTML parsing uses specialized handlers that process different element types
- **Annotation Model**: Text structure is represented via position-based annotations rather than nested objects. `Annotation` is an abstract base class that all annotation types inherit from
- **Document Record**: `Document` is a simple record with `Raw`, `Parsed`, and `Annotations` properties. The extensibility comes from the annotation types themselves, not from Document inheritance
- **Streaming gRPC**: Narrator returns relationships as a stream for efficiency
- **Service Wrapper Pattern**: `GrpcNarratorService` wraps generated gRPC client code with domain-friendly interface

### Separation of Concerns

- **Atlas.Clients**: Reusable external service integration (Wiki API, Narrator gRPC)
- **Atlas.Indexer**: Isolated batch processing logic - no dependencies on API layer
- **Atlas.Infrastructure**: Shared data access - used by both Indexer (write) and future API (read)
- **Atlas.Console**: Testing tool only - references Indexer and Clients for component testing

## Important Files

- `src/Atlas.Indexer/Models/Document.cs` - Document record with Raw, Parsed, and Annotations
- `src/Atlas.Indexer/Models/Annotations/Annotation.cs` - Abstract base class for all annotation types
- `src/Atlas.Indexer/Models/IStageHandler.cs` - Interface for pipeline stages
- `src/Atlas.Indexer/Extensions/` - IElementExtensions and StringExtensions for parsing
- `src/Atlas.Indexer/Narration/BatchedNarrator.cs` - Relationship extraction orchestrator
- `src/Atlas.Indexer/Parsing/WikiHtmlParser.cs` - HTML parsing with handler chain
- `src/Atlas.Clients/Wiki/WikiService.cs` - Wikipedia API client (renamed from WikiApiService)
- `src/Atlas.Clients/Narrator/GrpcNarratorService.cs` - Narrator gRPC client wrapper
- `src/Atlas.NarratorService/services/grpc/narrator_service.py` - spaCy-based relationship extraction
- `proto/Narrator/*.proto` - gRPC API contract (git submodule with namespace `Atlas.Clients.Generated`)

## Testing

- Run C# unit tests: `dotnet test`
- Test files in `src/Atlas.Web.Tests/`, `Atlas.Infrastructure.Tests/`
- Use `api/enwiki/*.http` files for Wikipedia API testing
- Use `scripts/grpcurl/` utilities for gRPC endpoint testing
- `lab/relationships.ipynb` Jupyter notebook for NLP algorithm experimentation
- **Atlas.Console**: Use for manual component testing during development
