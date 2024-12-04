// For more information see https://aka.ms/fsharp-console-apps
printfn "Hello from F#"

// 2534 too low

let mutable inputFileName = "Input/Example.txt"
// inputFileName <- "Input/3x3.txt"
// inputFileName <- "Input/4x3.txt"
// inputFileName <- "Input/3x4.txt"
inputFileName <- "Input/Input.txt"

let lines = seq { yield! System.IO.File.ReadLines inputFileName }

let toArray (input: 'a seq seq) : 'a array array =
    input |> Seq.map Seq.toArray |> Seq.toArray

let toSequence (input: 'a array array) : 'a seq seq =
    input |> Array.map Array.toSeq |> Array.toSeq

let rotate45degreesClockwise (input: 'a option array array) : 'a option array array =
    // https://math.stackexchange.com/questions/732679/how-to-rotate-a-matrix-by-45-degrees
    // if input |> Seq.length
    // let mutable output = seq { seq { Some(0) } }
    // let mutable output : 'a option seq seq = seq { seq { } }
    let rowsIn = input |> Seq.length
    let colsIn = input |> Seq.head |> Seq.length
    // let asymmetryFactor = System.Math.Abs(rowsIn - colsIn)
    let asymmetryFactorRows = if rowsIn > colsIn then rowsIn - colsIn else 0
    let asymmetryFactorColumns = if colsIn > rowsIn then colsIn - rowsIn else 0
    let rowsOut = rowsIn + colsIn - 1
    let colsOut = rowsIn + colsIn - 1

    let mutable output: 'a option array array =
        Array.init rowsOut (fun r -> Array.init colsOut (fun _ -> None))

    // printfn $"Dimensions:"
    // printfn $"Input rows: %A{rowsIn} columns: %A{colsIn}"
    // printfn $"Output rows: %A{rowsOut} columns: %A{colsOut}"

    for rowCounter = 0 to rowsIn - 1 do
        for colCounter = 0 to colsIn - 1 do
            // M[x][y] to cell RM[x+y+1][−x+y+n]
            let outputRow = rowCounter + colCounter + 1 - 1

            let outputColumn =
                0 - rowCounter + colCounter + colsIn - 1 - asymmetryFactorColumns
                + asymmetryFactorRows

            // printfn $"rowCounter: %A{rowCounter} colCounter: %A{colCounter} outputRow: %A{outputRow} outputColumn: %A{outputColumn}"

            let nextValue = input[rowCounter][colCounter]
            output[outputRow][outputColumn] <- nextValue

    output

let rotate90DegreesClockwise (input: 'a option array array) : 'a option array array =
    let rowsIn = input |> Seq.length
    let colsIn = input |> Seq.head |> Seq.length
    let mutable output: 'a option array array = Array.init colsIn (fun r -> Array.init rowsIn (fun _ -> None))
    for rowCounter = 0 to rowsIn - 1 do
        for colCounter = 0 to colsIn - 1 do
            let nextValue = input[rowCounter][colCounter]
            output[colCounter][rowsIn - rowCounter - 1] <- nextValue

    output

// let linesAsCharArrays = lines |> Seq.map Seq.toArray

let (|Default|) onNone value =
    match value with
    | None -> onNone
    | Some e -> e

let numberOfOccurrences (Default "XMAS" toFind) (toSearch: string) : int =
    (toSearch.Split(toFind) |> Seq.length) - 1

let toCollectionOfStrings (input: char option array array) : string seq =
    let result =
        input
        |> Seq.map (fun charSequence ->
            charSequence
            |> Seq.where (fun x -> x.IsSome)
            |> Seq.map (fun x -> string x.Value)
            |> Seq.reduce (+))
    // printfn $"Strings collected are %A{result}"
    result

let toArrayOfArraysOfChars input =
    input |> Seq.map Seq.toList |> Seq.map (Seq.map (fun x -> Some(x))) |> toArray

let foundXmasesInAllDirections (input: char option array array) : int =
    let mutable total = 0

    // Don't repeatedly apply rotate45, because the matrix grows a bit in size each time it's done.

    let initialRotationCount: int =
        input |> toCollectionOfStrings |> Seq.map (numberOfOccurrences None) |> Seq.sum

    let initialRotatedOnceCount: int =
        input
        |> rotate45degreesClockwise
        |> toCollectionOfStrings
        |> Seq.map (numberOfOccurrences None)
        |> Seq.sum

    let turned90Degrees = input |> rotate90DegreesClockwise
    let turnedOnceCount: int =
        turned90Degrees
        |> toCollectionOfStrings
        |> Seq.map (numberOfOccurrences None)
        |> Seq.sum

    let turned135Degrees = turned90Degrees |> rotate45degreesClockwise
    let turnedOnceRotatedCount: int =
        turned135Degrees
        |> toCollectionOfStrings
        |> Seq.map (numberOfOccurrences None)
        |> Seq.sum

    let turned180Degrees = input |> rotate90DegreesClockwise |> rotate90DegreesClockwise
    let turnedTwiceCount: int =
        input
        |> rotate90DegreesClockwise
        |> rotate90DegreesClockwise
        |> toCollectionOfStrings
        |> Seq.map (numberOfOccurrences None)
        |> Seq.sum

    let turned225Degrees = turned180Degrees |> rotate45degreesClockwise
    let turnedTwiceRotatedCount: int =
        turned225Degrees
        |> toCollectionOfStrings
        |> Seq.map (numberOfOccurrences None)
        |> Seq.sum

    let turned270Degrees = input |> rotate90DegreesClockwise |> rotate90DegreesClockwise |> rotate90DegreesClockwise
    let turnedThriceCount: int =
        turned270Degrees
        |> toCollectionOfStrings
        |> Seq.map (numberOfOccurrences None)
        |> Seq.sum

    let turned315Degrees = turned270Degrees |> rotate45degreesClockwise
    let turnedThriceRotatedCount: int =
        turned315Degrees
        |> toCollectionOfStrings
        |> Seq.map (numberOfOccurrences None)
        |> Seq.sum

    total <-
        total
        + initialRotationCount
        + initialRotatedOnceCount
        + turnedOnceCount
        + turnedOnceRotatedCount
        + turnedTwiceCount
        + turnedTwiceRotatedCount
        + turnedThriceCount
        + turnedThriceRotatedCount

    total

// printfn $"Original"
// printfn $"%A{lines |> Seq.toList}"
//
// let rotated =
//     lines
//     |> Seq.map Seq.toList
//     |> Seq.map (Seq.map (fun x -> Some(x)))
//     |> toArray
//     |> rotate45degreesClockwise
//
// printfn $"Rotated"
// printfn $"%A{rotated}"

// let rotated90 =
//     lines
//     |> Seq.map Seq.toList
//     |> Seq.map (Seq.map (fun x -> Some(x)))
//     |> toArray
//     |> rotate90DegreesClockwise
//
// printfn $"Rotated"
// printfn $"%A{rotated90}"


// let testText = "uneksmased string with XMAS"
// let occurrencesFound = testText |> numberOfOccurrences None
// printfn $"Number of occurrences of xmas is %A{occurrencesFound}"

printfn $"Total number of xmases found %A{lines |> toArrayOfArraysOfChars |> foundXmasesInAllDirections}"

let windowed2d (windowSizeRows: int) (windowSizeColumns: int) (input: 'a array array) : 'a array array seq =
    let rowsIn = (input |> Seq.length) - 1
    let colsIn = (input |> Seq.head |> Seq.length) - 1
    let wr = windowSizeRows - 1
    let wc = windowSizeColumns - 1
    let defaultInitValue : 'a = Seq.head (Seq.head input)
    seq {
        for rowCounter = 0 to rowsIn - wr do
            for colCounter = 0 to colsIn - wc do
                let mutable output : 'a array array =
                    // Array.create windowSizeRows (Array.create windowSizeColumns defaultInitValue)
                    Array.init windowSizeRows (fun _ -> Array.init windowSizeColumns (fun _ -> defaultInitValue))
                for windowRowCounter = 0 to wr do
                    for windowColumnCounter = 0 to wc do
                        let nextValue = input[rowCounter+windowRowCounter][colCounter+windowColumnCounter]
                        output[windowRowCounter][windowColumnCounter] <- nextValue
                yield output
    }
// Array.windowed
// let charArrayInput = lines |> toArrayOfArraysOfChars
// let myBestSlidingWindow = windowed2d 1 1 charArrayInput
// printfn $"Windowed 2d is %A{myBestSlidingWindow |> Seq.toList}"

let containsXmas (input: char option array array) : bool =
    if input[1][1] <> Some('A') then
        false
    else
        let lrTb = input[0][0] = Some('M') && input[2][2] = Some('S')
        let lrBt = input[0][0] = Some('S') && input[2][2] = Some('M')
        let lr = lrTb || lrBt
        let rlTb = input[0][2] = Some('M') && input[2][0] = Some('S')
        let rlBt = input[0][2] = Some('S') && input[2][0] = Some('M')
        let rl = rlTb || rlBt
        lr && rl
        // ((input[0][0] = Some('M') && input[2][2] = Some('S')) || (input[0][0] = Some('S') && input[2][2] = Some('M'))) && (input[0][2] = Some('M') && input[2][0] = Some('S')) || (input[0][2] = Some('S') && input[2][0] = Some('M'))

let charArrayInput = lines |> toArrayOfArraysOfChars
let windowsOf3x3 = charArrayInput |> windowed2d 3 3
let windowsThatAreXmas = windowsOf3x3 |> Seq.where containsXmas
let sumOfXmases = windowsThatAreXmas |> Seq.length

printfn $"The proper sum of x-MAS'es is %A{sumOfXmases}"
