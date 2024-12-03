// For more information see https://aka.ms/fsharp-console-apps

open System.Text.RegularExpressions

printfn "Hello from F#"

let mutable inputFileName = "Input/Example.txt"
inputFileName <- "Input/Example2.txt"
inputFileName <- "Input/Input.txt"

let lines = seq { yield! System.IO.File.ReadLines inputFileName }
// let rx = Regex(@"mul\(\d+,\d+\)", RegexOptions.Compiled)
//
// let singleText = System.IO.File.ReadAllText inputFileName
// let found = seq {
//     for m in rx.Matches(singleText) do
//         yield m.Value, m.Index
// }
// found
// |> Seq.iter (fun (e, idx) -> printfn "%s at %d" e idx)

let muls input =
    seq {
        for m in Regex(@"mul\(\d+,\d+\)", RegexOptions.Compiled).Matches(input) do
            let arguments = m.Value.Replace("mul(", "").Replace(")", "").Split(',')
            yield arguments, m.Value, m.Index
    }

// singleText
// |> muls
// |> Seq.iter (fun (ars, original, position) -> printfn $"The arguments are '%A{ars}', parsed from the original %A{original}, found at position '%A{position}'")
//
// printfn $"Iterating through the input line by line"

lines
|> Seq.map muls
|> Seq.collect id // Flattens the list. Probably not a good idea for this task long term, but nice for debugging!
|> Seq.iter (fun (ars, original, position) ->
    printfn $"The arguments are '%A{ars}', parsed from the original %A{original}, found at position '%A{position}'")

let products inputs =
    inputs
    |> Seq.map muls
    |> Seq.collect id
    |> Seq.map (fun (args, original, position) -> (int args[0]) * (int args[1]))

// printfn $"Products are '%A{products lines |> Seq.toList}'"

let sumsOfProducts inputs =
    inputs |> Seq.fold (fun accumulated nextValue -> accumulated + nextValue) 0

printfn $"Sums of products are '%A{lines |> products |> sumsOfProducts}'"

let removeSectionsAfterDonts (input: string) : string =
    // For task 2 it appears the do/don't setting is supposed to persist across lines, so remove the linebreaks before evaluating
    let trimNewlines = input.Replace("\n", "")

    // Explanation of this regex for future self:
    // `(?<groupname>expression)` creates a group named `groupname`.
    // When you have `?expression`, or `?(group)`, the leading ? will match the first occurrence, as opposed to the normal which is matching the last occurrence.
    // Having named groups here is kinda pointless, this could have been simplified to `(don't\(\)).*?(do\(\))`
    let firstPass =
        Regex(@"(?<frontgroup>don't\(\)).*?(?<backgroup>do\(\))", RegexOptions.Compiled)
            .Replace(trimNewlines, "")

    // Remove potential trailing occurrence of `don't()`
    // (no need for similar check at start because problem description says to start in do()-mode).
    Regex(@"(?<frontgroup>don't\(\)).*$", RegexOptions.Compiled)
        .Replace(firstPass, "")

printfn $"Don't sections removed: '%A{lines |> Seq.map removeSectionsAfterDonts}'"

printfn
    $"Sums of products where don'ts are excluded are '%A{seq { System.IO.File.ReadAllText inputFileName }
                                                         |> Seq.map removeSectionsAfterDonts
                                                         |> products
                                                         |> sumsOfProducts}'"
