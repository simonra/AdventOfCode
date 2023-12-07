using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;

using ILoggerFactory factory = LoggerFactory.Create(
    builder =>
    {
        builder.SetMinimumLevel(LogLevel.Information);
        builder.SetMinimumLevel(LogLevel.Debug);
        // builder.SetMinimumLevel(LogLevel.Trace);
        builder.AddConsole();
    });
ILogger logger = factory.CreateLogger("Day05");

GetLowestLocationPart1("sample_input-part_1.txt");
GetLowestLocationPart1("input.txt");
GetLowestLocationPart2("sample_input-part_1.txt");
GetLowestLocationPart2("input.txt");

List<SeedRange> ParseSeedsPart2(string[] input)
{
    var result = new List<SeedRange>();
    for (uint i = 0; i < input.Count(); i += 2)
    {
        result.Add(
            new SeedRange
            {
                SeedRangeStart = input[i].ParseUint(),
                RangeSize = input[i + 1].ParseUint()
            });
        // for (uint j = 0; j < input[i + 1].ParseUint(); j++)
        // {
        //     var seedNumber = input[i].ParseUint() + j;
        //     result.Add(seedNumber, seedNumber);
        // }
    }
    return result;
}

Dictionary<uint, uint> ParseSeedsPart1(string[] input)
{
    var result = new Dictionary<uint, uint>();
    foreach (var seedString in input)
    {
        var seedNumber = seedString.ParseUint();
        result.Add(seedNumber, seedNumber);
    }
    return result;
}

void GetLowestLocationPart2(string filePath)
{
    using (StreamReader sr = new StreamReader(filePath))
    {
        string? line;
        // Read and display lines from the file until the end of
        // the file is reached.
        var seeds = sr.ReadLine();
        logger.LogDebug($"Seeds are: {seeds}");
        var unparsedSeedsArray = seeds.Split(':')[1].Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var seedRanges = ParseSeedsPart2(unparsedSeedsArray);
        // var seedDict = ParseSeedsPart1(unparsedSeedsArray);
        // var seedDict = ParseSeedsPart2(unparsedSeedsArray);
        logger.LogDebug($"Seed ranges are {seedRanges.ToJson()}");

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

        uint lowestLocation = uint.MaxValue;

        for (uint location = 0; location < uint.MaxValue; location++)
        {
            var humidity = GetSourceValue(humidityToLocationMaps, location);
            var temperature = GetSourceValue(temperatureToHumidityMaps, humidity);
            var light = GetSourceValue(lightToTemperatureMaps, temperature);
            var water = GetSourceValue(waterToLightMaps, light);
            var fertilizer = GetSourceValue(fertilizerToWaterMaps, water);
            var soil = GetSourceValue(soilToFertilizerMaps, fertilizer);
            var seedCandidate = GetSourceValue(seedToSoilMaps, soil);
            if (IsInSeedRanges(seedRanges, seedCandidate))
            {
                lowestLocation = location;
                logger.LogInformation($"Lowest location is {lowestLocation}");
                return;
            }
            if (location % 1000 == 0)
            {
                logger.LogDebug($"Done processing up to location {location}");
            }
        }
    }
}

void GetLowestLocationPart1(string filePath)
{
    using (StreamReader sr = new StreamReader(filePath))
    {
        string? line;
        // Read and display lines from the file until the end of
        // the file is reached.
        var seeds = sr.ReadLine();
        logger.LogDebug($"Seeds are: {seeds}");
        var unparsedSeedsArray = seeds.Split(':')[1].Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var seedDict = ParseSeedsPart1(unparsedSeedsArray);
        // var seedDict = ParseSeedsPart2(unparsedSeedsArray);
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
        foreach (var seedToValuePair in seedDict)
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
        foreach (var seedToValuePair in seedDict)
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
        foreach (var seedToValuePair in seedDict)
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
        foreach (var seedToValuePair in seedDict)
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
        foreach (var seedToValuePair in seedDict)
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
        foreach (var seedToValuePair in seedDict)
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
        foreach (var seedToValuePair in seedDict)
        {
            seedDict[seedToValuePair.Key] = GetDestinationValue(humidityToLocationMaps, seedDict[seedToValuePair.Key]);
        }
        logger.LogDebug($"Given seeds : locations are {seedDict.ToJson()}");

        uint lowestLocation = uint.MaxValue;
        foreach (var seedToValuePair in seedDict)
        {
            if (seedDict[seedToValuePair.Key] < lowestLocation)
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
    foreach (var map in almanacMaps)
    {
        // If mapped, return value
        if (map.SourceRangeStart <= sourceValue)
        {
            var offset = sourceValue - map.SourceRangeStart;
            if (offset <= map.RangeSize - 1)
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
    foreach (var map in almanacMaps)
    {
        // If mapped, return value
        if (map.DestinationRangeStart <= destinationValue)
        {
            var offset = destinationValue - map.DestinationRangeStart;
            if (offset <= map.RangeSize - 1)
            {
                return map.SourceRangeStart + offset;
            }
        }
    }
    // If not found, return input value
    return destinationValue;
}

bool IsInSeedRanges(IEnumerable<SeedRange> ranges, uint value)
{
    foreach (var range in ranges)
    {
        if (IsInSeedRange(range, value))
        {
            return true;
        }
    }
    return false;
}

bool IsInSeedRange(SeedRange range, uint value)
{
    if (range.SeedRangeStart <= value)
    {
        var offset = value - range.SeedRangeStart;
        if (offset <= range.RangeSize - 1)
        {
            return true;
        }
    }
    return false;
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

public record SeedRange
{
    public uint SeedRangeStart { get; init; }
    public uint RangeSize { get; init; }
}

