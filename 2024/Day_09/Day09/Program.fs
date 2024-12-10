open System
// For more information see https://aka.ms/fsharp-console-apps
printfn $"{DateTime.Now} Hello from F#"

let mutable inputFileName = "Input/Example.txt"
inputFileName <- "Input/Input.txt"

let lines = seq { yield! System.IO.File.ReadLines inputFileName }

/// Decompresses the string "2333133121414131402" to the array [|Some(0); Some(0); None; None; None; Some(1); Some(1); Some(1); None; None; None; Some(2); None; None; None; Some(3); Some(3); Some(3); None; Some(4); Some(4); None; Some(5); Some(5); Some(5); Some(5); None; Some(6); Some(6); Some(6); Some(6); None; Some(7); Some(7); Some(7); None; Some(8); Some(8); Some(8); Some(8); Some(9); Some(9);|]
let decompressInput (input:string) : int option array =
    input
    |> Seq.map (fun (c: char) -> int (Char.ToString c))
    |> Seq.mapi (fun i v ->
        if i % 2 = 1 then
            Seq.init v (fun x -> None)
        else
            Seq.init v (fun x -> Some(i/2)))
    |> Seq.collect id
    |> Seq.toArray

// https://codereview.stackexchange.com/q/163713
let swap (first: int) (second: int) (array: 'a array): 'a array =
    let originalFirst = array.[first]
    array.[first] <- array.[second];
    array.[second] <- originalFirst;
    array

let firstNonePosition (input: 'a option array) : int =
    input |> Array.findIndex (fun item -> item = None)
let lastSomePosition (input: 'a option array) : int =
    input |> Array.findIndexBack (fun item -> item <> None)

// let swapFirstNoneWithLastSome(array: 'a option array): 'a option array =

[<TailCall>]
let rec defragFileArray (input: int option array) : int option array =
    let firstNonePos = input |> firstNonePosition
    let lastSomePos = input |> lastSomePosition
    if lastSomePos < firstNonePos then
        input
    else
        input |> swap firstNonePos lastSomePos |> defragFileArray

let calculateChecksum (input: int option array) : int64 =
    input
    |> Seq.takeWhile (fun x -> x <> None)
    |> Seq.mapi (fun i v -> (int64 i, int64 v.Value))
    |> Seq.fold (fun state (index,value) -> state + (index * value) ) 0

type processedValue = {
    value: int
    processed: bool
}
module Part2 =
    let swapRange (firstStart: int) (secondStart: int) (rangeSize: int) (array: 'a array): 'a array =
        for i = 0 to rangeSize - 1 do
            swap (firstStart + i) (secondStart + i) array |> ignore
        array
    let makProcessed (firstStart: int) (secondStart: int) (rangeSize: int) (array: processedValue option array): processedValue option array =
        for i = 0 to rangeSize - 1 do
            if firstStart < array.Length then
                array[firstStart + i] <- if array[firstStart + i] = None then None else Some({value = array[firstStart + i].Value.value; processed = true})
            if secondStart < array.Length then
                array[secondStart + i] <- if array[secondStart + i] = None then None else Some({value = array[secondStart + i].Value.value; processed = true})
        array
    let findLastUnprocessedSomePositions (input: processedValue option array) : int * int =
        let lastUnprocessedSome = input |> Array.findIndexBack (fun item -> item <> None && not item.Value.processed)
        let lastUnprocessedSomeValue = input[lastUnprocessedSome]
        let firstUnprocessedSome = input |> Array.findIndex (fun item -> item = lastUnprocessedSomeValue)
        (firstUnprocessedSome, lastUnprocessedSome)
    let findFistNoneGroupOfSize (size: int) (input: 'a option array) : int * int =
        let firstIndex =
            input
            |> Array.windowed (size)
            |> Array.tryFindIndex (fun window -> window |> Array.forall (fun value -> value = None))
            |> (fun x -> if x = None then (input.Length) else x.Value)
        let lastIndex = firstIndex + size - 1
        (firstIndex, lastIndex)
    [<TailCall>]
    let rec defragFileArray (input: processedValue option array) : processedValue option array =
        let allAreProcessed = input |> Array.forall (fun x -> x = None || x.Value.processed)
        if allAreProcessed then
            input
        else
        let lastSomesPositions = input |> findLastUnprocessedSomePositions
        let groupSize = snd lastSomesPositions - fst lastSomesPositions + 1
        let firstFittingNones = input |> findFistNoneGroupOfSize groupSize
        if (snd lastSomesPositions) < (fst firstFittingNones) then
            input
            |> makProcessed (fst firstFittingNones) (fst lastSomesPositions) groupSize
            |> defragFileArray
        else
            input
            |> swapRange (fst firstFittingNones) (fst lastSomesPositions) groupSize
            |> makProcessed (fst firstFittingNones) (fst lastSomesPositions) groupSize
            |> defragFileArray
    let calculateChecksum (input: processedValue option array) : int64 =
        input
        // |> Seq.takeWhile (fun x -> x <> None)
        |> Seq.mapi (fun i v -> (int64 i, if v <> None then int64 v.Value.value else 0))
        |> Seq.fold (fun state (index,value) -> state + (index * value) ) 0

lines
|> Seq.head
|> Seq.map (fun (c: char) -> int (Char.ToString c))
|> Seq.mapi (fun i v ->
    if i % 2 = 1 then
        Seq.init v (fun x -> None)
    else
        Seq.init v (fun x -> Some(i/2)))
|> Seq.collect id
|> Seq.toArray
|> defragFileArray
|> calculateChecksum
// |> Seq.iteri (fun i v -> printfn $"Found %A{v} at index %A{i}")
// |> Seq.iteri (fun i v -> printf $"(%A{v})")
|> (fun x -> printfn $"{DateTime.Now}  %A{x}")

// [|1;1;1;2;2;2;3;3;3;|]
// |> Part2.swapRange 0 6 2
// |> printfn "%A"

// [|Some(1);Some(1);Some(1);None;None;Some(2);Some(2);None;None;None;Some(3);Some(3);Some(3)|]
// |> Array.map (fun item -> if item = None then None else Some({value = item.Value; processed = false}))
// |> Part2.defragFileArray
// |> Array.iter (fun x -> printfn $"%A{if x = None then -1 else x.Value.value}")

lines
|> Seq.head
|> Seq.map (fun (c: char) -> int (Char.ToString c))
|> Seq.mapi (fun i v ->
    if i % 2 = 1 then
        Seq.init v (fun x -> None)
    else
        Seq.init v (fun x -> Some(i/2)))
|> Seq.collect id
|> Seq.toArray
|> Array.map (fun item -> if item = None then None else Some({value = item.Value; processed = false}))
|> Part2.defragFileArray
|> Part2.calculateChecksum
// |> Seq.iteri (fun i v -> printfn $"Found %A{v} at index %A{i}")
// |> Seq.iteri (fun i v -> printf $"(%A{v})")
|> (fun x -> printfn $"{DateTime.Now}  %A{x}")
