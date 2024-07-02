# Dalamud.DrunkenToad

A set of utilities to use in developing dalamud plugins. Equal in prestige to its in-game namesake.

## Features
- Provides thread-safe sorted collection type.
- Provides IPCs consumer for integrating with other dalamud plugins.
- Provides custom models for working with common game data objects.
- Provides Dalamud context service that instantiates dalamud services and custom service wrappers.
- Provides custom service classes to handle common tasks like managing player lists.
- Provides wrapper classes for Dalamud services to provide helper methods for common tasks.
- Provides extension classes for adding object helper methods.
- Provides localized ImGui components backed using Dalamud.Loc.
- Provides utility classes for common tasks.
- Provides a set of common ImGui components.

## How to use
Add as a submodule to your plugin project.

## Examples

### Using static context
```csharp
if (!DalamudContext.Initialize(pluginInterface))
{
    PluginLog.LogError("Failed to initialize properly.");
    return;
}
```

### Using dependency injection
```csharp
var services = new ServiceCollection();
var toadServiceInitializer = new ToadServiceInitializer(pluginInterface);
toadServiceInitializer.AddDalamudServices(services);
toadServiceInitializer.AddToadServices(services);
var serviceProvider = services.BuildServiceProvider();
```

