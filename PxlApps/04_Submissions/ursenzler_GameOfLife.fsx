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
            " XXX"
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
        let! ctx = getCtx()

        let timeText = text.var4x5($"{now:HH}:{now:mm}").color(Colors.white)
        let textWidth = timeText.measure()
        let marginLeft = (ctx.width - textWidth) / 2
        let marginTop = (ctx.height - (int timeText._data.fontSize) - 1) / 2
        timeText
            .xy(marginLeft, marginTop)
            .color(Colors.white.opacity(0.5))
    }

type World = bool array

let getInitialWorld hours minutes =
    let drawDigit (digit: string list) (world: World) x y =
        for r in 0..4 do
            let line = digit[r]
            for c in 0..3 do
                if line[c] = 'X' then
                    world[(r + y) * 24 + c + x] <- true

    let world = Array.create (24*24) false
    let h1 = (hours / 10)
    let h2 = (hours % 10)
    let m1 = (minutes / 10)
    let m2 = (minutes % 10)
    let d1 = numbers[h1]
    let d2 = numbers[h2]
    let d3 = numbers[m1]
    let d4 = numbers[m2]

    drawDigit d1 world 1 9
    drawDigit d2 world 6 9
    drawDigit d3 world 13 9
    drawDigit d4 world 18 9

    world[(11*24) +  11] <- true
    world[(13*24) + 11] <- true

    world


let getNext (world: World): World=
    let nextWorld = Array.create 576 false
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
            |> List.filter id
            |> List.length
        let alive =
            match world[i], aliveNeighbours with
            | true, 0
            | true, 1 -> false
            | true, 2
            | true, 3 -> true
            | true, _ -> false
            | false, 3 -> true
            | false, _ -> false
        nextWorld[i] <- alive
    nextWorld

let life =
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
                    if world[(r * 24) + c] then Color.rgb(100, 00, 00) else Color.rgb(50, 50, 50)
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