﻿using Microsoft.Extensions.Logging;
using System.Text.Json;

using ILoggerFactory factory = LoggerFactory.Create(
    builder =>
    {
        builder.SetMinimumLevel(LogLevel.Information);
        // builder.SetMinimumLevel(LogLevel.Debug);
        builder.AddConsole();
    });
ILogger logger = factory.CreateLogger("Day02");


// var sampleLine = "Game 1: 3 blue, 4 red; 1 red, 2 green, 6 blue; 2 green";
var part1Constraint = new Dictionary<string, uint>() {
    {"red", 12},
    {"green", 13},
    {"blue", 14},
};

var part1SampleDataFilePath = "sample_input-part_1.txt";
var part1SampleSum = SumOfIdsOfPossibleGames(part1SampleDataFilePath, part1Constraint);
var part1SampleDataExpectedSum = 8;

if (part1SampleSum == part1SampleDataExpectedSum)
{
    logger.LogInformation("Part 1 sample data successfully processed!");
}
else
{
    logger.LogError($"Failed to process part 1 sample data! Got {part1SampleSum}, expected {part1SampleDataExpectedSum}");
    var parsedGames = ParseGameLines(part1SampleDataFilePath);
    var listOfPossibleGames = IdsOfPossibleGames(parsedGames, part1Constraint);
    var serializerOptions = new JsonSerializerOptions { WriteIndented = true };
    logger.LogInformation(JsonSerializer.Serialize(parsedGames, serializerOptions));
    logger.LogInformation(JsonSerializer.Serialize(listOfPossibleGames, serializerOptions));
    logger.LogInformation(JsonSerializer.Serialize(part1Constraint, serializerOptions));
}

var part1DataFilePath = "input-part-1.txt";
var part1Sum = SumOfIdsOfPossibleGames(part1DataFilePath, part1Constraint);
logger.LogInformation($"Part 1 result is: {part1Sum}");


uint SumOfIdsOfPossibleGames(string filePath, Dictionary<string, uint> constraints)
{
    var games = ParseGameLines(filePath);
    var idsOfPossibleGames = IdsOfPossibleGames(games, constraints);
    uint result = 0;
    foreach (var id in idsOfPossibleGames)
    {
        result += id;
    }
    return result;
}

List<uint> IdsOfPossibleGames(List<Game> games, Dictionary<string, uint> constraints)
{
    var result = new List<uint>();
    foreach (var game in games)
    {
        if (IsGamePossible(game, constraints))
        {
            result.Add(game.Id);
        }
    }
    return result;
}

bool IsGamePossible(Game game, Dictionary<string, uint> constraints)
{
    logger.LogDebug($"Checking if game {game.Id} is possible");
    foreach (var drawSet in game.DrawSet)
    {
        logger.LogDebug($"    Checking draw set {JsonSerializer.Serialize(drawSet)}");
        foreach (KeyValuePair<string, uint> entry in drawSet)
        {
            if (constraints[entry.Key] < entry.Value)
            {
                logger.LogDebug($"        Draw set failed when checking drawn entry with key {entry.Key} and value {entry.Value} against constraints {constraints[entry.Key]}");
                logger.LogDebug($"Game {game.Id} is not possible");
                return false;
            }
            else
            {
                logger.LogDebug($"        Draw passed when checking drawn entry with key {entry.Key} and value {entry.Value} against constraints {constraints[entry.Key]}");
            }
        }
    }
    logger.LogDebug($"Game {game.Id} is possible");
    return true;
}

List<Game> ParseGameLines(string filePath)
{
    var result = new List<Game>();
    var lines = File.ReadLines(filePath);
    foreach (var line in lines)
    {
        var game = ParseGameLine(line);
        result.Add(game);
    }
    return result;
}

Game ParseGameLine(string gameLine)
{
    var splitIdAndDraws = gameLine.Split(':');
    var gameId = UInt16.Parse(splitIdAndDraws[0].Split(' ')[1]);
    var drawSet = ParseDrawSet(splitIdAndDraws[1]);
    var result = new Game
    {
        Id = gameId,
        DrawSet = drawSet,
    };
    return result;
    // throw new NotImplementedException();
}

List<Dictionary<string, uint>> ParseDrawSet(string drawSet)
{
    var result = new List<Dictionary<string, uint>>();
    var draws = drawSet.Split(';');
    foreach (var draw in draws)
    {
        result.Add(ParseDraw(draw));
    }
    return result;
    // throw new NotImplementedException();
}

Dictionary<string, uint> ParseDraw(string draw)
{
    var resultingDictionary = new Dictionary<string, uint>();
    var drawnCubes = draw.Split(',');
    foreach (var cubeCount in drawnCubes)
    {
        var valueTypePair = cubeCount.Trim().Split(' ');
        resultingDictionary.Add(valueTypePair[1], UInt16.Parse(valueTypePair[0]));
    }
    return resultingDictionary;
}

public record Game
{
    public required uint Id { get; init; }
    // public required List<DrawSet> Draws { get; set; } = new List<DrawSet>();
    public required List<Dictionary<string, uint>> DrawSet { get; init; } // = new List<Dictionary<string, uint>>();
}

// public record DrawSet
// {
//     public required uint Reds { get; set; } = 0;
//     public required uint Greens { get; set; } = 0;
//     public required uint Blues { get; set; } = 0;
// }
