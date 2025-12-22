// open System.Text.RegularExpressions
// open Microsoft.FSharp.Data.UnitSystems.SI.UnitNames

printfn $"""String split ranges works like this: "aabb" / 2 -> {"aabb"[..1]} and {"aabb"[1+1 ..]}"""

type idRange<'a> = {
    first: 'a
    second: 'a
}

let getNumber<'a> (input: string) : 'a =
    let inputDigits = System.Text.RegularExpressions.Regex.Replace(input, "[^0-9]", "")
    let typeConverter = System.ComponentModel.TypeDescriptor.GetConverter(typeof<'a>);
    typeConverter.ConvertFromInvariantString(inputDigits) :?> 'a

let parseIdRanges<'a>(fileName: string) : idRange<'a> seq =
    let stringContent = System.IO.File.ReadAllText fileName
    let stringRanges = stringContent.Split(',')
    stringRanges |> Seq.map (fun rangeString ->
        let candidatePair = rangeString.Split('-')
        if candidatePair.Length <> 2 then
            raise (System.ArgumentException($"Parsing failed, range split into more than 1 pair. Range was {rangeString}"))
        else ()
        {
            first = getNumber candidatePair[0]
            second = getNumber candidatePair[1]
        }
        )

let cutStringAtMiddle (input: string) : (string * string) =
    if input.Length % 2 <> 0 then
        raise (System.ArgumentException($"Cannot halve string of uneven length. Input string was {input}")) else ()
    // let halfPoint = fst (System.Math.DivRem( input.Length, 2))
    let halfPoint = input.Length / 2
    (input[..(halfPoint - 1)], input[halfPoint..])
    // raise (System.NotImplementedException())

let isInvalid<'a> (input: 'a) : bool =
    // printfn $"\tChecking {input}"
    let inputString = $"{input}"
    if inputString.Length % 2 <> 0
    then
        false
    else
        // printfn $"\t\tInput is of even lenght"
        let halves = cutStringAtMiddle inputString
        // printfn $"\t\tHalves are: %A{halves}"
        if fst halves = snd halves then
            // printfn $"\t\tFound invalid ID {inputString}"
            true
        else
            false
    // raise (System.NotImplementedException())

let inline invalidsInRange<'a
    when 'a: comparison
    and 'a: (static member One: 'a)
    and 'a: (static member (+) : 'a * 'a -> 'a)
>(input: idRange<'a>) : 'a seq =
    let smallest = min input.first input.second
    let largest = max input.second input.first
    seq {smallest .. largest } |> Seq.filter isInvalid
    // raise (System.NotImplementedException())

let mutable exampleInputFileName = "Input/Example.txt"
let exampleInputExpectedResult = "1227775554"
// let exampleInputFileContent = seq { yield! System.IO.File.ReadLines exampleInputFileName }
let exampleInputIdRanges = parseIdRanges<int> exampleInputFileName
let exampleInputInvalids = exampleInputIdRanges |> Seq.map invalidsInRange |> Seq.collect id
printf $"""Example input invalids: {exampleInputInvalids |> Seq.iter (printf "%d, ")}{printf "\n"}"""
printfn ""
let exampleInputInvalidsSum = exampleInputInvalids |> Seq.sum
if $"{exampleInputInvalidsSum}" = exampleInputExpectedResult then
    printfn $"Successfully processed example input for part 1!"
else
    printfn $"Failed to process example input for part 1. Expected {exampleInputExpectedResult}, but got {exampleInputInvalidsSum}"
printfn ""

let inputFileName = "Input/Input.txt"
let part1InputIdRanges = parseIdRanges<int64> inputFileName
let part1InputInvalidIds = part1InputIdRanges |> Seq.map invalidsInRange |> Seq.collect id
let part1InvalidIdSums = part1InputInvalidIds |> Seq.sum

printfn $"Part 1 result: {part1InvalidIdSums}"

printfn "\n"
printfn "Part 2:"

let chunkStringIntoPiecesBySize (inputString: string) (chunkSize: int) : string seq =
    if inputString.Length % chunkSize <> 0 then
        Seq.empty
    else
        // let result = inputString |> Seq.chunkBySize chunkSize |> Seq.map System.String
        // let concatenatedResult =  String.concat ", " result
        // printfn $" Concatenated: %A{concatenatedResult}"
        inputString |> Seq.chunkBySize chunkSize |> Seq.map System.String

let allElementsEqual(input: 'a seq): bool =
    let reIterableCollection = input |> Seq.toList
    if reIterableCollection.IsEmpty then
        false
    else
        reIterableCollection |> List.forall (fun elem -> elem = reIterableCollection[0])

let isInvalidPart2<'a> (input: 'a) : bool =
    // printfn $"\tChecking {input}"
    let inputString = $"{input}"
    let maxLengthToCheck = inputString.Length / 2
    seq { 1 .. maxLengthToCheck } |> Seq.exists (fun chunkSize ->
        let chunks = chunkStringIntoPiecesBySize inputString chunkSize |> Seq.toList
        // let concatenated = chunks |> String.concat(",")
        // printfn $"\t{inputString} chunksize {chunkSize} chunks: {new System.String(concatenated)}"
        chunks |> allElementsEqual
        )
    // raise (System.NotImplementedException())

let inline invalidsInRangePart2<'a
    when 'a: comparison
    and 'a: (static member One: 'a)
    and 'a: (static member (+) : 'a * 'a -> 'a)
>(input: idRange<'a>) : 'a seq =
    let smallest = min input.first input.second
    let largest = max input.second input.first
    seq {smallest .. largest } |> Seq.filter isInvalidPart2
    // raise (System.NotImplementedException())

// let mutable exampleInputFileName = "Input/Example.txt"
let exampleInputExpectedResultPart2 = "4174379265"
// let exampleInputFileContent = seq { yield! System.IO.File.ReadLines exampleInputFileName }
let exampleInputIdRangesPart2 = parseIdRanges<int64> exampleInputFileName
let exampleInputInvalidsPart2 = exampleInputIdRangesPart2 |> Seq.map invalidsInRangePart2 |> Seq.collect id
printf $"""Example input invalids part 2: {exampleInputInvalidsPart2 |> Seq.iter (printf "%d ")}{printf "\n"}"""
printfn ""
printfn $"Number of invalids part 2 example: %d{(exampleInputInvalidsPart2 |> Seq.toList).Length} (expected 13)"
let exampleInputInvalidsSumPart2 = exampleInputInvalidsPart2 |> Seq.sum
if $"{exampleInputInvalidsSumPart2}" = exampleInputExpectedResultPart2 then
    printfn $"Successfully processed example input for part 2!"
else
    printfn $"Failed to process example input for part 2. Expected {exampleInputExpectedResultPart2}, but got {exampleInputInvalidsSumPart2}"
printfn ""

// let inputFileName = "Input/Input.txt"
// let part1InputIdRanges = parseIdRanges<int64> inputFileName
//
let part2InputInvalidIds = part1InputIdRanges |> Seq.map invalidsInRangePart2 |> Seq.collect id
let part2InvalidIdSums = part2InputInvalidIds |> Seq.sum

printfn $"Part 2 result: {part2InvalidIdSums}"
