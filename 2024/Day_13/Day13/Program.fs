open System
open System.Text.RegularExpressions

// For more information see https://aka.ms/fsharp-console-apps
printfn $"{DateTime.Now:o} Hello from F#"

let mutable inputFileName = "Input/Example.txt"
// inputFileName <- "Input/Input.txt"

let inputContent = System.IO.File.ReadAllText inputFileName

type button = {
    xMovement: int
    yMovement: int
}

type prizeLocation = {
    X: int
    Y: int
}

type clawMachine = {
    A: button
    B: button
    prize: prizeLocation
}

let getNumber (input:string) : int = int (Regex.Replace(input, "[^0-9]", ""))
let getNumbers (input:string) : int list =
    let stringPairs = input.Split(',')
    [getNumber stringPairs[0]; getNumber stringPairs[1]]

let parseClawMachine (input:string) : clawMachine =
    let lines = input.Split("\n")
    let firstLine = getNumbers lines[0]
    let secondLine = getNumbers lines[1]
    let thirdLine = getNumbers lines[2]
    {
        A = { xMovement = firstLine[0]; yMovement = firstLine[1] }
        B = { xMovement = secondLine[0]; yMovement = secondLine[1] }
        prize = { X = thirdLine[0]; Y = thirdLine[1] }
    }

let parseClawMachines (input: string) : clawMachine seq =
    input.Split("\n\n")
    |> Seq.map parseClawMachine

let machines = parseClawMachines inputContent

printfn $"{DateTime.Now:o} Inputs are:"
machines |> Seq.iter (fun x -> printfn $"%A{x}")
