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

let findUniqueCharacters input =
    input
    |> Seq.collect (fun row -> row |> Seq.where (fun column -> column <> '.'))
    |> Seq.distinct

type coordinate = {
    row : int
    column: int
}

let findAllCoordinatesPerCharacter (input: char array array) : Map<char, coordinate list> =
    input
    |> Seq.mapi (fun rowNumber row ->
        row
        |> Seq.mapi (fun colNumber col ->
            if col <> '.' then
                Some(col, {row = rowNumber; column = colNumber})
            else
                None
            )
            |> Seq.where (fun x -> x <> None)
            |> Seq.map (fun x -> x.Value)
        )
    |> Seq.collect id
    |> Seq.groupBy (fun x -> fst x)
    |> Seq.map (fun g -> ((fst g) , ((snd g) |> Seq.map (fun x -> snd x) |> Seq.toList)))
    |> Seq.toList
    |> Map.ofList

let parsedInput = lines |> toArrayOfArraysOfChars
// printMap parsedInput
let uniqueCharacters = parsedInput |> findUniqueCharacters
// printfn $"Unique characters are %A{uniqueCharacters |> Seq.sort |> Seq.toList}"
let numberOfRows = parsedInput |> Seq.length
let numberOfColumns = parsedInput |> Seq.head |> Seq.length
