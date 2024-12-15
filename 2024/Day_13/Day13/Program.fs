open System
open System.Numerics
open System.Text.RegularExpressions

// For more information see https://aka.ms/fsharp-console-apps
printfn $"{DateTime.Now:o} Hello from F#"

let mutable inputFileName = "Input/Example.txt"
inputFileName <- "Input/Input.txt"

let inputContent = System.IO.File.ReadAllText inputFileName

// https://stackoverflow.com/a/65345593/2890086
let iterate f n = Seq.init n (fun _ -> f) |> Seq.reduce (>>)

let divideComplexNumber (numerator: 'a * 'a) (denominator: 'a * 'a) : 'a * 'a=
    let a = fst numerator
    let b = snd numerator
    let c = fst denominator
    let d = snd denominator
    let real = (a*c + b*d)/(c*c + d*d)
    let imaginary = (b*c - a*d)/(c*c + d*d)
    (real,imaginary)

module simplifiedVectors =
    type vec<'a> = 'a array
    let add (a:vec<'t>) (b:vec<'t>) : vec<'t> = (a, b) ||> Array.map2 (fun an bn -> an + bn)
    let subtract (a:vec<'t>) (b:vec<'t>) : vec<'t> = (a, b) ||> Array.map2 (fun (an:'t) (bn:'t) -> an - bn)
    let scale (k: 't) (a:vec<'t>) : vec<'t> = a |> Array.map (fun an -> k * an)
    let product (a:vec<'t>) (b:vec<'t>) : vec<'t> = (a, b) ||> Array.map2 (fun an bn -> an * bn)
    let dotProduct (a:vec<'t>) (b:vec<'t>) : 't = (product a b) |> Array.fold (fun aggregated next -> aggregated + next) LanguagePrimitives.GenericZero
    // let magnitude (a:vec<'a>) = sqrt (dotProduct a a)
    let areParallel (a:vec<'t>) (b:vec<'t>) : bool =
        if a.Length <> b.Length then raise (NotSupportedException()) else
        if a.Length = 1 then true
        elif a.Length = 2 then
            // https://math.stackexchange.com/a/1324023
            a[0] * b[1] = a[1] * b[2]
        else
            // a · b = ‖a‖ ‖b‖ cos(θ)
            // When parallel, θ = 0 or θ = π -> cos(θ) = 1 or cos(θ) = - 1
            // Remember, ‖a‖ = √(a · a)
            // (a · b)² = (‖a‖ ‖b‖ cos(θ))² -> (a · b)² = ‖a‖² ‖b‖² cos(θ)²
            // (a · b)² = ‖a‖² ‖b‖² -> (a · b)² = (a · a) (b · b)
            let ab = dotProduct a b
            let aa = dotProduct a a
            let bb = dotProduct b b
            ab * ab = aa * bb
    let arePerpendicular (a:vec<'t>) (b:vec<'t>) : bool =
        if a.Length <> b.Length then raise (NotSupportedException()) else
        (dotProduct a b) = LanguagePrimitives.GenericZero
    let divide (a : vec<'t>) (b : vec<'t>) : 't option =
        if not (areParallel a b) then None else
        Seq.initInfinite (fun x -> LanguagePrimitives.GenericOne)
        |> Seq.scan (fun (aggregate: 't) (next: 't) -> aggregate + next) (LanguagePrimitives.GenericZero)
        |> Seq.takeWhile (fun (x: 't) -> x * a[0] < b[0])
        |> Seq.last
        |> Some
    let modulo (a:vec<'a>) (b:vec<'a>) : 'a option =
        if not (areParallel a b) then None else
        let aa = dotProduct a a
        let bb = dotProduct b b
        if aa = bb then Some(LanguagePrimitives.GenericZero) else
        if aa < bb then Some(LanguagePrimitives.GenericOne) else
        let wholeAsInB = (a |> divide <| b).Value
        let amountOfAToDiscard = wholeAsInB |> scale <| a
        let remainingA = a |> subtract <| amountOfAToDiscard
        let asInRemainingA = remainingA |> divide <| a
        asInRemainingA

open simplifiedVectors
type button<'a> = {
    // movement: vec
    xMovement: 'a
    yMovement: 'a
}

type prizeLocation<'a> = {
    X: 'a
    Y: 'a
}

type clawMachine<'a> = {
    A: button<'a>
    B: button<'a>
    // targetPosition: Vector2
    prize: prizeLocation<'a>
}

type solution<'a> = {
    neededAs: 'a
    neededBs: 'a
}

let getNumber<'a> (input:string) : 'a =
    // raise (NotImplementedException())
    let inputDigits = Regex.Replace(input, "[^0-9]", "")
    // nameof<'a> inputDigits
    let foo = System.ComponentModel.TypeDescriptor.GetConverter(typeof<'a>);
    foo.ConvertFromInvariantString(inputDigits) :?> 'a
    // parse (Regex.Replace(input, "[^0-9]", ""))
let getNumbers<'a> (input:string) : 'a list =
    let stringPairs = input.Split(',')
    [getNumber stringPairs[0]; getNumber stringPairs[1]]

let parseClawMachine<'a> (input:string) : clawMachine<'a> =
    let lines = input.Split("\n")
    let firstLine = getNumbers lines[0]
    let secondLine = getNumbers lines[1]
    let thirdLine = getNumbers lines[2]
    {
        A = { xMovement = firstLine[0]; yMovement = firstLine[1] }
        B = { xMovement = secondLine[0]; yMovement = secondLine[1] }
        // targetPosition = Vector2(firstLine[0], firstLine[1])
        prize = { X = thirdLine[0]; Y = thirdLine[1] }
    }

// let cost (input:clawMachine) : float option =
//     let costOfA = LanguagePrimitives.GenericOne + LanguagePrimitives.GenericOne + LanguagePrimitives.GenericOne
//     // let costOfB = LanguagePrimitives.GenericOne
//     let xBound = input.prize.X / input.B.xMovement
//     let yBound = input.prize.Y / input.B.yMovement
//     if xBound = yBound && xBound % LanguagePrimitives.GenericOne = LanguagePrimitives.GenericZero then
//         Some(xBound)
//     else
//     let bCeiling = Math.Max(input.B.xMovement, input.B.yMovement)
//     let mutable nextB = bCeiling
//     // while
//     // for b = bCeiling to 0 do
//     //     raise (NotImplementedException())
//     raise (NotImplementedException())
//     // if

let inline trySolveEquation a0 b0 c0 a1 b1 c1 =
    let determinant = a0 * b1 - b0 * a1
    if determinant = LanguagePrimitives.GenericZero then
        printfn $"Equation determinant ≠ 0, none or many solutions"
        None
    else
    let x0 = (c0 * b1 - b0 * c1) / determinant
    let x1 = (c1 * a0 - c0 * a1) / determinant
    // Handle integer derp
    if a0 * x0 + b0 * x1 = c0 && a1 * x0 + b1 * x1 = c1 then
        Some([|x0; x1|])
    else
    printfn $"The solution didn't solve our equation, probably due to integer division"
    None

let inline findSolutions<'a when
    'a: (static member Zero: 'a) and
    'a: (static member (+): 'a * 'a -> 'a) and
    'a: (static member (-): 'a * 'a -> 'a) and
    'a: (static member (*): 'a * 'a -> 'a) and
    'a: (static member (/): 'a * 'a -> 'a) and
    'a: equality
    > (input:clawMachine<'a>) : solution<'a> list =
    let maybeEquationSolution = trySolveEquation input.A.xMovement input.B.xMovement input.prize.X input.A.yMovement input.B.yMovement input.prize.Y
    if maybeEquationSolution = None then []
    else
    [ {neededAs = maybeEquationSolution.Value[0]; neededBs = maybeEquationSolution.Value[1]} ]
    //
    // let xBoundB = input.prize.X / input.B.xMovement
    // let yBoundB = input.prize.Y / input.B.yMovement
    // let bMax = Seq.min (seq {xBoundB; yBoundB; 100.0})
    // let xBoundA = input.prize.X / input.A.xMovement
    // let yBoundA = input.prize.Y / input.A.yMovement
    // let aMax = Seq.min (seq {xBoundA; yBoundA; 100.0})
    // let mutable result : solution list = list.Empty
    // let aMap = {0.0 .. 1.0 .. aMax}
    // // for a in aMap do
    //     // let nextBMax =
    //     // let bMap = {0.0 .. 1.0 .. bMax}
    //     // for b in bMap do
    //     //
    //     // if input.prize.X - a * input.A.xMovement %
    // result
let inline priceSolution<'a
    when 'a: (static member One: 'a)
    and  'a: (static member (*): 'a * 'a -> 'a)
    and  'a: (static member (+): 'a * 'a -> 'a)
    > (input: solution<'a>) =
    let costOfA : 'a = LanguagePrimitives.GenericOne + LanguagePrimitives.GenericOne + LanguagePrimitives.GenericOne
    let costOfB : 'a = LanguagePrimitives.GenericOne
    input.neededAs * costOfA + input.neededBs * costOfB

let parseClawMachines<'a> (input: string) : clawMachine<'a> seq =
    input.Split("\n\n")
    |> Seq.map parseClawMachine

let machines = parseClawMachines inputContent

// printfn $"{DateTime.Now:o} Inputs are:"
// machines |> Seq.iter (fun x -> printfn $"%A{x}")

printfn $"{DateTime.Now:o} Part 1:"
machines
|> Seq.map findSolutions
|> Seq.collect id
|> Seq.map priceSolution
|> Seq.sum
|> (fun x -> printfn $"{DateTime.Now:o} Part 1 solution: {x}")
printfn $"{DateTime.Now:o} Part 1 done"

printfn $"{DateTime.Now:o} Parsing part 2 claw machines"
let part2Constant = 10_000_000_000_000L
let part2ClawMachines =
    machines
    |> Seq.map (fun m ->
        {m with prize.X = m.prize.X + part2Constant; prize.Y = m.prize.Y + part2Constant})
printfn $"{DateTime.Now:o} Part 2 claw machines parsed"
part2ClawMachines
|> Seq.map findSolutions
|> Seq.collect id
|> Seq.map priceSolution
|> Seq.sum
|> (fun x -> printfn $"{DateTime.Now:o} Part 2 solution: {x}")
printfn $"{DateTime.Now:o} Part 2 done"
