# ASP.NET Core

## Structure

`Program.cs`

```csharp
// ...
var app = builder.Build();

TodoEndpoints.Map(app);

app.Run();
```

`Endpoints/Todo.cs`

```csharp
public static class TodoEndpoints
{
    public static void Map(WebApplication app)
    {
        app.MapGet("/", async context =>
        {
            // Get all todo items
            await context.Response.WriteAsJsonAsync(new { Message = "All todo items" });
        });

        app.MapGet("/{id}", async context =>
        {
            // Get one todo item
            await context.Response.WriteAsJsonAsync(new { Message = "One todo item" });
        });
    }
}
```

Also use Route groups

- [Minimal APIs quick reference](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis?view=aspnetcore-8.0#endpoint-defined-outside-of-programcs)
