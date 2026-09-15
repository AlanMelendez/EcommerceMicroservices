
## Create the Catalog Service projects

### First ensure that you are located in the root of the project folder.
You can run the next command to verify your current working directory.
```cmd
pwd
```
And you can see something like this:
```cmd
Path
----
C:\Users\AlanCuevas\source\repos\EcommerceMicroservices\EcommerceMicroservices
```

To create the Catalog Service projects, run the following commands:
```cmd
dotnet new classlib --name Ecommerce.Catalog.Domain --output src/Services/Catalog/Ecommerce.Catalog.Domain --framework net10.0
dotnet new classlib --name Ecommerce.Catalog.Application --output src/Services/Catalog/Ecommerce.Catalog.Application --framework net10.0 
dotnet new classlib --name Ecommerce.Catalog.Infrastructure --output src/Services/Catalog/Ecommerce.Catalog.Infrastructure --framework net10.0
dotnet new webapi --name Ecommerce.Catalog.Api --output src/Services/Catalog/Ecommerce.Catalog.Api --framework net10.0 --use-controllers
```

Add the project references to the solution file by running the following commands:
```cmd	


```