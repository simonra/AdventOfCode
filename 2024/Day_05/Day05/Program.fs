// For more information see https://aka.ms/fsharp-console-apps
printfn "Hello from F#"

let mutable inputFileName = "Input/Example.txt"
inputFileName <- "Input/Input.txt"

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
    requirements |> Seq.where (fun rule -> Seq.contains rule.before pages || Seq.contains rule.after pages)

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

let inputSections = inputText.Split("\n\n")
let rulesTextSection = inputSections |> Seq.head
let manualPagesTextSection = inputSections |> Seq.last

let parsedRules = rulesTextSection |> parseOrderingRules
let parsedManuals = manualPagesTextSection |> parsePages

printfn $"Rules:"
printfn $"%A{parsedRules |> Seq.toList}"
printfn $"Pages"
printfn $"%A{parsedManuals |> Seq.map Seq.toList}"

let middlePages = parsedManuals |> Seq.map middlePage
printfn $"Middle pages:"
// middlePages |> Seq.iter (fun x -> printfn $"%A{x}")

// printfn $"Integer division works like this: %A{3/2}"

let safeManuals =
    parsedManuals
    |> Seq.where (fun manual ->
        let relevantRequirements = requirementsRelevantForManual parsedRules manual
        let reversedManual = manual |> Seq.rev
        not (pagesViolateBeforeRule relevantRequirements manual) && not (pagesViolateAfterRule relevantRequirements reversedManual)
        // raise (System.NotImplementedException(""))
        )
    |> Seq.map middlePage
    |> Seq.sum

printfn $"Sum of middle pages of safe manuals are: '%A{safeManuals}'"
