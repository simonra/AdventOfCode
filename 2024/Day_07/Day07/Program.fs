open System
// For more information see https://aka.ms/fsharp-console-apps
printfn "Hello from F#"

let mutable inputFileName = "Input/Example.txt"
inputFileName <- "Input/Input.txt"

let lines = seq { yield! System.IO.File.ReadLines inputFileName }

type equation<'a> = {
    result       : 'a
    inputNumbers : 'a seq
}

type solution<'a,'b,'c> = {
    isValid           : bool
    operatorSequence : ('a -> 'b -> 'c) seq
}

module PermutationFunctions =
    // https://stackoverflow.com/a/3129136/2890086
    let rec distribute e = function
      | [] -> [[e]]
      | x::xs' as xs -> (e::xs)::[for xs in distribute e xs' -> x::xs]

    let rec permute = function
      | [] -> [[]]
      | e::xs -> List.collect (distribute e) (permute xs)

    // https://stackoverflow.com/questions/4495597/combinations-and-permutations-in-f
    let rotate lst =
        List.tail lst @ [List.head lst]

    let getRotations lst =
        let rec getAll lst i = if i = 0 then [] else lst :: (getAll (rotate lst) (i - 1))
        getAll lst (List.length lst)

    let getPerms lst n=
        let rec getPermsImpl acc n lst = [
            match n, lst with
            | k, x :: xs ->
                if k > 0 then
                    for r in getRotations lst do
                        yield! getPermsImpl (List.head r :: acc) (k - 1) (r)
                if k >= 0 then yield! getPermsImpl acc k []
            | 0, [] -> yield acc
            | _, [] -> ()
        ]
        getPermsImpl List.empty n lst

let testSolution (equation:equation<'a>) (operators:('a -> 'b -> 'c) list) : bool =
    let operationsApplied = (Seq.head equation.inputNumbers, Seq.tail equation.inputNumbers, operators) |||> Seq.fold2 (fun acc nextNumber nextOperator -> nextOperator acc nextNumber)
    operationsApplied = equation.result

let findSolutions (allowedOperators:('a -> 'b -> 'c) list) (equation:equation<'a>) : solution<'a, 'b, 'c> seq =
    let numberOfInputs = equation.inputNumbers |> Seq.length
    let numberOfOperations = numberOfInputs - 1
    let candidateOperations = PermutationFunctions.getPerms allowedOperators numberOfOperations
    let solutions = candidateOperations |> Seq.map (fun x -> {isValid = testSolution equation x; operatorSequence = x})
    solutions

let numberType = int64
let equations =
    lines
    |> Seq.where (fun x -> x <> "")
    |> Seq.map _.Split(':')
    |> Seq.map (fun x ->
        let result = numberType x[0]
        let unparsedNumbers = x[1]
        let numbers =
            unparsedNumbers.Split(" ")
            |> Seq.where (fun x -> x <> "")
            |> Seq.map numberType
        { result = result
          inputNumbers = numbers }
        )

let operators = [(+); (*)]
let solutionsToEquations =
    equations |> Seq.map (fun x -> (x , (findSolutions operators x)))
let validSolutions =
    solutionsToEquations |> Seq.where (fun (_, solutions) -> solutions |> Seq.exists (fun x -> x.isValid))
let sumOfValidResults =
    validSolutions |> Seq.sumBy (fun (equation, _) -> equation.result)
printfn $"The sum of (valid) solutions to part 1 are: {sumOfValidResults}"

let concat first second =
    numberType $"{first}{second}"
let part2Operators = [(+); (*); (concat)]
let part2SolutionsToEquations =
    equations |> Seq.map (fun x -> (x , (findSolutions part2Operators x)))
let part2ValidSolutions =
    part2SolutionsToEquations |> Seq.where (fun (_, solutions) -> solutions |> Seq.exists (fun x -> x.isValid))
let part2SumOfValidResults =
    part2ValidSolutions |> Seq.sumBy (fun (equation, _) -> equation.result)
printfn $"The sum of (valid) solutions to part 1 are: {part2SumOfValidResults}"
