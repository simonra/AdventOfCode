open System
// For more information see https://aka.ms/fsharp-console-apps
printfn $"{DateTime.Now:o} Hello from F#"

let mutable inputFileName = "Input/Example.txt"
// inputFileName <- "Input/Input.txt"

let lines = seq { yield! System.IO.File.ReadLines inputFileName }

let initialNumbers =
    lines
    |> Seq.head
    |> _.Split(" ")
    |> Array.toSeq
    // |> Seq.map uint64

// https://stackoverflow.com/a/65345593/2890086
let repeat f n = Seq.init n (fun _ -> f) |> Seq.reduce (>>)

let mutable nextGenDict: Map<string, string seq> = Map.empty
let mutable next5GenDict: Map<string, string seq> = Map.empty

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

let getNextNumber (inputNumber: string) : string seq =
    let valueInDict = nextGenDict.TryFind inputNumber
    if valueInDict <> None then
        valueInDict.Value
    else
        let nextValuesForNumber = iterateNumber inputNumber
        let nextKvp = (inputNumber, nextValuesForNumber)
        nextGenDict <- nextGenDict.Add nextKvp
        nextValuesForNumber

let get5GenLaterNumbers (inputNumber: string) : string seq =
    let valueInDict = next5GenDict.TryFind inputNumber
    if valueInDict <> None then
        valueInDict.Value
    else
        let firstGen = getNextNumber inputNumber
        let secondGen = firstGen |> Seq.map getNextNumber |> Seq.collect id
        let thirdGen = secondGen |> Seq.map getNextNumber |> Seq.collect id
        let fourthGen = thirdGen |> Seq.map getNextNumber |> Seq.collect id
        let fifthGen = fourthGen |> Seq.map getNextNumber |> Seq.collect id
        let nextKvp = (inputNumber, fifthGen)
        next5GenDict <- next5GenDict.Add nextKvp
        fifthGen

let iterateLine5Gens (input: string seq) : string seq =
    // seq {
    //     for number in input do
    //         let result = (iterateNumber number)
    //         for r in result do yield r
    // }
    input
    |> Seq.map get5GenLaterNumbers
    |> Seq.collect id

let iterateLine (input: string seq) : string seq =
    // seq {
    //     for number in input do
    //         let result = (iterateNumber number)
    //         for r in result do yield r
    // }
    input
    |> Seq.map getNextNumber
    |> Seq.collect id

let repeatIterateLine = repeat iterateLine
let generations =
    Map [
        ("0", Map [
            (1, seq {"1"})
            (2, seq {"2024"})
        ])
        ("1", Map [
            (1, seq {"2024"})
            (2, seq {"20"; "24"})
            (3, seq {"2"; "0"; "2"; "4"})
        ])
        ("2", Map [
            (1, seq {"4048"})
            (2, seq {"40"; "48"})
            (3, seq {"4"; "0"; "4"; "8"})
        ])
        ("3", Map [
            (1, seq {"6072"})
            (2, seq {"60"; "72"})
            (3, seq {"6"; "0"; "7"; "2"})
        ])
        ("4", Map [
            (1, seq {"8096"})
            (2, seq {"80"; "96"})
            (3, seq {"8"; "0"; "9"; "6"})
        ])
        ("5", Map [
            (1, seq {"10120"})
            (2, seq {"20482880"})
            (3, seq {"2048";"2880"})
            (4, seq {"20"; "48"; "28"; "80"})
            (5, seq {"2"; "0"; "4"; "8"; "2"; "8"; "8"; "0"})
        ])
        ("6", Map [
            (1, seq {"12144"})
            (2, seq {"24579456"})
            (3, seq {"2457";"9456"})
            (4, seq {"24"; "57"; "94"; "56"})
            (5, seq {"2"; "4"; "5"; "7"; "9"; "4"; "5"; "6"})
        ])
        ("7", Map [
            (1, seq {"14168"})
            (2, seq {"28676032"})
            (3, seq {"2867";"6032"})
            (4, seq {"28"; "67"; "60"; "32"})
            (5, seq {"2"; "8"; "6"; "7"; "6"; "0"; "3"; "2"})
        ])
        ("8", Map [
            (1, seq {"16192"})
            (2, seq {"32772608"})
            (3, seq {"3277";"2608"})
            (4, seq {"32"; "77"; "26"; "8"})
            (5, seq {"3"; "2"; "7"; "7"; "2"; "6"; "16192"})
        ])
        ("9", Map [
            (1, seq {"18216"})
            (2, seq {"36869184"})
            (3, seq {"3686";"9184"})
            (4, seq {"36"; "86"; "91"; "84"})
            (5, seq {"3"; "6"; "8"; "6"; "9"; "1"; "8"; "4"})
        ])
    ]

// let rec score (stones: string seq) (numberOfGenerations: int) =
//     if numberOfGenerations = 0 then stones |> Seq.length
//     else
//     let successors = iterateLine stones
let oneAfter75 = seq { "1" } |> repeatIterateLine 20
printfn $"{DateTime.Now:o} Part 0 count after 25 generations: {oneAfter75 |> Seq.length}"

let after25Generations =
    initialNumbers
    |> repeat iterateLine5Gens 5

printfn $"{DateTime.Now:o} Part 1 count after 25 generations: {after25Generations |> Seq.length}"
printfn $"{DateTime.Now:o} Part 1 done"

let after30Generations =
    after25Generations
    |> repeat iterateLine5Gens 1
printfn $"{DateTime.Now:o} Count after 30 generations: {after30Generations |> Seq.length}"
printfn $"{DateTime.Now:o} Count 30 generations done"

let after40Generations =
    after30Generations
    |> repeat iterateLine5Gens 2
printfn $"{DateTime.Now:o} Count after 40 generations: {after40Generations |> Seq.length}"
printfn $"{DateTime.Now:o} Count 40 generations done"

let after50Generations =
    after40Generations
    |> repeat iterateLine5Gens 2
printfn $"{DateTime.Now:o} Count after 50 generations: {after50Generations |> Seq.length}"
printfn $"{DateTime.Now:o} Count 50 generations done"

let after60Generations =
    after50Generations
    |> repeat iterateLine5Gens 2
printfn $"{DateTime.Now:o} Count after 60 generations: {after50Generations |> Seq.length}"
printfn $"{DateTime.Now:o} Count 60 generations done"

let after70Generations =
    after60Generations
    |> repeat iterateLine5Gens 2
printfn $"{DateTime.Now:o} Count after 70 generations: {after60Generations |> Seq.length}"
printfn $"{DateTime.Now:o} Count 70 generations done"

let after75Generations =
    after70Generations
    |> repeat iterateLine5Gens 1
printfn $"{DateTime.Now:o} Count after 75 generations: {after60Generations |> Seq.length}"
printfn $"{DateTime.Now:o} Count 75 generations done"
