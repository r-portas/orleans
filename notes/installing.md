# Installing Orleans

> Tips for installing Orleans

```bash
dotnet new webapi -o <ProjectName>
```

```bash
dotnet add package Microsoft.Orleans.Server
dotnet add package OrleansDashboard
```

```csharp filename="Program.cs"
using Orleans.Runtime;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseOrleans(static siloBuilder =>
{
    siloBuilder.UseLocalhostClustering();
    siloBuilder.AddMemoryGrainStorageAsDefault();
    siloBuilder.UseDashboard();
});
```
