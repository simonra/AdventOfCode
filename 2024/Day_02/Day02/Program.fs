// For more information see https://aka.ms/fsharp-console-apps

printfn "Hello from F#"
open System

let mutable inputFileName = "Input/Example.txt"
// inputFileName <- "Input/Input.txt"

let lines = seq { yield! System.IO.File.ReadLines inputFileName }

type Level = int
type Report = { Levels: Level seq }
type ChangeDirection =
    | Increasing
    | Decreasing
    | Unchanging

let DifferenceIsSafe (first:Level) (second:Level) : bool =
    let difference = Math.Abs(second - first)
    1 <= difference && difference <= 3

// let inline IsMonotonicallyChanging<'T
//     when 'T: (static member (-): 'T * 'T -> 'T)
//     and INumber<'T>>
//     (xs: 'T seq)
//     : bool =
//         true

let ReportIsSafe (report:Report) : bool =
    // printfn $"Processing report %A{report}"
    let mutable allDifferencesAreSafe = true
    let mutable allValuesAreMonotonicallyChanging = true
    let mutable direction = Unchanging
    let firstPair = Seq.pairwise report.Levels |> Seq.head
    if fst firstPair = snd firstPair then
        direction <- Unchanging
        allValuesAreMonotonicallyChanging <- false
    else if fst firstPair < snd firstPair then
        direction <- Increasing
    else
        direction <- Decreasing

    // printfn $"Report has statuses: allDifferencesAreSafe: %A{allDifferencesAreSafe} allValuesAreMonotonicallyChanging %A{allValuesAreMonotonicallyChanging} direction %A{direction}"
    // printfn $"Starting pair processing"
    Seq.pairwise report.Levels
        |> Seq.takeWhile (fun levelPair ->
            // printfn $"Processing pair %A{levelPair}"
            if fst levelPair = snd levelPair then
                direction <- Unchanging
                allValuesAreMonotonicallyChanging <- false
            else if fst levelPair < snd levelPair then
                if direction <> Increasing then
                    allValuesAreMonotonicallyChanging <- false
                direction <-Increasing
            else
                if direction <> Decreasing then
                    allValuesAreMonotonicallyChanging <- false
                direction <- Decreasing
            allDifferencesAreSafe <- DifferenceIsSafe (fst levelPair) (snd levelPair)
            // printfn $"Pair %A{levelPair} has statuses: allDifferencesAreSafe: %A{allDifferencesAreSafe} allValuesAreMonotonicallyChanging %A{allValuesAreMonotonicallyChanging} direction %A{direction}"
            allDifferencesAreSafe && allValuesAreMonotonicallyChanging)
        |> Seq.iter ignore
        // |> ignore
    // printfn $"Done pair processing"

    allDifferencesAreSafe && allValuesAreMonotonicallyChanging

let ParseReportLine (line:string) : Report =
    {
        Levels =
            line.Split(' ')
            |> Seq.map int
    }

let parsedReports = Seq.map ParseReportLine lines
printfn $"The parsed reports are %A{parsedReports |> Seq.toList}"

let reportStatuses = Seq.map ReportIsSafe parsedReports
printfn $"The reports statuses are %A{reportStatuses |> Seq.toList}"

let numberOfSafeReports = reportStatuses |> Seq.where (fun x -> x = true) |> Seq.length
printfn $"The number of safe reports is %A{numberOfSafeReports}"

let ReportIsSafeWithBadnessTolerance (report:Report) numberOfUnsafeLevelPairsAllowed : bool =
    // printfn $"Processing report %A{report}"
    // let mutable allDifferencesAreSafe = true
    // let mutable allValuesAreMonotonicallyChanging = true
    let mutable direction = Unchanging
    let mutable numberOfUnsafePairs = 0
    let firstPair = Seq.pairwise report.Levels |> Seq.head
    if fst firstPair = snd firstPair then
        direction <- Unchanging
        // No need for updating unsafety of first pair here, will be handled in thingy below
        // numberOfUnsafePairs <- numberOfUnsafePairs + 1
    else if fst firstPair < snd firstPair then
        direction <- Increasing
    else
        direction <- Decreasing

    // printfn $"Report has statuses: numberOfUnsafePairs: %A{numberOfUnsafePairs} direction %A{direction}"
    // printfn $"Starting pair processing"
    Seq.pairwise report.Levels
        |> Seq.takeWhile (fun levelPair ->
            // printfn $"Processing pair %A{levelPair}"
            let mutable pairIsUnsafe = false
            if fst levelPair = snd levelPair then
                direction <- Unchanging
                pairIsUnsafe <- true
            else if fst levelPair < snd levelPair then
                if direction <> Increasing then
                    pairIsUnsafe <- true
                direction <-Increasing
            else
                if direction <> Decreasing then
                    pairIsUnsafe <- true
                direction <- Decreasing
            if not (DifferenceIsSafe (fst levelPair) (snd levelPair)) then
                pairIsUnsafe <- true
            if pairIsUnsafe then
                numberOfUnsafePairs <- numberOfUnsafePairs + 1
            // printfn $"Pair %A{levelPair} has statuses: numberOfUnsafePairs: %A{numberOfUnsafePairs} direction %A{direction}"
            numberOfUnsafePairs <= numberOfUnsafeLevelPairsAllowed)
        |> Seq.iter ignore
    // printfn $"Done pair processing"

    numberOfUnsafePairs <= numberOfUnsafeLevelPairsAllowed

let reportStatusesWithTolerance = Seq.map (fun x -> ReportIsSafeWithBadnessTolerance x 1) parsedReports
printfn $"The reports statuses are %A{reportStatusesWithTolerance |> Seq.toList}"

let numberOfSafeReportsWithTolerance = reportStatusesWithTolerance |> Seq.where (fun x -> x = true) |> Seq.length
printfn $"The number of safe reports with tolerance is %A{numberOfSafeReportsWithTolerance}"
