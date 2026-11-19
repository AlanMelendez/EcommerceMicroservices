# EcommerceMicroservices — Azure Scope

A practical enterprise backend training project built with **.NET 10** and focused on modern .NET backend development on **Microsoft Azure**.

> This branch is intentionally different from the `main` learning track.
>
> - **`main`** keeps the broader distributed-systems path, including Kafka, Saga, Event Sourcing, and other advanced patterns.
> - **`azure-scope`** focuses on Azure technologies and the target .NET/Azure backend role.

Both branches share the same initial .NET and Clean Architecture foundation, but they can evolve independently.

---

## Learning Goal

The objective is not only to make the application work.

For every important topic, the training should explain:

1. What is it?
2. What problem does it solve?
3. Why are we using it here?
4. Where does it belong in the architecture?
5. How do we implement it?
6. How do we test it?
7. What can go wrong?
8. How would we use it in production?

The project follows a **rapid training approach**: implement each important concept in one representative flow instead of repeating the same pattern across every service.

---

# Target Skills

- .NET 10
- ASP.NET Core REST APIs
- Clean Architecture
- Microservices
- Domain-Driven Design (DDD)
- CQRS
- CQRS with separate physical read/write databases
- Entity Framework Core
- SQL Server
- Redis distributed caching
- Event-Driven Architecture (EDA)
- Azure Service Bus
- Outbox Pattern
- Eventual Consistency
- Azure Functions
- JWT
- OAuth 2.0
- Microsoft Entra ID
- Role, scope, and policy-based authorization
- Azure Key Vault
- Managed Identity
- Application Insights
- OpenTelemetry
- Azure App Service
- Docker
- Azure Container Registry (ACR)
- Azure Kubernetes Service (AKS)
- xUnit
- Integration testing
- Performance and security practices
- Git
- Azure DevOps CI/CD

---

# Branch Strategy

## `main` — Distributed Systems / Pattern Track

The `main` branch continues the broader distributed-systems and architecture path.

Representative topics include:

- Apache Kafka
- Producers and consumers
- Consumer groups
- Retry and dead-letter strategies
- Outbox Pattern
- Idempotent consumers
- Saga
- Compensating transactions
- Event Sourcing
- API Gateway / BFF where useful
- Advanced distributed-system patterns

This track exists to explore patterns and technologies that are valuable but are not required to block the Azure-focused training.

## `azure-scope` — Azure / Job-Focused Track

This branch focuses on:

- Azure Service Bus as the primary message broker
- Azure Functions for asynchronous event processing
- Microsoft Entra ID as the primary Identity Provider
- Azure Key Vault
- Managed Identity
- Application Insights
- OpenTelemetry
- Azure App Service
- Azure Container Registry
- AKS
- Azure DevOps CI/CD

Kafka, Saga, and Event Sourcing are **not required to finish this branch**.

---

# Architecture Principles

## Clean Architecture

Representative services use:

```text
Service
├── Domain
├── Application
├── Infrastructure
└── API
```

### Domain

Contains:

- Entities
- Value Objects
- Domain Events
- Business rules
- Business invariants

The Domain must not depend on infrastructure.

### Application

Contains:

- Commands
- Queries
- Handlers
- DTOs
- Validators
- Interfaces
- Application use cases

### Infrastructure

Contains:

- Entity Framework Core
- SQL Server access
- Repository implementations
- Redis
- Azure Service Bus integration
- External infrastructure integrations

### API

Contains:

- Controllers/endpoints
- HTTP contracts
- Middleware
- Authentication and authorization configuration
- Dependency Injection composition
- OpenAPI

The dependency direction should remain:

```text
API
├── Application
└── Infrastructure

Infrastructure
├── Application
└── Domain

Application
└── Domain

Domain
└── No dependencies
```

---

# Current Repository Structure

The currently implemented service is Catalog:

```text
src/
└── Services/
    └── Catalog/
        ├── Ecommerce.Catalog.Api/
        ├── Ecommerce.Catalog.Application/
        ├── Ecommerce.Catalog.Domain/
        └── Ecommerce.Catalog.Infrastructure/
```

The repository grows only when the current learning step requires a new project or service.

---

# Catalog Service

Catalog is the first representative microservice.

Responsibilities:

- Products
- Categories
- Product queries
- Product management

Concepts practiced here:

- Clean Architecture
- EF Core
- SQL Server
- Repository abstraction
- Repository implementation
- CQRS fundamentals
- MediatR
- FluentValidation
- Result Pattern
- DTOs
- Dependency Injection
- Redis later

Representative operations:

```http
POST /api/products
GET /api/products/{id}
```

We intentionally do not need complete CRUD to understand the architecture.

Redis will later be applied to:

```http
GET /api/products/{id}
```

---

# Ordering Service

Ordering will be the main advanced business service.

It will introduce **DDD** and later **physical CQRS**.

Representative domain model:

```text
Order                  ← Aggregate Root
├── OrderItem          ← Entity
├── Money              ← Value Object
└── Domain Events
```

Representative command flow:

```text
CreateOrderCommand
        ↓
Command Handler
        ↓
Order Aggregate
        ↓
OrderingWriteDb
```

DDD topics:

- Bounded Context
- Aggregate Root
- Entity
- Value Object
- Business Invariant
- Domain Event
- Domain Event vs Integration Event
- Ubiquitous Language

---

# CQRS

CQRS means **Command Query Responsibility Segregation**.

The main idea is to separate operations that **change state** from operations that **read state**.

Commands change state:

```text
CreateProductCommand
CreateOrderCommand
```

Queries read state:

```text
GetProductByIdQuery
GetOrderByIdQuery
```

Catalog first teaches CQRS using one database.

Ordering then teaches the advanced form using separate physical databases.

---

# Physical CQRS — Required

Physical CQRS is a required learning objective for this branch.

Ordering will use:

```text
                  Ordering
                     │
          ┌──────────┴──────────┐
          │                     │
          ▼                     ▼
      Write Side             Read Side
          │                     │
          ▼                     ▼
 OrderingWriteDb        OrderingReadDb
```

Write flow:

```text
POST /api/orders
      ↓
CreateOrderCommand
      ↓
Command Handler
      ↓
OrderingWriteDb
      ↓
Event / Projection
      ↓
OrderingReadDb
```

Read flow:

```text
GET /api/orders/{id}
      ↓
GetOrderByIdQuery
      ↓
OrderingReadDb
```

The command handler must **not manually dual-write** to both databases.

Instead:

```text
OrderingWriteDb
      ↓
Event / Projection
      ↓
OrderingReadDb
```

This introduces **eventual consistency**.

Topics:

- Write Model
- Read Model
- Projection
- Independent data models
- Eventual Consistency
- Independent read/write scaling

---

# Database-per-Service

Each microservice owns its data.

A service must not directly query another service's database.

```text
Catalog
   ↓
CatalogDb

Ordering
   ├── OrderingWriteDb
   └── OrderingReadDb
```

Services communicate through APIs or integration events instead of sharing database tables.

---

# Redis

Redis will be used as a distributed cache for one representative query:

```http
GET /api/products/{id}
```

Cache-aside flow:

```text
Request
   ↓
Redis
   ├── HIT → Return cached data
   │
   └── MISS
         ↓
      SQL Server
         ↓
      Cache result
         ↓
       Return
```

Topics:

- Distributed caching
- Cache-aside
- Cache hit
- Cache miss
- TTL
- Cache invalidation
- Serialization
- Redis vs in-memory cache

---

# Event-Driven Architecture with Azure Service Bus

Azure Service Bus is the primary message broker for this branch.

Representative event:

```text
OrderCreated
```

Conceptual flow:

```text
Ordering
    ↓
OrderingWriteDb
    ↓
Outbox
    ↓
Outbox Publisher
    ↓
Azure Service Bus
    ↓
Azure Function
    ↓
Notification
```

Topics:

- Event-Driven Architecture
- Integration Events
- Queues
- Topics
- Subscriptions
- Publisher
- Consumer
- Retry
- Dead-letter handling
- Eventual Consistency

---

# Outbox Pattern

We want to avoid this failure:

```text
Save Order        ✓
Publish Event     ✗
```

If the database operation succeeds but publishing fails, the system becomes inconsistent.

Instead, the Order and Outbox Message are saved in the same local database transaction:

```text
Database Transaction
      ├── Order
      └── OutboxMessage
             ↓
           Commit
```

A background publisher later sends pending messages to Azure Service Bus.

Conceptual flow:

```text
Order Transaction
      ↓
OrderingWriteDb
      ↓
OutboxMessages
      ↓
Background Publisher
      ↓
Azure Service Bus
```

This separates database consistency from broker availability.

---

# Azure Functions

A .NET isolated Azure Function will process one Service Bus event.

Representative flow:

```text
OrderCreated
     ↓
Azure Service Bus
     ↓
Service Bus Trigger
     ↓
Azure Function
     ↓
Simulated Notification
```

Topics:

- .NET isolated worker
- Triggers
- Bindings
- Asynchronous processing
- Retry
- Idempotency
- Configuration
- Logging

---

# Authentication and Authorization

The Azure track uses **Microsoft Entra ID** as the primary Identity Provider.

```text
Microsoft Entra ID
       ↓
OAuth 2.0
       ↓
JWT Access Token
       ↓
ASP.NET Core API
       ↓
Authorization Policy
```

Topics:

- Authentication vs Authorization
- JWT
- OAuth 2.0
- App registrations
- Scopes
- App roles
- Claims
- ASP.NET Core authorization policies

Representative protection:

```text
GET  /api/products/{id}
        ↓
Public or less restricted

POST /api/products
        ↓
Authenticated + authorized

POST /api/orders
        ↓
Authenticated + authorized
```

---

# Azure Key Vault and Managed Identity

Secrets must not be committed to Git.

Representative cloud configuration:

```text
Application
     ↓
Managed Identity
     ↓
Azure Key Vault
     ↓
Secret / Configuration
```

Topics:

- Secrets
- Key Vault
- Managed Identity
- Azure RBAC
- Environment-based configuration

---

# Observability

Representative distributed flow:

```text
POST /orders
     ↓
Ordering API
     ↓
SQL Server
     ↓
Azure Service Bus
     ↓
Azure Function
```

We will observe:

- Requests
- Dependencies
- Exceptions
- Structured logs
- Latency
- Correlation
- Distributed traces

Main technologies:

- Application Insights
- Azure Monitor
- OpenTelemetry
- ASP.NET Core Health Checks

Sensitive values such as passwords, access tokens, connection secrets, and private keys must never be logged.

---

# Azure Deployment

We intentionally practice both App Service and AKS because they solve different deployment problems.

## Azure App Service

A representative API will be deployed to Azure App Service to learn managed PaaS deployment.

```text
Catalog API
     ↓
Azure App Service
```

## Containers and AKS

Representative microservices will later be containerized:

```text
.NET Service
     ↓
Docker Image
     ↓
Azure Container Registry
     ↓
AKS
```

Kubernetes topics:

- Pod
- Deployment
- Service
- ConfigMap
- Secret
- Liveness probe
- Readiness probe
- Replicas
- Rolling updates
- Resource requests
- Resource limits

The goal is not to build an unnecessarily large Kubernetes platform.

---

# Testing

The project uses the same representative-example strategy for tests.

Required examples:

- One xUnit unit test for an Application handler
- One API integration test
- Mocking only where useful
- Optional container-backed integration test when it adds learning value

Topics:

- Unit vs integration testing
- Arrange / Act / Assert
- Test doubles
- Mocking boundaries
- API integration testing

---

# Performance, Scalability, and Security

Representative topics include:

- EF Core tracking vs `AsNoTracking`
- Database indexes
- Efficient queries
- Pagination where useful
- Redis caching
- Async I/O
- Cancellation tokens
- Rate limiting
- Input validation
- Authorization policies
- Secure secret management
- Avoiding sensitive-data exposure
- Horizontal scaling
- Stateless APIs
- Health checks

---

# Azure DevOps CI/CD

The project finishes with a representative Azure DevOps YAML pipeline.

```text
Git Push
   ↓
Restore
   ↓
Build
   ↓
xUnit
   ↓
Publish Test Results
   ↓
Docker Build
   ↓
Push to ACR
   ↓
Deploy
```

Topics:

- Continuous Integration
- Continuous Delivery / Deployment
- YAML pipelines
- Build artifacts
- Test results
- Container images
- Environment configuration
- Deployment

---

# Official Azure-Scope Roadmap

## Step 0 — Foundation and Solution Structure — COMPLETE

Topics:

- E-commerce domain
- Repository
- .NET 10 solution
- Clean Architecture project structure
- Git

---

## Step 1 — Catalog Domain + Persistence — COMPLETE

Topics:

- Product
- Category
- EF Core
- SQL Server
- Fluent entity configuration
- Migration
- CatalogDb

---

## Step 2 — Catalog CQRS + Validation + Result Pattern — IN PROGRESS

Representative operations:

```http
POST /api/products
GET /api/products/{id}
```

Topics:

- CQRS fundamentals
- MediatR
- FluentValidation
- Result Pattern
- DTOs
- Repository abstraction
- Repository implementation
- Dependency Injection

---

## Step 3 — Redis Distributed Caching

Apply Redis to:

```http
GET /api/products/{id}
```

Topics:

- Distributed caching
- Cache-aside
- TTL
- Cache invalidation

---

## Step 4 — Ordering + DDD

Implement one representative order flow.

Topics:

- Bounded Context
- Aggregate Root
- Entity
- Value Object
- Invariants
- Domain Events
- Repository / Unit of Work where useful

---

## Step 5 — CQRS with Separate Physical Databases

Required databases:

```text
OrderingWriteDb
OrderingReadDb
```

Topics:

- Write model
- Read model
- Projection
- Eventual Consistency
- Separate persistence paths

---

## Step 6 — Azure Service Bus + EDA + Outbox

Representative event:

```text
OrderCreated
```

Topics:

- Event-Driven Architecture
- Azure Service Bus
- Topics and subscriptions
- Integration events
- Outbox
- Reliable publication
- Retry
- Idempotency
- Dead-letter handling

---

## Step 7 — Azure Functions

Consume the representative Service Bus event.

Topics:

- .NET isolated Function
- Service Bus trigger
- Retry
- Idempotency
- Dead-letter concepts

---

## Step 8 — Microsoft Entra ID Security

Topics:

- JWT
- OAuth 2.0
- Entra ID
- Scopes
- Roles
- Policies
- Protected endpoints

---

## Step 9 — Key Vault + Application Insights

Topics:

- Managed Identity
- Key Vault
- Structured logging
- OpenTelemetry
- Application Insights
- Error tracing
- Health Checks

---

## Step 10 — Azure Deployment

Topics:

- App Service
- Docker
- Azure Container Registry
- AKS
- Kubernetes essentials
- Scaling
- Health probes

---

## Step 11 — Testing + Performance + Security

Topics:

- xUnit
- Unit testing
- Integration testing
- EF Core optimization
- Indexes
- `AsNoTracking`
- Rate limiting
- Security review

---

## Step 12 — Azure DevOps CI/CD

Topics:

- Git
- Azure DevOps
- YAML pipeline
- Restore
- Build
- Test
- Docker image
- ACR
- Deployment

---

## Step 13 — Production and Architecture Review

Review:

- Clean Architecture
- DDD
- CQRS
- Physical CQRS
- EDA
- Outbox
- Redis
- Security
- Azure services
- Observability
- Scalability
- Failure handling
- Production readiness

---

# Progress Tracker

| Step | Activity | Status |
|---|---|---|
| 0 | Foundation / solution / Git | Complete |
| 1 | Catalog Domain + EF Core + SQL Server | Complete |
| 2 | Catalog CQRS + validation + Result Pattern | In Progress |
| 3 | Redis | Pending |
| 4 | Ordering + DDD | Pending |
| 5 | Physical CQRS | Pending |
| 6 | Azure Service Bus + EDA + Outbox | Pending |
| 7 | Azure Functions | Pending |
| 8 | Entra ID + OAuth2 + JWT | Pending |
| 9 | Key Vault + Application Insights | Pending |
| 10 | App Service + Docker + ACR + AKS | Pending |
| 11 | Testing + performance + security | Pending |
| 12 | Azure DevOps CI/CD | Pending |
| 13 | Production architecture review | Pending |

---

# Topics Intentionally Outside the Required Azure Scope

The following topics are valuable, but they are not required to complete this branch:

- Kafka
- Saga
- Compensating transactions
- Event Sourcing
- BFF
- YARP API Gateway
- Advanced Kubernetes
- Helm
- Service Mesh

They can be explored in the `main` distributed-systems track or in later dedicated branches.

This separation is intentional:

> **Patterns should be introduced because they solve a problem, not simply because they are considered enterprise.**

---

# Expected Final Architecture

```text
                         Client
                           │
                           ▼
                   Microsoft Entra ID
                           │
                      OAuth2 / JWT
                           │
             ┌─────────────┴─────────────┐
             │                           │
             ▼                           ▼
       Catalog API                  Ordering API
   Clean Architecture              DDD + CQRS
             │                           │
             ▼                    ┌──────┴──────┐
         CatalogDb                │             │
             │                    ▼             ▼
             ▼             OrderingWriteDb OrderingReadDb
           Redis                  │
                                  ▼
                                Outbox
                                  │
                                  ▼
                         Azure Service Bus
                                  │
                                  ▼
                          Azure Function
                                  │
                                  ▼
                           Notification

             Azure Key Vault / Managed Identity
                           │
                           ▼
              Application Insights / OTel
```

Deployment:

```text
Catalog API
     ↓
Azure App Service
```

and:

```text
.NET Microservices
      ↓
Docker
      ↓
Azure Container Registry
      ↓
AKS
```

Delivery:

```text
Git
 ↓
Azure DevOps
 ↓
Build
 ↓
Test
 ↓
Containerize
 ↓
Deploy
```

---

# Main Learning Outcome

By the end of the `azure-scope` branch, the objective is to be able to explain and demonstrate how a modern .NET backend combines:

```text
.NET 10
ASP.NET Core
Clean Architecture
DDD
Microservices
EF Core
SQL Server
CQRS
Physical Read/Write Databases
Redis
Event-Driven Architecture
Azure Service Bus
Outbox
Azure Functions
JWT / OAuth2 / Entra ID
Key Vault
Managed Identity
Application Insights
OpenTelemetry
App Service
Docker / ACR / AKS
xUnit
Azure DevOps CI/CD
```

The most important outcome is not memorizing technologies.

It is understanding:

> **what problem each technology or pattern solves, why it belongs in the architecture, how to implement it, how to test it, and what trade-offs it creates.**
