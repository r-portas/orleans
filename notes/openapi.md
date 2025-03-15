# OpenAPI

> Notes for using OpenAPI with .NET, which makes it easy to call REST APIs

- `Microsoft.AspNetCore.OpenAPI`, available in .NET 9
- [Scalar](https://github.com/scalar/scalar/blob/main/integrations/aspnetcore/README.md)

- [https://learn.microsoft.com/en-us/aspnet/core/fundamentals/openapi/using-openapi-documents?view=aspnetcore-9.0#use-swagger-ui-for-local-ad-hoc-testing](Adhoc testing)

Install

```bash
dotnet add package Swashbuckle.AspNetCore.SwaggerUi
```

Then add the following to the `Program.cs`

```csharp
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "v1");
    });
}
```

The UI is hosted on the `/swagger/` path
