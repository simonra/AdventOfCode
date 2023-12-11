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
ILogger logger = factory.CreateLogger("Day09");

// TestSlidingWindow();
TestPart1OnSampleInput();
RunPart1();
TestPart2OnSampleInput();
RunPart2();

void RunPart1()
{
    logger.LogInformation($"Starting {nameof(RunPart1)}");
    var inputFile = "input.txt";
    var result = SumOfPredictionsOfLast<Int128>(inputFile);
    logger.LogInformation($"Part 1 result: {result} (expected 2005352194)");
}

void RunPart2()
{
    logger.LogInformation($"Starting {nameof(RunPart2)}");
    var inputFile = "input.txt";
    var result = SumOfPredictionsOfFirst<Int128>(inputFile);
    logger.LogInformation($"Part 1 result: {result} (expected 1077)");
}

void TestSlidingWindow()
{
    var original = Enumerable.Range(1, 15).ToList();
    logger.LogInformation($"Original list is {original.ToJson()}");

    // var largerWindow = original.SlidingWindow(20);
    foreach(var window in original.SlidingWindow(20))
    {
        logger.LogInformation($"When window is 20, next entry is {window.ToJson()}");
    }

    foreach(var window in original.SlidingWindow(2))
    {
        logger.LogInformation($"When window is 2, next entry is {window.ToJson()}");
    }

    foreach(var window in original.SlidingWindow(3))
    {
        logger.LogInformation($"When window is 3, next entry is {window.ToJson()}");
    }

    foreach(var window in original.Select(x => x * 2).Differences())
    {
        logger.LogInformation($"Differences are {window.ToJson()}");
    }
}

void TestPart1OnSampleInput()
{
    logger.LogInformation($"Starting {nameof(TestPart1OnSampleInput)}");
    var inputFile = "sample_input-part_1.txt";

    var result = SumOfPredictionsOfLast<Int128>(inputFile);
    uint expectedResult = 114;
    if (result != expectedResult)
    {
        logger.LogError($"Testing part 1 on sample data failed. Got {result}, expected {expectedResult}");
    }
    else
    {
        logger.LogInformation("Testing Part 1: Processing sample data yielded expected result!");
    }
}

void TestPart2OnSampleInput()
{
    logger.LogInformation($"Starting {nameof(TestPart2OnSampleInput)}");
    var inputFile = "sample_input-part_1.txt";

    var result = SumOfPredictionsOfFirst<Int128>(inputFile);
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

T SumOfPredictionsOfLast<T>(string filePath)
where T : INumber<T>
{
    // string instructions;
    // var network = new Dictionary<string, NodeConnections>();
    T result = T.Zero;
    using (StreamReader sr = new StreamReader(filePath))
    {
        string? line;
        while ((line = sr.ReadLine()) != null)
        {
            result += line.ParseOasisLine<T>().OasisLinePredictionOfLast<T>();
        }
    }
    return result;
}

T SumOfPredictionsOfFirst<T>(string filePath)
where T : INumber<T>
{
    // string instructions;
    // var network = new Dictionary<string, NodeConnections>();
    T result = T.Zero;
    using (StreamReader sr = new StreamReader(filePath))
    {
        string? line;
        while ((line = sr.ReadLine()) != null)
        {
            result += line.ParseOasisLine<T>().OasisLinePredictionOfFirst<T>();
        }
    }
    return result;
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

    public static T OasisLinePredictionOfLast<T>(this IEnumerable<T> input)
    where T : INumber<T>
    {
        var reductions = new List<List<T>>();
        reductions.Add(input.ToList());
        while(reductions.Last().Any(x => x != T.Zero))
        {
            reductions.Add(reductions.Last().Differences().ToList());
        }
        // .TakeLast(2).
        var result = reductions.Select(x => x.Last()).Reverse().Aggregate(T.Zero, (accumulated, next) => accumulated + next);
        return result;
    }

    public static T OasisLinePredictionOfFirst<T>(this IEnumerable<T> input)
    where T : INumber<T>
    {
        var reductions = new List<List<T>>();
        reductions.Add(input.ToList());
        while(reductions.Last().Any(x => x != T.Zero))
        {
            reductions.Add(reductions.Last().Differences().ToList());
        }
        // .TakeLast(2).
        var result = reductions.Select(x => x.First()).Reverse().Aggregate(T.Zero, (accumulated, next) => next - accumulated);
        return result;
    }

    public static List<T> ParseOasisLine<T>(this string input)
    where T : INumber<T>
    {
        return input.Split(' ').Select(x => T.Parse(x, null)).ToList();
    }

    // public static List<Int128> ParseOasisLine(this string input)
    // {
    //     return input.Split(' ').Select(x => x.ToInt128()).ToList();
    // }

    public static uint ToUint(this string s)
    {
        return UInt16.Parse(s);
    }

    public static Int128 ToInt128(this string s)
    {
        return Int128.Parse(s);
    }

    public static IEnumerable<T> Differences<T>(this IEnumerable<T> input)
    where T : INumber<T>
    {
        return input.SlidingWindow(2).Select(pair => pair.Skip(1).First() - pair.First());
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
