using Microsoft.Extensions.Logging;

using ILoggerFactory factory = LoggerFactory.Create(
    builder =>
    {
        // builder.SetMinimumLevel(LogLevel.Debug);
        builder.SetMinimumLevel(LogLevel.Information);
        builder.AddConsole();
    });
ILogger logger = factory.CreateLogger("Day01");

TestSampleInput();
RunPart1();

void RunPart1()
{
    logger.LogInformation("Running part 1");
    var sampleInputFile = "input-part-1.txt";
    var sum = GetSum(sampleInputFile);
    logger.LogInformation($"Part 1 gave result \"{sum}\"");
}

bool TestSampleInput()
{
    logger.LogInformation("Testing sample input");
    var sampleInputFile = "sample_input.txt";
    var sum = GetSum(sampleInputFile);
    if (sum == 142)
    {
        logger.LogInformation("Sample input processed as expected!");
        return true;
    }
    else
    {
        logger.LogInformation("Failed when processing sample input, did not get expected result.");
        return false;
    }
}

uint GetSum(string filePath)
{
    uint sum = 0;
    var lines = File.ReadLines(filePath);
    foreach (var line in lines)
    {
        var number = GetNumberForLine(line);
        if (number != null)
        {
            sum += (uint)number;
        }
    }
    logger.LogDebug($"For file {filePath} sum is {sum}");
    return sum;
}

uint? GetNumberForLine(string line)
{
    logger.LogDebug($"Processing line \"{line}\"");
    var firstDigit = GetFirstDigit(line);
    if (firstDigit != null)
    {
        logger.LogDebug($"    First digit in line is '{firstDigit}'");
        var lastDigit = GetLastDigit(line);
        if (lastDigit != null)
        {
            logger.LogDebug($"    Last digit in line is '{lastDigit}'");
            return GetNumber(firstDigit, lastDigit);
        }
    }
    else
    {
        logger.LogDebug($"    Line contained no digit :o");
    }
    return null;
}

uint? GetNumber(char? firstChar, char? secondChar)
{
    if (firstChar == null || secondChar == null)
    {
        return null;
    }
    return UInt16.Parse($"{firstChar}{secondChar}");
}

char? GetFirstDigit(string inputString)
{
    foreach (var character in inputString)
    {
        if (Char.IsDigit(character))
        {
            // return UInt16.Parse($"{character}");
            return character;
        }
    }
    logger.LogWarning($"WARNING: got string without digit!. Problematic string is\"{inputString}\"");
    return null;
}

char? GetLastDigit(string inputString)
{
    return GetFirstDigit(ReverseString(inputString));
}

string ReverseString(string inputString)
{
    char[] array = inputString.ToCharArray();
    Array.Reverse(array);
    return new string(array);
}
