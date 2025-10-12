# ToggleHub

A modern, full-stack feature flag management platform built with .NET 9 and React. ToggleHub provides a comprehensive solution for managing feature flags across organizations, projects, and environments with advanced rule-based targeting and real-time evaluation.

🌐 **Live Demo**: [https://toggle-hub.com/](https://toggle-hub.com/)

## 🚀 Features

### Core Functionality
- **Feature Flag Management**: Create, update, and manage feature flags across multiple environments
- **Multi-Environment Support**: Dev, Staging, and Production environments with independent flag states
- **Rule-Based Targeting**: Advanced targeting rules with conditions and attributes
- **Real-time Evaluation**: Fast flag evaluation with caching and performance optimization
- **API Key Authentication**: Secure API access for programmatic flag evaluation

### Organization & Project Management
- **Multi-tenant Architecture**: Organizations with projects and team members
- **Role-based Access Control**: Owner, Admin, and Flag Manager roles
- **Team Collaboration**: Invite team members and manage permissions
- **Project Organization**: Organize flags by projects and environments

### Developer Experience
- **RESTful API**: Comprehensive API with OpenAPI/Swagger documentation
- **.NET SDK**: Lightweight SDK for easy integration
- **Modern Web UI**: React-based dashboard with Material-UI components
- **Real-time Updates**: Live flag state changes and notifications

### Observability & Monitoring
- **OpenTelemetry Integration**: Comprehensive tracing, metrics, and logging
- **Performance Monitoring**: Built-in performance metrics and health checks
- **Grafana Dashboards**: Pre-configured monitoring dashboards

## 🏗️ Architecture

ToggleHub follows Clean Architecture principles with the following layers:

```
src/
├── ToggleHub.API/              # Web API layer (ASP.NET Core 9)
├── ToggleHub.Application/      # Application layer (business logic)
├── ToggleHub.Domain/           # Domain layer (entities, repositories)
├── ToggleHub.Infrastructure/   # Infrastructure layer (data access, external services)
├── ToggleHub.Infrastructure.Identity/ # Identity management
├── ToggleHub.Sdk/              # .NET SDK for client integration
└── ToggleHub.Web.React/        # React frontend application
```

### Technology Stack

**Backend:**
- .NET 9 (ASP.NET Core)
- Entity Framework Core with SQL Server
- OpenTelemetry for observability
- In-memory cache
- In-memory event handling
- SendGrid/Mailpit for email services

**Frontend:**
- React 19 with Vite
- Material-UI (MUI) for components
- Redux Toolkit for state management
- React Router for navigation

**Infrastructure:**
- Docker & Docker Compose
- SQL Server 2022
- Grafana, Prometheus, Loki, Tempo
- OpenTelemetry Collector

## 🚀 Quick Start

### Prerequisites

- Docker and Docker Compose
- SQL Server (or use Docker)

### Development Setup

1. **Clone the repository**
   ```bash
   git clone https://github.com/bohdanzinchenkodev/togglehub.git
   cd togglehub
   ```

2. **Set up environment variables**
   ```bash
   cp .env.development .env
   # Edit .env with your configuration
   ```

3. **Start the development environment**
   ```bash
   docker-compose up -d
   ```

4. **Access the application**
   - Frontend: http://localhost:3001
   - API: http://localhost:5160
   - Swagger UI: http://localhost:5160/swagger
   - Grafana: http://localhost:3000

## 📖 Usage

### Creating Your First Feature Flag

1. **Create an Organization**
   - Sign up and create your first organization
   - Invite team members with appropriate roles

2. **Create a Project**
   - Add a new project within your organization
   - Set up environments (Dev, Staging, Production)

3. **Create a Feature Flag**
   - Navigate to your project
   - Click "Create Flag" in the desired environment
   - Configure flag settings and targeting rules

4. **Integrate with Your Application**
   - Generate an API key for your project
   - Use the ToggleHub SDK to evaluate flags

### .NET SDK

For detailed SDK usage and examples, see the [ToggleHub SDK documentation](https://github.com/bohdanzinchenkodev/ToggleHub/tree/master/src/ToggleHub.Sdk).

### API Endpoints

The API provides comprehensive endpoints for:

- **Organizations**: `/api/organizations`
- **Projects**: `/api/organizations/{orgId}/projects`
- **Environments**: `/api/organizations/{orgId}/projects/{projectId}/environments`
- **Flags**: `/api/organizations/{orgId}/projects/{projectId}/environments/{envId}/flags`
- **Flag Evaluation**: `/api/evaluate`
- **API Keys**: `/api/organizations/{orgId}/projects/{projectId}/apikeys`

## 🧪 Testing

### Running Tests

```bash
# Run all tests
dotnet test

# Run specific test project
dotnet test Tests/ToggleHub.Application.UnitTests
dotnet test Tests/ToggleHub.Infrastructure.UnitTests
```

### Test Coverage

The project includes comprehensive unit tests for:
- Application services
- Infrastructure components


## 📊 Monitoring & Observability

ToggleHub includes comprehensive monitoring:

- **Metrics**: Application performance, database queries, cache hits
- **Logs**: Structured logging with correlation IDs
- **Traces**: Distributed tracing across services
- **Dashboards**: Pre-configured Grafana dashboards

Access monitoring tools:
- Grafana: http://localhost:3000

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

### Development Guidelines

- Follow Clean Architecture principles
- Write comprehensive unit tests
- Use conventional commit messages
- Ensure all tests pass before submitting PR

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🔗 Links

- **Repository**: https://github.com/bohdanzinchenkodev/togglehub
- **Issues**: https://github.com/bohdanzinchenkodev/togglehub/issues
