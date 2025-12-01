open System
// For more information see https://aka.ms/fsharp-console-apps
printfn $"{DateTime.Now:o} Hello from F#"

printfn $"95 + 10 %% 100 = %d{(95 + 10) % 100}"
printfn $"5 - 10 %% 100 = %d{(5 - 10) % 100}"
printfn $"5 - 110 %% 100 = %d{(5 - 110) % 100}"

let mutable inputFileName = "Input/Example.txt"
// inputFileName <- "Input/Input.txt"

let lines = seq { yield! System.IO.File.ReadLines inputFileName }

type state = {
    currentDialPosition: int
}

type dialInstruction = {
    direction: char
    amount: int
}

let parseLine (input: string) : dialInstruction =
    {
        direction = input[0];
        amount = Int32.Parse(input[1..]);
    }

let parseInputFile (inputLines: string seq) : dialInstruction seq =
    inputLines
    |> Seq.map parseLine

let applyRotation (currentDialPosition: int, instruction: dialInstruction) : int =
    if instruction.direction = 'L' then
        let candidate = (currentDialPosition - instruction.amount) % 100
        if candidate >= 0 then
            candidate
        else
            candidate + 100
        // if instruction.amount <= currentDialPosition then
        //     currentDialPosition - instruction.amount
        // else
        //     100 + ((currentDialPosition - instruction.amount) % 100)
    else if instruction.direction = 'R' then
        (currentDialPosition + instruction.amount) % 100
    else
        raise (System.ArgumentException($"Unsupported direction {instruction.direction}"))

let parsedExampleFile = parseInputFile(lines)
// let applyAndPrint (initialPosition: int32, instructions: dialInstruction seq)
let exampleFinalPosition =
    parsedExampleFile
    |> Seq.fold (
        fun accumulated nextElement ->
            let nextValue = applyRotation(accumulated, nextElement)
            printfn $"Current: %A{accumulated},\tApply: %A{nextElement.direction}%A{nextElement.amount}\tNext: %A{nextValue}"
            nextValue
        ) 50
printfn $"Final example position is: %A{exampleFinalPosition}"
// let exampleNumberOfZeroes: (int * int) =
//     parsedExampleFile
//     |> Seq.fold (
//         fun (nextTuple, nextElement: dialInstruction) ->
//             let nextValue = applyRotation(nextPosition, nextElement)
//             if nextValue = 0 then
//                 (numberOfZeroes + 1, nextValue)
//             else
//                 (numberOfZeroes, nextValue)
//             // printfn $"Current: %A{accumulated},\tApply: %A{nextElement.direction}%A{nextElement.amount}\tNext: %A{nextValue}"
//             // nextValue
//         ) (0, 50)

let findZeroCountAndFinalDialPosition (instructions: dialInstruction seq, initialDialPosition: int) : (int * int)=
    let foldingFunc (zeroes, position) nextDialInstruction =
        let nextPosition = applyRotation(position, nextDialInstruction)
        if nextPosition = 0 then
            (zeroes + 1, nextPosition)
        else
            (zeroes, nextPosition)

    instructions
    |> Seq.fold foldingFunc (0, initialDialPosition)

let exampleNumberOfZeroes = findZeroCountAndFinalDialPosition(parsedExampleFile, 50)
printfn $"Example input number of zeroes is: %A{fst exampleNumberOfZeroes} (expected 3)"
printfn $"Example input final dial position is: %A{snd exampleNumberOfZeroes} (expected 32)"

// The dial starts by pointing at 50.
// The dial is rotated L68 to point at 82.
// The dial is rotated L30 to point at 52.
// The dial is rotated R48 to point at 0.
// The dial is rotated L5 to point at 95.
// The dial is rotated R60 to point at 55.
// The dial is rotated L55 to point at 0.
// The dial is rotated L1 to point at 99.
// The dial is rotated L99 to point at 0.
// The dial is rotated R14 to point at 14.
// The dial is rotated L82 to point at 32.
// Expected result: 3

printfn $"{DateTime.Now:o} Done processing Example"
let inputFilePath = "Input/Input.txt"
let inputLines = seq { yield! System.IO.File.ReadLines inputFilePath }
let parsedInputFile = parseInputFile(inputLines)
printfn $"{DateTime.Now:o} Done parsing input file for part 1"
let partOneNumberOfZeroes = findZeroCountAndFinalDialPosition(parsedInputFile, 50)
printfn $"{DateTime.Now:o} Part one number of zeroes is: %A{fst partOneNumberOfZeroes}"
printfn $"{DateTime.Now:o} Part one final dial position is: %A{snd partOneNumberOfZeroes}"
