open System
// For more information see https://aka.ms/fsharp-console-apps
printfn $"{DateTime.Now} Hello from F#"

let mutable inputFileName = "Input/Example.txt"
inputFileName <- "Input/Input.txt"

let lines = seq { yield! System.IO.File.ReadLines inputFileName }

let toArrayOfArraysOfInts (input: string seq) : int array array =
    input
    |> Seq.map Seq.toList
    |> Seq.map (Seq.map (fun x -> int (Char.ToString x)))
    |> Seq.map Seq.toArray
    |> Seq.toArray

let upperBound (input: int array array) =
    let flattened =
        input
        |> Seq.collect id
    let zeroes =
        flattened
        |> Seq.where (fun x -> x = 0)
        |> Seq.length
    let nines =
        flattened
        |> Seq.where (fun x -> x = 9)
        |> Seq.length
    zeroes * nines

let coordinateIsWithinBounds (input: int array array) (coordinate: int * int) =
    let row = fst coordinate
    let col = snd coordinate
    if row < 0 then false
    elif col < 0 then false
    elif row >= (input |> Seq.length) then false
    elif col >= (input |> Seq.head |> Seq.length) then false
    else true

let validRoutesFromZero (inputMap: int array array) (zeroCoordinate: int * int) =
    let coordinateIsWithinMap = coordinateIsWithinBounds inputMap
    let rec nextValidPositions (coordinate: int * int) (remainingDistance: int): (int * int) seq =
        seq {
            if remainingDistance = 0 then
                yield coordinate
            // elif remainingDistance < 0 then
            //     yield! Seq.empty
            else
            let newRemainingDistance = remainingDistance - 1
            let row = fst coordinate
            let col = snd coordinate
            let valueAtCurrent = inputMap[row][col]
            let expandFromCurrent = expandBranch newRemainingDistance valueAtCurrent
            let up = (row - 1, col)
            yield! expandFromCurrent up
            let down = (row + 1, col)
            yield! expandFromCurrent down
            let left = (row, col - 1)
            yield! expandFromCurrent left
            let right = (row, col + 1)
            yield! expandFromCurrent right
        }
    and expandBranch (remainingDistance: int) (valueAtCurrent: int) (coordinate: int * int): (int * int) seq =
        seq {
            if coordinateIsWithinMap coordinate then
                let valueAtNext = inputMap[fst coordinate][snd coordinate]
                if valueAtNext = valueAtCurrent + 1 then
                    // yield coordinate
                    yield! nextValidPositions coordinate remainingDistance
                else yield! Seq.empty
            else yield! Seq.empty
        }
    // for i = 1 to 9 do
    let validPaths = nextValidPositions zeroCoordinate 9
    validPaths
    // |> Seq.distinct
    |> Seq.length

let score (input: int array array) =
    let numberOfRows = input |> Seq.length
    let numberOfColumns = input |> Seq.head |> Seq.length
    let zeroPositions =
        seq {
            for i = 0 to numberOfRows - 1 do
                for j = 0 to numberOfColumns - 1 do
                    if input[i][j] = 0 then yield (i,j)
                    else ()
        }
    let evaluateValidForMap = validRoutesFromZero input
    let numberOfValidRoutesPerZero =
        zeroPositions
        |> Seq.map (fun x -> evaluateValidForMap x)
        |> Seq.sum
    // let validRoutesToNines =
    //     seq {
    //         for zeroPos in zeroPositions do
    //             for i = 1 to 9 do // Paths can max be 9 long
    //                 raise (NotImplementedException())
    //     }
    numberOfValidRoutesPerZero

let topoMap =
    lines
    |> toArrayOfArraysOfInts

topoMap
|> upperBound
|> (fun x -> printfn $"{DateTime.Now} Upper score bound for map of size {topoMap.Length} rows x {topoMap |> Seq.head |> Seq.length} columns is {x}")

topoMap
|> score
|> (fun x -> printfn $"{DateTime.Now} Part 1 score is {x}")
