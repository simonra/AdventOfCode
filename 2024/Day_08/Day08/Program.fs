open System
// For more information see https://aka.ms/fsharp-console-apps
printfn "Hello from F#"

let mutable inputFileName = "Input/Example.txt"
inputFileName <- "Input/Input.txt"

let lines = seq { yield! System.IO.File.ReadLines inputFileName }

let printMap (input: 'a array array) =
    printfn $"%A{DateTime.Now} Map state to print"
    input |> Seq.iter (fun row ->
        row |> Seq.iter (fun x -> printf $"{x}")
        printfn ""
        )
    printfn $"%A{DateTime.Now} Done printing map state"

let toArrayOfArraysOfChars input =
    input
    |> Seq.map Seq.toList
    |> Seq.map (Seq.map id)
    |> Seq.map Seq.toArray
    |> Seq.toArray

let findUniqueCharacters input =
    input
    |> Seq.collect (fun row -> row |> Seq.where (fun column -> column <> '.'))
    |> Seq.distinct

[<TailCall>]
let rec findAllPairsOfItems (input: 'a seq) : ('a * 'a) seq =
    seq {
        match input with
        | s when Seq.isEmpty s -> ()
        | _ ->
            let head = input |> Seq.head
            let tail = input |> Seq.tail
            for element in tail do
                yield head, element
            yield! findAllPairsOfItems tail
    }

type coordinate = {
    row : int
    column: int
}

let findAntiNodesForCoordinatePair (input: (coordinate * coordinate)) : (coordinate * coordinate) =
    // let sortedInput = seq {fst input; snd input} |> Seq.sortBy (fun x -> x.row, x.column)
    // let firstNode = sortedInput |> Seq.head
    // let secondNode = sortedInput |> Seq.tail |> Seq.head
    let firstNode = fst input
    let secondNode = snd input
    // let rowDistance = Math.Abs(secondNode.row - firstNode.row)
    let rowDistance = secondNode.row - firstNode.row
    // let colDistance = Math.Abs(secondNode.column - firstNode.column)
    let colDistance = secondNode.column - firstNode.column
    let firstAntiNode = {row = (firstNode.row - (rowDistance)); column = firstNode.column - (colDistance)}
    let secondAntiNode = {row = (secondNode.row + (rowDistance)); column = secondNode.column + (colDistance)}
    (firstAntiNode, secondAntiNode)

let coordinateIsWithinBounds
    (lowerBound: coordinate)
    (upperBound: coordinate)
    (input: coordinate)
    : bool =
        lowerBound.row <= input.row
        && input.row < upperBound.row
        && lowerBound.column <= input.column
        && input.column < upperBound.column

let findAntiNodesForCoordinatePairPart2
    (lowerBound: coordinate)
    (upperBound: coordinate)
    (input: coordinate * coordinate)
    : coordinate seq =
        let firstNode = fst input
        let secondNode = snd input
        let rowDistance = secondNode.row - firstNode.row
        let colDistance = secondNode.column - firstNode.column
        let antiNodesBefore =
            firstNode
            |> Seq.unfold (fun (iteratorState:coordinate) ->
                if coordinateIsWithinBounds lowerBound upperBound iteratorState then
                // if lowerBound.row <= iteratorState.row && lowerBound.column <= iteratorState.column then
                    let nextState = {row = (iteratorState.row - rowDistance); column = iteratorState.column - colDistance}
                    Some(iteratorState, nextState)
                else
                    None
                )
        let antiNodesAfter =
            secondNode
            |> Seq.unfold (fun (iteratorState:coordinate) ->
                if coordinateIsWithinBounds lowerBound upperBound iteratorState then
                // if iteratorState.row < upperBound.row && iteratorState.column < upperBound.column then
                    let nextState = {row = (iteratorState.row + rowDistance); column = iteratorState.column + colDistance}
                    Some(iteratorState, nextState)
                else
                    None
                )
            // |> Seq.takeWhile (fun x -> x<>None )
        Seq.append antiNodesBefore antiNodesAfter
        // antiNodesBefore

let findAllCoordinatesPerCharacter (input: char array array) : Map<char, coordinate list> =
    input
    |> Seq.mapi (fun rowNumber row ->
        row
        |> Seq.mapi (fun colNumber col ->
            if col <> '.' then
                Some(col, {row = rowNumber; column = colNumber})
            else
                None
            )
            |> Seq.where (fun x -> x <> None)
            |> Seq.map (fun x -> x.Value)
        )
    |> Seq.collect id
    |> Seq.groupBy (fun x -> fst x)
    |> Seq.map (fun g -> (
        (fst g),
        ((snd g)
            |> Seq.map (fun x -> snd x)
            |> Seq.toList)
        )
    )
    |> Seq.toList
    |> Map.ofList

let findAllAntiNodesPerCharacters (input: Map<char, coordinate list>) : Map<char, (coordinate * coordinate) list> =
    let nodePairsPerChar =
        input
        |> Map.map (fun _ value -> value |> findAllPairsOfItems)
    let antiNodesPerPair =
        nodePairsPerChar
        |> Map.map (fun _ value -> value |> Seq.map findAntiNodesForCoordinatePair)
    antiNodesPerPair |> Map.map (fun k v -> v |> Seq.toList)

let findAllAntiNodesPerCharacterPart2
    (lowerBound: coordinate)
    (upperBound: coordinate)
    (input: Map<char, coordinate list>)
    : Map<char, coordinate seq> =
        // let nodePairsPerChar =
        input
        |> Map.map (fun _ value ->
            value
            |> findAllPairsOfItems
            |> Seq.map (fun pair -> pair |> findAntiNodesForCoordinatePairPart2 lowerBound upperBound)
            |> Seq.collect id
            )

let removeAntiNodesOutsideBounds
    (lowerBound: coordinate)
    (upperBound: coordinate)
    (antiNodes: coordinate seq)
    : coordinate seq =
        antiNodes
        |> Seq.where (fun antiNode ->
            lowerBound.row <= antiNode.row
            && antiNode.row < upperBound.row
            && lowerBound.column <= antiNode.column
            && antiNode.column < upperBound.column
            )

let parsedInput = lines |> toArrayOfArraysOfChars
// printMap parsedInput
let uniqueCharacters = parsedInput |> findUniqueCharacters
// printfn $"Unique characters are %A{uniqueCharacters |> Seq.sort |> Seq.toList}"
let numberOfRows = parsedInput |> Seq.length
let numberOfColumns = parsedInput |> Seq.head |> Seq.length

let coordPerCharacter = parsedInput |> findAllCoordinatesPerCharacter
printfn $"CoordinateParis:"
coordPerCharacter
|> Map.iter (fun k v ->
    printfn $"key: {k}"
    printf $"Values: "
    v |> Seq.sort |> Seq.iter (fun coord -> printf $"({coord.row},{coord.column})")
    printfn ""
    )

let allAntiNodesPerCharacter = coordPerCharacter |> findAllAntiNodesPerCharacters
let allAntiNodes =
    allAntiNodesPerCharacter
    |> Map.values
    |> Seq.collect id

printfn $"All anti nodes:"
allAntiNodes
|> Seq.map (fun pair -> seq {(fst pair); (snd pair)})
|> Seq.collect id
|> Seq.distinct
|> Seq.sort
|> Seq.iter (fun x -> printf $"({x.row},{x.column})")
printfn ""

let antiNodesWithinBounds =
    allAntiNodes
    |> Seq.map (fun pair -> seq {(fst pair); (snd pair)})
    |> Seq.collect id
    |> Seq.distinct
    |> removeAntiNodesOutsideBounds {row = 0; column = 0} {row = numberOfRows; column = numberOfColumns}
printfn $"Anti Nodes within bounds:"
antiNodesWithinBounds
|> Seq.sort
|> Seq.iter (fun x -> printf $"({x.row},{x.column})")
printfn ""

let numberOfAntiNodesWithinBounds =
    antiNodesWithinBounds |> Seq.length

printfn $"The number of anti nodes within the bounds are %A{numberOfAntiNodesWithinBounds}"

let part2AllAntiNodesPerCharacter =
    coordPerCharacter
    |> findAllAntiNodesPerCharacterPart2 {row = 0; column = 0} {row = numberOfRows; column = numberOfColumns}

let part2AllAntiNodes =
    part2AllAntiNodesPerCharacter
    |> Map.values
    |> Seq.collect id

let part2UniqueNumberOfAntNodes =
    part2AllAntiNodes
    |> Seq.distinct
    |> Seq.length

printfn $"The number of anti nodes for part 2 are %A{part2UniqueNumberOfAntNodes}"
