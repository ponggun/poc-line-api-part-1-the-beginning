# Clean Code POC

This project demonstrates the implementation of a semantic search application using MongoDB for data persistence and Line Messaging API webhook integration, following Clean Architecture principles.

- [Blog]()
- [Video]()

## Features

### Core Features
- **Document Management**: Create, read, update, and delete documents with metadata support
- **Vector Search**: Semantic search capabilities using embedding services
- **Line Webhook Integration**: Receive and process webhooks from Line Messaging API
- **MongoDB Integration**: Persistent data storage with MongoDB

### Clean Architecture Structure

The project is organized into four main layers according to Clean Architecture:

### 1. Domain Layer

The core layer containing business entities and interfaces:
- **Entities**: Basic models like `Document`, `LineEvent`, `LineMessage` that are independent of any framework
- **Interfaces**: Core abstractions like `IRepository`, `ILineEventRepository`, `ILineMessageRepository`

### 2. Application Layer

Contains business logic and application-specific interfaces:
- **Services**: Business logic implementations like `DocumentService`
- **Interfaces**: Application-specific interfaces like `IDocumentService` and `IEmbeddingService`

### 3. Infrastructure Layer

Implements interfaces defined in the domain and application layers:
- **Repositories**: Data access implementations like `MongoDbRepository`, `MongoDbLineEventRepository`, `MongoDbLineMessageRepository`
- **Services**: External service integrations like `OpenAIEmbeddingService`
- **Configuration**: MongoDB settings and connection management

### 4. Presentation Layer

User interface implementations:
- **WebAPI**: RESTful API endpoints for programmatic access, including Line webhook endpoints
- **WebApp**: Browser-based user interface using MVC
- **Evaluator**: Console application for testing and evaluation

## Dependencies

The dependencies between layers flow inward according to Clean Architecture:
- Presentation → Application → Domain
- Infrastructure → Application → Domain

This ensures the core domain remains independent of external frameworks and technologies.

```mermaid
flowchart TD
    subgraph Domain
        D1[Entities]
        D2[Interfaces]
    end

    subgraph Application
        A1[Services]
        A2[Interfaces]
    end

    subgraph Infrastructure
        I1[Repositories]
        I2[Services]
    end

    subgraph Presentation
        P1[WebApi]
        P2[WebApp]
        P3[Evaluator]
    end

    %% Dependency arrows
    P1 --> A1
    P2 --> A1
    P3 --> A1
    A1 --> D1
    A1 --> D2
    A2 --> D2
    I1 --> D2
    I2 --> A2
    I1 --> A2
    I2 --> D2

    %% Group dependencies
    Infrastructure --> Application
    Application --> Domain
    Presentation --> Application
```

## Getting Started

### Prerequisites
- .NET 8.0 SDK
- Docker and Docker Compose (for MongoDB)

### Setup Instructions

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd poc-line-api-part-1-the-beginning
   ```

2. **Start MongoDB using Docker Compose**
   ```bash
   docker-compose up -d
   ```
   
   This will start a MongoDB instance with:
   - Username: `admin`
   - Password: `password123`
   - Port: `27017`
   - Database: `poc_line_api`

3. **Build the solution**
   ```bash
   dotnet build
   ```

4. **Run tests**
   ```bash
   dotnet test
   ```

5. **Run the desired presentation layer**
   - **Web API**: `dotnet run --project src/Presentation/WebApi`
   - **Web App**: `dotnet run --project src/Presentation/WebApp`
   - **Evaluator**: `dotnet run --project src/Presentation/Evaluator`

## API Endpoints

### Document Management
- `GET /api/Document` - Get all documents
- `GET /api/Document/{id}` - Get document by ID
- `POST /api/Document` - Create a new document
- `PUT /api/Document/{id}` - Update a document
- `DELETE /api/Document/{id}` - Delete a document
- `GET /api/Document/search?query={query}&limit={limit}` - Search documents

### Line Webhook Integration
- `POST /api/LineWebhook/webhook` - Receive Line webhook events
- `GET /api/LineWebhook/events` - Get all Line events
- `GET /api/LineWebhook/messages` - Get all Line messages

### Line Webhook Usage

To integrate with Line Messaging API, configure your Line Bot webhook URL to point to:
```
https://your-domain/api/LineWebhook/webhook
```

#### Sample Line Webhook Payload
```json
{
  "events": [
    {
      "type": "message",
      "timestamp": 1640995200000,
      "source": {
        "type": "user",
        "userId": "U1234567890abcdef"
      },
      "replyToken": "replytoken123",
      "message": {
        "id": "message123",
        "type": "text",
        "text": "Hello, World!"
      }
    }
  ]
}
```

The webhook will:
1. Log the received payload using Serilog
2. Parse Line events and messages
3. Store events in MongoDB (`line_events` collection)
4. Store messages in MongoDB (`line_messages` collection)
5. Return a success response

## Configuration

### MongoDB Settings
Configure MongoDB connection in `appsettings.json`:

```json
{
  "MongoDbSettings": {
    "ConnectionString": "mongodb://admin:password123@localhost:27017",
    "DatabaseName": "poc_line_api",
    "DocumentCollection": "documents",
    "LineEventCollection": "line_events",
    "LineMessageCollection": "line_messages"
  }
}
```

### Docker Compose Configuration
The included `docker-compose.yml` provides:
- MongoDB 7.x with authentication
- Persistent volume for data storage
- Network isolation
- Environment variables for initial setup

## Testing

Run all tests:
```bash
dotnet test
```

The project includes:
- **Application Tests**: Unit tests for business logic (6 tests)
- **Infrastructure Tests**: Unit tests for repository implementations (3 tests)

## Logging

The application uses Serilog for structured logging with:
- Console output for development
- File output in timestamped directories (`Logs/yyyyMMddHHmm/`)
- Rolling file policy (hourly)
- Structured JSON logging for webhook events
