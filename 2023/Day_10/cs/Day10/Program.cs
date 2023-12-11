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

var symbols = new List<List<char>>();

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
        var nextSymbolRow = new List<char>();
        foreach (var entry in line)
        {
            nextSymbolRow.Add(entry);
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
                // throw new NotSupportedException($"Cannot parse {entry} for neighbor matrix, is not part of defined input");
            }
            nextRow.Add(nextNeighbors);
            currentX++;
        }
        symbols.Add(nextSymbolRow);
        pipeSystemNeighborMatrix.Add(nextRow);
        currentY++;
    }
}

logger.LogInformation($"Parsed input is {pipeSystemNeighborMatrix.ToJson()}");
logger.LogInformation($"Start position is {startPosition}");

// Add start node neighbors
var startNodeNeighbors = new List<Position>();
var neighborPosition = new Position { X = startPosition.X, Y = startPosition.Y };
// Check surrounding nodes for potential starting points
var numberOfRows = pipeSystemNeighborMatrix.Count();
var numberOfColumns = pipeSystemNeighborMatrix.First().Count();
logger.LogInformation($"System dimensions are {numberOfRows} x {numberOfColumns}");
bool startNeighborWest = false;
bool startNeighborEast = false;
bool startNeighborNorth = false;
bool startNeighborSouth = false;
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
        startNeighborWest = true;
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
        startNeighborEast = true;
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
        startNeighborNorth = true;
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
        startNeighborSouth = true;
    }
}

logger.LogInformation($"Starting pos neighbors are {pipeSystemNeighborMatrix[startPosition.Y][startPosition.X].ToJson()}");
// Update symbols
if(startNeighborNorth && startNeighborSouth) symbols[startPosition.Y][startPosition.X] = '|';
else if(startNeighborEast && startNeighborWest) symbols[startPosition.Y][startPosition.X] = '-';
else if(startNeighborNorth && startNeighborEast) symbols[startPosition.Y][startPosition.X] = 'L';
else if(startNeighborNorth && startNeighborWest) symbols[startPosition.Y][startPosition.X] = 'J';
else if(startNeighborSouth && startNeighborWest) symbols[startPosition.Y][startPosition.X] = '7';
else if(startNeighborSouth && startNeighborEast) symbols[startPosition.Y][startPosition.X] = 'F';

var totalLength = FindCycleLength(pipeSystemNeighborMatrix, startPosition);
logger.LogInformation($"Total pipe cycle length is {totalLength}, meaning half way point is {totalLength / 2}");

var numberOfPositionsEnclosed = FindNumberOfPositionsInsideLoop(pipeSystemNeighborMatrix, startPosition);
logger.LogInformation($"Number of positions enclosed by loop: {numberOfPositionsEnclosed}");

int FindCycleLength(List<List<List<Position>>> neighborMatrix, Position initialPosition)
{
    var result = 0;
    var nextPosition = neighborMatrix[initialPosition.Y][initialPosition.X].First();
    var previousPosition = initialPosition;
    // logger.LogInformation($"Previous position = {previousPosition}");
    while (nextPosition != initialPosition)
    {
        logger.LogDebug($"Next pos = {nextPosition}, neighbors {neighborMatrix[nextPosition.Y][nextPosition.X].ToJson()}");
        // var nextNeighbors = neighborMatrix[nextPosition.Y][nextPosition[x]].First(x => x != previousPosition)
        (previousPosition, nextPosition) = (nextPosition, neighborMatrix[nextPosition.Y][nextPosition.X].First(x => x != previousPosition));
        result++;
    }

    result++;

    return result;
}

int FindNumberOfPositionsInsideLoop(List<List<List<Position>>> neighborMatrix, Position initialPosition)
{
    var sizeY = pipeSystemNeighborMatrix.Count();
    var sizeX = pipeSystemNeighborMatrix.First().Count();
    // Make a map with positions being 'P' for the pipe/path/loop, 'O' for outside, 'I' for inside, and 'U' for unknown
    var map = new List<List<char>>();
    for (int y = 0; y < sizeY; y++)
    {
        var nextColumn = new List<char>();
        for (int x = 0; x < sizeX; x++)
        {
            nextColumn.Add('U');
        }
        map.Add(nextColumn);
    }

    var nextPosition = neighborMatrix[initialPosition.Y][initialPosition.X].First();
    var previousPosition = initialPosition;
    map[previousPosition.Y][previousPosition.X] = 'P';
    map[nextPosition.Y][nextPosition.X] = 'P';
    // logger.LogInformation($"Previous position = {previousPosition}");
    while (nextPosition != initialPosition)
    {
        logger.LogInformation($"Next pos = {nextPosition}, neighbors {neighborMatrix[nextPosition.Y][nextPosition.X].ToJson()}");
        // var nextNeighbors = neighborMatrix[nextPosition.Y][nextPosition[x]].First(x => x != previousPosition)
        (previousPosition, nextPosition) = (nextPosition, neighborMatrix[nextPosition.Y][nextPosition.X].First(x => x != previousPosition));
        map[nextPosition.Y][nextPosition.X] = 'P';
    }

    // logger.LogInformation($"Map after mapping pipe loop is {map.ToJson()}");

    var numberOfPositionsInside = 0;
    // Positions along the edges never have to be considered, so skip those
    for (int y = 1; y < sizeY - 1; y++)
    {
        for (int x = 1; x < sizeX - 1; x++)
        {
            if (map[y][x] != 'U')
            {
                continue;
            }
            // var smallestNumberOfPipesToCross = int.MaxValue;
            var numberOfPipesToCrossForCurrentDirection = 0;
            var countBendL = 0;
            var countBend7 = 0;
            var countBendF = 0;
            var countBendJ = 0;
            // Check east
            for (int i = x - 1; i >= 0; i--)
            {
                if (map[y][i] == 'P')
                {
                    if (symbols[y][i] == '|')
                    {
                        numberOfPipesToCrossForCurrentDirection++;
                    }
                    else if(symbols[y][i] == 'L')
                    {
                        countBendL++;
                    }
                    else if(symbols[y][i] == '7')
                    {
                        countBend7++;
                    }
                    else if(symbols[y][i] == 'F')
                    {
                        countBendF++;
                    }
                    else if(symbols[y][i] == 'J')
                    {
                        countBendJ++;
                    }
                }
            }
            // Each pair of fj counts as 1
            // Each pair of L7 counts as 1
            // Each else of these counts as 1 alone?
            // 2 * abs diff + rest?
            if(countBendF == countBendJ)
            {
                numberOfPipesToCrossForCurrentDirection += countBendF;
            }
            else if(countBendF > countBendJ)
            {
                numberOfPipesToCrossForCurrentDirection += countBendF;
            }
            else
            {
                numberOfPipesToCrossForCurrentDirection += countBendJ;
            }
            if(countBendL == countBend7)
            {
                numberOfPipesToCrossForCurrentDirection += countBendL;
            }
            else if(countBendL > countBend7)
            {
                numberOfPipesToCrossForCurrentDirection += countBendL;
            }
            else
            {
                numberOfPipesToCrossForCurrentDirection += countBend7;
            }
            if (numberOfPipesToCrossForCurrentDirection % 2 == 0)
            {
                map[y][x] = 'O';
                continue;
            }
            // if(numberOfPipesToCrossForCurrentDirection < smallestNumberOfPipesToCross)
            // {
            //     smallestNumberOfPipesToCross = numberOfPipesToCrossForCurrentDirection;
            // }
            numberOfPipesToCrossForCurrentDirection = 0;
            countBendL = 0;
            countBend7 = 0;
            countBendF = 0;
            countBendJ = 0;
            // Check west
            for (int i = x + 1; i < sizeX; i++)
            {
                if (map[y][i] == 'P')
                {
                    if (symbols[y][i] == '|')
                    {
                        numberOfPipesToCrossForCurrentDirection++;
                    }
                    else if(symbols[y][i] == 'L')
                    {
                        countBendL++;
                    }
                    else if(symbols[y][i] == '7')
                    {
                        countBend7++;
                    }
                    else if(symbols[y][i] == 'F')
                    {
                        countBendF++;
                    }
                    else if(symbols[y][i] == 'J')
                    {
                        countBendJ++;
                    }
                }
            }
            if(countBendF == countBendJ)
            {
                numberOfPipesToCrossForCurrentDirection += countBendF;
            }
            else if(countBendF > countBendJ)
            {
                numberOfPipesToCrossForCurrentDirection += countBendF;
            }
            else
            {
                numberOfPipesToCrossForCurrentDirection += countBendJ;
            }
            if(countBendL == countBend7)
            {
                numberOfPipesToCrossForCurrentDirection += countBendL;
            }
            else if(countBendL > countBend7)
            {
                numberOfPipesToCrossForCurrentDirection += countBendL;
            }
            else
            {
                numberOfPipesToCrossForCurrentDirection += countBend7;
            }
            if (numberOfPipesToCrossForCurrentDirection % 2 == 0)
            {
                map[y][x] = 'O';
                continue;
            }
            // if(numberOfPipesToCrossForCurrentDirection < smallestNumberOfPipesToCross)
            // {
            //     smallestNumberOfPipesToCross = numberOfPipesToCrossForCurrentDirection;
            // }
            numberOfPipesToCrossForCurrentDirection = 0;
            countBendL = 0;
            countBend7 = 0;
            countBendF = 0;
            countBendJ = 0;
            // Check north
            for (int i = y - 1; i >= 0; i--)
            {
                if (map[i][x] == 'P')
                {
                    if (symbols[i][x] == '-')
                    {
                        numberOfPipesToCrossForCurrentDirection++;
                    }
                    else if(symbols[i][x] == 'L')
                    {
                        countBendL++;
                    }
                    else if(symbols[i][x] == '7')
                    {
                        countBend7++;
                    }
                    else if(symbols[i][x] == 'F')
                    {
                        countBendF++;
                    }
                    else if(symbols[i][x] == 'J')
                    {
                        countBendJ++;
                    }
                }
            }
            if(countBendF == countBendJ)
            {
                numberOfPipesToCrossForCurrentDirection += countBendF;
            }
            else if(countBendF > countBendJ)
            {
                numberOfPipesToCrossForCurrentDirection += countBendF;
            }
            else
            {
                numberOfPipesToCrossForCurrentDirection += countBendJ;
            }
            if(countBendL == countBend7)
            {
                numberOfPipesToCrossForCurrentDirection += countBendL;
            }
            else if(countBendL > countBend7)
            {
                numberOfPipesToCrossForCurrentDirection += countBendL;
            }
            else
            {
                numberOfPipesToCrossForCurrentDirection += countBend7;
            }
            if (numberOfPipesToCrossForCurrentDirection % 2 == 0)
            {
                map[y][x] = 'O';
                continue;
            }
            numberOfPipesToCrossForCurrentDirection = 0;
            countBendL = 0;
            countBend7 = 0;
            countBendF = 0;
            countBendJ = 0;
            // Check south
            for (int i = y + 1; i < sizeY; i++)
            {
                if (map[i][x] == 'P')
                {
                    if (symbols[i][x] == '-')
                    {
                        numberOfPipesToCrossForCurrentDirection++;
                    }
                    else if(symbols[i][x] == 'L')
                    {
                        countBendL++;
                    }
                    else if(symbols[i][x] == '7')
                    {
                        countBend7++;
                    }
                    else if(symbols[i][x] == 'F')
                    {
                        countBendF++;
                    }
                    else if(symbols[i][x] == 'J')
                    {
                        countBendJ++;
                    }
                }
            }
            if(countBendF == countBendJ)
            {
                numberOfPipesToCrossForCurrentDirection += countBendF;
            }
            else if(countBendF > countBendJ)
            {
                numberOfPipesToCrossForCurrentDirection += countBendF;
            }
            else
            {
                numberOfPipesToCrossForCurrentDirection += countBendJ;
            }
            if(countBendL == countBend7)
            {
                numberOfPipesToCrossForCurrentDirection += countBendL;
            }
            else if(countBendL > countBend7)
            {
                numberOfPipesToCrossForCurrentDirection += countBendL;
            }
            else
            {
                numberOfPipesToCrossForCurrentDirection += countBend7;
            }
            if (numberOfPipesToCrossForCurrentDirection % 2 == 0)
            {
                map[y][x] = 'O';
                continue;
            }
            numberOfPipesToCrossForCurrentDirection = 0;
            countBendL = 0;
            countBend7 = 0;
            countBendF = 0;
            countBendJ = 0;

            map[y][x] = 'I';
            numberOfPositionsInside++;
        }
    }

    logger.LogInformation($"Map after mapping pipe loop is\n{map.ToJson().Replace("]", "]\n")}");

    return numberOfPositionsInside;
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
