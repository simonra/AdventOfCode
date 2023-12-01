using Microsoft.Extensions.Logging;

using ILoggerFactory factory = LoggerFactory.Create(
    builder =>
    {
        builder.SetMinimumLevel(LogLevel.Information);
        // builder.SetMinimumLevel(LogLevel.Debug);
        builder.AddConsole();
    });
ILogger logger = factory.CreateLogger("Day01");

TestPart1SampleInput();
RunPart1();
TestPart2SampleInput();
RunPart2();

void RunPart1()
{
    logger.LogInformation("Running part 1");
    var sampleInputFile = "input-part-1.txt";
    var sum = GetSum(sampleInputFile);
    logger.LogInformation($"Part 1 gave result \"{sum}\"");
}

void RunPart2()
{
    logger.LogInformation("Running part 2");
    var sampleInputFile = "input-part-1.txt";
    var sum = GetSumPart2(sampleInputFile);
    logger.LogInformation($"Part 2 gave result \"{sum}\"");
}

bool TestPart1SampleInput()
{
    logger.LogInformation("Testing sample input for part 1");
    var sampleInputFile = "sample_input-part_1.txt";
    var sum = GetSum(sampleInputFile);
    var expectedSum = 142;
    if (sum == expectedSum)
    {
        logger.LogInformation("Sample input for part 1 processed as expected!");
        return true;
    }
    else
    {
        logger.LogInformation("Failed when processing sample input for part 1, did not get expected result.");
        logger.LogDebug($"    Expected {expectedSum}, got {sum}");
        return false;
    }
}

bool TestPart2SampleInput()
{
    logger.LogInformation("Testing sample input for part 2");
    var sampleInputFile = "sample_input-part_2.txt";
    var sum = GetSumPart2(sampleInputFile);
    var expectedSum = 281;
    if (sum == expectedSum)
    {
        logger.LogInformation("Sample input for part 2 processed as expected!");
        return true;
    }
    else
    {
        logger.LogInformation("Failed when processing sample input for part 2, did not get expected result.");
        logger.LogDebug($"    Expected {expectedSum}, got {sum}");
        return false;
    }
}

uint GetSum(string filePath)
{
    logger.LogDebug($"\n\n\nBeginning processing of file {filePath}");
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

uint GetSumPart2(string filePath)
{
    logger.LogDebug($"\n\n\nBeginning processing of file {filePath}");
    uint sum = 0;
    var lines = File.ReadLines(filePath);
    foreach (var line in lines)
    {
        var numbers = GetNumbersForLine(line);
        if (numbers.Count > 0)
        {
            sum += UInt16.Parse($"{numbers[0]}{numbers[numbers.Count - 1]}");
        }
    }
    logger.LogDebug($"For file {filePath} sum is {sum}");
    return sum;
}

List<char> GetNumbersForLine(string line)
{
    logger.LogDebug($"Processing line \"{line}\"");
    var result = new List<char>();
    for (int i = 0; i < line.Length; i++)
    {
        // logger.LogDebug($"    Processing position {i}");
        if (Char.IsDigit(line[i]))
        {
            logger.LogDebug($"    Found {line[i]} at position {i}");
            result.Add(line[i]);
            continue;
        }
        if (i < line.Length - 2)
        {
            // Check: one, two, six
            if (line[i] == 'o' && line[i + 1] == 'n' && line[i + 2] == 'e')
            {
                logger.LogDebug($"    Found 1 at position {i}");
                result.Add('1');
                continue;
            }
            if (line[i] == 't' && line[i + 1] == 'w' && line[i + 2] == 'o')
            {
                logger.LogDebug($"    Found 2 at position {i}");
                result.Add('2');
                continue;
            }
            if (line[i] == 's' && line[i + 1] == 'i' && line[i + 2] == 'x')
            {
                logger.LogDebug($"    Found 6 at position {i}");
                result.Add('6');
                continue;
            }

            if (i < line.Length - 3)
            {
                // Check four, five, nine
                if (line[i] == 'f' && line[i + 1] == 'o' && line[i + 2] == 'u' && line[i + 3] == 'r')
                {
                    logger.LogDebug($"    Found 4 at position {i}");
                    result.Add('4');
                    continue;
                }
                if (line[i] == 'f' && line[i + 1] == 'i' && line[i + 2] == 'v' && line[i + 3] == 'e')
                {
                    logger.LogDebug($"    Found 5 at position {i}");
                    result.Add('5');
                    continue;
                }
                if (line[i] == 'n' && line[i + 1] == 'i' && line[i + 2] == 'n' && line[i + 3] == 'e')
                {
                    logger.LogDebug($"    Found 9 at position {i}");
                    result.Add('9');
                    continue;
                }
                if (i < line.Length - 4)
                {
                    // check three, seven, eight
                    if (line[i] == 't' && line[i + 1] == 'h' && line[i + 2] == 'r' && line[i + 3] == 'e' && line[i + 4] == 'e')
                    {
                        logger.LogDebug($"    Found 3 at position {i}");
                        result.Add('3');
                        continue;
                    }
                    if (line[i] == 's' && line[i + 1] == 'e' && line[i + 2] == 'v' && line[i + 3] == 'e' && line[i + 4] == 'n')
                    {
                        logger.LogDebug($"    Found 7 at position {i}");
                        result.Add('7');
                        continue;
                    }
                    if (line[i] == 'e' && line[i + 1] == 'i' && line[i + 2] == 'g' && line[i + 3] == 'h' && line[i + 4] == 't')
                    {
                        logger.LogDebug($"    Found 8 at position {i}");
                        result.Add('8');
                        continue;
                    }
                }
            }
        }
    }

    logger.LogDebug($"    List of numbers for line is: \"{string.Join(", ", result)}\"");
    return result;
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
