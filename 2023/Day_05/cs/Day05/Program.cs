using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Logging;

using ILoggerFactory factory = LoggerFactory.Create(
    builder =>
    {
        builder.SetMinimumLevel(LogLevel.Information);
        // builder.SetMinimumLevel(LogLevel.Debug);
        // builder.SetMinimumLevel(LogLevel.Trace);
        builder.AddConsole();
    });
ILogger logger = factory.CreateLogger("Day05");

ParseInput("sample_input-part_1.txt");
ParseInput("input.txt");

void ParseInput(string filePath)
{
    // var lines = File.ReadLines(filePath);
    // var seeds = lines.Take(1);
    // logger.LogInformation($"Seeds are: {seeds.ToJson()}");
    // foreach (var line in lines)
    // {
    //     logger.LogInformation($"Next line is: {line}");
    // }

    using (StreamReader sr = new StreamReader(filePath))
    {
        string? line;
        // Read and display lines from the file until the end of
        // the file is reached.
        var seeds = sr.ReadLine();
        logger.LogDebug($"Seeds are: {seeds}");
        var seedDict = new Dictionary<uint, uint>();
        var unparsedSeedsArray = seeds.Split(':')[1].Split(' ', StringSplitOptions.RemoveEmptyEntries);
        foreach(var seedString in unparsedSeedsArray)
        {
            var seedNumber = seedString.ParseUint();
            seedDict.Add(seedNumber, seedNumber);
        }
        logger.LogDebug($"Seeds dict is {seedDict.ToJson()}");

        sr.ReadLine(); // Skip empty line

        string nextSectionExpected = "seed-to-soil map:";
        string? nextSection = sr.ReadLine();
        if (nextSection != nextSectionExpected)
        {
            throw new InvalidDataException($"Expected line to be section heading {nextSectionExpected}, got {nextSection}");
        }
        var seedToSoilMaps = new List<AlmanacMap>();
        while ((line = sr.ReadLine()) != "")
        {
            logger.LogTrace($"Next line in section {nextSection} is: {line}");
            seedToSoilMaps.Add(ParseAlmanacMapLine(line));
        }
        logger.LogDebug($"Seed to soils maps are {seedToSoilMaps.ToJson()}");
        foreach(var seedToValuePair in seedDict)
        {
            seedDict[seedToValuePair.Key] = GetDestinationValue(seedToSoilMaps, seedDict[seedToValuePair.Key]);
        }
        logger.LogDebug($"Given seeds : soils are {seedDict.ToJson()}");

        nextSectionExpected = "soil-to-fertilizer map:";
        nextSection = sr.ReadLine();
        if (nextSection != nextSectionExpected)
        {
            throw new InvalidDataException($"Expected line to be section heading {nextSectionExpected}, got {nextSection}");
        }
        var soilToFertilizerMaps = new List<AlmanacMap>();
        while ((line = sr.ReadLine()) != "")
        {
            logger.LogTrace($"Next line in section {nextSection} is: {line}");
            soilToFertilizerMaps.Add(ParseAlmanacMapLine(line));
        }
        logger.LogDebug($"Soil to fertilizer maps are {soilToFertilizerMaps.ToJson()}");
        foreach(var seedToValuePair in seedDict)
        {
            seedDict[seedToValuePair.Key] = GetDestinationValue(soilToFertilizerMaps, seedDict[seedToValuePair.Key]);
        }
        logger.LogDebug($"Given seeds : fertilizers are {seedDict.ToJson()}");

        nextSectionExpected = "fertilizer-to-water map:";
        nextSection = sr.ReadLine();
        if (nextSection != nextSectionExpected)
        {
            throw new InvalidDataException($"Expected line to be section heading {nextSectionExpected}, got {nextSection}");
        }
        var fertilizerToWaterMaps = new List<AlmanacMap>();
        while ((line = sr.ReadLine()) != "")
        {
            logger.LogTrace($"Next line in section {nextSection} is: {line}");
            fertilizerToWaterMaps.Add(ParseAlmanacMapLine(line));
        }
        logger.LogDebug($"Fertilizer to water maps are {fertilizerToWaterMaps.ToJson()}");
        foreach(var seedToValuePair in seedDict)
        {
            seedDict[seedToValuePair.Key] = GetDestinationValue(fertilizerToWaterMaps, seedDict[seedToValuePair.Key]);
        }
        logger.LogDebug($"Given seeds : water are {seedDict.ToJson()}");

        nextSectionExpected = "water-to-light map:";
        nextSection = sr.ReadLine();
        if (nextSection != nextSectionExpected)
        {
            throw new InvalidDataException($"Expected line to be section heading {nextSectionExpected}, got {nextSection}");
        }
        var waterToLightMaps = new List<AlmanacMap>();
        while ((line = sr.ReadLine()) != "")
        {
            logger.LogTrace($"Next line in section {nextSection} is: {line}");
            waterToLightMaps.Add(ParseAlmanacMapLine(line));
        }
        logger.LogDebug($"Water to light maps are {waterToLightMaps.ToJson()}");
        foreach(var seedToValuePair in seedDict)
        {
            seedDict[seedToValuePair.Key] = GetDestinationValue(waterToLightMaps, seedDict[seedToValuePair.Key]);
        }
        logger.LogDebug($"Given seeds : water are {seedDict.ToJson()}");

        nextSectionExpected = "light-to-temperature map:";
        nextSection = sr.ReadLine();
        if (nextSection != nextSectionExpected)
        {
            throw new InvalidDataException($"Expected line to be section heading {nextSectionExpected}, got {nextSection}");
        }
        var lightToTemperatureMaps = new List<AlmanacMap>();
        while ((line = sr.ReadLine()) != "")
        {
            logger.LogTrace($"Next line in section {nextSection} is: {line}");
            lightToTemperatureMaps.Add(ParseAlmanacMapLine(line));
        }
        logger.LogDebug($"Light to temperature maps are {lightToTemperatureMaps.ToJson()}");
        foreach(var seedToValuePair in seedDict)
        {
            seedDict[seedToValuePair.Key] = GetDestinationValue(lightToTemperatureMaps, seedDict[seedToValuePair.Key]);
        }
        logger.LogDebug($"Given seeds : temperatures are {seedDict.ToJson()}");

        nextSectionExpected = "temperature-to-humidity map:";
        nextSection = sr.ReadLine();
        if (nextSection != nextSectionExpected)
        {
            throw new InvalidDataException($"Expected line to be section heading {nextSectionExpected}, got {nextSection}");
        }
        var temperatureToHumidityMaps = new List<AlmanacMap>();
        while ((line = sr.ReadLine()) != "")
        {
            logger.LogTrace($"Next line in section {nextSection} is: {line}");
            temperatureToHumidityMaps.Add(ParseAlmanacMapLine(line));
        }
        logger.LogDebug($"Temperature to humidity lines are {temperatureToHumidityMaps.ToJson()}");
        foreach(var seedToValuePair in seedDict)
        {
            seedDict[seedToValuePair.Key] = GetDestinationValue(temperatureToHumidityMaps, seedDict[seedToValuePair.Key]);
        }
        logger.LogDebug($"Given seeds : humidities are {seedDict.ToJson()}");

        nextSectionExpected = "humidity-to-location map:";
        nextSection = sr.ReadLine();
        if (nextSection != nextSectionExpected)
        {
            throw new InvalidDataException($"Expected line to be section heading {nextSectionExpected}, got {nextSection}");
        }
        var humidityToLocationMaps = new List<AlmanacMap>();
        while ((line = sr.ReadLine()) != null)
        {
            logger.LogTrace($"Next line in section {nextSection} is: {line}");
            humidityToLocationMaps.Add(ParseAlmanacMapLine(line));
        }
        logger.LogDebug($"Humidity to LocationMaps are {humidityToLocationMaps.ToJson()}");
        foreach(var seedToValuePair in seedDict)
        {
            seedDict[seedToValuePair.Key] = GetDestinationValue(humidityToLocationMaps, seedDict[seedToValuePair.Key]);
        }
        logger.LogDebug($"Given seeds : locations are {seedDict.ToJson()}");

        uint lowestLocation = uint.MaxValue;
        foreach(var seedToValuePair in seedDict)
        {
            if(seedDict[seedToValuePair.Key] < lowestLocation)
            {
                lowestLocation = seedDict[seedToValuePair.Key];
            }
        }

        logger.LogInformation($"Lowest location is {lowestLocation}");
    }
}

AlmanacMap ParseAlmanacMapLine(string input)
{
    var parts = input.Split(' ');
    return new AlmanacMap
    {
        DestinationRangeStart = parts[0].ParseUint(),
        SourceRangeStart = parts[1].ParseUint(),
        RangeSize = parts[2].ParseUint()
    };
}

uint GetDestinationValue(IEnumerable<AlmanacMap> almanacMaps, uint sourceValue)
{
    foreach(var map in almanacMaps)
    {
        // If mapped, return value
        if(map.SourceRangeStart <= sourceValue)
        {
            var offset = sourceValue - map.SourceRangeStart;
            if(offset <= map.RangeSize - 1)
            {
                return map.DestinationRangeStart + offset;
            }
        }
    }
    // If not found, return input value
    return sourceValue;
}

uint GetSourceValue(IEnumerable<AlmanacMap> almanacMaps, uint destinationValue)
{
    foreach(var map in almanacMaps)
    {
        // If mapped, return value
        if(map.DestinationRangeStart <= destinationValue)
        {
            var offset = destinationValue - map.DestinationRangeStart;
            if(offset <= map.RangeSize - 1)
            {
                return map.SourceRangeStart + offset;
            }
        }
    }
    // If not found, return input value
    return destinationValue;
    throw new NotImplementedException();
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

    public static uint ParseUint(this string s)
    {
        return UInt32.Parse(s);
    }
}

public record AlmanacMap
{
    public uint DestinationRangeStart { get; init; }
    public uint SourceRangeStart { get; init; }
    public uint RangeSize { get; init; }
}


