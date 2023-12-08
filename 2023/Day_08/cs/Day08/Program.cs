using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

using ILoggerFactory factory = LoggerFactory.Create(
    builder =>
    {
        builder.SetMinimumLevel(LogLevel.Information);
        builder.SetMinimumLevel(LogLevel.Debug);
        builder.AddConsole();
    });
ILogger logger = factory.CreateLogger("Day08");

TestPart1OnSampleInput();
RunPart1();
RunPart2();

void RunPart1()
{
    logger.LogInformation("Starting RunPart1");
    var inputFile = "input.txt";
    var hauntedWastelands = ParseHauntedWasteland(inputFile);
    var result = NumberOfStepsFromAAAToZZZ(hauntedWastelands.Instructions, hauntedWastelands.Network);
    logger.LogInformation($"Part 1 result: {result}");
}

void RunPart2()
{
    logger.LogInformation("Starting RunPart2");
    var inputFile = "input.txt";
    var hauntedWastelands = ParseHauntedWasteland(inputFile);
    var result = NumberOfStepsFromAllXANodesTillAllOnXZNodes(hauntedWastelands.Instructions, hauntedWastelands.Network);
    logger.LogInformation($"Part 2 result: {result}");
}

void TestPart1OnSampleInput()
{
    logger.LogInformation("Starting TestPart1OnSampleInput");
    var inputFile = "sample_input-part_1.txt";
    var hauntedWastelands = ParseHauntedWasteland(inputFile);
    logger.LogInformation($"Haunted wastelands are {hauntedWastelands.Network.ToJson()}");
    var result = NumberOfStepsFromAAAToZZZ(hauntedWastelands.Instructions, hauntedWastelands.Network);
    uint expectedResult = 2;
    if (result != expectedResult)
    {
        logger.LogError($"Testing part 1 on sample data failed. Got {result}, expected {expectedResult}");
    }
    else
    {
        logger.LogInformation("Testing Part 1: Processing sample data yielded expected result!");
    }
}

UInt64 NumberOfStepsFromAllXANodesTillAllOnXZNodes(string instructions, Dictionary<string, NodeConnections> network)
{
    UInt64 result = 0;
    var currentNodes = network.Keys.Where(k => k.EndsWith('A')).ToArray();
    var numberOfPaths = currentNodes.Count();
    UInt64 numberOfInstructions = (UInt64)instructions.Length;
    int nextInstructionOffset;
    var startTime = DateTime.Now;
    while(true)
    {
        if(currentNodes.All(n => n.EndsWith('Z')))
        {
            break;
        }
        nextInstructionOffset = (int)(result % numberOfInstructions);
        for (int i = 0; i < numberOfPaths; i++)
        {
            var currentNodeId = currentNodes[i];
            if(instructions[nextInstructionOffset] == 'L')
            {
                currentNodes[i] = network[currentNodeId].Left;
            }
            else if(instructions[nextInstructionOffset] == 'R')
            {
                currentNodes[i] = network[currentNodeId].Right;
            }
        }
        result++;

        if(result % 10000000 == 0)
        {
            logger.LogInformation($"{DateTime.Now} Running for {DateTime.Now - startTime} At iteration {result} ({(double)result/(double)UInt64.MaxValue} of uint.MaxValue)");
        }
    }
    return result;
}

// uint NumberOfStepsToNodeEndingInZ(string instructions, Dictionary<string, NodeConnections> network)
// {

// }
// Distance to cycle start
// Cycle start point
// Points of interest in cycle
// Offsets of points of interest in cycle
//

uint NumberOfStepsFromAAAToZZZ(string instructions, Dictionary<string, NodeConnections> network)
{
    uint result = 0;
    string currentNodeId = "AAA";
    while(true)
    {
        if(currentNodeId == "ZZZ")
        {
            break;
        }
        if(instructions[(int)result % instructions.Length] == 'L')
        {
            currentNodeId = network[currentNodeId].Left;
        }
        else if(instructions[(int)result % instructions.Length] == 'R')
        {
            currentNodeId = network[currentNodeId].Right;
        }
        result++;
    }
    return result;
}

(string Instructions, Dictionary<string, NodeConnections> Network) ParseHauntedWasteland(string filePath)
{

    string instructions;
    var network = new Dictionary<string, NodeConnections>();
    using (StreamReader sr = new StreamReader(filePath))
    {
        instructions = sr.ReadLine();
        sr.ReadLine(); // Skip empty separator line

        string line;
        while ((line = sr.ReadLine()) != null)
        {
            var idAndConnections = line.Split(" = ");
            var id = idAndConnections[0];
            var connections = idAndConnections[1].Replace("(", string.Empty).Replace(" ", string.Empty).Replace(")", string.Empty).Split(',');
            var left = connections[0];
            var right = connections[1];
            network.Add(id, new NodeConnections { Left = left, Right = right});
        }
    }
    return (Instructions: instructions, Network: network);
}

static class ExtensionMethods
{
    private static System.Text.Json.JsonSerializerOptions serializerOptions = new System.Text.Json.JsonSerializerOptions { WriteIndented = true };
    public static string ToJson(this object o, bool indented = true)
    {
        if (indented)
        {
            return System.Text.Json.JsonSerializer.Serialize(o, serializerOptions);
        }
        return System.Text.Json.JsonSerializer.Serialize(o);
    }
}

public record NodeConnections
{
    public required string Left { get; init; }
    public required string Right { get; init; }
}
