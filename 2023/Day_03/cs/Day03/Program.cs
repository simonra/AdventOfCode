using Microsoft.Extensions.Logging;
using System.Text.Json;

using ILoggerFactory factory = LoggerFactory.Create(
    builder =>
    {
        builder.SetMinimumLevel(LogLevel.Information);
        // builder.SetMinimumLevel(LogLevel.Debug);
        builder.AddConsole();
    });
ILogger logger = factory.CreateLogger("Day03");
var serializerOptions = new JsonSerializerOptions { WriteIndented = true };

var part1SampleDataFilePath = "sample_input-part_1.txt";
uint part1SampleDataExpectedResult = 4361;
var part1SampleDataSum = AllPartNumbersSum(part1SampleDataFilePath);
if (part1SampleDataSum == part1SampleDataExpectedResult)
{
    logger.LogInformation("Processing sample data yielded expected result!");
}
else
{
    logger.LogError($"Processing sample data failed. Sum is {part1SampleDataSum}, expected {part1SampleDataExpectedResult}");

}

var part1InputDataFilePath = "input-part-1.txt";
var part1Sum = AllPartNumbersSum(part1InputDataFilePath);
logger.LogInformation($"Part 1 result is {part1Sum}");

uint AllPartNumbersSum(string filePath)
{
    uint sum = 0;
    // string previousLine = string.Empty;
    var previousSchematicLine = new EngineSchematicLine()
    {
        LineNumber = uint.MaxValue,
        PartNumbers = new List<PartNumber>(),
        Symbols = new List<Symbol>(),
    };
    uint lineNumber = 0;
    var lines = File.ReadLines(filePath);
    var uncountedPartNumbersPrevious = new List<PartNumber>();
    var uncountedPartNumbersCurrent = new List<PartNumber>();
    foreach (var line in lines)
    {
        var currentSchematicLine = ParseEngineSchematicLine(line, lineNumber);
        logger.LogDebug($"Parsing line {line} yields {JsonSerializer.Serialize(currentSchematicLine, serializerOptions)}");
        uncountedPartNumbersCurrent.AddRange(currentSchematicLine.PartNumbers);
        foreach (var symbol in currentSchematicLine.Symbols)
        {
            for (int i = uncountedPartNumbersPrevious.Count - 1; i >= 0; i--)
            {
                if (IsAdjacent(symbol, uncountedPartNumbersPrevious[i]))
                {
                    sum += uncountedPartNumbersPrevious[i].Value;
                    uncountedPartNumbersPrevious.RemoveAt(i);
                }
            }
            for (int i = uncountedPartNumbersCurrent.Count - 1; i >= 0; i--)
            {
                if (IsAdjacent(symbol, uncountedPartNumbersCurrent[i]))
                {
                    sum += uncountedPartNumbersCurrent[i].Value;
                    uncountedPartNumbersCurrent.RemoveAt(i);
                }
            }
        }
        foreach (var symbol in previousSchematicLine.Symbols)
        {
            for (int i = uncountedPartNumbersCurrent.Count - 1; i >= 0; i--)
            {
                if (IsAdjacent(symbol, uncountedPartNumbersCurrent[i]))
                {
                    sum += uncountedPartNumbersCurrent[i].Value;
                    uncountedPartNumbersCurrent.RemoveAt(i);
                }
            }
            if (uncountedPartNumbersCurrent.Count == 0)
            {
                break;
            }
        }
        // ToDo: The actual work
        // For each symbol in current
        //    check adjacency to number in current and previous
        // For each number
        //    check adjacency to symbol in previous
        previousSchematicLine = currentSchematicLine;
        uncountedPartNumbersPrevious.Clear();
        uncountedPartNumbersPrevious.AddRange(uncountedPartNumbersCurrent);
        uncountedPartNumbersCurrent.Clear();
        lineNumber++;
    }
    return sum;
}

// Checks if number is adjacent on line above, below, or current
bool IsAdjacent(Symbol symbol, PartNumber partNumber)
{
    if (partNumber.OffsetBegin <= symbol.Offset && symbol.Offset <= partNumber.OffsetEnd)
    {
        return true;
    }

    if (symbol.Offset + 1 == partNumber.OffsetBegin)
    {
        return true;
    }

    if (symbol.Offset == partNumber.OffsetEnd + 1)
    {
        return true;
    }

    return false;
}

EngineSchematicLine ParseEngineSchematicLine(string input, uint lineNumber)
{
    var partNumbers = new List<PartNumber>();
    var symbols = new List<Symbol>();

    bool previousIsDigit = false;
    string currentNumberUnparsed = string.Empty;
    uint currentNumberFirstOffset = 0;
    // uint currentNumberLastOffset = 0;

    for (int i = 0; i < input.Length; i++)
    {
        if (input[i].CharIsDigit())
        {
            // ToDo
            if (previousIsDigit)
            {
                currentNumberUnparsed += input[i];
            }
            else
            {
                currentNumberUnparsed = $"{input[i]}";
                currentNumberFirstOffset = (uint)i;
                previousIsDigit = true;
            }
        }
        else
        {
            if (previousIsDigit)
            {
                // currentNumberLastOffset = i - 1;
                partNumbers.Add(
                    new PartNumber()
                    {
                        Value = UInt16.Parse(currentNumberUnparsed),
                        OffsetBegin = currentNumberFirstOffset,
                        OffsetEnd = (uint)i - 1,
                        // OffsetEnd = currentNumberLastOffset
                    });
            }
            if (input[i] != '.')
            {
                symbols.Add(new Symbol() { Value = input[i], Offset = (uint)i });
            }
            previousIsDigit = false;
        }
    }
    if (previousIsDigit)
    {
        // currentNumberLastOffset = i - 1;
        partNumbers.Add(
            new PartNumber()
            {
                Value = UInt16.Parse(currentNumberUnparsed),
                OffsetBegin = currentNumberFirstOffset,
                OffsetEnd = (uint)input.Length - 1,
                // OffsetEnd = currentNumberLastOffset
            });
    }

    var result = new EngineSchematicLine()
    {
        LineNumber = lineNumber,
        PartNumbers = partNumbers,
        Symbols = symbols,
    };
    return result;
}

static class ExtensionMethods
{
    public static bool CharIsDigit(this char c)
    {
        if (c == '0') return true;
        if (c == '1') return true;
        if (c == '2') return true;
        if (c == '3') return true;
        if (c == '4') return true;
        if (c == '5') return true;
        if (c == '6') return true;
        if (c == '7') return true;
        if (c == '8') return true;
        if (c == '9') return true;
        return false;
    }
}


// bool IsSymbol(char c)
// {
//     return !Char.IsDigit(c) && c != '.';
// }

// line
// {
//     numbers [
//         {value, offsetStart, offsetEnd},
//         { value, offsetStart, offsetEnd}
//     ]
//     symbols[
//         { offset, (value)}
//     ]
// }

public record EngineSchematicLine
{
    public required uint LineNumber { get; init; }
    public required List<PartNumber> PartNumbers { get; init; }
    public required List<Symbol> Symbols { get; init; }
}

public record PartNumber
{
    public required uint Value { get; init; }
    public required uint OffsetBegin { get; init; }
    public required uint OffsetEnd { get; init; }
}

public record Symbol
{
    public required char Value { get; init; }
    public required uint Offset { get; init; }
}
