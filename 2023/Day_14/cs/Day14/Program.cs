using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Microsoft.Extensions.Logging;

using ILoggerFactory factory = LoggerFactory.Create(
    builder =>
    {
        builder.SetMinimumLevel(LogLevel.Information);
        builder.SetMinimumLevel(LogLevel.Debug);
        builder.AddConsole();
    });
ILogger logger = factory.CreateLogger("Day14");

TestPart1OnSampleInput();
RunPart1();

void RunPart1()
{
    logger.LogInformation($"Starting {nameof(RunPart1)}");
    var inputFile = "input.txt";
    var result = inputFile.ParseMap().TiltNorth().TotalLoad();
    logger.LogInformation($"Part 1 result: {result} (expected 109939)");
}


void TestPart1OnSampleInput()
{
    logger.LogInformation($"Starting {nameof(TestPart1OnSampleInput)}");
    var inputFile = "sample_input-part_1.txt";

    var tiltedNorth = inputFile.ParseMap().TiltNorth();
    logger.LogInformation($"System tilted north is\n{tiltedNorth.ToJson(false).Replace("]","\n]")}");

    var result = inputFile.ParseMap().TiltNorth().TotalLoad();
    uint expectedResult = 136;
    if (result != expectedResult)
    {
        logger.LogError($"Testing part 1 on sample data failed. Got {result}, expected {expectedResult}");
    }
    else
    {
        logger.LogInformation("Testing Part 1: Processing sample data yielded expected result!");
    }
}

// TestIndexOfApproaches();
// void TestIndexOfApproaches()
// {
//     var input = "O.#..O.#.#".ToCharArray();
//     var indexes = input.IndexesOf('#');
//     logger.LogInformation($"Indexes are: {indexes.ToJson(false)}");
// }


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

    public static List<List<char>> ParseMap(this string filePath)
    {
        var resultingMap = new List<List<char>>();
        using (StreamReader sr = new StreamReader(filePath))
        {
            string? line;
            while ((line = sr.ReadLine()) != null)
            {
                var nextLine = new List<char>();
                foreach(var character in line)
                {
                    nextLine.Add(character);
                }
                resultingMap.Add(nextLine);
            }
        }
        return resultingMap;
    }

    public static List<List<char>> TiltNorth(this List<List<char>> source)
    {
        var transposed = source.Transpose().CollectLists();
        var sortedSystem = new List<List<char>>();
        foreach(var row in transposed)
        {
            // Console.WriteLine($"Row before\t{row.ToJson(false)}");
            // var sectionsBetweenFixedPositions =
            var fixedPointsInRow = row.IndexesOf('#').ToList();
            var relevantFixedPointIndexes = new List<int>();
            if(fixedPointsInRow.Any())
            {
                relevantFixedPointIndexes.AddRange(fixedPointsInRow.Where(x => !fixedPointsInRow.Any(y => y == x + 1)));
                if(relevantFixedPointIndexes.First() != 0)  relevantFixedPointIndexes = relevantFixedPointIndexes.Prepend(-1).ToList();
                if(relevantFixedPointIndexes.Last() != row.Count() - 1)  relevantFixedPointIndexes = relevantFixedPointIndexes.Append(row.Count()).ToList();
            }
            else
            {
                relevantFixedPointIndexes.Add(-1);
                relevantFixedPointIndexes.Add(row.Count());
            }
            // var numberOfOsPerGroup = relevantFixedPointIndexes.SlidingWindow(2).Select(fixedPointPair => row.CountInRange('O', fixedPointPair.First() + 1, fixedPointPair.Skip(1).First() - 1));

            var rowSorted = new List<char>();
            for (int i = 0; i < row.Count(); i++)
            {
                rowSorted.Add('.');
            }
            foreach(var pointIndex in fixedPointsInRow)
            {
                rowSorted[pointIndex] = '#';
            }
            foreach(var fixedPointPair in relevantFixedPointIndexes.SlidingWindow(2))
            {
                var firstOffset = fixedPointPair.First() + 1;
                var lastOffset = fixedPointPair.Skip(1).First();
                var numberOfOs = row.CountInRange('O', firstOffset, lastOffset);
                for (int i = 0; i < numberOfOs; i++)
                {
                    rowSorted[firstOffset + i] = 'O';
                }
            }

            // Console.WriteLine($"Row after\t{rowSorted.ToJson(false)}\n");
            sortedSystem.Add(rowSorted);
        }

        return sortedSystem.Transpose().CollectLists();
    }

    public static int TotalLoad(this List<List<char>> source)
    {
        var multiplier = source.Count();
        var sumLoad = 0;
        for (int i = 0; i < source.Count(); i++)
        {
            var numberOfOs = source[i].Count(x => x == 'O');
            sumLoad += numberOfOs * multiplier;
            multiplier--;
        }
        return sumLoad;
    }

    public static int CountInRange<T>(this IEnumerable<T> source, T value, int earliestOffset, int latestOffset)
    where T : IEqualityOperators<T, T, bool>
    {
        var rangeSize = latestOffset - earliestOffset;
        return source.Skip(earliestOffset).Take(rangeSize).Count(x => x == value);
    }

    public static IEnumerable<int> IndexesOf<T>(this IEnumerable<T> source, T value)
    where T : IEqualityOperators<T, T, bool>
    {
        return source.IndexesWhere(x => x == value);
    }

    public static IEnumerable<int> IndexesWhere<T>(this IEnumerable<T> source, Func<T, bool> predicate)
    {
        int i = 0;
        foreach(var item in source)
        {
            if(predicate(item)) yield return i;
            i++;
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

    public static IEnumerable<IEnumerable<T>> SlidingWindow<T>(this IEnumerable<T> source, int windowSize)
    {
        if(source.Count() <= windowSize)
        {
            yield return source;
        }
        else
        {
            for (int i = 0; i < source.Count() - windowSize + 1; i++)
            {
                yield return source.Skip(i).Take(windowSize);
            }
        }
    }
}
