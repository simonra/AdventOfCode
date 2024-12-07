open System
// For more information see https://aka.ms/fsharp-console-apps
printfn "Hello from F#"

let mutable inputFileName = "Input/Example.txt"
inputFileName <- "Input/Input.txt"

let lines = seq { yield! System.IO.File.ReadLines inputFileName }

let printMap (input: 'a array array) =
    printfn $"%A{DateTime.Now} Map state to print"
    input |> Seq.iter (fun row ->
        row |> Seq.iter (fun x -> printf $"{x}")
        printfn ""
        )
    printfn $"%A{DateTime.Now} Done printing map state"

let toArrayOfArraysOfChars input =
    input
    |> Seq.map Seq.toList
    |> Seq.map (Seq.map id )
    |> Seq.map Seq.toArray
    |> Seq.toArray

let rotate90DegreesClockwise
    (input: 'a array array)
    : 'a array array =
        let rowsIn = input |> Seq.length
        let colsIn = input |> Seq.head |> Seq.length
        let firstItem: 'a = input |> Seq.head |> Seq.head
        let mutable output: 'a array array = Array.init colsIn (fun _ -> Array.init rowsIn (fun _ -> firstItem))
        for rowCounter = 0 to rowsIn - 1 do
            for colCounter = 0 to colsIn - 1 do
                let nextValue = input[rowCounter][colCounter]
                output[colCounter][rowsIn - rowCounter - 1] <- nextValue
        output

let rotate90DegreesCounterClockwise
    (input: 'a array array)
    : 'a array array =
        let rowsIn = input |> Seq.length
        let colsIn = input |> Seq.head |> Seq.length
        let firstItem: 'a = input |> Seq.head |> Seq.head
        let mutable output: 'a array array = Array.init colsIn (fun _ -> Array.init rowsIn (fun _ -> firstItem))
        for rowCounter = 0 to rowsIn - 1 do
            for colCounter = 0 to colsIn - 1 do
                let nextValue = input[rowCounter][colCounter]
                output[colsIn - colCounter - 1][rowCounter] <- nextValue
        output

let coverLineWithXes (input:'a array) : 'a array =
    let size = input |> Seq.length
    // let mutable output: 'a array = Array.init size (fun _ -> input |> Seq.head)
    let mutable output: 'a array = Array.copy input
    let startingPos = input |> Seq.tryFindIndex (fun x -> x = '^')
    if startingPos = None then
        input
    else
        let firstHashtagAfter = input |> Seq.skip startingPos.Value |> Seq.tryFindIndex (fun x -> x = '#')
        if firstHashtagAfter = None then
            //We are at the end, the '^ leaves the map'
            for i = startingPos.Value to size - 1 do
                output[i] <- 'X'
            output
        else
            for i = startingPos.Value to startingPos.Value + firstHashtagAfter.Value - 2 do
                output[i] <- 'X'
            output[startingPos.Value + firstHashtagAfter.Value - 1] <- '^'
            output
    // let mutable startingPointCrossed = false
    // for i = 0 to size - 1 do
    //     startingPointCrossed <- input[i] = '^'
    //     output [i]
    // output
let coverMapWithXes (input:'a array) : 'a array =
    let mutable guardStillOnMap = true
    let mutable output = Array.copy input
    while guardStillOnMap do
        output <- output
            |> Array.map coverLineWithXes
        guardStillOnMap <- output |> Array.map (fun row -> row |> Array.contains '^') |> Array.fold (fun found isInRow -> isInRow || found ) false
        if guardStillOnMap then
            output <- rotate90DegreesCounterClockwise output
    output

let parsedInput = lines |> toArrayOfArraysOfChars
// printMap parsedInput
let rotatedClockwise = parsedInput |> rotate90DegreesClockwise
// printMap rotatedClockwise
// let rotatedBack = rotatedClockwise |> rotate90DegreesCounterClockwise
// printMap rotatedBack
// let forwardsAndBackAgainAreEqual = parsedInput = rotatedBack
// printfn $"%A{DateTime.Now} Rotating forwards and then back again results in same state: {forwardsAndBackAgainAreEqual}"
// let firstIteration =
//     rotatedClockwise
//     |> Array.map coverLineWithXes
// printMap (firstIteration |> rotate90DegreesCounterClockwise)

let finalMap = rotatedClockwise |> coverMapWithXes
// printMap (finalMap |> rotate90DegreesClockwise)
let countXes =
    finalMap
    |> Array.map (fun nextRow -> nextRow |> Array.fold (fun rowSum next -> rowSum + if next = 'X' then 1 else 0) 0)
    |> Array.sum
printfn $"%A{DateTime.Now} Final sum of Xes is '%A{countXes}'"
// printMap finalMap

// Part 2
// Candidate blockage places: Previous X-es
