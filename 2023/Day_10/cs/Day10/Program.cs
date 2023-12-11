using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

using ILoggerFactory factory = LoggerFactory.Create(
    builder =>
    {
        builder.SetMinimumLevel(LogLevel.Information);
        // builder.SetMinimumLevel(LogLevel.Debug);
        builder.AddConsole();
    });
ILogger logger = factory.CreateLogger("Day10");

var pipeSystemNeighborMatrix = new List<List<List<Position>>>();
var startPosition = new Position { X = -1, Y = -1 };

// var filePath = "sample_input-part_1.txt";
var filePath = "input.txt";

using (StreamReader sr = new StreamReader(filePath))
{
    string? line;
    var currentX = 0;
    var currentY = 0;
    while ((line = sr.ReadLine()) != null)
    {
        currentX = 0;
        var nextRow = new List<List<Position>>();
        foreach (var entry in line)
        {
            var nextNeighbors = new List<Position>();
            if (entry == '|')
            {
                nextNeighbors.Add(new Position { X = currentX, Y = currentY - 1 });
                nextNeighbors.Add(new Position { X = currentX, Y = currentY + 1 });
            }
            else if (entry == '-')
            {
                nextNeighbors.Add(new Position { X = currentX - 1, Y = currentY });
                nextNeighbors.Add(new Position { X = currentX + 1, Y = currentY });
            }
            else if (entry == 'L')
            {
                nextNeighbors.Add(new Position { X = currentX + 0, Y = currentY - 1 });
                nextNeighbors.Add(new Position { X = currentX + 1, Y = currentY + 0 });
            }
            else if (entry == 'J')
            {
                nextNeighbors.Add(new Position { X = currentX + 0, Y = currentY - 1 });
                nextNeighbors.Add(new Position { X = currentX - 1, Y = currentY + 0 });
            }
            else if (entry == '7')
            {
                nextNeighbors.Add(new Position { X = currentX - 1, Y = currentY + 0 });
                nextNeighbors.Add(new Position { X = currentX + 0, Y = currentY + 1 });
            }
            else if (entry == 'F')
            {
                nextNeighbors.Add(new Position { X = currentX + 0, Y = currentY + 1 });
                nextNeighbors.Add(new Position { X = currentX + 1, Y = currentY + 0 });
            }
            else if (entry == '.')
            {
                // Nothing to add to neighbors
            }
            else if (entry == 'S')
            {
                // Nothing to add to neighbors for now
                startPosition = new Position { X = currentX, Y = currentY };
            }
            else
            {
                throw new NotSupportedException($"Cannot parse {entry} for neighbor matrix, is not part of defined input");
            }
            nextRow.Add(nextNeighbors);
            currentX++;
        }
        pipeSystemNeighborMatrix.Add(nextRow);
        currentY++;
    }
}

logger.LogInformation($"Parsed input is {pipeSystemNeighborMatrix.ToJson()}");
logger.LogInformation($"Start position is {startPosition}");

// Add start node neighbors
var startNodeNeighbors = new List<Position>();
var neighborPosition = new Position { X = startPosition.X, Y = startPosition.Y};
// Check surrounding nodes for potential starting points
var numberOfRows = pipeSystemNeighborMatrix.Count();
var numberOfColumns = pipeSystemNeighborMatrix.First().Count();
logger.LogInformation($"System dimensions are {numberOfRows} x {numberOfColumns}");
// if(pipeSystemNeighborMatrix[startPosition.Y][startPosition.X])
if (startPosition.X - 1 > 0)
{
    // check west
    if (pipeSystemNeighborMatrix[startPosition.Y][startPosition.X - 1].Any(neighbor => neighbor == startPosition))
    {
        logger.LogInformation($"Found neighbor to the west");
        neighborPosition = new Position { X = startPosition.X - 1, Y = startPosition.Y };
        startNodeNeighbors.Add(neighborPosition);
        pipeSystemNeighborMatrix[startPosition.Y][startPosition.X].Add(neighborPosition);
    }
}
if (startPosition.X + 1 < numberOfColumns)
{
    // Check east
    if (pipeSystemNeighborMatrix[startPosition.Y][startPosition.X + 1].Any(neighbor => neighbor == startPosition))
    {
        logger.LogInformation($"Found neighbor to the east");
        neighborPosition = new Position { X = startPosition.X + 1, Y = startPosition.Y };
        startNodeNeighbors.Add(neighborPosition);
        pipeSystemNeighborMatrix[startPosition.Y][startPosition.X].Add(neighborPosition);
    }
}
if (startPosition.Y - 1 > 0)
{
    // Check north
    if (pipeSystemNeighborMatrix[startPosition.Y - 1][startPosition.X].Any(neighbor => neighbor == startPosition))
    {
        logger.LogInformation($"Found neighbor to the north");
        neighborPosition = new Position { X = startPosition.X, Y = startPosition.Y - 1 };
        startNodeNeighbors.Add(neighborPosition);
        pipeSystemNeighborMatrix[startPosition.Y][startPosition.X].Add(neighborPosition);
    }
}
if (startPosition.Y + 1 < numberOfRows)
{
    // check south
    if (pipeSystemNeighborMatrix[startPosition.Y + 1][startPosition.X].Any(neighbor => neighbor == startPosition))
    {
        logger.LogInformation($"Found neighbor to the south");
        neighborPosition = new Position { X = startPosition.X, Y = startPosition.Y + 1 };
        startNodeNeighbors.Add(neighborPosition);
        pipeSystemNeighborMatrix[startPosition.Y][startPosition.X].Add(neighborPosition);
    }
}

logger.LogInformation($"Starting pos neighbors are {pipeSystemNeighborMatrix[startPosition.Y][startPosition.X].ToJson()}");

var totalLength = FindCycleLength(pipeSystemNeighborMatrix, startPosition);
logger.LogInformation($"Total pipe cycle length is {totalLength}, meaning half way point is {totalLength/2}");

int FindCycleLength(List<List<List<Position>>> neighborMatrix, Position initialPosition)
{
    var result = 0;
    var nextPosition = neighborMatrix[initialPosition.Y][initialPosition.X].First();
    var previousPosition = initialPosition;
    // logger.LogInformation($"Previous position = {previousPosition}");
    while(nextPosition != initialPosition)
    {
        logger.LogInformation($"Next pos = {nextPosition}, neighbors {neighborMatrix[nextPosition.Y][nextPosition.X].ToJson()}");
        // var nextNeighbors = neighborMatrix[nextPosition.Y][nextPosition[x]].First(x => x != previousPosition)
        (previousPosition, nextPosition) = (nextPosition, neighborMatrix[nextPosition.Y][nextPosition.X].First(x => x != previousPosition));
        result++;
    }

    result++;

    return result;
}


static class ExtensionMethods
{
    private static System.Text.Json.JsonSerializerOptions serializerOptions = new System.Text.Json.JsonSerializerOptions { WriteIndented = false };
    public static string ToJson(this object o, bool indented = true)
    {
        if (indented)
        {
            return System.Text.Json.JsonSerializer.Serialize(o, serializerOptions);
        }
        return System.Text.Json.JsonSerializer.Serialize(o);
    }
}

public record Position
{
    public required int X { get; init; }
    public required int Y { get; init; }
}
