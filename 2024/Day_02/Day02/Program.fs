// For more information see https://aka.ms/fsharp-console-apps

printfn "Hello from F#"
open System

let mutable inputFileName = "Input/Example.txt"
inputFileName <- "Input/EdgeCasesFirst2Invalid.txt"
// inputFileName <- "Input/Input.txt"

let lines = seq { yield! System.IO.File.ReadLines inputFileName }

type Level = int
type Report = { Levels: Level seq }

type ChangeDirection =
    | Increasing
    | Decreasing
    | Unchanging

let DifferenceIsSafe (first: Level) (second: Level) : bool =
    let difference = Math.Abs(second - first)
    1 <= difference && difference <= 3

// let inline IsMonotonicallyChanging<'T
//     when 'T: (static member (-): 'T * 'T -> 'T)
//     and INumber<'T>>
//     (xs: 'T seq)
//     : bool =
//         true

let ReportIsSafe (report: Report) : bool =
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

            direction <- Increasing
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

let ParseReportLine (line: string) : Report =
    { Levels = line.Split(' ') |> Seq.map int }

let parsedReports = Seq.map ParseReportLine lines
printfn $"The parsed reports are %A{parsedReports |> Seq.toList}"

let reportStatuses = Seq.map ReportIsSafe parsedReports
printfn $"The reports statuses are %A{reportStatuses |> Seq.toList}"

let numberOfSafeReports =
    reportStatuses |> Seq.where (fun x -> x = true) |> Seq.length

printfn $"The number of safe reports is %A{numberOfSafeReports}"

let StepIsSafe (firstReading: Level) (secondReading: Level) (direction: ChangeDirection) : bool =
    let mutable safe = true

    if firstReading = secondReading then
        safe <- false
    else if direction = Increasing && secondReading < firstReading then
        safe <- false
    else if direction = Decreasing && firstReading < secondReading then
        safe <- false

    if not (DifferenceIsSafe firstReading secondReading) then
        safe <- false

    safe

let hamming s1 s2 =
    Seq.map2 ((=)) s1 s2 |> Seq.sumBy (fun b -> if b then 0 else 1)

let ReportIsSafeWithBadnessTolerance (report: Report) numberOfUnsafeLevelPairsAllowed : bool =
    printfn $"\nProcessing report %A{report}"
    let mutable reportIsSafe = true
    let mutable direction = Unchanging
    let mutable ignoredReadings = 0
    let firstTriplet = Seq.windowed 3 report.Levels |> Seq.head

    if firstTriplet[0] = firstTriplet[1] then
        ignoredReadings <- ignoredReadings + 1

        if firstTriplet[1] = firstTriplet[2] then
            reportIsSafe <- false
            direction <- Unchanging
        else
            if not (DifferenceIsSafe firstTriplet[1] firstTriplet[2]) then
                reportIsSafe <- false

            if firstTriplet[1] < firstTriplet[2] then
                direction <- Increasing
            else
                direction <- Decreasing
    else if firstTriplet[0] < firstTriplet[1] then
        if DifferenceIsSafe firstTriplet[0] firstTriplet[1] then
            direction <- Increasing
        else
            ignoredReadings <- ignoredReadings + 1

            if not (DifferenceIsSafe firstTriplet[1] firstTriplet[2]) then
                reportIsSafe <- false

            if firstTriplet[1] = firstTriplet[2] then
                reportIsSafe <- false
                direction <- Unchanging
            else if firstTriplet[1] < firstTriplet[2] then
                direction <- Increasing
            else
                direction <- Decreasing
    else if DifferenceIsSafe firstTriplet[0] firstTriplet[1] then
        direction <- Decreasing
    else
        ignoredReadings <- ignoredReadings + 1

        if not (DifferenceIsSafe firstTriplet[1] firstTriplet[2]) then
            reportIsSafe <- false

        if firstTriplet[1] = firstTriplet[2] then
            reportIsSafe <- false
            direction <- Unchanging
        else if firstTriplet[1] < firstTriplet[2] then
            direction <- Increasing
        else
            direction <- Decreasing

    printfn
        $"After considering first triplet, report is safe: '%A{reportIsSafe}', direction is '%A{direction}', number of ignore readings is '%A{ignoredReadings}'"

    if reportIsSafe then
        // printfn $"Report has statuses: allDifferencesAreSafe: %A{allDifferencesAreSafe} allValuesAreMonotonicallyChanging %A{allValuesAreMonotonicallyChanging} direction %A{direction}"
        // printfn $"Starting pair processing"
        Seq.windowed 3 report.Levels
        |> Seq.skip 1
        |> Seq.takeWhile (fun levelTriplet ->
            // printfn $"Processing pair %A{levelTriplet}"
            if not (StepIsSafe levelTriplet[1] levelTriplet[2] direction) then
                ignoredReadings <- ignoredReadings + 1

                if not (StepIsSafe levelTriplet[0] levelTriplet[2] direction) then
                    reportIsSafe <- false
            // printfn $"Pair %A{levelPair} has statuses: allDifferencesAreSafe: %A{allDifferencesAreSafe} allValuesAreMonotonicallyChanging %A{allValuesAreMonotonicallyChanging} direction %A{direction}"
            reportIsSafe && ignoredReadings <= numberOfUnsafeLevelPairsAllowed)
        |> Seq.iter ignore
    // |> ignore
    // printfn $"Done pair processing"
    // ToDo: Check tail?

    reportIsSafe && ignoredReadings <= numberOfUnsafeLevelPairsAllowed



// let reportStatusesWithinTolerance = Seq.map (fun report -> ReportIsSafeWithBadnessTolerance report 1) parsedReports
//
// printfn $"Compiling safety list"
// let safetyList = reportStatusesWithinTolerance |> Seq.toList
// printfn $"The reports statuses are %A{safetyList}"
//
// let numberOfSafeReportsWithTolerance = reportStatusesWithinTolerance |> Seq.where (fun reportStatus -> reportStatus = true) |> Seq.length
// printfn $"The number of safe reports with tolerance is %A{numberOfSafeReportsWithTolerance}"

let ReportIsSafeWithBadnessToleranceBruteForce (report: Report) : bool =
    let mutable reportIsSafe = ReportIsSafe report

    if not reportIsSafe then
        report.Levels
        |> Seq.takeWhile (fun _ -> not reportIsSafe)
        |> Seq.iteri (fun i v ->
            let skippedI = Seq.removeAt i report.Levels

            if (ReportIsSafe { Levels = skippedI }) then
                reportIsSafe <- true)

    reportIsSafe

let reportStatusesWithinTolerance =
    Seq.map (fun report -> ReportIsSafeWithBadnessToleranceBruteForce report) parsedReports

printfn $"Compiling safety list"
let safetyList = reportStatusesWithinTolerance |> Seq.toList
printfn $"The reports statuses are %A{safetyList}"

let numberOfSafeReportsWithTolerance =
    reportStatusesWithinTolerance
    |> Seq.where (fun reportStatus -> reportStatus = true)
    |> Seq.length

printfn $"The number of safe reports with tolerance is %A{numberOfSafeReportsWithTolerance}"
