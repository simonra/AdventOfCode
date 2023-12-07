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
ILogger logger = factory.CreateLogger("Day06");

logger.LogInformation($"Part 1: Winning way products for sample input: {WaysToWinProductPart1("sample_input-part_1.txt")}");
logger.LogInformation($"Part 1 solution: {WaysToWinProductPart1("input.txt")}");
logger.LogInformation($"Part 2: Winning ways for sample input: {WaysToWinProductPart2("sample_input-part_1.txt")}");
logger.LogInformation($"Part 2 solution: {WaysToWinProductPart2("input.txt")}");

uint WaysToWinProductPart1(string filePath)
{
    var racesToBeat = ParseRacingInput(filePath);
    if(racesToBeat.Times.Count() != racesToBeat.Distances.Count()){
        throw new Exception($"Expected number of race time limits ({racesToBeat.Times.Count()}) to match number of records ({racesToBeat.Distances.Count()})");
    }
    uint product = 1;
    for (int i = 0; i < racesToBeat.Times.Count(); i++)
    {
        var time = racesToBeat.Times[i];
        var distance = racesToBeat.Distances[i];
        // var shortestHoldTime = ShortestHoldTimeToBeatRecord(time, distance);
        // var longestHoldTime = LongestHoldTimeToBeatRecord(time, distance);
        // var numberOfPossibleWinningTimings = longestHoldTime - shortestHoldTime + 2;
        // logger.LogDebug($"When processing race with time {time} and distance {distance}, got shortest hold time {shortestHoldTime} and longest hold time {longestHoldTime} yielding {numberOfPossibleWinningTimings} possible winning timings");
        var winningTimings = WinningTimings(time, distance);
        product *= (uint)winningTimings.Count();
    }

    return product;
}

(uint[] Times, uint[] Distances) ParseRacingInput(string filePath)
{
    uint[] times;
    uint[] distances;
    using (StreamReader sr = new StreamReader(filePath))
    {
        times = sr.ReadLine()
            .Split(':')[1]
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Select(t => t.ToUint())
            .ToArray();
        distances = sr.ReadLine()
            .Split(':')[1]
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Select(t => t.ToUint())
            .ToArray();
    }
    return (Times: times, Distances: distances);
}

(UInt128 TimeLimit, UInt128 RecordDistance) ParseRacingInputPart2(string filePath)
{
    UInt128 time;
    UInt128 distance;
    using (StreamReader sr = new StreamReader(filePath))
    {
        time = sr.ReadLine()
            .Split(':')[1]
            .Replace(" ", "")
            .ParseUint128();
        distance = sr.ReadLine()
            .Split(':')[1]
            .Replace(" ", "")
            .ParseUint128();
    }
    return (TimeLimit: time, RecordDistance: distance);
}

UInt128 WaysToWinProductPart2(string filePath)
{
    var raceToBeat = ParseRacingInputPart2(filePath);
    UInt128 numberOfWaysToWin = 0;
    UInt128 previousBest = 0;
    for (UInt128 timing = 0; timing < raceToBeat.TimeLimit; timing++)
    {
        var distance = Distance128(timing, raceToBeat.TimeLimit);
        if(raceToBeat.RecordDistance < distance)
        {
            numberOfWaysToWin++;
            previousBest = numberOfWaysToWin;
        }
        else{
            if(previousBest > 0)
            {
                break;
            }
        }
        if(timing % 1000 == 0)
        {
            logger.LogDebug($"Timing {timing} (of {raceToBeat.TimeLimit}) checked, current count of ways to win is {numberOfWaysToWin}");
        }
    }
    return numberOfWaysToWin;
}

List<uint> WinningTimings(uint timeLimit, uint recordDistance)
{
    var result = new List<uint>();
    for (uint timing = 0; timing < timeLimit; timing++)
    {
        var distance = Distance(timing, timeLimit);
        if(recordDistance < distance)
        {
            result.Add(timing);
        }
    }
    return result;
}

// So, as the task is given, the times have to be positive whole integers.
// The distances traveled also have to be positive whole integers.
// And therefore the times I find also have to be positive whole integers.
// Naturally, then, I should use the type system to help me, so that the language can highlight where I'm going the wrong way
uint ShortestHoldTimeToBeatRecord(uint timeLimit, uint recordDistance)
{
    var initialGuess = Math.Ceiling((timeLimit - Math.Pow((timeLimit*timeLimit - 4 * 1 * recordDistance), 0.5))/(2 * 1)).ToUint();
    if(Distance(initialGuess, timeLimit) == recordDistance)
    {
        return initialGuess + 1;
    }
    return initialGuess;
}

uint LongestHoldTimeToBeatRecord(uint timeLimit, uint recordDistance)
{
    var initialGuess = Math.Floor((timeLimit + Math.Pow((timeLimit*timeLimit - 4 * 1 * recordDistance), 0.5))/(2 * 1)).ToUint();
    if(Distance(initialGuess, timeLimit) == recordDistance)
    {
        return initialGuess - 1;
    }
    return initialGuess;
}

uint Distance(uint holdTime, uint timeLimit)
{
    return holdTime * (timeLimit - holdTime);
}

UInt128 Distance128(UInt128 holdTime, UInt128 timeLimit)
{
    return holdTime * (timeLimit - holdTime);
}

uint IncreaseHoldTimeUntilRecordBeaten(uint holdTime, uint recordDistance)
{
    uint current;
    uint next;
    uint distanceCurrent;
    uint distanceNext;
    while(true)
    {

    }
    throw new NotImplementedException();
}

uint DecreaseHoldTimeUntilRecordBeaten(uint holdTime, uint recordDistance)
{
    throw new NotImplementedException();
}

// uint Distance(uint timeHeld, uint timeLimit)
// {
//     var speed = timeHeld;
//     var travelTime = timeLimit - timeHeld;
//     var distance = speed * travelTime;

//     var travelTime = timeLimit - speed;
//     var distance = speed * (timeLimit - speed);
//     - speed ^ 2 + speed * timeLimit - distance = 0;
//     speed ^ 2 - timeLimit * speed + distance = 0;

//     speed = (timeLimit + Math.Pow((timeLimit*timeLimit - 4 * 1 * distance), 0.5))/(2 * 1);

//     currentRecordMaxHeld = (timeLimit + Math.Pow((timeLimit*timeLimit - 4 * 1 * recordDistance), 0.5))/(2 * 1);
//     currentRecordMinHeld = (timeLimit - Math.Pow((timeLimit*timeLimit - 4 * 1 * recordDistance), 0.5))/(2 * 1);

//     diff = currentRecordMaxHeld - currentRecordMinHeld;
// }

// List<uint> WinningTimes(uint timeLimit, uint distanceToBeat)
// {
//     var minimumHeld = "x";
//     var minimumTravelTime = timeLimit - minimumHeld;
//     var minimumSpeed = distanceToBeat/minimumTravelTime;

// }

// uint Velocity(uint timeHeld)
// {

// }

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

    public static uint ToUint(this string s)
    {
        return UInt16.Parse(s);
    }

    public static UInt128 ParseUint128(this string s)
    {
        return UInt128.Parse(s);
    }

    public static uint ToUint(this double d)
    {
        if(d < 0)
        {
            throw new ArgumentOutOfRangeException($"Error when converting {d} to uint. Cannot make unsigned integer out of negative value");
        }
        if(d > uint.MaxValue)
        {
            throw new ArgumentOutOfRangeException($"Error when converting {d} to uint. Number is too large to convert to uint");
        }
        if(d % 1 != 0)
        {
            throw new ArgumentOutOfRangeException($"Error when converting {d} to uint. Number has remainder when divided by 1");
        }
        return (uint) d;
    }
}
