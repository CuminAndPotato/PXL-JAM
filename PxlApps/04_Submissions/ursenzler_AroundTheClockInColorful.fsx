#load "../../.deps/PxlLocalDevShadow.fsx"
open PxlLocalDevShadow

open System
open Pxl
open Pxl.Ui

(*

The 3 lines indicate seconds (0-59), minutes(0-59) and hours(0-23).

Idea and Design: Nico und Urs Enzler
Programming: Nico und Urs Enzler
Color optimizations: Nico Enzler

*)

/// Converts HSV to RGB.
/// h: Hue in degrees (0-360)
/// s: Saturation (0.0-1.0)
/// v: Value (0.0-1.0)
/// Returns a tuple (R, G, B) where each value is in the range 0-255.
let hsvToRgb (h: float) (s: float) (v: float) =
    let h = h % 360.0
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
        let! ctx = getCtx ()
        let timeText =
            text.var4x5($"%d{hour}:%02d{minute}").color (hsvToRgb 200.0 1.0 1.0)

        let textWidth = timeText.measure ()
        let marginLeft = (ctx.width - textWidth) / 2
        let marginTop =
            (ctx.height - (int timeText._data.fontSize) - 1)
            / 2

        timeText
            .xy(marginLeft, marginTop)
            .color Colors.white

    }

let seconds second =
    let getColor s =
        let delta = second - s |> float
        let v = 0.7 * (59.0 - delta) / 60.0 + 0.3
        hsvToRgb (140.0 + (5.0 * float s)) 1.0 v
    scene {
        let seconds = second - 1
        for s in 0..(min 21 seconds) do
            pxl.xy(s + 1, 1).stroke(getColor s)
        for s in 22..(min 25 seconds) do
            pxl.xy(22, s - 20).stroke(getColor s)
        for s in 26..(min 29 seconds) do
            pxl.xy(22, s - 9).stroke(getColor s)
        for s in 30..(min 51 seconds) do
            pxl.xy( 52 - s, 21).stroke(getColor s)
        for s in 52..(min 55 seconds) do
            pxl.xy(1, 72 - s).stroke(getColor s)
        for s in 56..seconds do
            pxl.xy(1, 61 - s).stroke(getColor s)
    }

let minutes minutes =
    let getColor s =
        let delta = minutes - s |> float
        let v = 0.7 * (59.0 - delta) / 60.0 + 0.3
        hsvToRgb (240.0 + (5.0 * float s)) 1.0 v
    scene {
        let minutes = minutes - 1
        for s in 0..(min 19 minutes) do
            pxl.xy(s + 2, 2).stroke(getColor s)
        for s in 20..(min 24 minutes) do
            pxl.xy(21, s - 17).stroke(getColor s)
        for s in 25..(min 29 minutes) do
            pxl.xy(21, s - 10).stroke(getColor s)
        for s in 30..(min 49 minutes) do
            pxl.xy( 51 - s, 20).stroke(getColor s)
        for s in 50..(min 54 minutes) do
            pxl.xy(2, 69 - s).stroke(getColor s)
        for s in 55..minutes do
            pxl.xy(2, 62 - s).stroke(getColor s)
    }

let hours hour =
    let getColor h =
        let delta = hour - h |> float
        let v = 0.8 * (59.0 - delta) / 60.0 + 0.2
        hsvToRgb (40.0 + (15.0 * float h)) 1.0 v
    scene {
        let hour = hour - 1
        for s in 0..(min 9 hour) do
            pxl.xy(s + 7, 6).stroke(getColor s)
        for s in 10..(min 10 hour) do
            pxl.xy(16, s - 3).stroke(getColor s)
        for s in 11..(min 11 hour) do
            pxl.xy(16, s + 4).stroke(getColor s)
        for s in 12..(min 21 hour) do
            pxl.xy( 28 - s, 16).stroke(getColor s)
        for s in 22..(min 22 hour) do
            pxl.xy(7, 37 - s).stroke(getColor s)
        for s in 23..hour do
            pxl.xy(7, 30 - s).stroke(getColor s)
    }

let all =
    scene {
        let! ctx = getCtx ()
        time ctx.now.Hour ctx.now.Minute
        seconds ctx.now.Second
        minutes ctx.now.Minute
        hours ctx.now.Hour
    }

all |> Simulator.start

(*
Simulator.stop ()
*)