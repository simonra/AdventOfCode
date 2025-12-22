type batteryBank = {
    numberOfBatteries: int
    positions9s: int seq
    positions8s: int seq
    positions7s: int seq
    positions6s: int seq
    positions5s: int seq
    positions4s: int seq
    positions3s: int seq
    positions2s: int seq
    positions1s: int seq
    positions0s: int seq
}

/// <summary>
/// Function to find indexes of all elements in a sequence mathcing the criteria specified by the supplied function.
/// Example usage:
/// <code>
///let inputs = [ 1; 2; 3; 4; 5 ]
///inputs |> indexesMatching (fun x -> x = 3)</code>
/// </summary>
/// Evaluates to <c>2</c>.
/// <param name="filter">Filtering function. Example: <c>fun x -> 0 &lt; x &amp;&amp; x &lt; 2</c></param>
/// <param name="xs">Sequence to filter. For instance: <c>[ 1; 2; 3; 4; 5 ]</c> </param>
let indexesMatching (filter: 'a -> bool) (xs: 'a seq) : int seq =
    xs
    |> Seq.indexed
    |> Seq.filter (snd >> filter)
    |> Seq.map fst

let parseBatteryBank (input: string) : batteryBank =
    let nines = input |> indexesMatching (fun x -> x = '9')
    raise (System.NotImplementedException())

// let largestNumberAndIndex (input: string) : (string * int) =
//     let mutable largest = -1
//     let mutable indexOfLargest = -1
//     for i in [0 .. input.Length - 1] do
//         let nextAsNumber = input[i] |> int
//         if nextAsNumber > largest then
//             indexOfLargest <- i
//             largest <- nextAsNumber
//         else
//             ()
//     ($"{largest}", indexOfLargest)
    // raise (System.NotImplementedException())
// let largestNumberAndIndex (startingPosition: int) (input : string) : (string * int) =
//     if startingPosition >= input.Length then raise(System.ArgumentException("Stating position cannot be greater than input length"))
//     let mutable largest = -1
//     let mutable indexOfLargest = -1
//     for i in [startingPosition .. input.Length - 1] do
//         let nextAsNumber = input[i] |> int
//         if nextAsNumber > largest then
//             indexOfLargest <- i
//             largest <- nextAsNumber
//         else
//             ()
//     ($"{largest}", indexOfLargest)

let batteryBankMaxCapacity (input: string) : int =
    let mutable largest = -1
    let mutable indexOfLargest = -1
    for i in [0 .. input.Length - 2] do
        let nextAsNumber = $"{input[i]}" |> int
        if nextAsNumber > largest then
            indexOfLargest <- i
            largest <- nextAsNumber
        else
            ()
    let mutable secondLargest = -1
    for i in [(indexOfLargest + 1) .. input.Length - 1] do
        let nextAsNumber = $"{input[i]}" |> int
        if nextAsNumber > secondLargest then
            secondLargest <- nextAsNumber
        else
            ()
    $"{largest}{secondLargest}" |> int
    // raise (System.NotImplementedException())

let testBatteryBankMaxCapacity =
    printfn $"Testing {nameof(batteryBankMaxCapacity)}"
    let batteryBankMaxCapacityTest0Input = "987654321111111"
    let batteryBankMaxCapacityTest0Expected = 98
    let batteryBankMaxCapacityTest0Actual = batteryBankMaxCapacity batteryBankMaxCapacityTest0Input
    if batteryBankMaxCapacityTest0Actual <> batteryBankMaxCapacityTest0Expected then printfn $"Testing {nameof(batteryBankMaxCapacity)} failed. When supplied input {batteryBankMaxCapacityTest0Input} expected result {batteryBankMaxCapacityTest0Expected} but was {batteryBankMaxCapacityTest0Actual}" else ()

    let batteryBankMaxCapacityTest1Input = "811111111111119"
    let batteryBankMaxCapacityTest1Expected = 89
    let batteryBankMaxCapacityTest1Actual = batteryBankMaxCapacity batteryBankMaxCapacityTest1Input
    if batteryBankMaxCapacityTest1Actual <> batteryBankMaxCapacityTest1Expected then printfn $"Testing {nameof(batteryBankMaxCapacity)} failed. When supplied input {batteryBankMaxCapacityTest1Input} expected result {batteryBankMaxCapacityTest1Expected} but was {batteryBankMaxCapacityTest1Actual}" else ()

    let batteryBankMaxCapacityTest2Input = "234234234234278"
    let batteryBankMaxCapacityTest2Expected = 78
    let batteryBankMaxCapacityTest2Actual = batteryBankMaxCapacity batteryBankMaxCapacityTest2Input
    if batteryBankMaxCapacityTest2Actual <> batteryBankMaxCapacityTest2Expected then printfn $"Testing {nameof(batteryBankMaxCapacity)} failed. When supplied input {batteryBankMaxCapacityTest2Input} expected result {batteryBankMaxCapacityTest2Expected} but was {batteryBankMaxCapacityTest2Actual}" else ()

    let batteryBankMaxCapacityTest3Input = "818181911112111"
    let batteryBankMaxCapacityTest3Expected = 92
    let batteryBankMaxCapacityTest3Actual = batteryBankMaxCapacity batteryBankMaxCapacityTest3Input
    if batteryBankMaxCapacityTest3Actual <> batteryBankMaxCapacityTest3Expected then printfn $"Testing {nameof(batteryBankMaxCapacity)} failed. When supplied input {batteryBankMaxCapacityTest3Input} expected result {batteryBankMaxCapacityTest3Expected} but was {batteryBankMaxCapacityTest3Actual}" else ()
testBatteryBankMaxCapacity

let totalJoltage (inputPath: string) : int =
    let lines = seq { yield! System.IO.File.ReadLines inputPath }
    lines
    |> Seq.map batteryBankMaxCapacity
    |> Seq.sum
    // raise (System.NotImplementedException())

let testPart1 =
    let sampleInputFilePath = "Input/Example.txt"
    let expectedExampleValue = 357
    let exampleInputJoltage = totalJoltage sampleInputFilePath
    if exampleInputJoltage = expectedExampleValue then
        printfn $"Successfully processed example input for part 1!"
    else
        printfn $"Failed to process example input for part 1. Expected {expectedExampleValue}, but got {exampleInputJoltage}"
testPart1

let inputFilePath = "Input/Input.txt"
let part1Joltage = totalJoltage inputFilePath
printfn $"Part 1 total joltage is: '{part1Joltage}'"
