open System
// For more information see https://aka.ms/fsharp-console-apps
printfn $"{DateTime.Now:o} Hello from F#"

let mutable inputFileName = "Input/Example.txt"
inputFileName <- "Input/Input.txt"

let lines = seq { yield! System.IO.File.ReadLines inputFileName }

let initialNumbers =
    lines
    |> Seq.head
    |> _.Split(" ")
    |> Array.toSeq
    // |> Seq.map uint64

let iterateNumber (inputNumber: string) : string seq =
    if inputNumber = "0" then seq { "1" }
    elif inputNumber.Length % 2 = 0 then
        let halfPoint = inputNumber.Length / 2
        seq {
            inputNumber
                |> Seq.take halfPoint
                |> String.Concat
            inputNumber
                |> Seq.skip halfPoint
                |> Seq.skipWhile (fun x -> x = '0')
                |> String.Concat
                |> (fun x -> if x.Length = 0 then "0" else x)
        }
    else
        seq { ((uint64 inputNumber) * 2024UL).ToString() }

let iterateLine (input: string seq) : string seq =
    // seq {
    //     for number in input do
    //         let result = (iterateNumber number)
    //         for r in result do yield r
    // }
    input
    |> Seq.map iterateNumber
    |> Seq.collect id

let repeat n fn = Seq.init n (fun _ -> fn) |> Seq.reduce (>>)

// let afterDebugGenerations = initialNumbers |> repeat 1 iterateLine

let after25Generations =
    initialNumbers
    |> repeat 25 iterateLine

printfn $"{DateTime.Now:o} Part 1 count after 25 generations: {after25Generations |> Seq.length}"

let after75Generations =
    initialNumbers
    |> repeat 75 iterateLine
printfn $"{DateTime.Now:o} Part 2 count after 75 generations: {after75Generations |> Seq.length}"
