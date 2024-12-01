// For more information see https://aka.ms/fsharp-console-apps

open System
open System.IO
open System.Numerics

printfn "Hello from F#"

// let lines = seq { yield! System.IO.File.ReadLines "Input/Example.txt" }
let lines = seq { yield! System.IO.File.ReadLines "Input/Input.txt" }
// for line in lines do
//     printfn $"%A{line}"

printfn "Done printing all lines as read into lines variable"
let mutable firstColumn = Seq.empty
let mutable secondColumn = List.empty

for line in lines do
    let split = line.Split("   ")
    firstColumn <- Seq.append firstColumn [ int split[0] ]
    secondColumn <- List.append secondColumn [ int split[1] ]

printfn $"First column content after iteration: %A{firstColumn |> List.ofSeq}"
printfn $"Second column content after iteration: %A{secondColumn}"

let sortedFirst = firstColumn |> Seq.sort
let sortedSecond = secondColumn |> List.sort

printfn $"First column content after sorting: %A{sortedFirst |> List.ofSeq}"
printfn $"Second column content after sorting: %A{sortedSecond}"

let absoluteDiffs = (sortedFirst, sortedSecond) ||> Seq.map2(fun x y -> Math.Abs(x-y))
printfn $"Content after diffing: %A{absoluteDiffs |> List.ofSeq}"

let folded = Seq.fold (fun accumulated nextElement -> accumulated + nextElement) 0 absoluteDiffs
printfn $"Sum of diffs: %A{folded}"

// File.ReadAllLines("Input/Example.txt") |> Seq.iter (fun s1 -> printfn $"%A{s1}")

// File.ReadAllLines("Input/Example.txt")
// |> Seq.iter (fun line ->
//     // printfn $"%A{line}"
//     line.Split "   "
//     |> (fun pair ->
//         printfn $"Read pair in line '%A{line}' are '%A{pair[0]}' in first column, and '%A{pair[1]}' in second column"
//     )
// )
