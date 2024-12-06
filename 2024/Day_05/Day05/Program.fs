// For more information see https://aka.ms/fsharp-console-apps

open System

printfn "Hello from F#"

let mutable inputFileName = "Input/Example.txt"
inputFileName <- "Input/Input.txt" // 4662

let inputText = System.IO.File.ReadAllText inputFileName

type pageNumber = int
type pageOrderRequirement = { before : pageNumber
                              after: pageNumber }
// type pageCollection = {pages: pageNumber seq}

let pageCollectionSatisfiesOrderingRequirement (requirements: pageOrderRequirement seq) (pages: pageNumber seq) : bool =
    // let rulesThatApplyToPage = requirements |> Seq.where (fun requirement -> requirement.before )
    raise (System.NotImplementedException(""))

let middlePage (pages: pageNumber seq) : pageNumber =
    let numberOfPages = Seq.length pages
    let halfWayPoint = (numberOfPages / 2)
    pages |> Seq.skip halfWayPoint |> Seq.head

let parsePages (allPages:string) : pageNumber seq seq =
    let unparsedManuals = allPages.Split("\n")
    unparsedManuals |> Seq.where (fun x -> x <> "") |> Seq.map (fun pageLine -> pageLine.Split(',') |> Seq.map int)

let parseOrderingRules (allRules:string) : pageOrderRequirement seq =
    let unparsedRules = allRules.Split("\n")
    let rulePairs = unparsedRules |> Seq.where (fun x -> x <> "") |> Seq.map (fun ruleLine -> ruleLine.Split('|') |> Seq.map int)
    let parsedPairs = rulePairs |> Seq.map (fun pair -> { before = Seq.head pair
                                                          after = Seq.last pair })
    parsedPairs

let requirementsRelevantForManual (requirements: pageOrderRequirement seq) (pages: pageNumber seq) : pageOrderRequirement seq =
    requirements |> Seq.where (fun rule -> Seq.contains rule.before pages && Seq.contains rule.after pages)

let pagesWhichMustComeBefore (requirements: pageOrderRequirement seq) (page: pageNumber) : pageNumber seq =
    requirements |> Seq.where (fun rule -> rule.after = page) |> Seq.map (fun rule -> rule.before)
let pagesWhichMustComeAfter (requirements: pageOrderRequirement seq) (page: pageNumber) : pageNumber seq =
    requirements |> Seq.where (fun rule -> rule.before = page) |> Seq.map (fun rule -> rule.after)

let rec pagesViolateBeforeRule
    (requirements: pageOrderRequirement seq)
    (manual: pageNumber seq)
    : bool =
        if Seq.isEmpty manual then
            false
        else
            let currentElement = manual |> Seq.head
            let followingPages = manual |> Seq.tail
            let pagesThatMustComeBeforeCurrent = pagesWhichMustComeBefore requirements currentElement
            if followingPages |> Seq.exists (fun x -> Seq.contains x pagesThatMustComeBeforeCurrent) then
                true
            else
                pagesViolateBeforeRule requirements followingPages
let rec pagesViolateAfterRule
    (requirements: pageOrderRequirement seq)
    (reversedManual: pageNumber seq)
    : bool =
        if Seq.isEmpty reversedManual then
            false
        else
            let currentElement = reversedManual |> Seq.head
            let followingPages = reversedManual |> Seq.tail
            let pagesThatMustComeAfterCurrent = pagesWhichMustComeAfter requirements currentElement
            if followingPages |> Seq.exists (fun x -> Seq.contains x pagesThatMustComeAfterCurrent) then
                true
            else
                pagesViolateAfterRule requirements followingPages

let sortByRules
    (requirements: pageOrderRequirement seq)
    (manual: pageNumber seq)
    : pageNumber seq
    =
    let mutable result : pageNumber list = [Seq.head manual]
    let mutable nextCandidateFound = false
    // let manualAsList = manual |> Seq.toList
    // Insertion sort
    let firstSkipped = manual |> Seq.skip 1 |> Seq.toList
    for manPage in firstSkipped do
        nextCandidateFound <- false
        let mutable i = 0
        while (not nextCandidateFound) && (i < result.Length) do
            let candidate = List.insertAt i manPage result
            let candidateIsValid =
                if pagesViolateBeforeRule requirements candidate then
                    false
                else
                    let reversedCandidate = candidate |> Seq.rev
                    if pagesViolateAfterRule requirements reversedCandidate then
                        false
                    else
                        true
            result <- if candidateIsValid then candidate else result
            nextCandidateFound <- candidateIsValid
            i <- i + 1
        result <- if nextCandidateFound then result else List.append result [manPage]
        // for i = 0 to result.Length do
        //     let candidate = List.insertAt i manPage result
        //     ignore
    result
    // raise (new NotImplementedException())

let rec validOrderingByRules
    (requirements: pageOrderRequirement seq)
    (previous: pageOrderRequirement option)
    : pageNumber seq =
        if previous = None then
            let startingPair =
                requirements
                |> Seq.find (fun r ->
                    (Seq.forall (fun r2 -> r2.after <> r.before) requirements)
                )
            let nextResult: pageNumber seq = validOrderingByRules (requirements |> Seq.except (seq {startingPair})) (Some startingPair)
            let firstPairAsSequence: pageNumber seq = seq {startingPair.before}
            let combined: pageNumber seq = Seq.append firstPairAsSequence nextResult
            combined
        // else if (Seq.isEmpty requirements) then
        //     seq {previous.Value.before; previous.Value.after}
        else if (Seq.length requirements = 1) then
            let lastRequirement = Seq.head requirements
            seq { lastRequirement.before; lastRequirement.after }
        else
            let next =
                requirements
                |> Seq.tryFind (fun r -> previous.Value.after = r.before)
            if next <> None then
                Seq.append (seq {next.Value.before}) (validOrderingByRules (requirements |> Seq.except (seq {next.Value})) (next))
            else
                let startingPair =
                    requirements
                    |> Seq.find (fun r ->
                        (Seq.forall (fun r2 -> r2.after <> r.before) requirements)
                    )
                Seq.append (seq {startingPair.before}) (validOrderingByRules (requirements |> Seq.except (seq {startingPair})) (Some startingPair))
let inputSections = inputText.Split("\n\n")
let rulesTextSection = inputSections |> Seq.head
let manualPagesTextSection = inputSections |> Seq.last

let parsedRules = rulesTextSection |> parseOrderingRules
let parsedManuals = manualPagesTextSection |> parsePages

// printfn $"Rules:"
// printfn $"%A{parsedRules |> Seq.toList}"
// printfn $"Pages"
// printfn $"%A{parsedManuals |> Seq.map Seq.toList}"

// let middlePages = parsedManuals |> Seq.map middlePage
// printfn $"%A{DateTime.Now} Middle pages:"
// middlePages |> Seq.iter (fun x -> printfn $"%A{x}")

// printfn $"Integer division works like this: %A{3/2}"

printfn $"%A{DateTime.Now} Creating ruleset paris"
let manualRulesetPairs: ((pageNumber seq) * (pageOrderRequirement seq)) list =
    parsedManuals
    |> Seq.map (fun manual ->
        let relevantRequirements = requirementsRelevantForManual parsedRules manual
        manual, relevantRequirements)
    |> Seq.toList
printfn $"%A{DateTime.Now} Done creating {manualRulesetPairs |> Seq.length} ruleset paris"
printfn $"%A{DateTime.Now} Starting work on task 1"
let safeManuals =
    manualRulesetPairs
    |> Seq.mapi (fun i x ->
        if i % 10 = 0 then
            printfn $"%A{DateTime.Now} Processing manual #%A{i}"
        x)
    |> Seq.where (fun (manual , ruleset) ->
        if pagesViolateBeforeRule ruleset manual then
            false
        else
            let reversedManual = manual |> Seq.rev
            if pagesViolateAfterRule ruleset reversedManual then
                false
            else
                true
        // raise (System.NotImplementedException(""))
        )
    |> Seq.map (fun (manual , ruleset) -> manual)
    |> Seq.map middlePage
    |> Seq.sum

printfn $"%A{DateTime.Now} Sum of middle pages of safe manuals are: '%A{safeManuals}'"
// exit 0
printfn $"%A{DateTime.Now} Starting work on part 2"
// let incorrectlyOrdered =
//     manualRulesetPairs
//     |> Seq.where (fun (manual , ruleset) ->
//         if pagesViolateBeforeRule ruleset manual then
//             true
//         else
//             let reversedManual = manual |> Seq.rev
//             pagesViolateAfterRule ruleset reversedManual
//         )
// printfn $"%A{DateTime.Now} Done finding incorrect manuals"
// // let forcedValidOrdersWithDuplicates =
// //     incorrectlyOrdered
// //     |> Seq.map (fun (manual, ruleset) -> validOrderingByRules ruleset None)
// //     |> Seq.toList
// // let forcedValidOrders =
// //     forcedValidOrdersWithDuplicates
// //     |> Seq.map (fun x -> Seq.distinct x)
// // printfn $"%A{DateTime.Now} Done finding correct orders of incorrect manuals"
// let forcedValidOrders =
//     incorrectlyOrdered
//     |> Seq.map (fun (manual, ruleset) -> sortByRules ruleset manual)
// let middleNumbers =
//     forcedValidOrders
//     |> Seq.map middlePage
// printfn $"%A{DateTime.Now} Done finding middle pages of corrected manuals"
// let sumMiddleNumbers =
//     middleNumbers
//     |> Seq.sum
// printfn $"%A{DateTime.Now} The sum of the middle numbers of the corrected pages: %A{sumMiddleNumbers}"

let sumOfMiddleNumbersOfCorrectedUpdates =
    manualRulesetPairs
    |> Seq.where (fun (manual , ruleset) ->
        if pagesViolateBeforeRule ruleset manual then
            true
        else
            let reversedManual = manual |> Seq.rev
            pagesViolateAfterRule ruleset reversedManual
        )
    |> Seq.mapi (fun i x ->
        if i % 10 = 0 then
            printfn $"%A{DateTime.Now} Processing incorrect manual #%A{i}"
        x)
    |> Seq.map (fun (manual, ruleset) -> sortByRules ruleset manual)
    |> Seq.map middlePage
    |> Seq.sum
printfn $"%A{DateTime.Now} The sum of the middle numbers of the corrected pages: %A{sumOfMiddleNumbersOfCorrectedUpdates}"
