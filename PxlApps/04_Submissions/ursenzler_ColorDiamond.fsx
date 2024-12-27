#load "../../.deps/PxlLocalDevShadow.fsx"
open PxlLocalDevShadow

open System
open Pxl
open Pxl.Ui

(*

Idea and Design: Nico Enzler
Programming: Nico und Urs Enzler
Color optimizations: Nico Enzler

*)

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

let time hour minute =
    scene {
        let! ctx = getCtx()

        let timeText = text.var4x5($"%02d{hour}:%02d{minute}").color(Colors.white)
        let textWidth = timeText.measure()
        let marginLeft = (ctx.width - textWidth) / 2
        let marginTop = (ctx.height - (int timeText._data.fontSize) - 1) / 2
        timeText
            .xy(marginLeft, marginTop)
            .color (Colors.white)
    }

let lines hour minute second =
    scene {
        let saturation =
            if hour <= 15 then
                0.3 + (0.7 * float hour / 15.0)
            else
                0.3 + (0.7 * (23.0 - (float hour)) / 9.0)
        let value =
            if hour <= 15 then
                0.5 + (0.5 * float hour / 15.0)
            else
                0.5 + (0.5 * (23.0 - (float hour)) / 9.0)
        let seed = hour * minute * 20 |> float
        for s in 0..(min second 11) do
            rect.xywh(s, s, 23 - (2*s), 23 - (2*s)).stroke(hsv (seed + float s * 10.0) saturation value).strokeThickness(1).noAntiAlias()
        for s in 12..(min second 23) do
            rect.xywh(s, s, 23 - (2*s), 23 - (2*s)).stroke(hsv (seed + float s * 10.0) saturation value).strokeThickness(1).noAntiAlias()
        for s in 24..(min second 35) do
            let s = s % 24
            rect.xywh(s, s, 23 - (2*s), 23 - (2*s)).stroke(hsv (seed + (float s + 24.0) * 10.0) saturation value).strokeThickness(1).noAntiAlias()
        for s in 36..(min second 47) do
            let s = s % 24
            rect.xywh(s, s, 23 - (2*s), 23 - (2*s)).stroke(hsv (seed + (float s + 24.0) * 10.0) saturation value).strokeThickness(1).noAntiAlias()
        for s in 48..(min second 59) do
            let s = s % 24
            rect.xywh(s, s, 23 - (2*s), 23 - (2*s)).stroke(hsv (seed + (float s + 48.0) * 10.0) saturation value).strokeThickness(1).noAntiAlias()
    }

let hand second =
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
                .useAntiAlias()
    }

let all =
    scene {
        let! ctx = getCtx ()
        lines ctx.now.Hour ctx.now.Minute ctx.now.Second
        hand ctx.now.Second
        time ctx.now.Hour ctx.now.Minute
    }

all |> Simulator.start

(*
Simulator.stop ()
*)