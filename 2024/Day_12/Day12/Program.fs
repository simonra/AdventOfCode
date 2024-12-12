open System
// For more information see https://aka.ms/fsharp-console-apps
printfn $"{DateTime.Now:o} Hello from F#"

let mutable inputFileName = "Input/Example.txt"
inputFileName <- "Input/Example-bigger.txt"
inputFileName <- "Input/Input.txt"

let lines = seq { yield! System.IO.File.ReadLines inputFileName }

type coordinate = {
    row: int
    column: int
}

type region = {
    contentType: char
    mutable memberCoordinates: Set<coordinate>
}

type coordinate with
    member c.isBelow (other:coordinate) : bool =
        c.row = other.row + 1
        && c.column = other.column
    member c.isAbove (other:coordinate) : bool =
        c.row = other.row - 1
        && c.column = other.column
    member c.isToTheRightOf (other:coordinate) : bool =
        c.row = other.row
        && c.column = other.column + 1
    member c.isToTheLeftOf (other:coordinate) : bool =
        c.row = other.row
        && c.column = other.column - 1
    member c.isAdjacentTo (other:coordinate) : bool =
        c.isBelow other
        || c.isAbove other
        || c.isToTheRightOf other
        || c.isToTheLeftOf other

type region with
    member r.area : int = r.memberCoordinates.Count
    member r.circumference : int =
        if r.memberCoordinates.Count = 1 then
            4
        elif r.memberCoordinates.Count = 2 then
            6
        else
        r.memberCoordinates
        |> Seq.map (fun c ->
                    let otherCoordinates = r.memberCoordinates |> Set.remove c
                    seq {
                        if (otherCoordinates |> Seq.forall (fun o -> not (c.isAbove o) )) then
                            yield 1
                        if (otherCoordinates |> Seq.forall (fun o -> not (c.isBelow o) )) then
                            yield 1
                        if (otherCoordinates |> Seq.forall (fun o -> not (c.isToTheRightOf o) )) then
                            yield 1
                        if (otherCoordinates |> Seq.forall (fun o -> not (c.isToTheLeftOf o) )) then
                            yield 1
                    })
        |> Seq.collect id
        |> Seq.length
    member r.fencingPrice = r.area * r.circumference

let parseRegions (input: char array array) : region seq =
    let mutable foundRegions: region list = List.empty
    for row = 0 to input.Length - 1 do
        printfn $"{DateTime.Now:o} Parsing row {row}"
        for column = 0 to input[row].Length - 1 do
            let nextItemValue = input[row][column]
            let nextCoordinate: coordinate = {row = row; column = column}
            let matchingRegions = foundRegions |> Seq.indexed |> Seq.where (fun (i, r) -> r.contentType = nextItemValue)
            let adjacentRegions =
                matchingRegions
                // |> Seq.indexed
                |> Seq.where (fun (i,r) ->
                    r.memberCoordinates
                    |> Seq.exists (fun m -> (nextCoordinate.isAdjacentTo m)))
            let numberOfAdjacentRegions = adjacentRegions |> Seq.length
            if numberOfAdjacentRegions = 0 then
                let newRegion = { contentType = nextItemValue; memberCoordinates = Set.empty.Add(nextCoordinate) }
                foundRegions <- List.append foundRegions [newRegion]
            elif numberOfAdjacentRegions = 1 then
                let updatedRegion = adjacentRegions |> Seq.head
                foundRegions <- List.removeAt (fst updatedRegion) foundRegions
                foundRegions <- List.append foundRegions [
                    { contentType = nextItemValue; memberCoordinates = Set((snd updatedRegion).memberCoordinates).Add(nextCoordinate) }
                ]
            else
                // merge and join
                let mergedRegion = {
                    contentType = nextItemValue
                    memberCoordinates =
                        adjacentRegions
                        |> Seq.map (fun (i,r) -> r.memberCoordinates)
                        |> Seq.collect id
                        |> (fun x -> Set(x).Add(nextCoordinate))
                }
                let indexesToReplace =
                    adjacentRegions
                    |> Seq.map (fun (i,x) -> i)
                    |> Seq.sortDescending
                indexesToReplace |> Seq.iter (fun i -> foundRegions <- foundRegions |> List.removeAt i)
                foundRegions <- List.append foundRegions [ mergedRegion ]
    foundRegions

let toArrayOfArraysOfChars input =
    input
    |> Seq.map Seq.toList
    |> Seq.map (Seq.map id)
    |> Seq.map Seq.toArray
    |> Seq.toArray

let regions =
    lines
    |> toArrayOfArraysOfChars
    |> parseRegions
let part1Result =
    regions
    |> Seq.map (fun r -> r.fencingPrice)
    |> Seq.sum

printfn $"{DateTime.Now:o} Part 1 result is '{part1Result}'"
printfn $"{DateTime.Now:o}"
