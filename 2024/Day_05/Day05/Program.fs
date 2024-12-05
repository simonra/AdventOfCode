// For more information see https://aka.ms/fsharp-console-apps
printfn "Hello from F#"

let mutable inputFileName = "Input/Example.txt"
// inputFileName <- "Input/Input.txt"

let inputText = System.IO.File.ReadAllText inputFileName

type pageNumber = int
type pageOrderRequirement = { before : pageNumber
                              after: pageNumber }
// type pageCollection = {pages: pageNumber seq}

let pageCollectionSatisfiesOrderingRequirement (requirements: pageOrderRequirement seq) (pages: pageNumber seq) : bool =
    // let rulesThatApplyToPage = requirements |> Seq.where (fun requirement -> requirement.before )
    false

let middlePage (pages: pageNumber seq) : pageNumber =
    let numberOfPages = Seq.length pages
    let halfWayPoint = (numberOfPages / 2) % 1
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
middlePages |> Seq.iter (fun x -> printfn $"%A{x}")
