using Microsoft.Extensions.Logging;
using System.Text.Json;

using ILoggerFactory factory = LoggerFactory.Create(
    builder =>
    {
        builder.SetMinimumLevel(LogLevel.Information);
        builder.SetMinimumLevel(LogLevel.Debug);
        builder.AddConsole();
    });
ILogger logger = factory.CreateLogger("Day03");
var serializerOptions = new JsonSerializerOptions { WriteIndented = true };

TestParseCard();
TestVeqListEquals();
TestCorrectNumbers();
TestScore();
TestPart1OnSampleInput();
RunPart1();

void RunPart1()
{
    var inputFilePath = "input.txt";
    var result = TotalScore(inputFilePath);
    logger.LogInformation($"Part 1 result is {result}");
}

void TestPart1OnSampleInput()
{
    var inputFilePath = "sample_input-part_1.txt";
    var expectedResult = 13;
    var result = TotalScore(inputFilePath);
    if (result == expectedResult)
    {
        logger.LogInformation("Testing Part 1: Processing sample data yielded expected result!");
    }
    else
    {
        logger.LogError($"Testing Part 1: Processing sample data failed. Sum is {result}, expected {expectedResult}");
    }
}

void TestParseCard()
{
    string input = "Card 1: 41 48 83 86 17 | 83 86  6 31 17  9 48 53";
    var expected = new ScratchCard
    {
        Id = 1,
        WinningNumbers = new VeqList<uint> { 41, 48, 83, 86, 17 },
        NumbersYouHave = new VeqList<uint> { 83, 86, 6, 31, 17, 9, 48, 53 }
    };
    var result = ParseScratchCard(input);
    if (result == expected)
    {
        logger.LogDebug($"Parsing scratch card line worked as expected");
    }
    else
    {
        logger.LogError($"Parsing scratch card line {input} failed. Got\n{JsonSerializer.Serialize(result, serializerOptions)}\nexpected {JsonSerializer.Serialize(result, serializerOptions)}");
    }
}

void TestVeqListEquals()
{
    var unequalDuplicatesFirst = new VeqList<int> { 1, 2, 3, 3, 3, 3, 4 };
    var unequalDuplicatesSecond = new VeqList<int> { 1, 1, 1, 2, 3, 4, 4 };
    if (unequalDuplicatesFirst != unequalDuplicatesSecond)
    {
        logger.LogDebug("Value equal lists are unequal when they contain unequal duplicates");
    }
    else
    {
        logger.LogError($"Value equal lists are claimed to be equal, even when they contain different duplicates! Something is very wrong.");
    }

    var unequalDuplicatesIgnoreOrderFirst = new VeqList<int>(requireMatchingOrder: false) { 1, 2, 3, 3, 3, 3, 4 };
    var unequalDuplicatesIgnoreOrderSecond = new VeqList<int>(requireMatchingOrder: false) { 1, 1, 1, 2, 3, 4, 4 };
    if (unequalDuplicatesIgnoreOrderFirst != unequalDuplicatesIgnoreOrderSecond)
    {
        logger.LogDebug("Value equal lists are unequal when they contain unequal duplicates and we ignore ordering for equality");
    }
    else
    {
        logger.LogError($"Value equal lists are claimed to be equal, even when they contain different duplicates and we ignore ordering for equality! Something is very wrong.");
    }

    var equalValuesDifferentOrderIgnoreOrderFirst = new VeqList<int>(requireMatchingOrder: false) { 1, 2, 3, 4 };
    var equalValuesDifferentOrderIgnoreOrderSecond = new VeqList<int>(requireMatchingOrder: false) { 4, 3, 2, 1 };
    if (equalValuesDifferentOrderIgnoreOrderFirst == equalValuesDifferentOrderIgnoreOrderSecond)
    {
        logger.LogDebug("Value equal lists are equal when they contain no duplicates and have different ordering when we compare and ignore ordering for equality");
    }
    else
    {
        logger.LogError($"Value equal lists are claimed to be unequal, even when they contain no duplicates and have different ordering when we ignore ordering for equality! Something is very wrong.");
    }

    var equalValuesDifferentOrderFirst = new VeqList<int>(requireMatchingOrder: true) { 1, 2, 3, 4 };
    var equalValuesDifferentOrderSecond = new VeqList<int>(requireMatchingOrder: true) { 4, 3, 2, 1 };
    if (equalValuesDifferentOrderFirst != equalValuesDifferentOrderSecond)
    {
        logger.LogDebug("Value equal lists are unequal when they contain no duplicates and have different ordering when we compare and care about ordering for equality");
    }
    else
    {
        logger.LogError($"Value equal lists are claimed to be equal, when they contain no duplicates and have different ordering when we ignore ordering for equality! Something is very wrong.");
    }
}

void TestCorrectNumbers()
{
    // string input = "Card 1: 41 48 83 86 17 | 83 86  6 31 17  9 48 53";
    //48, 83, 17, and 86
    var input = new ScratchCard
    {
        Id = 1,
        WinningNumbers = new VeqList<uint> { 41, 48, 83, 86, 17 },
        NumbersYouHave = new VeqList<uint> { 83, 86, 6, 31, 17, 9, 48, 53 }
    };
    var expected = new VeqList<uint> {48, 83, 86, 17};
    var result = CorrectNumbers(input);
    if (result == expected)
    {
        logger.LogDebug($"Getting correct numbers works as expected");
    }
    else
    {
        logger.LogError($"Checking correct numbers on scratch card {JsonSerializer.Serialize(input, serializerOptions)} failed. Got\n{JsonSerializer.Serialize(result, serializerOptions)}\nexpected\n{JsonSerializer.Serialize(expected, serializerOptions)}");
    }
}

void TestScore()
{
    var emptyInput = new VeqList<uint>();
    var emptyExpected = 0;
    var emptyResult = Score(emptyInput);
    if(emptyResult == emptyExpected)
    {
        logger.LogDebug($"Testing score for empty input gave expected result");
    }
    else
    {
        logger.LogError($"Testing score for empty input failed. Got {emptyResult} expected {emptyExpected}");
    }

    var lonelyInput = new VeqList<uint> { 1 };
    var lonelyExpected = 1;
    var lonelyResult = Score(lonelyInput);
    if(lonelyResult == lonelyExpected)
    {
        logger.LogDebug($"Testing score for lonely input gave expected result");
    }
    else
    {
        logger.LogError($"Testing score for lonely input failed. Got {lonelyResult} expected {lonelyExpected}");
    }

    var duoInput = new VeqList<uint> { 1, 2 };
    var duoExpected = 2;
    var duoResult = Score(duoInput);
    if(duoResult == duoExpected)
    {
        logger.LogDebug($"Testing score for duo input gave expected result");
    }
    else
    {
        logger.LogError($"Testing score for duo input failed. Got {duoResult} expected {duoExpected}");
    }

    var trioInput = new VeqList<uint> { 1, 2, 3 };
    var trioExpected = 4;
    var trioResult = Score(trioInput);
    if(trioResult == trioExpected)
    {
        logger.LogDebug($"Testing score for trio input gave expected result");
    }
    else
    {
        logger.LogError($"Testing score for trio input failed. Got {trioResult} expected {trioExpected}");
    }
}

uint TotalScore(string filePath)
{
    uint totalScore = 0;
    var lines = File.ReadLines(filePath);
    foreach (var line in lines)
    {
        var scratchCard = ParseScratchCard(line);
        var correctNumbers = CorrectNumbers(scratchCard);
        var score = Score(correctNumbers);
        totalScore += score;
    }
    return totalScore;
}

uint Score(VeqList<uint> correctNumbers)
{
    uint score = 0;
    if(correctNumbers.Count == 0)
    {
        return 0;
    }
    return (uint) Math.Pow(2, correctNumbers.Count - 1);
}

VeqList<uint> CorrectNumbers(ScratchCard scratchCard)
{
    var result = new VeqList<uint>();
    foreach (var winningNumber in scratchCard.WinningNumbers)
    {
        if(scratchCard.NumbersYouHave.Contains(winningNumber))
        {
            result.Add(winningNumber);
        }
    }
    // Task 1 example seems to be based on the lookup above instead, so let's use that and see if it helps with task 2 later
    // On further inspection, the ordering of the correct numbers matches neither side in the examples.
    // foreach (var numberHad in scratchCard.NumbersYouHave)
    // {
    //     if (scratchCard.WinningNumbers.Contains(numberHad))
    //     {
    //         result.Add(numberHad);
    //     }
    // }
    return result;
}

ScratchCard ParseScratchCard(string input)
{
    var headerSplit = input.Split(':');
    var cardId = headerSplit[0].Remove(0, 5).ParseUint();
    var numberSectionsSplit = headerSplit[1].Split('|');
    var winningNumbers = numberSectionsSplit[0].Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(x => x.ParseUint()).ToVeqList();
    var numbersYouHave = numberSectionsSplit[1].Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(x => x.ParseUint()).ToVeqList();
    return new ScratchCard { Id = cardId, WinningNumbers = winningNumbers, NumbersYouHave = numbersYouHave };
}

static class ExtensionMethods
{
    public static uint ParseUint(this string s)
    {
        return UInt16.Parse(s);
    }
    public static VeqList<TSource> ToVeqList<TSource>(this System.Collections.Generic.IEnumerable<TSource> source)
    {
        var result = new VeqList<TSource>();
        result.AddRange(source);
        return result;
    }
}

public record ScratchCard
{
    public required uint Id { get; init; }
    public required VeqList<uint> WinningNumbers { get; init; }
    public required VeqList<uint> NumbersYouHave { get; init; }
}

/// <summary>
/// An override of List that uses value equality instead of reference equality.
/// Useful when working with records that contain collections and you want to retain the value equality all the way down.
/// </summary>
/// <typeparam name="T"></typeparam>
public class VeqList<T> : List<T>
{
    // Based on https://stackoverflow.com/a/69366347
    private readonly bool _requireMatchingOrder;
    public VeqList(bool requireMatchingOrder = true) => _requireMatchingOrder = requireMatchingOrder;

    public override bool Equals(object other)
    {
        if (!(other is IEnumerable<T> otherAsEnumerable))
        {
            return false;
        }
        if (otherAsEnumerable.Count() != this.Count)
        {
            return false;
        }
        if (_requireMatchingOrder)
        {
            return otherAsEnumerable.SequenceEqual(this);
        }

        // Check if all elements present in both, including duplicates, but order doesn't matter
        // Based on https://stackoverflow.com/a/22173807
        var counts = this
            .GroupBy(v => v)
            .ToDictionary(g => g.Key, g => g.Count());
        var allValuesPresentInBoth = true;
        foreach (var otherElement in otherAsEnumerable)
        {
            int countInOther;
            if (counts.TryGetValue(otherElement, out countInOther))
            {
                counts[otherElement] = countInOther - 1;
            }
            else
            {
                allValuesPresentInBoth = false;
                break;
            }
        }
        var allValuesFoundInBoth = allValuesPresentInBoth && counts.Values.All(c => c == 0);

        return allValuesFoundInBoth;
    }

    public static bool operator ==(VeqList<T> x, VeqList<T> y)
    {
        return x.Equals(y);
    }
    public static bool operator !=(VeqList<T> x, VeqList<T> y)
    {
        return !(x.Equals(y));
    }

    public override int GetHashCode()
    {
        var hashCode = 0;
        if(_requireMatchingOrder){
            foreach (var item in this)
            {
                hashCode ^= item.GetHashCode();
            }
        }
        else
        {
            foreach (var item in this.Select(x => x).OrderBy(x => x).ToList())
            {
                hashCode ^= item.GetHashCode();
            }
        }

        return hashCode;
    }

}
