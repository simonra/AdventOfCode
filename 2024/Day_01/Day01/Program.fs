// For more information see https://aka.ms/fsharp-console-apps

open System
open System.IO
open System.Numerics

printfn "Hello from F#"

let lines = seq { yield! System.IO.File.ReadLines "Input/Example.txt" }
// let lines = seq { yield! System.IO.File.ReadLines "Input/Input.txt" }
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

let absoluteDiffs =
    (sortedFirst, sortedSecond) ||> Seq.map2 (fun x y -> Math.Abs(x - y))

printfn $"Content after diffing: %A{absoluteDiffs |> List.ofSeq}"

let folded =
    Seq.fold (fun accumulated nextElement -> accumulated + nextElement) 0 absoluteDiffs

printfn $"Sum of diffs: %A{folded}"
printfn $"Part 1 done"
printfn $"Part 2 starting"

let countsInSecond = sortedSecond |> Seq.countBy (fun x -> x)
printfn $"Counts of each element in second column: %A{countsInSecond}"
// let scoreIfPresent tupleCollection value =

let countsInFirst = sortedFirst |> Seq.countBy id
printfn $"Counts of each element in first column: %A{countsInFirst}"

let countsInSecondIfInFirst =
    countsInSecond |> Seq.where (fun pair -> Seq.contains (fst pair) firstColumn)

printfn $"Counts of each element in second column that are present in first: %A{countsInSecondIfInFirst}"

let countsInFirstIfInSecond =
    countsInFirst |> Seq.where (fun pair -> Seq.contains (fst pair) secondColumn)

printfn $"Counts of each element in first column that are present in second: %A{countsInFirstIfInSecond}"

let similarityScore3 =
    Seq.fold2
        (fun accumulated firstPair secondPair -> accumulated + (fst firstPair) * (snd secondPair) * (snd firstPair))
        0
        countsInFirstIfInSecond
        countsInSecondIfInFirst

printfn $"Similarity score take 3 is %A{similarityScore3}"

exit 0

let similarityScore =
    Seq.fold
        (fun accumulated nextPair ->
            accumulated
            + snd nextPair
              * (firstColumn
                 |> Seq.tryPick (fun n -> if n = fst nextPair then Some(int n) else Some(int 0)))
                  .Value)
        0
        countsInSecond

printfn $"Similarity score is %A{similarityScore}"

let similarityScore2 =
    Seq.fold
        (fun accumulated nextValue ->
            accumulated
            + nextValue
              * (Seq.tryPick
                  (fun valueCountPairFromSecondColumn ->
                      if fst valueCountPairFromSecondColumn = nextValue then
                          Some(int (snd valueCountPairFromSecondColumn))
                      else
                          Some(int 0))
                  countsInSecond)
                  .Value)
        0
        firstColumn

printfn $"Similarity score take 2 is %A{similarityScore2}"
// let similarityScore = firstColumn |> (fun x -> x * countsInSecond.)
// let similarityScore = firstColumn |> (fun x -> x * (match x with countsInSecond when countsInSecond[0] == x else 0))

// File.ReadAllLines("Input/Example.txt") |> Seq.iter (fun s1 -> printfn $"%A{s1}")

// File.ReadAllLines("Input/Example.txt")
// |> Seq.iter (fun line ->
//     // printfn $"%A{line}"
//     line.Split "   "
//     |> (fun pair ->
//         printfn $"Read pair in line '%A{line}' are '%A{pair[0]}' in first column, and '%A{pair[1]}' in second column"
//     )
// )
