# Clean Code POC

This project demonstrates the implementation of a semantic search application using Qdrant vector database with Clean Architecture principles.

- [Blog](https://medium.com/t-t-software-solution/%E0%B8%9E%E0%B8%B1%E0%B8%92%E0%B8%99%E0%B8%B2-line-messaging-api-%E0%B8%94%E0%B9%89%E0%B8%A7%E0%B8%A2-net-f00a7e887a0b)
- [Video](https://youtu.be/R24-6j9cnsQ)

## Clean Architecture Structure

The project is organized into four main layers according to Clean Architecture:

### 1. Domain Layer

The core layer containing business entities and interfaces:
- **Entities**: Basic models like `Document` that are independent of any framework
- **Interfaces**: Core abstractions like `IRepository`

### 2. Application Layer

Contains business logic and application-specific interfaces:
- **Services**: Business logic implementations like `DocumentBusinessService`
- **Interfaces**: Application-specific interfaces like `IDocumentBusinessService` and `IEmbeddingInfraService`

### 3. Infrastructure Layer

Implements interfaces defined in the domain and application layers:
- **Repositories**: Data access implementations like `QdrantRepository`
- **Services**: External service integrations like `OpenAIEmbeddingInfraService`

### 4. Presentation Layer

User interface implementations:
- **WebAPI**: RESTful API endpoints for programmatic access
- **WebUI**: Modern responsive web interface using ASP.NET Core MVC and Bootstrap
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
        P2[WebUI]
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

1. Ensure you have .NET 8.0 SDK installed
2. Clone the repository
3. Build the solution: `dotnet build`
4. Run desired presentation layer:
   - Web API: `dotnet run --project src/4.Presentation/5.WebApi`
   - Web UI: `dotnet run --project src/4.Presentation/6.WebUI`
   - Evaluator: `dotnet run --project src/Presentation/Evaluator`

### Running the Complete System

To run both API and Web UI together:

1. **Start the Web API first:**
   ```bash
   dotnet run --project src/4.Presentation/5.WebApi
   ```
   This will start the API at `https://localhost:7116`

2. **Start the Web UI in a separate terminal:**
   ```bash
   dotnet run --project src/4.Presentation/6.WebUI
   ```
   This will start the Web UI at `https://localhost:7111`

3. **Access the Web UI:**
   Open your browser and navigate to `https://localhost:7111` to access the responsive web interface for document management.

### Web UI Features

The Web UI provides a complete responsive interface with:
- **Document Management**: Full CRUD operations (Create, Read, Update, Delete)
- **Bootstrap 5 Design**: Modern, responsive layout that works on all devices
- **Real-time API Integration**: Communicates with the Web API via HTTP calls
- **User Feedback**: Success/error messages for all operations
- **Mobile Responsive**: Optimized for desktop, tablet, and mobile devices
