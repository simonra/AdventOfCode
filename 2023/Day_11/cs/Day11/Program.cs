using System.Numerics;
using Microsoft.Extensions.Logging;

using ILoggerFactory factory = LoggerFactory.Create(
    builder =>
    {
        builder.SetMinimumLevel(LogLevel.Information);
        builder.SetMinimumLevel(LogLevel.Debug);
        builder.AddConsole();
    });
ILogger logger = factory.CreateLogger("Day11");

TestTranspose();
TestPart1OnSampleInput();
RunPart1();

void RunPart1()
{
    logger.LogInformation($"Starting {nameof(RunPart1)}");
    var inputFile = "input.txt";
    var result = inputFile.ParseRawImage().ExpandEmptySpace().FindGalaxyPositions().FindManhattanDistances().Sum();
    logger.LogInformation($"Part 1 result: {result}");
}

void TestPart1OnSampleInput()
{
    logger.LogInformation($"Starting {nameof(TestPart1OnSampleInput)}");
    var inputFile = "sample_input-part_1.txt";

    logger.LogInformation($"Parsed input is \n{inputFile.ParseRawImage().ToJson(false).Replace("]", "]\n")}");
    var result = inputFile.ParseRawImage().ExpandEmptySpace();
    logger.LogInformation($"ParsedAndExpanded input is \n{inputFile.ParseRawImage().ExpandEmptySpace().ToJson(false).Replace("]", "]\n")}");

    var galaxiesPositions = inputFile.ParseRawImage().ExpandEmptySpace().FindGalaxyPositions();
    foreach(var galaxyPosition in galaxiesPositions){
        logger.LogInformation($"Galaxy position is X: {galaxyPosition.X} Y: {galaxyPosition.Y}");
    }
    var manhattanDistances = inputFile.ParseRawImage().ExpandEmptySpace().FindGalaxyPositions().FindManhattanDistances();
    logger.LogInformation($"Manhattan distances between galaxies are {manhattanDistances.ToJson()}");
    var sumOfManhattanDistances = inputFile.ParseRawImage().ExpandEmptySpace().FindGalaxyPositions().FindManhattanDistances().Aggregate(0, (foldedValue, nextItem) => nextItem + foldedValue);
    logger.LogInformation($"Sum of manhattan distances between galaxies is {sumOfManhattanDistances}");

    sumOfManhattanDistances = inputFile.ParseRawImage().ExpandEmptySpace().FindGalaxyPositions().FindManhattanDistances().Sum();
    logger.LogInformation($"Sum of manhattan distances between galaxies is {sumOfManhattanDistances}");
    // uint expectedResult = 374;
    // if (result != expectedResult)
    // {
    //     logger.LogError($"Testing part 1 on sample data failed. Got {result}, expected {expectedResult}");
    // }
    // else
    // {
    //     logger.LogInformation("Testing Part 1: Processing sample data yielded expected result!");
    // }
}

void TestTranspose()
{
    var original = new List<List<int>> { new List<int> { 1, 2 }, new List<int> { 3, 4 } };
    var expected = new List<List<int>> { new List<int> { 1, 3 }, new List<int> { 2, 4 } };
    var transposed = original.Transpose();
    logger.LogDebug($"Original is\n{original.ToJson(false).Replace("]", "]\n")}");
    logger.LogDebug($"Transposed is\n{transposed.ToJson(false).Replace("]", "]\n")}");
    logger.LogDebug($"Expected is\n{transposed.ToJson(false).Replace("]", "]\n")}");
    logger.LogDebug($"Transpose result equals expected: {transposed.Equal(expected)}");

    original = new List<List<int>> { new List<int> { 1, 2, 3 }, new List<int> { 4, 5, 6 } };
    expected = new List<List<int>> { new List<int> { 1, 4 }, new List<int> { 2, 5 }, new List<int> { 3, 6 } };
    transposed = original.Transpose();
    logger.LogDebug($"Original is\n{original.ToJson(false).Replace("]", "]\n")}");
    logger.LogDebug($"Transposed is\n{transposed.ToJson(false).Replace("]", "]\n")}");
    logger.LogDebug($"Expected is\n{transposed.ToJson(false).Replace("]", "]\n")}");
    logger.LogDebug($"Transpose result equals expected: {transposed.Equal(expected)}");

    original = new List<List<int>> { new List<int> { 1, 4 }, new List<int> { 2, 5 }, new List<int> { 3, 6 } };
    expected = new List<List<int>> { new List<int> { 1, 2, 3 }, new List<int> { 4, 5, 6 } };
    transposed = original.Transpose();
    logger.LogDebug($"Original is\n{original.ToJson(false).Replace("]", "]\n")}");
    logger.LogDebug($"Transposed is\n{transposed.ToJson(false).Replace("]", "]\n")}");
    logger.LogDebug($"Expected is\n{transposed.ToJson(false).Replace("]", "]\n")}");
    logger.LogDebug($"Transpose result equals expected: {transposed.Equal(expected)}");

    original = new List<List<int>>();
    expected = new List<List<int>>();
    transposed = original.Transpose();
    logger.LogDebug($"Original is\n{original.ToJson(false).Replace("]", "]\n")}");
    logger.LogDebug($"Transposed is\n{transposed.ToJson(false).Replace("]", "]\n")}");
    logger.LogDebug($"Expected is\n{transposed.ToJson(false).Replace("]", "]\n")}");
    logger.LogDebug($"Transpose result equals expected: {transposed.Equal(expected)}");

    original = new List<List<int>> { new List<int> { 1, 2, 3 }, new List<int> { 4, 5, 6 }, new List<int> { 7, 8, 9 } };
    expected = new List<List<int>> { new List<int> { 1, 4, 7 }, new List<int> { 2, 5, 8 }, new List<int> { 3, 6, 9 } };
    transposed = original.Transpose();
    logger.LogDebug($"Original is\n{original.ToJson(false).Replace("]", "]\n")}");
    logger.LogDebug($"Transposed is\n{transposed.ToJson(false).Replace("]", "]\n")}");
    logger.LogDebug($"Expected is\n{transposed.ToJson(false).Replace("]", "]\n")}");
    logger.LogDebug($"Transpose result equals expected: {transposed.Equal(expected)}");
    logger.LogDebug($"Double transposed equals original: {original.Transpose().Transpose().Equal(original)}");
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

    public static List<List<char>> ParseRawImage(this string filePath)
    {
        var result = new List<List<char>>();
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
                result.Add(nextLine);
            }
        }
        return result;
    }

    public static List<List<char>> ExpandEmptySpace(this List<List<char>> rawImage)
    {
        var expandedInX = new List<List<char>>();
        foreach(var line in rawImage)
        {
            expandedInX.Add(line);
            if(line.All(x => x == '.'))
            {
                expandedInX.Add(line);
            }
        }
        // return result;
        var expandedInY = new List<List<char>>();
        foreach(var line in expandedInX.Transpose().Select(x => x.ToList()))
        {
            expandedInY.Add(line);
            if(line.All(x => x == '.'))
            {
                expandedInY.Add(line);
            }
        }
        return expandedInY.Transpose().Select(x => x.ToList()).ToList();
    }

    public static List<(int X, int Y)> FindGalaxyPositions(this IEnumerable<IEnumerable<char>> image)
    {
        var result = new List<(int X, int Y)>();
        for (int row = 0; row < image.Count(); row++)
        {
            var nextLine = image.Skip(row).First(); // image[i]
            for (int column = 0; column < nextLine.Count(); column++)
            {
                if(nextLine.Skip(column).First() == '#') // image[i][j] == '#'
                {
                    result.Add((X: column, Y: row));
                }
            }
        }
        return result;
    }

    public static IEnumerable<T> FindManhattanDistances<T>(this IEnumerable<(T X, T Y)> pairs)
    where T: IBinaryInteger<T>
    {
        for (int i = 0; i < pairs.Count(); i++)
        {
            var current = pairs.Skip(i).First();
            for (int j = i + 1; j < pairs.Count(); j++)
            {
                var compareNext = pairs.Skip(j).First();
                var distanceX = (current.X > compareNext.X) ? current.X - compareNext.X : compareNext.X - current.X;
                var distanceY = (current.Y > compareNext.Y) ? current.Y - compareNext.Y : compareNext.Y - current.Y;
                yield return distanceX + distanceY;
            }
        }
    }

    public static bool Equal<T>(this IEnumerable<IEnumerable<T>> first, IEnumerable<IEnumerable<T>> second)
    {
        if(first.Count() != second.Count())
        {
            return false;
        }
        for (int i = 0; i < first.Count(); i++)
        {
            if(!first.Skip(i)
                .First()
                .Count()
                .Equals(second.Skip(i).First().Count()))
            {
                return false;
            }
            if(first.Skip(i).First().Count() > 0)
            {
                for (int j = 0; j < first.Skip(i).First().Count(); j++)
                {
                    var elementOfFirst = first.Skip(i).First().Skip(j).First();
                    var elementOfSecond = second.Skip(i).First().Skip(j).First();
                    if(elementOfFirst == null)
                    {
                        if(elementOfSecond != null)
                        {
                            return false;
                        }
                        continue;
                    }
                    if(!elementOfFirst.Equals(elementOfSecond))
                    {
                        return false;
                    }
                }
            }
        }
        return true;
    }

    public static IEnumerable<IEnumerable<T>> Transpose<T>(this IEnumerable<IEnumerable<T>> source)
    {
        return source
            .Select(a => a.Select(b => Enumerable.Repeat(b, 1)))
            .DefaultIfEmpty(Enumerable.Empty<IEnumerable<T>>())
            .Aggregate((a, b) => a.Zip(b, Enumerable.Concat));
    }
}

