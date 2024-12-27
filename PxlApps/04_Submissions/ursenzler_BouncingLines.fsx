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

let time (now: DateTime) =
    scene {
        let! ctx = getCtx ()

        let timeText =
            text.var4x5($"{now.Hour}:{now:ss}").color(Colors.white)

        let textWidth = timeText.measure ()
        let marginLeft = (ctx.width - textWidth) / 2

        let marginTop =
            (ctx.height - (int timeText._data.fontSize) - 1)
            / 2

        timeText
            .xy(marginLeft, marginTop)
            .color(Colors.white)
    }

let diffuser =
    scene {
        rect
            .xywh(0, 7, 24, 9)
            .fill(Color.argb (80, 0, 0, 0))
            .useAntiAlias ()

        rect
            .xywh(0, 8, 24, 7)
            .fill(Color.argb (80, 0, 0, 0))
            .useAntiAlias ()
    }

let colorsScene =
    scene {
        for i in 0..24 do
            for j in 0..24 do
                let index = (float (i*25+j)) * 360.0 / (25.0*25.0)
                pxl.xy(j,i).stroke(hsvToRgb index 0.8 0.8 |> Color.rgb)
    }

let hand second =
    let degreesToRadians (degrees: float) =
        degrees * Math.PI / 180.0
    scene {
        match second with
        | 0 -> 11, 0
        | 1 -> 13, 0
        | 2 -> 14, 1
        | 3 -> 15, 1
        | 4 -> 16, 2
        | 5 -> 17, 2
        | 6 -> 18, 3
        | 7 -> 19, 3
        | 8 -> 20, 4
        | 9 -> 21, 5
        | 10 -> 21, 6
        | 11 -> 22, 7
        | 12 -> 22, 8
        | 13 -> 23, 9
        | 14 -> 23, 10
        | 15 -> 23, 11
        | 16 -> 23, 13
        | 17 -> 22, 14
        | 18 -> 22, 15
        | 19 -> 21, 16
        | 20 -> 21, 17
        | 21 -> 20, 18
        | 22 -> 19, 19
        | 23 -> 18, 19
        | 24 -> 17, 20
        | 25 -> 16, 21
        | 26 -> 15, 22
        | 27 -> 14, 22
        | 28 -> 13, 23
        | 29 -> 12, 23
        | 30 -> 11, 23
        | 31 -> 10, 23
        | 32 -> 9, 23
        | 33 -> 8, 22
        | 34 -> 7, 22
        | 35 -> 6, 21
        | 36 -> 5, 20
        | 37 -> 4, 19
        | 38 -> 3, 19
        | 39 -> 2, 18
        | 40 -> 1, 17
        | 41 -> 1, 16
        | 42 -> 0, 15
        | 43 -> 0, 14
        | 44 -> 0, 13
        | 45 -> 0, 11
        | 46 -> 0, 9
        | 47 -> 1, 8
        | 48 -> 1, 7
        | 49 -> 2, 6
        | 50 -> 2, 5
        | 51 -> 2, 4
        | 52 -> 3, 4
        | 53 -> 3, 3
        | 54 -> 4, 3
        | 55 -> 5, 2
        | 56 -> 6, 2
        | 57 -> 7, 1
        | 58 -> 8, 1
        | 59 -> 9, 0
        | _ -> 11, 11
        |> fun (x, y) ->
            pxl.xy(x, y)
                .stroke(Colors.black)
                .strokeThickness(3)
                .useAntiAlias()//Color.argb(1.0, 0.0, 0.0, 0.0))

        // let length = 11.0
        // let angle = float ((second - 15.0) % 60.0) * 6.0 |> degreesToRadians
        // let dx = (cos angle) * length
        // let dy = (sin angle) * length
        //
        // pxl
        //     .xy(int (round(11.5 + dx)), int (round (11.5 + dy)))
        //     .stroke(Colors.black)//Color.argb(1.0, 0.0, 0.0, 0.0))
        //     .strokeThickness(1)
        //     .noAntiAlias()

    }

let all =
    scene {
        let! ctx = getCtx ()
        colorsScene
        //for i in 0..59 do
        hand ctx.now.Second
        time ctx.now
    }

all |> Simulator.start
(*
Simulator.stop ()
*)

