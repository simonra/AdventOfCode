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

// Part 1 example dimensions: x = 11; y = 7
let part1ExampleMapMaxX = 11
let part1ExampleMapMaxY = 7
let part1exampleSectors = [
    {xLower = 0; yLower = 0; xUpper = 4; yUpper = 2}
    {xLower = 6; yLower = 0; xUpper = 10; yUpper = 2}
    {xLower = 0; yLower = 4; xUpper = 4; yUpper = 6}
    {xLower = 6; yLower = 4; xUpper = 10; yUpper = 6}
]

let part1ExampleRobotLines = seq { yield! System.IO.File.ReadLines "Input/Example.txt" }
let part1ExampleRobots : robot<int> seq =
    part1ExampleRobotLines
    |> Seq.map parseRobot
let moveForExample = move part1ExampleMapMaxX part1ExampleMapMaxY
let part1ExampleRobotsAfter100Moves =
    part1ExampleRobots
    |> Seq.map (fun r -> iterate moveForExample 100 r)
    // |> Seq.toArray
let part1ExampleRobotsInFirstQuadrant =
    part1ExampleRobotsAfter100Moves
    |> Seq.where (fun r -> positionIsWithinSector part1exampleSectors[0] r.position)
    |> Seq.length
let part1ExampleRobotsInSecondQuadrant =
    part1ExampleRobotsAfter100Moves
    |> Seq.where (fun r -> positionIsWithinSector part1exampleSectors[1] r.position)
    |> Seq.length
let part1ExampleRobotsInThirdQuadrant =
    part1ExampleRobotsAfter100Moves
    |> Seq.where (fun r -> positionIsWithinSector part1exampleSectors[2] r.position)
    |> Seq.length
let part1ExampleRobotsInFourthQuadrant =
    part1ExampleRobotsAfter100Moves
    |> Seq.where (fun r -> positionIsWithinSector part1exampleSectors[3] r.position)
    |> Seq.length

let part1ExampleSafetyScore = part1ExampleRobotsInFirstQuadrant * part1ExampleRobotsInSecondQuadrant * part1ExampleRobotsInThirdQuadrant * part1ExampleRobotsInFourthQuadrant
printfn $"{DateTime.Now:o} Part 1 safety score components: %A{[part1ExampleRobotsInFirstQuadrant; part1ExampleRobotsInSecondQuadrant; part1ExampleRobotsInThirdQuadrant; part1ExampleRobotsInFourthQuadrant]}"
printfn $"{DateTime.Now:o} Part 1 example SafetyScore: '{part1ExampleSafetyScore}'"
printfn $"{DateTime.Now:o} Done with part 1 example"

// // Part 1 actual dimensions: x = 101; y = 103
let part1sectors = [
    {xLower = 0; yLower = 0; xUpper = 49; yUpper = 50}
    {xLower = 51; yLower = 0; xUpper = 100; yUpper = 50}
    {xLower = 0; yLower = 52; xUpper = 49; yUpper = 102}
    {xLower = 51; yLower = 52; xUpper = 100; yUpper = 102}
]

// let parsedRobots =
