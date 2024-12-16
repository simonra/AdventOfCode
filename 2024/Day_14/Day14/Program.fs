open System

// For more information see https://aka.ms/fsharp-console-apps
printfn $"{DateTime.Now:o} Hello from F#"

// https://stackoverflow.com/a/65345593/2890086
let iterate f n = Seq.init n (fun _ -> f) |> Seq.reduce (>>)

type sectorBounds<'a> = {
    xLower: 'a
    yLower: 'a
    xUpper: 'a
    yUpper: 'a
}

type position<'a> = {
    px: 'a
    py: 'a
}

type velocity<'a> = {
    vx: 'a
    vy: 'a
}

type robot<'a> = {
    position: position<'a>
    velocity: velocity<'a>
}

let positionIsWithinSector<'a when 'a: comparison> (sector: sectorBounds<'a>) (position: position<'a>) : bool =
    sector.xLower <= position.px && position.px <= sector.xUpper
    && sector.yLower <= position.py && position.py <= sector.yUpper

// https://stackoverflow.com/a/35848799/2890086
let inline (%!) a b = (a % b + b) % b

let inline move<'a when
    'a: (static member (+): 'a * 'a -> 'a) and
    'a: (static member (%): 'a * 'a -> 'a)> (xBound: 'a) (yBound: 'a) (robot: robot<'a>) : robot<'a> =
    // printfn $"Hello from move"
    let nextX = (robot.position.px + robot.velocity.vx) %! xBound
    let nextY = (robot.position.py + robot.velocity.vy) %! yBound
    let nextPosition: position<'a> = {px = nextX; py = nextY}
    // printfn $"For\n%A{robot}\nnext position is\n%A{nextPosition}"
    {robot with position = nextPosition}

let parseA<'a> (input: string) : 'a =
    // https://stackoverflow.com/a/8626476/2890086
    let typeConverter = System.ComponentModel.TypeDescriptor.GetConverter(typeof<'a>);
    typeConverter.ConvertFromInvariantString(input) :?> 'a

let parseRobot<'a> (input: string) : robot<'a>=
    let pvSplit = input.Split(' ')
    let pNums = pvSplit[0].Replace("p=", "").Split(',')
    let vNums = pvSplit[1].Replace("v=", "").Split(',')
    let pos : position<'a> = { px = parseA pNums[0] ; py = parseA pNums[1] }
    let vel : velocity<'a> = { vx = parseA vNums[0] ; vy = parseA vNums[1] }
    {position = pos; velocity = vel}

let inline getSafetyScore<'a
when 'a: (static member One: 'a)
and  'a: (static member Zero: 'a)
and  'a: comparison
and  'a: (static member (+): 'a * 'a -> 'a)
and  'a: (static member (-): 'a * 'a -> 'a)
and  'a: (static member (/): 'a * 'a -> 'a)>
    (mapSizeX: 'a) (mapSizeY: 'a) (robots: robot<'a> seq) : int =
    let zero = LanguagePrimitives.GenericZero
    let one = LanguagePrimitives.GenericOne
    let two = LanguagePrimitives.GenericOne + LanguagePrimitives.GenericOne
    let halfX = (mapSizeX - one) / two
    let halfY = (mapSizeY - one) / two
    let quadrants = [
        {xLower = zero;        yLower = zero;        xUpper = halfX - one;      yUpper = halfY - one}
        {xLower = halfX + one; yLower = zero;        xUpper = (mapSizeX - one); yUpper = halfY - one}
        {xLower = zero;        yLower = halfY + one; xUpper = halfX - one;      yUpper = (mapSizeY - one)}
        {xLower = halfX + one; yLower = halfY + one; xUpper = (mapSizeX - one); yUpper = (mapSizeY - one)}
    ]
    let robotsPerQuadrant =
        quadrants
        |> Seq.map (fun q ->
            robots
            |> Seq.where (fun r -> positionIsWithinSector q r.position)
            |> Seq.length
            )
    let safetyScore =
        robotsPerQuadrant
        |> Seq.fold (fun aggregated next -> aggregated * next) 1
    safetyScore

let part1ExampleMapMaxX = 11
let part1ExampleMapMaxY = 7
let part1ExampleRobotLines = seq { yield! System.IO.File.ReadLines "Input/Example.txt" }
let part1ExampleRobots : robot<int> seq =
    part1ExampleRobotLines
    |> Seq.map parseRobot
let moveForExample = move part1ExampleMapMaxX part1ExampleMapMaxY
let part1ExampleRobotsAfter100Moves =
    part1ExampleRobots
    |> Seq.map (fun r -> iterate moveForExample 100 r)

let alternativeSafetyScore = getSafetyScore part1ExampleMapMaxX part1ExampleMapMaxY part1ExampleRobotsAfter100Moves
printfn $"{DateTime.Now:o} Part 1 example Alternative safety score: '{alternativeSafetyScore}'"

let part1MapMaxX = 101
let part1MapMaxY = 103
let part1RobotLines = seq { yield! System.IO.File.ReadLines "Input/Input.txt" }
let part1Robots : robot<int> seq =
    part1RobotLines
    |> Seq.map parseRobot
let part1move = move part1MapMaxX part1MapMaxY
let part1RobotsAfter100Moves =
    part1Robots
    |> Seq.map (fun r -> iterate part1move 100 r)
let part1SafetyScore = getSafetyScore part1MapMaxX part1MapMaxY part1RobotsAfter100Moves
printfn $"{DateTime.Now:o} Part 1 example safety score: '{part1SafetyScore}'"
printfn $"{DateTime.Now:o} Part 1 done"

let robotPositionsAreInteresting (positions:position<int> Set) =
    let columns =
        positions
        |> Seq.groupBy _.py
    let interestingColumns =
        columns |> Seq.exists (fun (col, positions) ->
                positions
                |> Seq.sort
                |> Seq.windowed 24
                |> Seq.exists (fun window ->
                    window
                    |> Seq.pairwise
                    |> Seq.fold (fun aggregate (nextPair0, nextPair1) ->
                        aggregate && nextPair0.px + 1 = nextPair1.px
                        ) true

                    )
            )
    interestingColumns

let printRobots (mapSizeX: int) (mapSizeY: int) (robots: robot<int> seq) =
    let robotCoordinates =
        robots
        |> Seq.groupBy _.position
        |> Seq.map fst
        |> Set

    if robotPositionsAreInteresting robotCoordinates then
        for row = 0 to mapSizeX do
            for col = 0 to mapSizeY do
                if robotCoordinates.Contains({px = row; py = col}) then
                    printf "+"
                else
                    printf "#"
            printfn ""

let mutable iteration = 0
let part2moveAndPrint robots =
    printfn $"\n{DateTime.Now:o} Iteration {iteration}\n"
    iteration <- iteration + 1
    printRobots part1MapMaxX part1MapMaxY robots
    seq {
        for robot in robots do
            yield (move part1MapMaxX part1MapMaxY robot)
    }
part1Robots
    |> iterate part2moveAndPrint 10000
    |> ignore
