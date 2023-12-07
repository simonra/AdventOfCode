Setting up new dotnet projects (should I ever do this in a terminal without history)

```sh
$DAY_NUMBER="00"
cd $repo/2023
mkdir -p "Day_${DAY_NUMBER}/cs"
cd "Day_${DAY_NUMBER}/cs"
dotnet new console --name "Day${DAY_NUMBER}"
cd "Day${DAY_NUMBER}"
dotnet add package Microsoft.Extensions.Logging.Console
touch "sample_input-part_1.txt"
touch "input.txt"
```

Add logger setup so that output amount can be easily tweaked and comes out in pretty colors when errors are produced, etc

```cs
using Microsoft.Extensions.Logging;

using ILoggerFactory factory = LoggerFactory.Create(
    builder =>
    {
        builder.SetMinimumLevel(LogLevel.Information);
        builder.SetMinimumLevel(LogLevel.Debug);
        builder.AddConsole();
    });
ILogger logger = factory.CreateLogger("Day03");
```

Add json serializer options so that printed output becomes useful when it contains reference types like dictionaries (always fun to to be in the situation "after the fact, I now wonder if the keys are all messed up", to only see "memory reference x").

```cs
using System.Text.Json;

var serializerOptions = new JsonSerializerOptions { WriteIndented = true };

// Usage
var numbers = new List<int> { 1, 2, 3, 4 };
logger.LogError($"The numbers printed indented: {JsonSerializer.Serialize(numbers, serializerOptions)}");
logger.LogError($"The numbers printed inline: {JsonSerializer.Serialize(numbers)}");
```
