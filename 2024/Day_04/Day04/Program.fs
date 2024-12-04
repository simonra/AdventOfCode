// For more information see https://aka.ms/fsharp-console-apps
printfn "Hello from F#"

let mutable inputFileName = "Input/Example.txt"
inputFileName <- "Input/3x3.txt"
inputFileName <- "Input/4x3.txt"
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
    let rowsIn = input |> Seq.length
    let colsIn = input |> Seq.head |> Seq.length
    // let asymmetryFactor = System.Math.Abs(rowsIn - colsIn)
    let asymmetryFactorRows = if rowsIn > colsIn then rowsIn - colsIn else 0
    let asymmetryFactorColumns = if colsIn > rowsIn then colsIn - rowsIn else 0
    let rowsOut = rowsIn + colsIn - 1
    let colsOut = rowsIn + colsIn - 1
    let inputAsArrays = input |> toArray
    let mutable output : 'a option array array = Array.init rowsOut (fun r -> Array.init colsOut (fun _ -> None))
    printfn $"Dimensions:"
    printfn $"Input rows: %A{rowsIn} columns: %A{colsIn}"
    printfn $"Output rows: %A{rowsOut} columns: %A{colsOut}"
    for rowCounter = 0 to rowsIn - 1 do
        for colCounter = 0 to colsIn - 1 do
            // M[x][y] to cell RM[x+y+1][−x+y+n]
            let outputRow = rowCounter + colCounter + 1 - 1
            let outputColumn = 0 - rowCounter + colCounter + colsIn - 1 - asymmetryFactorColumns + asymmetryFactorRows
            printfn $"rowCounter: %A{rowCounter} colCounter: %A{colCounter} outputRow: %A{outputRow} outputColumn: %A{outputColumn}"
            let nextValue = inputAsArrays[rowCounter][colCounter]
            output[outputRow][outputColumn] <- nextValue
    output |> toSequence

// let linesAsCharArrays = lines |> Seq.map Seq.toArray


printfn $"Original"
printfn $"%A{lines |> Seq.toList}"
let rotated = lines |> Seq.map Seq.toList |> Seq.map (Seq.map (fun x -> Some(x))) |> rotate45degreesClockwise
printfn $"Rotated"
printfn $"%A{rotated |> Seq.map Seq.toList |> Seq.toList}"
