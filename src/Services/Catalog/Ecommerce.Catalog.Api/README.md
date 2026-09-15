## Catalog Service setup

All commands in this document use PowerShell. Run them from the repository root:

```powershell
$repoRoot = "C:\Users\AlanCuevas\source\repos\EcommerceMicroservices"
Set-Location $repoRoot
Get-Location
```

### Create the Catalog projects

Run these commands only when creating the projects for the first time:

```powershell
dotnet new classlib --name Ecommerce.Catalog.Domain --output src/Services/Catalog/Ecommerce.Catalog.Domain --framework net10.0
dotnet new classlib --name Ecommerce.Catalog.Application --output src/Services/Catalog/Ecommerce.Catalog.Application --framework net10.0
dotnet new classlib --name Ecommerce.Catalog.Infrastructure --output src/Services/Catalog/Ecommerce.Catalog.Infrastructure --framework net10.0
dotnet new webapi --name Ecommerce.Catalog.Api --output src/Services/Catalog/Ecommerce.Catalog.Api --framework net10.0 --use-controllers
```

Add the projects to the solution and add their project references:

```powershell
dotnet sln EcommerceMicroservices.slnx add `
  src/Services/Catalog/Ecommerce.Catalog.Domain/Ecommerce.Catalog.Domain.csproj `
  src/Services/Catalog/Ecommerce.Catalog.Application/Ecommerce.Catalog.Application.csproj `
  src/Services/Catalog/Ecommerce.Catalog.Infrastructure/Ecommerce.Catalog.Infrastructure.csproj `
  src/Services/Catalog/Ecommerce.Catalog.Api/Ecommerce.Catalog.Api.csproj

dotnet add src/Services/Catalog/Ecommerce.Catalog.Application/Ecommerce.Catalog.Application.csproj `
  reference src/Services/Catalog/Ecommerce.Catalog.Domain/Ecommerce.Catalog.Domain.csproj

dotnet add src/Services/Catalog/Ecommerce.Catalog.Infrastructure/Ecommerce.Catalog.Infrastructure.csproj `
  reference src/Services/Catalog/Ecommerce.Catalog.Application/Ecommerce.Catalog.Application.csproj `
  src/Services/Catalog/Ecommerce.Catalog.Domain/Ecommerce.Catalog.Domain.csproj

dotnet add src/Services/Catalog/Ecommerce.Catalog.Api/Ecommerce.Catalog.Api.csproj `
  reference src/Services/Catalog/Ecommerce.Catalog.Application/Ecommerce.Catalog.Application.csproj `
  src/Services/Catalog/Ecommerce.Catalog.Infrastructure/Ecommerce.Catalog.Infrastructure.csproj
```

### Install the Catalog packages

Install the EF Core CLI tool and the packages used by the existing Catalog projects:

```powershell
dotnet tool update --global dotnet-ef --version 10.0.12

$infrastructureProject = "src/Services/Catalog/Ecommerce.Catalog.Infrastructure/Ecommerce.Catalog.Infrastructure.csproj"
$apiProject = "src/Services/Catalog/Ecommerce.Catalog.Api/Ecommerce.Catalog.Api.csproj"

dotnet add $infrastructureProject package Microsoft.EntityFrameworkCore.Design --version 10.0.12
dotnet add $infrastructureProject package Microsoft.EntityFrameworkCore.SqlServer --version 10.0.12
dotnet add $infrastructureProject package Microsoft.Extensions.Configuration.Abstractions --version 10.0.12
dotnet add $infrastructureProject package Microsoft.Extensions.DependencyInjection.Abstractions --version 10.0.12

dotnet add $apiProject package Microsoft.AspNetCore.OpenApi --version 10.0.11
dotnet add $apiProject package Microsoft.EntityFrameworkCore.Design --version 10.0.12
```

### Create and apply EF Core migrations

`CatalogDbContext` is in the Infrastructure project, while the API supplies the startup configuration and `CatalogDatabase` connection string. The current repository already contains the `InitialCatalog` migration.

```powershell
$context = "Ecommerce.Catalog.Infrastructure.Persistence.CatalogDbContext"

# Use this command only when no migration exists yet.
dotnet ef migrations add InitialCatalog `
  --context $context `
  --project $infrastructureProject `
  --startup-project $apiProject `
  --output-dir Persistence/Migrations

# Use a new name for subsequent model changes.
dotnet ef migrations add AddCatalogChanges `
  --context $context `
  --project $infrastructureProject `
  --startup-project $apiProject `
  --output-dir Persistence/Migrations

dotnet ef migrations list `
  --context $context `
  --project $infrastructureProject `
  --startup-project $apiProject

dotnet ef database update `
  --context $context `
  --project $infrastructureProject `
  --startup-project $apiProject
```

To remove the most recently created migration before it has been applied to a shared database:

```powershell
dotnet ef migrations remove `
  --context $context `
  --project $infrastructureProject `
  --startup-project $apiProject
```

### Build and test each Catalog layer

Restore once, then build and run test discovery for each project:

```powershell
dotnet restore EcommerceMicroservices.slnx

$catalogProjects = @(
  "src/Services/Catalog/Ecommerce.Catalog.Domain/Ecommerce.Catalog.Domain.csproj",
  "src/Services/Catalog/Ecommerce.Catalog.Application/Ecommerce.Catalog.Application.csproj",
  "src/Services/Catalog/Ecommerce.Catalog.Infrastructure/Ecommerce.Catalog.Infrastructure.csproj",
  "src/Services/Catalog/Ecommerce.Catalog.Api/Ecommerce.Catalog.Api.csproj"
)

foreach ($project in $catalogProjects) {
  dotnet build $project --no-restore --configuration Release
  if ($LASTEXITCODE -ne 0) { throw "Build failed: $project" }

  dotnet test $project --no-restore --no-build --configuration Release
  if ($LASTEXITCODE -ne 0) { throw "Tests failed: $project" }
}
```

There is currently no Catalog test project in the solution, so `dotnet test` will report that no tests were found for the four production projects. Add a test project and include it in `$catalogProjects` when Catalog unit or integration tests are created.
