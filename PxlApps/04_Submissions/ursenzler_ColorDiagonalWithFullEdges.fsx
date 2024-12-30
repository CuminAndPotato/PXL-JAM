#load "../../.deps/PxlLocalDevShadow.fsx"
open PxlLocalDevShadow

open System
open Pxl
open Pxl.Ui

(*

Idea and Design: Nora Enzler + Urs Enzler
Programming: Nora und Urs Enzler
Color optimizations: Nora Enzler

*)

let time (now: DateTime) =
    scene {
        let! ctx = getCtx()

        let timeText = text.var4x5($"{now:HH}:{now:mm}").color(Colors.white)
        let textWidth = timeText.measure()
        let marginLeft = (ctx.width - textWidth) / 2
        let marginTop = (ctx.height - (int timeText._data.fontSize) - 1) / 2
        timeText
            .xy(marginLeft, marginTop)
            .color (Colors.white)
    }

/// Converts HSV to RGB.
/// h: Hue in degrees (0-360)
/// s: Saturation (0.0-1.0)
/// v: Value (0.0-1.0)
/// Returns a tuple (R, G, B) where each value is in the range 0-255.
let hsv (h: float) (s: float) (v: float) =
    let h = h % 360.0
    let s = s
    let v = v
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

    (r, g, b) |> Color.rgb

let colors =
    [
        for i in 0..29 do
            hsv (float i * 12.0) 0.7 0.9
    ]

let lines1 =
    [
        for i in 15..-1..0 do
            i, 0, 24, 24-i
        for i in 1..14 do
            0, i, 24-i, 24
    ]

let lines2 =
    [
        for i in 9..24 do
            i, 0, 0, i
        for i in 1..14 do
            i, 24, 24, i
    ]

let cornersTopLeft =
    [
        for i in 0..8 do
            i, 0, 0, i
    ]
let cornersTopRight =
    [
        for i in 0..8 do
            (24-i), 0, 24, i
    ]
let cornersBottomLeft =
    [
        for i in 0..9 do
            0, (24-i), i, 24
    ]
let cornersBottomRight =
    [
        for i in 0..9 do
            (24-i), 24, 24, (24-i)
    ]

let linesScene minutes (seconds: int) =
    let lMin = if seconds <= 30 then 0 else seconds - 30
    let lMax = min seconds 29
    let lines = if minutes % 2 = 0 then lines1 else lines2
    let corners1 =
        match minutes % 2, seconds with
        | 0, s when s < 30 -> cornersTopRight
        | 0, _ -> []
        | 1, s when s < 30 -> cornersTopLeft
        | _ -> []
    let corners2 =
        match minutes % 2, seconds with
        | 0, s when s >= 30 -> cornersBottomLeft
        | 0, _ -> []
        | 1, s when s >= 30 -> cornersBottomRight
        | _ -> []
    scene {
        for l in lMin..lMax do
            let color = colors[l % colors.Length]
            let x1, y1, x2, y2 = lines[l%lines.Length]
            line.p1p2(x1, y1, x2, y2).stroke(color).noAntiAlias()

        for i, l in corners1 |> List.mapi (fun i l -> i, l) do
            let color = hsv 0.0 0.7 (0.7 - float (8 - i) * 0.1)
            let x1, y1, x2, y2 = l
            line.p1p2(x1, y1, x2, y2).stroke(color).noAntiAlias()

        for i, l in corners2 |> List.mapi (fun i l -> i, l) do
            let color = hsv 0.0 0.7 (0.7 - float (8 - i) * 0.1)
            let x1, y1, x2, y2 = l
            line.p1p2(x1, y1, x2, y2).stroke(color).noAntiAlias()
    }

let diffuser =
    scene {
        rect.xywh(0, 07, 24, 9).fill(Color.argb(80, 0, 0, 0)).useAntiAlias()
        rect.xywh(0, 08, 24, 7).fill(Color.argb(80, 0, 0, 0)).useAntiAlias()
    }

let all =
    scene {
        let! ctx = getCtx ()
        linesScene ctx.now.Minute ctx.now.Second
        diffuser
        time ctx.now
    }

all |> Simulator.start

(*
Simulator.stop ()
*)