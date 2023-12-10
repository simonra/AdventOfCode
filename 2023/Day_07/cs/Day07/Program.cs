using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

using ILoggerFactory factory = LoggerFactory.Create(
    builder =>
    {
        builder.SetMinimumLevel(LogLevel.Information);
        // builder.SetMinimumLevel(LogLevel.Debug);
        builder.AddConsole();
    });
ILogger logger = factory.CreateLogger("Day07");

TestPart1OnSampleInput();
RunPart1();
TestPart2OnSampleInput();
RunPart2();
TestPart2OnMoreAdvancedInput();


void RunPart1()
{
    logger.LogInformation("Starting RunPart1");
    var inputFile = "input.txt";
    var parsedHands = ParseHands(inputFile);
    var winnings = Winnings(parsedHands);
    logger.LogInformation($"Part 1 result: {winnings}");
}

void TestPart1OnSampleInput()
{
    logger.LogInformation("Starting TestPart1OnSampleInput");
    var inputFile = "sample_input-part_1.txt";
    var parsedHands = ParseHands(inputFile);
    logger.LogTrace($"Parsed hands are {parsedHands.ToJson()}");
    var winnings = Winnings(parsedHands);
    var expectedResult = 6440;
    if (winnings != expectedResult)
    {
        logger.LogError($"Testing part 1 on sample data failed. Got {winnings} as sum of winnings, expected {expectedResult}");
    }
    else
    {
        logger.LogInformation("Testing Part 1: Processing sample data yielded expected result!");
    }
}

void RunPart2()
{
    logger.LogInformation("Starting RunPart2");
    var inputFile = "input.txt";
    var parsedHands = ParseHandsPart2(inputFile);
    var winnings = Winnings(parsedHands);
    logger.LogInformation($"Part 2 result: {winnings}");
}

void TestPart2OnSampleInput()
{
    logger.LogInformation("Starting TestPart2OnSampleInput");
    var inputFile = "sample_input-part_1.txt";
    var parsedHands = ParseHandsPart2(inputFile);
    logger.LogTrace($"Parsed hands are {parsedHands.ToJson()}");
    var winnings = Winnings(parsedHands);
    var expectedResult = 5905;
    if (winnings != expectedResult)
    {
        logger.LogError($"Testing part 2 on sample data failed. Got {winnings} as sum of winnings, expected {expectedResult}");
    }
    else
    {
        logger.LogInformation("Testing Part 2: Processing sample data yielded expected result!");
    }
}

void TestPart2OnMoreAdvancedInput()
{
    logger.LogInformation("Starting TestPart2OnMoreAdvancedInput");
    var inputFile = "test-input-advanced.txt";
    var parsedHands = ParseHandsPart2(inputFile);
    logger.LogTrace($"Parsed hands are {parsedHands.ToJson()}");
    var winnings = Winnings(parsedHands);

    parsedHands.Sort();
    logger.LogInformation($"Bids of sorted hands: {parsedHands.Select(x => x.Bid).ToJson()}");
    foreach(var hand in parsedHands)
    {
        foreach(var card in hand.Cards)
        {
            if(card.Label == 'J' && card.Power != 1)
            {
                logger.LogInformation($"Power of 'J' is {card.Power}");
            }
        }
    }
}

uint Winnings(List<Hand> hands)
{
    uint winnings = 0;
    hands.Sort();
    logger.LogDebug($"Bids of sorted hands: {hands.Select(x => x.Bid).ToJson()}");
    for (int i = 0; i < hands.Count(); i++)
    {
        winnings += hands[i].Bid * ((uint)i + 1);
    }
    return winnings;
}

List<Hand> ParseHands(string filePath)
{
    List<Hand> hands = new List<Hand>();
    var lines = File.ReadLines(filePath);
    foreach (var line in lines)
    {
        hands.Add(line.ParseHand());
    }
    return hands;
}

List<Hand> ParseHandsPart2(string filePath)
{
    List<Hand> hands = new List<Hand>();
    var lines = File.ReadLines(filePath);
    foreach (var line in lines)
    {
        hands.Add(line.ParseHandPart2());
    }
    return hands;
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

    public static uint ToUint(this string s)
    {
        return UInt16.Parse(s);
    }

    public static Card ParseCard(this char c)
    {
        uint power = 0;
        if (c == 'A') power = 14;
        else if (c == 'K') power = 13;
        else if (c == 'Q') power = 12;
        else if (c == 'J') power = 11;
        else if (c == 'T') power = 10;
        else if (c == '9') power = 9;
        else if (c == '8') power = 8;
        else if (c == '7') power = 7;
        else if (c == '6') power = 6;
        else if (c == '5') power = 5;
        else if (c == '4') power = 4;
        else if (c == '3') power = 3;
        else if (c == '2') power = 2;
        else throw new ArgumentOutOfRangeException(nameof(c), $"The input {c} is not valid for a card");
        return new Card
        {
            Label = c,
            Power = power
        };
    }

    public static Hand ParseHand(this string input)
    {
        List<Card> cards = new List<Card>();
        var cardsAndBid = input.Split(' ');
        foreach (char card in cardsAndBid[0])
        {
            cards.Add(card.ParseCard());
        }
        var bid = cardsAndBid[1].ToUint();
        var handType = cards.HandType();

        return new Hand
        {
            Cards = cards.ToArray(),
            Bid = bid,
            Type = handType
        };
    }

    static uint HandType(this IEnumerable<Card> cards)
    {
        var cardGroups = cards.GroupBy(c => c.Power);
        if (cardGroups.Count() == 1)
        {
            // Five of a kind
            return 7;
        }
        if (cardGroups.Count() == 2)
        {
            if (cardGroups.Any(g => g.Count() == 4))
            {
                // Four of a kind
                return 6;
            }
            else
            {
                // Full house, only other alternative if there are 2 groups of 5 cards
                return 5;
            }
        }
        if (cardGroups.Count() == 3)
        {
            if (cardGroups.Any(g => g.Count() == 3))
            {
                // Three of a kind
                return 4;
            }
            else
            {
                // Two pair
                return 3;
            }
        }
        if (cardGroups.Count() == 4)
        {
            // One pair
            return 2;
        }
        if (cardGroups.Count() == 5)
        {
            // High card
            return 1;
        }

        throw new NotImplementedException($"You've found a case I haven't considered! It happens when the cards are {cards.ToJson()}. Good luck");
    }

    public static Card ParseCardPart2(this char c)
    {
        uint power = 0;
        if (c == 'A') power = 14;
        else if (c == 'K') power = 13;
        else if (c == 'Q') power = 12;
        else if (c == 'J') power = 1;
        else if (c == 'T') power = 10;
        else if (c == '9') power = 9;
        else if (c == '8') power = 8;
        else if (c == '7') power = 7;
        else if (c == '6') power = 6;
        else if (c == '5') power = 5;
        else if (c == '4') power = 4;
        else if (c == '3') power = 3;
        else if (c == '2') power = 2;
        else throw new ArgumentOutOfRangeException(nameof(c), $"The input {c} is not valid for a card");
        return new Card
        {
            Label = c,
            Power = power
        };
    }

    public static Hand ParseHandPart2(this string input)
    {
        List<Card> cards = new List<Card>();
        var cardsAndBid = input.Split(' ');
        foreach (char card in cardsAndBid[0])
        {
            cards.Add(card.ParseCardPart2());
        }
        var bid = cardsAndBid[1].ToUint();
        var handType = cards.HandTypePart2();

        return new Hand
        {
            Cards = cards.ToArray(),
            Bid = bid,
            Type = handType
        };
    }

    static uint HandTypePart2(this IEnumerable<Card> cards)
    {
        var numberOfJokers = cards.Count(c => c.Label == 'J');
        if (numberOfJokers == 0)
        {
            return cards.HandType();
        }
        var cardGroups = cards.GroupBy(c => c.Power);
        if (cardGroups.Count() == 1)
        {
            // Five of a kind
            return 7;
        }
        if (cardGroups.Count() == 2)
        {
            // Five of a kind
            return 7;
        }
        if (cardGroups.Count() == 3)
        {
            if (numberOfJokers == 3)
            {
                // Four of a kind
                return 6;
            }
            if (numberOfJokers == 2)
            {
                // One of the remaining groups has to have at least 2 cards, making this the most powerful combo
                // Four of a kind
                return 6;
            }
            else
            {
                // Because we only consider hands having jokers at this point, then there now has to be 1 joker
                if (cardGroups.Any(g => g.Count() == 3))
                {
                    // Four of a kind
                    return 6;
                }
                else
                {
                    // Full house
                    return 5;
                }
                // if (cardGroups.Count(g => g.Count() == 2) == 2)
                // {
                //     // Full house
                //     return 5;
                // }
            }
        }
        if (cardGroups.Count() == 4)
        {
            // Three of a kind
            return 4;
        }
        if (cardGroups.Count() == 5)
        {
            // One pair
            return 2;
        }

        throw new NotImplementedException($"You've found a case I haven't considered! It happens when the cards are {cards.ToJson()}. Good luck");
    }
}

public record Hand : IComparable<Hand>
{
    public required Card[] Cards { get; init; }
    public required uint Bid { get; init; }
    public required uint Type { get; init; }

    public int CompareTo(Hand other)
    {
        if (this.Type > other.Type)
        {
            return 1;
        }
        if (this.Type < other.Type)
        {
            return -1;
        }
        if (this.Cards.Count() != other.Cards.Count())
        {
            throw new ArgumentException($"Cannot compare hands with different amounts of cards. Failed when comparing this {this.ToJson()} with {other.ToJson()}");
        }
        for (int i = 0; i < this.Cards.Count(); i++)
        {
            if (this.Cards[i] > other.Cards[i])
            {
                return 1;
            }
            if (this.Cards[i] < other.Cards[i])
            {
                return -1;
            }
        }
        return 0;
    }

    public static bool operator >(Hand operand1, Hand operand2)
    {
        return operand1.CompareTo(operand2) > 0;
    }

    // Define the is less than operator.
    public static bool operator <(Hand operand1, Hand operand2)
    {
        return operand1.CompareTo(operand2) < 0;
    }

    // Define the is greater than or equal to operator.
    public static bool operator >=(Hand operand1, Hand operand2)
    {
        return operand1.CompareTo(operand2) >= 0;
    }

    // Define the is less than or equal to operator.
    public static bool operator <=(Hand operand1, Hand operand2)
    {
        return operand1.CompareTo(operand2) <= 0;
    }
}

public record Card : IComparable<Card>
{
    public required char Label { get; init; }
    public required uint Power { get; init; }

    public int CompareTo(Card other)
    {
        return this.Power.CompareTo(other.Power);
    }

    public static bool operator >(Card operand1, Card operand2)
    {
        return operand1.CompareTo(operand2) > 0;
    }

    // Define the is less than operator.
    public static bool operator <(Card operand1, Card operand2)
    {
        return operand1.CompareTo(operand2) < 0;
    }

    // Define the is greater than or equal to operator.
    public static bool operator >=(Card operand1, Card operand2)
    {
        return operand1.CompareTo(operand2) >= 0;
    }

    // Define the is less than or equal to operator.
    public static bool operator <=(Card operand1, Card operand2)
    {
        return operand1.CompareTo(operand2) <= 0;
    }
}
