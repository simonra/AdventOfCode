// For more information see https://aka.ms/fsharp-console-apps
printfn "Hello from F#"

let mutable inputFileName = "Input/Example.txt"
inputFileName <- "Input/3x3.txt"
// inputFileName <- "Input/4x3.txt"
// inputFileName <- "Input/3x4.txt"
// inputFileName <- "Input/Input.txt"

// Need to handle forwards, backwards, diagonally up, diagonally down, both diagonals backwards
let lines = seq { yield! System.IO.File.ReadLines inputFileName }

let toArray (input : 'a seq seq) : 'a array array =
    input |> Seq.map Seq.toArray |> Seq.toArray
let toSequence (input: 'a array array) : 'a seq seq =
    input |> Array.map Array.toSeq |> Array.toSeq

let rotate45degreesClockwise (input : 'a option seq seq) : 'a option seq seq =
    // https://math.stackexchange.com/questions/732679/how-to-rotate-a-matrix-by-45-degrees
    // if input |> Seq.length
    // let mutable output = seq { seq { Some(0) } }
    // let mutable output : 'a option seq seq = seq { seq { } }
    let rowsIn = input |> Seq.head |> Seq.length
    let colsIn = input |> Seq.length
    let rowsOut = rowsIn + colsIn - 1
    let colsOut = rowsIn + colsIn - 1
    let inputAsArrays = input |> toArray
    let mutable output : 'a option array array = Array.init colsOut (fun r -> Array.init rowsOut (fun _ -> None))
    printfn $"Dimentions:"
    printfn $"Input rows: %A{rowsIn} columns: %A{colsIn}"
    printfn $"Output rows: %A{rowsOut} columns: %A{colsOut}"
    for colCounter = 0 to colsIn - 1 do
        for rowCounter = 0 to rowsIn - 1 do
            let outputColumn = 0 - rowCounter + colCounter + colsIn - 1
            let outputRow = rowCounter + colCounter + 1 - 1
            printfn $"colCounter: %A{colCounter} rowCounter: %A{rowCounter} outputColumn: %A{outputColumn} outputRow: %A{outputRow}"
            output[outputColumn][outputRow] <- inputAsArrays[colCounter][rowCounter]
    output |> toSequence

// let linesAsCharArrays = lines |> Seq.map Seq.toArray


printfn $"Original"
printfn $"%A{lines |> Seq.toList}"
let rotated = lines |> Seq.map Seq.toList |> Seq.map (Seq.map (fun x -> Some(x))) |> rotate45degreesClockwise
printfn $"Rotated"
printfn $"%A{rotated |> Seq.map Seq.toList |> Seq.toList}"
