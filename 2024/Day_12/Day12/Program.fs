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
    member c.isDiagonallyUpperLeftOf (other:coordinate) : bool =
        c.row + 1 = other.row
        && c.column + 1 = other.column
    member c.isDiagonallyUpperRightOf (other:coordinate) : bool =
        c.row + 1 = other.row
        && c.column - 1 = other.column
    member c.isDiagonallyLowerLeftOf (other:coordinate) : bool =
        c.row - 1 = other.row
        && c.column + 1 = other.column
    member c.isDiagonallyLowerRightOf (other:coordinate) : bool =
        c.row - 1 = other.row
        && c.column - 1 = other.column

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
    member r.numberOfSides : int =
        if r.memberCoordinates.Count = 0 then
            printfn $"This is wrong, debug me"
            0
        elif r.memberCoordinates.Count < 3 then
            4
        else
        let numberOfCorners : int =
            r.memberCoordinates
            |> Seq.fold (fun aggregated mc ->
                // let otherCoordinates = r.memberCoordinates |> Set.remove mc
                let mutable partialSum = 0
                let noneAbove = not (r.memberCoordinates.Contains({row = mc.row - 1; column = mc.column}))
                let noneBelow = not (r.memberCoordinates.Contains({row = mc.row + 1; column = mc.column}))
                let noneLeft  = not (r.memberCoordinates.Contains({row = mc.row; column = mc.column - 1}))
                let noneRight = not (r.memberCoordinates.Contains({row = mc.row; column = mc.column + 1}))
                let noneDiagonallyAboveLeft  = not(r.memberCoordinates.Contains({row = mc.row - 1; column = mc.column - 1}))
                let noneDiagonallyAboveRight = not(r.memberCoordinates.Contains({row = mc.row - 1; column = mc.column + 1}))
                let noneDiagonallyBelowLeft  = not(r.memberCoordinates.Contains({row = mc.row + 1; column = mc.column - 1}))
                let noneDiagonallyBelowRight = not(r.memberCoordinates.Contains({row = mc.row + 1; column = mc.column + 1}))
                let isUpperLeftCorner  = noneAbove && noneLeft
                let isUpperRightCorner = noneAbove && noneRight
                let isLowerLeftCorner  = noneBelow && noneLeft
                let isLowerRightCorner = noneBelow && noneRight
                let isInnerCornerUpperLeft  = noneDiagonallyAboveLeft  && not noneAbove && not noneLeft
                let isInnerCornerUpperRight = noneDiagonallyAboveRight && not noneAbove && not noneRight
                let isInnerCornerLoweLeft   = noneDiagonallyBelowLeft  && not noneBelow && not noneLeft
                let isInnerCornerLowerRight = noneDiagonallyBelowRight && not noneBelow && not noneRight
                if isUpperLeftCorner || isInnerCornerUpperLeft then
                    partialSum <- partialSum + 1
                if isUpperRightCorner || isInnerCornerUpperRight then
                    partialSum <- partialSum + 1
                if isLowerLeftCorner || isInnerCornerLoweLeft then
                    partialSum <- partialSum + 1
                if isLowerRightCorner || isInnerCornerLowerRight then
                    partialSum <- partialSum + 1

                aggregated + partialSum
                // ┌ ┐
                // └ ┘
                ) 0
        numberOfCorners
        // let outsidePoints : coordinate list =
        //     r.memberCoordinates
        //     |> Seq.map (fun c ->
        //         let otherCoordinates = r.memberCoordinates |> Set.remove c
        //         seq {
        //             if (otherCoordinates |> Seq.forall (fun o -> not (c.isAbove o) )) then
        //                 yield { row = c.row - 1; column = c.column }
        //             if (otherCoordinates |> Seq.forall (fun o -> not (c.isBelow o) )) then
        //                 yield { row = c.row + 1; column = c.column }
        //             if (otherCoordinates |> Seq.forall (fun o -> not (c.isToTheRightOf o) )) then
        //                 yield { row = c.row; column = c.column + 1 }
        //             if (otherCoordinates |> Seq.forall (fun o -> not (c.isToTheLeftOf o) )) then
        //                 yield { row = c.row; column = c.column - 1 }
        //         })
        //     |> Seq.collect id
        //     |> Seq.distinct
        //     |> Seq.toList
        // // for op in outsidePoints do
        // //
        // let sharesRows =
        //     outsidePoints
        //     |> Seq.groupBy (fun p -> p.row)
        // let sharesColumns =
        //     outsidePoints
        //     |> Seq.groupBy (fun p -> p.column)
        // let uniquePerRow =
        //     sharesRows
        //     |> Seq.map (fun (g,p) ->
        //         p
        //         |> Seq.pairwise
        //         |> Seq.fold (fun (aggregate: Set<coordinate>) (first,second) ->
        //             let areNeighbours = first.isToTheLeftOf second || first.isToTheRightOf second
        //             if areNeighbours then
        //                 aggregate
        //             else
        //                 aggregate.Add(first).Add(second)
        //             ) Set.empty
        //         )
        //         // |> Seq.fold (fun (aggregate: Set<coordinate>) nextPoint ->
        //         //     let isAdjacent =
        //         //         p
        //         //         |> Seq.exists (fun o -> nextPoint.isToTheLeftOf o || nextPoint.isToTheRightOf o)
        //         //     if isAdjacent then
        //         //         aggregate
        //         //     else
        //         //         aggregate.Add(nextPoint)
        //         //     ) Set.empty
        //         // )
        //     |> Seq.collect id
        //     |> (fun x -> Set(x))
        // let uniquePerCol =
        //     sharesColumns
        //     |> Seq.map (fun (g,p) ->
        //         p
        //         |> Seq.pairwise
        //         |> Seq.fold (fun (aggregate: Set<coordinate>) (first,second) ->
        //             let areNeighbours = first.isAbove second || first.isBelow second
        //             if areNeighbours then
        //                 aggregate
        //             else
        //                 aggregate.Add(first).Add(second)
        //             ) Set.empty
        //         )
        //     |> Seq.collect id
        //     |> Set
        // let rowAdjacentSides =
        //     uniquePerRow
        //     |> ignore
        // // Fold adjacents
        // // For each, check if above/below i row, or left/right if column, is in set. If yes, +1
        // raise (NotImplementedException())
    member r.bulkDiscountedFencingPrice = r.area * r.numberOfSides

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
let part2Result =
    regions
    |> Seq.map (fun r -> r.bulkDiscountedFencingPrice)
    |> Seq.sum
printfn $"{DateTime.Now:o} Part 2 result is '{part2Result}'"
printfn $"{DateTime.Now:o}"
