#load "../../.deps/PxlLocalDevShadow.fsx"
open PxlLocalDevShadow

open System
open Pxl
open Pxl.Ui

let numbers =
    [
        [
            "XXXX"
            "X XX"
            "XX X"
            "X  X"
            "XXXX"
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
            .xy(1, 5)
            .color(Colors.white.opacity(0.8))
        text
            .var4x5($"{now:dd}.{now:MM}.")
            .color(Colors.white)
            .xy(1, 13)
            .color(Colors.white.opacity(0.8))
    }

let initialLife = 3
type World = int array

let getInitialWorld day month hours minutes =
    let drawDigit (digit: string list) (world: World) x y =
        for r in 0..4 do
            let line = digit[r]
            for c in 0..3 do
                if line[c] = 'X' then
                    world[(r + y) * 24 + c + x] <- initialLife

    let world = Array.create (24*24) 0
    let h1 = hours / 10
    let h2 = hours % 10
    let m1 = minutes / 10
    let m2 = minutes % 10
    let d1 = day / 10
    let d2 = day % 10
    let month1 = month / 10
    let month2 = month % 10

    drawDigit numbers[h1] world 1 5
    drawDigit numbers[h2] world 6 5
    drawDigit numbers[m1] world 13 5
    drawDigit numbers[m2] world 18 5

    drawDigit numbers[d1] world 1 13
    drawDigit numbers[d2] world 6 13
    drawDigit numbers[month1] world 13 13
    drawDigit numbers[month2] world 18 13

    world[(6*24) + 11] <- initialLife
    world[(8*24) + 11] <- initialLife

    world[(17*24) + 11] <- initialLife
    world[(17*24) + 23] <- initialLife

    world


let getNext (world: World): World=
    let nextWorld = Array.create 576 0
    for i in 0..575 do
        let tl = if i > 24 && i % 24 > 0 then Some world[i - 25] else None
        let t = if i - 24 >= 0 then Some world[i - 24] else None
        let tr = if i - 23 >= 0 && i % 24 < 23 then Some world[i - 23] else None
        let l = if i - 1 >= 0 && i % 24 > 0 then Some world[i - 1] else None
        let r = if i % 24 < 23 then Some world[i + 1] else None
        let bl = if i + 23 < 576 && i % 24 > 0 then Some world[i + 23] else None
        let b = if i + 24 < 576 then Some world[i + 24] else None
        let br = if i + 25 < 576 && i % 24 < 23 then Some world[i + 25] else None

        let aliveNeighbours =
            [tl; t; tr; l; r; bl; b; br]
            |> List.choose id
            |> List.filter (fun l -> l > 0)
            |> List.length
        let delta =
            match world[i] > 0, aliveNeighbours with
            | true, 0 -> -3
            | true, 1 -> -2
            | true, 2 -> 0
            | true, 3 -> 0
            | true, 4 -> -2
            | true, 5 -> -3
            | true, 6 -> -3
            | true, _ -> -4
            | false, 3 -> initialLife
            | false, _ -> 0
        nextWorld[i] <- max 0 (world[i] + delta)
    nextWorld

/// Converts HSV to RGB.
/// h: Hue in degrees (0-360)
/// s: Saturation (0.0-1.0)
/// v: Value (0.0-1.0)
/// Returns a tuple (R, G, B) where each value is in the range 0-255.
let hsvToRgb (h: float) (s: float) (v: float) =
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

    let r = (r' + m) * 255.0 |> int
    let g = (g' + m) * 255.0 |> int
    let b = (b' + m) * 255.0 |> int

    r, g, b

let life =
    scene {
        let! ctx = getCtx()
        let! worldState = useState { getInitialWorld ctx.now.Day ctx.now.Month ctx.now.Hour ctx.now.Minute }

        let! minChanged = Trigger.valueChanged ctx.now.Minute
        let! frameTrigger = Trigger.valueChanged (ctx.now.Millisecond / 500)
        let world =
            if minChanged then
                let world = getInitialWorld ctx.now.Day ctx.now.Month ctx.now.Hour ctx.now.Minute
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
                    if world[(r * 24) + c] = 0 then
                        Color.rgb(50, 50, 50)
                    else
                        let r,g,b = hsvToRgb 200 0.8 ((0.1 * (float (world[(r * 24) + c]))) % 1.0)
                        Color.rgb(r, g, b)
                pxl.xy(c,r).stroke(color)
    }

let diffuser =
    scene {
        rect.xywh(0, 07, 24, 9).fill(Color.argb(80, 0, 0, 0)).useAntiAlias()
        rect.xywh(0, 08, 24, 7).fill(Color.argb(80, 0, 0, 0)).useAntiAlias()
    }

let all =
    scene {
        let! ctx = getCtx ()
        life
        //diffuser
        time ctx.now
    }

all |> Simulator.start

(*
Simulator.stop ()
*)