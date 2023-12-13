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
ILogger logger = factory.CreateLogger("Day13");

TestPart1OnSampleInput();
RunPart1();
TestPart2OnSampleInput();
RunPart2();

void RunPart1()
{
    logger.LogInformation($"Starting {nameof(RunPart1)}");
    var inputFile = "input.txt";
    var lavaMaps = inputFile.ParseLavaMaps();
    var rowsAboveReflections = lavaMaps.Select(x => x.IndexesOfRowReflections()).Aggregate(0, (aggregatedValue, nextItem) => aggregatedValue + nextItem.Aggregate(0, (av, ni) => av + ni*100));
    var columnsLeftOfReflections = lavaMaps.Select(x => x.IndexesOfColumnReflections()).Aggregate(0, (aggregatedValue, nextItem) => aggregatedValue + nextItem.Sum());
    var result = rowsAboveReflections + columnsLeftOfReflections;
    logger.LogInformation($"Part 1 result: {result}");
}

void RunPart2()
{
    logger.LogInformation($"Starting {nameof(RunPart1)}");
    var inputFile = "input.txt";
    var lavaMaps = inputFile.ParseLavaMaps();
    var rowsAboveReflections = lavaMaps.Select(x => x.IndexesOfRowReflectionsGiven1Smudge()).Aggregate(0, (aggregatedValue, nextItem) => aggregatedValue + nextItem.Aggregate(0, (av, ni) => av + ni*100));
    var columnsLeftOfReflections = lavaMaps.Select(x => x.IndexesOfColumnReflectionsGiven1Smudge()).Aggregate(0, (aggregatedValue, nextItem) => aggregatedValue + nextItem.Sum());
    var result = rowsAboveReflections + columnsLeftOfReflections;
    logger.LogInformation($"Part 2 result: {result}");
}

void TestPart1OnSampleInput()
{
    logger.LogDebug($"Starting {nameof(TestPart1OnSampleInput)}");
    var inputFile = "sample_input-part_1.txt";

    logger.LogDebug($"Parsed input is \n{inputFile.ParseLavaMaps().ToJson(false).Replace("]", "]\n")}");
    logger.LogDebug($"Indexes of row reflections are {inputFile.ParseLavaMaps().Select(x => x.IndexesOfRowReflections()).ToJson()}");

    logger.LogDebug($"Indexes of column reflections are {inputFile.ParseLavaMaps().Select(x => x.IndexesOfColumnReflections()).ToJson()}");

    var lavaMaps = inputFile.ParseLavaMaps();
    var rowsAboveReflections = lavaMaps.Select(x => x.IndexesOfRowReflections()).Aggregate(0, (aggregatedValue, nextItem) => aggregatedValue + nextItem.Aggregate(0, (av, ni) => av + ni*100));
    var columnsLeftOfReflections = lavaMaps.Select(x => x.IndexesOfColumnReflections()).Aggregate(0, (aggregatedValue, nextItem) => aggregatedValue + nextItem.Sum());
    var result = rowsAboveReflections + columnsLeftOfReflections;
    logger.LogInformation($"Test file result is result: {result}");
}

void TestPart2OnSampleInput()
{
    logger.LogDebug($"Starting {nameof(TestPart1OnSampleInput)}");
    var inputFile = "sample_input-part_1.txt";

    logger.LogDebug($"Parsed input is \n{inputFile.ParseLavaMaps().ToJson(false).Replace("]", "]\n")}");
    logger.LogDebug($"Indexes of row reflections are {inputFile.ParseLavaMaps().Select(x => x.IndexesOfRowReflectionsGiven1Smudge()).ToJson()}");

    logger.LogDebug($"Indexes of column reflections are {inputFile.ParseLavaMaps().Select(x => x.IndexesOfColumnReflectionsGiven1Smudge()).ToJson()}");

    var lavaMaps = inputFile.ParseLavaMaps();
    var rowsAboveReflections = lavaMaps.Select(x => x.IndexesOfRowReflectionsGiven1Smudge()).Aggregate(0, (aggregatedValue, nextItem) => aggregatedValue + nextItem.Aggregate(0, (av, ni) => av + ni*100));
    var columnsLeftOfReflections = lavaMaps.Select(x => x.IndexesOfColumnReflectionsGiven1Smudge()).Aggregate(0, (aggregatedValue, nextItem) => aggregatedValue + nextItem.Sum());
    var result = rowsAboveReflections + columnsLeftOfReflections;
    logger.LogInformation($"Test file result is result: {result}");
}

static class ExtensionMethods
{
    public static string ToJson(this object o, bool indented = true)
    {
        if (indented)
        {
            return System.Text.Json.JsonSerializer.Serialize(o, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
        }
        return System.Text.Json.JsonSerializer.Serialize(o);
    }

    public static IEnumerable<int> IndexesOfColumnReflections(this IEnumerable<IEnumerable<char>> lavaMap)
    {
        return lavaMap.Transpose().IndexesOfRowReflections();
    }

    public static IEnumerable<int> IndexesOfRowReflections(this IEnumerable<IEnumerable<char>> lavaMap)
    {
        var randomAccessLavaMap = lavaMap.CollectLists();
        var size = randomAccessLavaMap.Count();
        var halfPointIndex = size / 2;
        var evenSize = size % 2 == 0;
        // var numberOfRowReflections = 0;
        if(evenSize)
        {
            for (int i = 1; i <= halfPointIndex; i++)
            {
                if(lavaMap.Take(i*2).IsReflectedAboutRowMiddle()) yield return i;
                if(i != halfPointIndex)
                {
                    if(lavaMap.Skip(size - i*2).Take(i*2).IsReflectedAboutRowMiddle()) yield return size - i;
                }
            }
        }
        else
        {
            for (int i = 1; i <= halfPointIndex; i++)
            {
                if(lavaMap.Take(i*2).IsReflectedAboutRowMiddle()) yield return i;
                if(lavaMap.Skip(size - i*2).Take(i*2).IsReflectedAboutRowMiddle()) yield return size - i;
            }
        }

        // Console.WriteLine($"Done processing map");
        // return numberOfRowReflections;
    }

    public static IEnumerable<int> IndexesOfColumnReflectionsGiven1Smudge(this IEnumerable<IEnumerable<char>> lavaMap)
    {
        return lavaMap.Transpose().IndexesOfRowReflectionsGiven1Smudge();
    }

    public static IEnumerable<int> IndexesOfRowReflectionsGiven1Smudge(this IEnumerable<IEnumerable<char>> lavaMap)
    {
        var randomAccessLavaMap = lavaMap.CollectLists();
        var size = randomAccessLavaMap.Count();
        var halfPointIndex = size / 2;
        var evenSize = size % 2 == 0;
        // var numberOfRowReflections = 0;
        if(evenSize)
        {
            for (int i = 1; i <= halfPointIndex; i++)
            {
                if(lavaMap.Take(i*2).IsReflectedAboutRowMiddleGiven1Smudge()) yield return i;
                if(i != halfPointIndex)
                {
                    if(lavaMap.Skip(size - i*2).Take(i*2).IsReflectedAboutRowMiddleGiven1Smudge()) yield return size - i;
                }
            }
        }
        else
        {
            for (int i = 1; i <= halfPointIndex; i++)
            {
                if(lavaMap.Take(i*2).IsReflectedAboutRowMiddleGiven1Smudge()) yield return i;
                if(lavaMap.Skip(size - i*2).Take(i*2).IsReflectedAboutRowMiddleGiven1Smudge()) yield return size - i;
            }
        }

        // Console.WriteLine($"Done processing map");
        // return numberOfRowReflections;
    }

    public static bool IsReflectedAboutRowMiddleGiven1Smudge(this IEnumerable<IEnumerable<char>> lavaMap)
    {
        var randomAccessLavaMap = lavaMap.CollectLists();
        // Console.WriteLine($"Reflection checking rows\n{randomAccessLavaMap.ToJson(false).Replace("]","\n]").Replace("[[","[ [")}");
        var numberOfSmudges = 0;
        if(randomAccessLavaMap.Count() % 2 != 0)
        {
            throw new NotSupportedException($"Collection to check for reflections about middle in row dimension has to have an even number of rows. Input had {randomAccessLavaMap.Count()} rows. Input was {randomAccessLavaMap.ToJson(false)}");
        }

        for (int row = 0; row < randomAccessLavaMap.Count() / 2; row++)
        {
            for (int column = 0; column < randomAccessLavaMap[row].Count(); column++)
            {
                if(randomAccessLavaMap[row][column] != randomAccessLavaMap[randomAccessLavaMap.Count() - 1 - row][column])
                {
                    numberOfSmudges++;
                }
            }
            if(numberOfSmudges > 1)
            {
                // Console.WriteLine($"Number of smudges exceeded 1");
                return false;
            }
        }
        // Console.WriteLine($"Number of smudges at end {numberOfSmudges}");
        // if(numberOfSmudges == 1) return true;
        return numberOfSmudges == 1;
    }

    public static bool IsReflectedAboutRowMiddle(this IEnumerable<IEnumerable<char>> lavaMap)
    {
        var randomAccessLavaMap = lavaMap.CollectLists();
        // Console.WriteLine($"Reflection checking rows\n{randomAccessLavaMap.ToJson(false).Replace("]","\n]").Replace("[[","[ [")}");
        if(randomAccessLavaMap.Count() % 2 != 0)
        {
            throw new NotSupportedException($"Collection to check for reflections about middle in row dimension has to have an even number of rows. Input had {randomAccessLavaMap.Count()} rows. Input was {randomAccessLavaMap.ToJson(false)}");
        }

        for (int i = 0; i < randomAccessLavaMap.Count() / 2; i++)
        {
            if(!randomAccessLavaMap[i].SequenceEqual(randomAccessLavaMap[randomAccessLavaMap.Count() - 1 - i]))
            {
                return false;
            }
        }
        return true;
    }

    public static IEnumerable<List<List<char>>> ParseLavaMaps(this string filePath)
    {
        var nextMap = new List<List<char>>();
        using (StreamReader sr = new StreamReader(filePath))
        {
            string? line;
            while ((line = sr.ReadLine()) != null)
            {
                if(line == string.Empty)
                {
                    yield return nextMap;
                    nextMap = new List<List<char>>();
                    continue;
                }
                var nextLine = new List<char>();
                foreach(var character in line)
                {
                    nextLine.Add(character);
                }
                nextMap.Add(nextLine);
            }
        }
        if(nextMap.Any())
        {
            yield return nextMap;
        }
    }

    public static IEnumerable<IEnumerable<T>> Transpose<T>(this IEnumerable<IEnumerable<T>> source)
    {
        return source
            .Select(a => a.Select(b => Enumerable.Repeat(b, 1)))
            .DefaultIfEmpty(Enumerable.Empty<IEnumerable<T>>())
            .Aggregate((a, b) => a.Zip(b, Enumerable.Concat));
    }

    public static List<List<T>> CollectLists<T>(this IEnumerable<IEnumerable<T>> source)
    {
        return source.Select(x => x.ToList()).ToList();
    }

    public static T[][] CollectArrays<T>(this IEnumerable<IEnumerable<T>> source)
    {
        return source.Select(x => x.ToArray()).ToArray();
    }
}
