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
ILogger logger = factory.CreateLogger("Day08");

TestPart1OnSampleInput();
RunPart1();
TestExtendedEuclideanAlgorithmGcd();
TestLeastCommonMultipleOfPair();
TestLeastCommonMultipleOfCollection();
RunPart2();
// TestEnumeratePrimeFactors();
TestChineseRemainderTheoremTryFindSimultaneousSolution();

void TestChineseRemainderTheoremTryFindSimultaneousSolution()
{
    var congruences = new List<(Int128 a, Int128 m)> {(a: 2, m: 3), (a: 3, m: 5), (a: 2, m: 7)};
    var hasSolution = ChineseRemainderTheorem.TryFindSimultaneousSolution(congruences, out var solution);
    logger.LogInformation($"Processing input {congruences.ToJson()} hasSolution: {hasSolution}, the solution is {solution} (expected (23, 105))");

    congruences = new List<(Int128 a, Int128 m)> {(a: 0, m: 16897),(a: 0, m: 16343),(a: 0, m: 21883),(a: 0, m: 13019),(a: 0, m: 14681),(a: 0, m: 20221),};
    hasSolution = ChineseRemainderTheorem.TryFindSimultaneousSolution(congruences, out solution);
    logger.LogInformation($"Processing input {congruences.ToJson()} hasSolution: {hasSolution}, the solution is {solution} (expected (0, 14321394058031))");

    congruences = new List<(Int128 a, Int128 m)> {(a: 0, m: (16897/277)),(a: 0, m: (16343/277)),(a: 0, m: (21883/277)),(a: 0, m: (13019/277)),(a: 0, m: (14681/277)),(a: 0, m: (20221/277)),};
    hasSolution = ChineseRemainderTheorem.TryFindSimultaneousSolution(congruences, out solution);
    logger.LogInformation($"Processing input {congruences.ToJson()} hasSolution: {hasSolution}, the solution is {solution} (expected (0, 51701783603))");
}

void TestEnumeratePrimeFactors()
{
    var number = 337500; //  2*2*3*3*3*5*5*5*5*5
    var primeFactors = number.EnumeratePrimeFactors().ToList();
    logger.LogInformation($"Factorization of {number} yielded {primeFactors.ToJson()}");

    number = 3;
    primeFactors = number.EnumeratePrimeFactors().ToList();
    logger.LogInformation($"Factorization of {number} yielded {primeFactors.ToJson()}");

    var collection = new List<(int Remainder, int Prime, int Power)>() {(1, 2, 3), (1, 2, 4), (1, 3, 1), (1, 5, 1), (1, 5, 2), (1, 5, 3), (1, 2, 3), (1, 2, 3), (1, 2, 3), (1, 2, 3), (1, 2, 3) };
    collection = collection.OrderBy(x => x.Prime).ThenBy(x => x.Power).ToList();
    foreach(var item in collection)
    {
        logger.LogInformation($"The collection item is {item}");
    }
    logger.LogInformation($"Now the distinct items are");
    collection = collection.Distinct().ToList();
    foreach(var item in collection)
    {
        logger.LogInformation($"The collection item is {item}");
    }
    var groups = collection.GroupBy(x => x.Prime);
    logger.LogInformation($"The groups are {groups.ToJson()}");
    // for (int i = 0; i < groups.Count(); i++)
    // {
    //     logger.LogInformation($"The group is {groups.Skip(i).Take(1).ToJson()}");
    //     if(i > 0)
    //     {
    //         logger.LogInformation($"The previous group is {groups.Skip(i - 1).Take(1).ToJson()}");
    //     }
    // }
    logger.LogInformation("For eaching the groups");
    foreach(var group in groups)
    {
        var firstElement = group.First();
        logger.LogInformation($"The first element is {firstElement}");
        if(group.Count() > 1)
        {
            logger.LogInformation($"The second element is {group.Skip(1).First()}");
        }
    }
}

void RunPart1()
{
    logger.LogInformation("Starting RunPart1");
    var inputFile = "input.txt";
    var hauntedWastelands = ParseHauntedWasteland(inputFile);
    var result = NumberOfStepsFromAAAToZZZ(hauntedWastelands.Instructions, hauntedWastelands.Network);
    logger.LogInformation($"Part 1 result: {result}");
}

void RunPart2()
{
    logger.LogInformation("Starting RunPart2");
    var inputFile = "input.txt";
    var hauntedWastelands = ParseHauntedWasteland(inputFile);
    // var result = NumberOfStepsFromAllXANodesTillAllOnXZNodesBruteForce(hauntedWastelands.Instructions, hauntedWastelands.Network);
    var result = NumberOfStepsFromAllXANodesTillAllOnXZNodesCycleMath(hauntedWastelands.Instructions, hauntedWastelands.Network);
    logger.LogInformation($"Part 2 result: {result}");
}

void TestPart1OnSampleInput()
{
    logger.LogInformation("Starting TestPart1OnSampleInput");
    var inputFile = "sample_input-part_1.txt";
    var hauntedWastelands = ParseHauntedWasteland(inputFile);
    logger.LogInformation($"Haunted wastelands are {hauntedWastelands.Network.ToJson()}");
    var result = NumberOfStepsFromAAAToZZZ(hauntedWastelands.Instructions, hauntedWastelands.Network);
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

UInt64 NumberOfStepsFromAllXANodesTillAllOnXZNodesBruteForce(string instructions, Dictionary<string, NodeConnections> network)
{
    UInt64 result = 0;
    var currentNodes = network.Keys.Where(k => k.EndsWith('A')).ToArray();
    var numberOfPaths = currentNodes.Count();
    UInt64 numberOfInstructions = (UInt64)instructions.Length;
    int nextInstructionOffset;
    var startTime = DateTime.Now;
    while (true)
    {
        if (currentNodes.All(n => n.EndsWith('Z')))
        {
            break;
        }
        nextInstructionOffset = (int)(result % numberOfInstructions);
        for (int i = 0; i < numberOfPaths; i++)
        {
            var currentNodeId = currentNodes[i];
            if (instructions[nextInstructionOffset] == 'L')
            {
                currentNodes[i] = network[currentNodeId].Left;
            }
            else if (instructions[nextInstructionOffset] == 'R')
            {
                currentNodes[i] = network[currentNodeId].Right;
            }
        }
        result++;

        if (result % 10000000 == 0)
        {
            logger.LogInformation($"{DateTime.Now} Running for {DateTime.Now - startTime} At iteration {result.ToString("N0", new System.Globalization.NumberFormatInfo { NumberGroupSeparator = " " })} ({(double)result / (double)UInt64.MaxValue} of uint.MaxValue)");
        }
    }
    return result;
}

// bool IsNodeOfInterest(string node)
// {
//     return node.EndsWith('Z');
// }

UInt64 NumberOfStepsFromAllXANodesTillAllOnXZNodesCycleMath(string instructions, Dictionary<string, NodeConnections> network)
{
    UInt64 result = 1;
    var startingNodes = network.Keys.Where(k => k.EndsWith('A')).ToArray();
    var cyclesByOrigin = new Dictionary<string, Dictionary<string, List<uint>>>();
    foreach (var node in startingNodes)
    {
        var foundCycles = FindCycles(instructions, network, node);
        cyclesByOrigin.Add(node, foundCycles);
    }

    logger.LogInformation($"Cycles per origin {cyclesByOrigin.ToJson()}");
    var cycleLengths = new List<UInt64>();
    UInt64 allCycleLengthsProduct = 277;
    foreach (var originCycles in cyclesByOrigin.Values)
    {
        foreach (var cycleByDestination in originCycles.Values)
        {
            var cycleLength = cycleByDestination[1] - cycleByDestination[0];
            cycleLengths.Add(cycleLength);
            allCycleLengthsProduct *= (cycleLength / 277);
        }
    }
    logger.LogInformation($"Product of all cycle lengths is {allCycleLengthsProduct}");
    logger.LogInformation($"Product of all cycle lengths without common divisor is {allCycleLengthsProduct/277}");
    // var cycleLengths = cyclesByOrigin.Values.Select(x => x.First()).Select(x => x.Value.First()).ToList();
    var lcm = LeastCommonMultipleOfCollection(cycleLengths);
    // var lcm = LeastCommonMultipleOfCollection(cycleLengths.Select(x => x/277).ToList());
    logger.LogInformation($"LCM is {lcm}");

    UInt64 longestCycleLength = cycleLengths.Max();

    return 0;
    var startTime = DateTime.Now;
    while (true)
    {
        if (cycleLengths.All(x => result % x == 0))
        {
            break;
        }

        result += longestCycleLength;

        if ((result / longestCycleLength) % 10000 == 5)
        {
            logger.LogInformation($"{DateTime.Now} Running for {DateTime.Now - startTime} At iteration {result.ToString("N0", new System.Globalization.NumberFormatInfo { NumberGroupSeparator = " " })} ({(double)result / (double)UInt64.MaxValue} of uint.MaxValue)");
        }
    }

    // var numberOfPaths = currentNodes.Count();
    // UInt64 numberOfInstructions = (UInt64)instructions.Length;
    // int nextInstructionOffset;
    // var startTime = DateTime.Now;
    // while(true)
    // {
    //     if(currentNodes.All(n => n.EndsWith('Z')))
    //     {
    //         break;
    //     }
    //     nextInstructionOffset = (int)(result % numberOfInstructions);
    //     for (int i = 0; i < numberOfPaths; i++)
    //     {
    //         var currentNodeId = currentNodes[i];
    //         if(instructions[nextInstructionOffset] == 'L')
    //         {
    //             currentNodes[i] = network[currentNodeId].Left;
    //         }
    //         else if(instructions[nextInstructionOffset] == 'R')
    //         {
    //             currentNodes[i] = network[currentNodeId].Right;
    //         }
    //     }
    //     result++;

    //     if(result % 10000000 == 0)
    //     {
    //         logger.LogInformation($"{DateTime.Now} Running for {DateTime.Now - startTime} At iteration {result.ToString("N0", new System.Globalization.NumberFormatInfo {NumberGroupSeparator = " "})} ({(double)result/(double)UInt64.MaxValue} of uint.MaxValue)");
    //     }
    // }
    // return result;

    return 0;

    // throw new NotImplementedException();
}

(T BezoutCoefficientS, T BezoutCoefficientT, T GreatestCommonDivisor, T GcdQuotientS, T GcdQuotientT) ExtendedEuclideanAlgorithmGcd<T>(T a, T b)
where T : System.Numerics.IBinaryInteger<T>
{
    // Based on https://en.wikipedia.org/wiki/Extended_Euclidean_algorithm
    T zero = a - a;
    if (a == zero || b == zero)
    {
        return (BezoutCoefficientS: zero, BezoutCoefficientT: zero, GreatestCommonDivisor: zero, GcdQuotientS: zero, GcdQuotientT: zero);
    }
    T one = a / a;

    T old_r = a;
    T r = b;
    T old_s = one;
    T s = zero;
    T old_t = zero;
    T t = one;
    T quotient = zero;
    T temp = zero;
    while (r != zero)
    {
        quotient = old_r / r; // integer division! Make sure to floor if using floats. Maybe consider Math.DivRem() in dotnet
        temp = r;
        r = old_r - quotient * r;
        old_r = temp;
        temp = s;
        s = old_s - quotient * s;
        old_s = temp;
        temp = t;
        t = old_t - quotient * t;
        old_t = temp;
    }
    return (BezoutCoefficientS: old_s, BezoutCoefficientT: old_t, GreatestCommonDivisor: old_r, GcdQuotientS: s, GcdQuotientT: t);
}

T LeastCommonMultiple<T>(T a, T b)
where T : System.Numerics.IBinaryInteger<T>
{
    checked
    {
        var greatestCommonDivisor = ExtendedEuclideanAlgorithmGcd(a, b).GreatestCommonDivisor;
        logger.LogInformation($"Greatest common divisor of {a} and {b} for LCM is {greatestCommonDivisor}");
        if(greatestCommonDivisor == T.Zero)
        {
            return T.Zero;
        }
        return (a / greatestCommonDivisor) * b;
    }
}

T LeastCommonMultipleOfCollection<T>(IEnumerable<T> numbers)
where T : System.Numerics.IBinaryInteger<T>
{
    return numbers.Aggregate(LeastCommonMultiple);
}

void TestLeastCommonMultipleOfPair()
{
    var a = 8;
    var b = 9;
    var result = LeastCommonMultiple(a, b);
    logger.LogInformation($"Least common multiple of {a} and {b} is {result}");

    a = 21;
    b = 6;
    result = LeastCommonMultiple(a, b);
    logger.LogInformation($"Least common multiple of {a} and {b} is {result} (expected 42)");
}

void TestLeastCommonMultipleOfCollection()
{
    var numbers = new List<uint> { 8, 9, 21 };
    var result = LeastCommonMultipleOfCollection(numbers);
    logger.LogInformation($"Least common multiple of collection {numbers.ToJson()} is {result} (expected 504)");
}

void TestExtendedEuclideanAlgorithmGcd()
{
    var a = 3;
    var b = 7;
    var result = ExtendedEuclideanAlgorithmGcd(a, b);
    logger.LogInformation($"When a is {a} and b is {b} BezoutCoefficientS: {result.BezoutCoefficientS}, BezoutCoefficientT: {result.BezoutCoefficientT}, GreatestCommonDivisor: {result.GreatestCommonDivisor}, GcdQuotientS: {result.GcdQuotientS}, GcdQuotientT: {result.GcdQuotientT}");
    logger.LogInformation($"The GCD {result.GreatestCommonDivisor} should be equal to a * s + b * t, so {a} * {result.BezoutCoefficientS} + {b} * {result.BezoutCoefficientT} = {result.GreatestCommonDivisor}, which for our result is {a * result.BezoutCoefficientS + b * result.BezoutCoefficientT == result.GreatestCommonDivisor} ");

    a = 3;
    b = 35;
    result = ExtendedEuclideanAlgorithmGcd(a, b);
    logger.LogInformation($"When a is {a} and b is {b} BezoutCoefficientS: {result.BezoutCoefficientS}, BezoutCoefficientT: {result.BezoutCoefficientT}, GreatestCommonDivisor: {result.GreatestCommonDivisor}, GcdQuotientS: {result.GcdQuotientS}, GcdQuotientT: {result.GcdQuotientT}");
    logger.LogInformation($"The GCD {result.GreatestCommonDivisor} should be equal to a * s + b * t, so {a} * {result.BezoutCoefficientS} + {b} * {result.BezoutCoefficientT} = {result.GreatestCommonDivisor}, which for our result is {a * result.BezoutCoefficientS + b * result.BezoutCoefficientT == result.GreatestCommonDivisor} ");

    a = 21;
    b = 5;
    result = ExtendedEuclideanAlgorithmGcd(a, b);
    logger.LogInformation($"When a is {a} and b is {b} BezoutCoefficientS: {result.BezoutCoefficientS}, BezoutCoefficientT: {result.BezoutCoefficientT}, GreatestCommonDivisor: {result.GreatestCommonDivisor}, GcdQuotientS: {result.GcdQuotientS}, GcdQuotientT: {result.GcdQuotientT}");
    logger.LogInformation($"The GCD {result.GreatestCommonDivisor} should be equal to a * s + b * t, so {a} * {result.BezoutCoefficientS} + {b} * {result.BezoutCoefficientT} = {result.GreatestCommonDivisor}, which for our result is {a * result.BezoutCoefficientS + b * result.BezoutCoefficientT == result.GreatestCommonDivisor} ");

    a = 1;
    b = 0;
    result = ExtendedEuclideanAlgorithmGcd(a, b);
    logger.LogInformation($"When a is {a} and b is {b} BezoutCoefficientS: {result.BezoutCoefficientS}, BezoutCoefficientT: {result.BezoutCoefficientT}, GreatestCommonDivisor: {result.GreatestCommonDivisor}, GcdQuotientS: {result.GcdQuotientS}, GcdQuotientT: {result.GcdQuotientT}");
    logger.LogInformation($"The GCD {result.GreatestCommonDivisor} should be equal to a * s + b * t, so {a} * {result.BezoutCoefficientS} + {b} * {result.BezoutCoefficientT} = {result.GreatestCommonDivisor}, which for our result is {a * result.BezoutCoefficientS + b * result.BezoutCoefficientT == result.GreatestCommonDivisor} ");

    a = 0;
    b = 1;
    result = ExtendedEuclideanAlgorithmGcd(a, b);
    logger.LogInformation($"When a is {a} and b is {b} BezoutCoefficientS: {result.BezoutCoefficientS}, BezoutCoefficientT: {result.BezoutCoefficientT}, GreatestCommonDivisor: {result.GreatestCommonDivisor}, GcdQuotientS: {result.GcdQuotientS}, GcdQuotientT: {result.GcdQuotientT}");
    logger.LogInformation($"The GCD {result.GreatestCommonDivisor} should be equal to a * s + b * t, so {a} * {result.BezoutCoefficientS} + {b} * {result.BezoutCoefficientT} = {result.GreatestCommonDivisor}, which for our result is {a * result.BezoutCoefficientS + b * result.BezoutCoefficientT == result.GreatestCommonDivisor} ");
}

Dictionary<string, List<uint>> FindCycles(string instructions, Dictionary<string, NodeConnections> network, string startingNode)
{
    var nodesOfInterest = network.Keys.Where(x => x.EndsWith('Z'));
    var visits = new Dictionary<string, List<uint>>();
    uint numberOfSteps = 0;
    uint numberOfInstructions = (uint)instructions.Length;
    int nextInstructionOffset;
    string currentNodeId = startingNode;
    while (true)
    {
        if (currentNodeId.EndsWith('Z'))
        {
            if (visits.ContainsKey(currentNodeId))
            {
                // if(visits[currentNodeId].Count() == 2)
                if (visits.Values.All(x => x.Count() == 2))
                {
                    break;
                }
                else
                {
                    visits[currentNodeId].Add(numberOfSteps);
                }
            }
            else
            {
                visits.Add(currentNodeId, new List<uint> { numberOfSteps });
            }
        }
        nextInstructionOffset = (int)(numberOfSteps % numberOfInstructions);
        if (instructions[nextInstructionOffset] == 'L')
        {
            currentNodeId = network[currentNodeId].Left;
        }
        else if (instructions[nextInstructionOffset] == 'R')
        {
            currentNodeId = network[currentNodeId].Right;
        }
        numberOfSteps++;
    }

    return visits;
    // var cycleLengthFromNodeOfInterest = new Dictionary<string, uint>();
    // foreach(var kvPair in visits)
    // {
    //     cycleLengthFromNodeOfInterest.Add(kvPair.Key, kvPair.Value[1]);
    // }

    // return (NumberOfStepsToFirstNodeOfInterest: visits.Min(x => x.Value.First()), CycleLengthFromNodeOfInterest: cycleLengthFromNodeOfInterest);
}

// uint NumberOfStepsToNodeEndingInZ(string instructions, Dictionary<string, NodeConnections> network)
// {

// }
// Distance to cycle start
// Cycle start point
// Points of interest in cycle
// Offsets of points of interest in cycle
//

uint NumberOfStepsFromAAAToZZZ(string instructions, Dictionary<string, NodeConnections> network)
{
    uint result = 0;
    string currentNodeId = "AAA";
    while (true)
    {
        if (currentNodeId == "ZZZ")
        {
            break;
        }
        if (instructions[(int)result % instructions.Length] == 'L')
        {
            currentNodeId = network[currentNodeId].Left;
        }
        else if (instructions[(int)result % instructions.Length] == 'R')
        {
            currentNodeId = network[currentNodeId].Right;
        }
        result++;
    }
    return result;
}

(string Instructions, Dictionary<string, NodeConnections> Network) ParseHauntedWasteland(string filePath)
{

    string instructions;
    var network = new Dictionary<string, NodeConnections>();
    using (StreamReader sr = new StreamReader(filePath))
    {
        instructions = sr.ReadLine();
        sr.ReadLine(); // Skip empty separator line

        string line;
        while ((line = sr.ReadLine()) != null)
        {
            var idAndConnections = line.Split(" = ");
            var id = idAndConnections[0];
            var connections = idAndConnections[1].Replace("(", string.Empty).Replace(" ", string.Empty).Replace(")", string.Empty).Split(',');
            var left = connections[0];
            var right = connections[1];
            network.Add(id, new NodeConnections { Left = left, Right = right });
        }
    }
    return (Instructions: instructions, Network: network);
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
}

public record NodeConnections
{
    public required string Left { get; init; }
    public required string Right { get; init; }
}
