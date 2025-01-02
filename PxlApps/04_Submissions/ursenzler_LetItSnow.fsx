#load "../../.deps/PxlLocalDevShadow.fsx"
open PxlLocalDevShadow

open System
open Pxl
open Pxl.Ui

/// Converts HSV to RGB.
/// h: Hue in degrees (0-360)
/// s: Saturation (0.0-1.0)
/// v: Value (0.0-1.0)
/// Returns a tuple (R, G, B) where each value is in the range 0-255.
let hsva (h: float) (s: float) (v: float) a =
    let c = v * s
    let x = c * (1.0 - abs ((h / 60.0) % 2.0 - 1.0))
    let m = v - c

    let r', g', b' =
        if h < 60.0 then c, x, 0.0
        elif h < 120.0 then x, c, 0.0
        elif h < 180.0 then 0.0, c, x
        elif h < 240.0 then 0.0, x, c
        elif h < 300.0 then x, 0.0, c
        else c, 0.0, x

    let a = a * 255.0 |> int
    let r = (r' + m) * 255.0 |> int
    let g = (g' + m) * 255.0 |> int
    let b = (b' + m) * 255.0 |> int

    Color.argb(a, r, g, b)

let numbers =
    [
        [
            " XX "
            "X XX"
            "XX X"
            "X  X"
            " XX "
        ]
        [
            "  X "
            " XX "
            "  X "
            "  X "
            "  X "
        ]
        [
            " XX "
            "X  X"
            "  X "
            " X  "
            "XXXX"
        ]
        [
            "XXX "
            "   X"
            " XX "
            "   X"
            "XXX "
        ]
        [
            "X  X"
            "X  X"
            "XXXX"
            "   X"
            "   X"
        ]
        [
            "XXXX"
            "X   "
            "XXX "
            "   X"
            "XXX "
        ]
        [
            " XX "
            "X   "
            "XXX "
            "X  X"
            " XX "
        ]
        [
            "XXXX"
            "   X"
            "  X "
            "  X "
            "  X "
        ]
        [
            " XX "
            "X  X"
            " XX "
            "X  X"
            " XX "
        ]
        [
            " XX "
            "X  X"
            " XXX"
            "   X"
            " XX "
        ]
    ]

let time (now: DateTime) =
    scene {
        text
            .var4x5($"{now:HH}:{now:mm}")
            .color(Colors.white)
            .xy(1, 19)
            .color(hsva 220 0.7 1 1)
    }

type Snowflake = Falling | Lying | Empty
type World = Snowflake array

let getInitialWorld hours minutes =
    let drawDigit (digit: string list) (world: World) x y =
        for r in 0..4 do
            let line = digit[r]
            for c in 0..3 do
                if line[c] = 'X' then
                    world[(r + y) * 24 + c + x] <- Snowflake.Lying

    let world = Array.create (24*24) Snowflake.Empty
    let h1 = hours / 10
    let h2 = hours % 10
    let m1 = minutes / 10
    let m2 = minutes % 10

    drawDigit numbers[h1] world 1 19
    drawDigit numbers[h2] world 6 19
    drawDigit numbers[m1] world 13 19
    drawDigit numbers[m2] world 18 19

    world[(20*24) + 11] <- Snowflake.Lying
    world[(22*24) + 11] <- Snowflake.Lying

    world

let getNext (world: World): World=
    let nextWorld = Array.create 576 Snowflake.Empty
    let mutable lastSnowflakeCreated = -2
    for i in 0..575 do
        let above = if i >= 24 then Some world[i - 24] else None
        let below = if i + 24 < 576 then Some world[i + 24] else None
        let current = world[i]

        let state =
            match above, current, below with
            | None, Empty, _ ->
                if Random.Shared.Next(10) > 8 && i > lastSnowflakeCreated + 1 then
                    lastSnowflakeCreated <- i
                    Snowflake.Falling else Snowflake.Empty
            | None, Falling, Some Empty -> Empty
            | None, Falling, Some Lying -> Lying
            | _, Lying, _ -> Lying
            | Some Falling, Empty, Some Empty -> Falling
            | _, Falling, Some Lying -> Lying
            | Some Empty, Falling, Some Empty -> Empty
            | Some Empty, Falling, Some Falling -> Empty
            | Some Empty, Empty, _ -> Empty
            | Some Lying, c, _ -> c
            | Some Falling, Empty, Some Lying -> Falling
            | _, Falling, None -> Lying
            | Some Falling, Empty, None -> Falling
            | _ -> Empty
        nextWorld[i] <- state
    nextWorld

let snowflake = hsva 0 0 1 1
let empty = hsva 200 0.8 0.1 1.0
let getSnowColor snowHeight = hsva 220 (0.1 + float snowHeight * 0.025) 1.0 1.0

let getSnowHeight (world: World) c r =
    [0..r]
    |> List.sumBy (fun row ->
        match world[row * 24 + c] with
        | Empty
        | Falling -> 0
        | Lying -> 1)

let snowing =
    scene {
        let! ctx = getCtx()
        let! worldState = useState { getInitialWorld ctx.now.Hour ctx.now.Minute }

        let! minChanged = Trigger.valueChanged ctx.now.Minute
        let! frameTrigger = Trigger.valueChanged (ctx.now.Millisecond / 500)
        let world =
            if minChanged then
                let world = getInitialWorld ctx.now.Hour ctx.now.Minute
                do worldState.value <- world
                world
            else if frameTrigger then
                let world = worldState.value
                let nextWorld = getNext world
                do worldState.value <- nextWorld
                nextWorld
            else
                worldState.value
        for r in 0..23 do
            for c in 0..23 do
                let color =
                    match world[(r * 24) + c] with
                    | Snowflake.Empty -> empty
                    | Snowflake.Falling -> snowflake
                    | Snowflake.Lying ->
                        let height = getSnowHeight world c r
                        getSnowColor height
                pxl.xy(c,r).stroke(color).noAntiAlias()
    }

let all =
    scene {
        let! ctx = getCtx ()
        snowing
        time ctx.now
    }

all |> Simulator.start

(*
Simulator.stop ()
*)