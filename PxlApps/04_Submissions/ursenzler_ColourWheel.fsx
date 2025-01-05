#load "../../.deps/PxlLocalDevShadow.fsx"
open PxlLocalDevShadow

open System
open Pxl
open Pxl.Ui

(*
    This clock needs 6 minutes to walk through the whole HSV colour space once.
    Every second, the hue value of the second hand is increased by one.
    During every minute the visualized seconds are dimmed.
*)

// Converts HSV to RGB.
/// h: Hue in degrees (0-360)
/// s: Saturation (0.0-1.0)
/// v: Value (0.0-1.0)
/// Returns a tuple (R, G, B) where each value is in the range 0-255.
let hsva (h: float) (s: float) (v: float) a =
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

    let a = a * 255.0 |> int
    let r = (r' + m) * 255.0 |> int
    let g = (g' + m) * 255.0 |> int
    let b = (b' + m) * 255.0 |> int

    Color.argb(a, r, g, b)

let time hour minute =
    scene {
        text.mono4x5($"%02d{hour}").color(hsva 200.0 0.0 1.0 1.0).xy(6, 6)
        text.mono4x5($"%02d{minute}").color(hsva 200.0 0.0 1.0 1.0).xy(9, 13)
    }

let seconds minute second =
    let getColor (s: int) step =
        hsva
            (float ((minute * 60) + s))
            (1.0 - ((float step) * 0.15))
            (1.0 - float (second - s) / 100.0)
            1
    scene {
        for s in 0..(min 6 second) do
            pxl.xy(12 + s, 4).stroke(getColor s 0).noAntiAlias()
            pxl.xy(12 + s, 3).stroke(getColor s 1).noAntiAlias()
            pxl.xy(12 + s, 2).stroke(getColor s 2).noAntiAlias()
            pxl.xy(12 + s, 1).stroke(getColor s 3).noAntiAlias()
            pxl.xy(12 + s, 0).stroke(getColor s 4).noAntiAlias()
        for s in 7..(min 7 second) do
            pxl.xy(19, 4).stroke(getColor s 0).noAntiAlias()
            pxl.xy(20, 3).stroke(getColor s 1).noAntiAlias()
            pxl.xy(21, 2).stroke(getColor s 2).noAntiAlias()
            pxl.xy(22, 1).stroke(getColor s 3).noAntiAlias()
            pxl.xy(23, 0).stroke(getColor s 4).noAntiAlias()

            pxl.xy(19, 3).stroke(getColor s 1).noAntiAlias()
            pxl.xy(19, 2).stroke(getColor s 2).noAntiAlias()
            pxl.xy(19, 1).stroke(getColor s 3).noAntiAlias()
            pxl.xy(19, 0).stroke(getColor s 4).noAntiAlias()

            pxl.xy(20, 2).stroke(getColor s 2).noAntiAlias()
            pxl.xy(20, 1).stroke(getColor s 3).noAntiAlias()
            pxl.xy(20, 0).stroke(getColor s 4).noAntiAlias()

            pxl.xy(21, 1).stroke(getColor s 3).noAntiAlias()
            pxl.xy(21, 0).stroke(getColor s 4).noAntiAlias()

            pxl.xy(22, 0).stroke(getColor s 4).noAntiAlias()
        for s in 8..(min 8 second) do
            pxl.xy(20, 4).stroke(getColor s 1).noAntiAlias()
            pxl.xy(21, 4).stroke(getColor s 2).noAntiAlias()
            pxl.xy(22, 4).stroke(getColor s 3).noAntiAlias()
            pxl.xy(23, 4).stroke(getColor s 4).noAntiAlias()

            pxl.xy(21, 3).stroke(getColor s 2).noAntiAlias()
            pxl.xy(22, 3).stroke(getColor s 3).noAntiAlias()
            pxl.xy(23, 3).stroke(getColor s 4).noAntiAlias()

            pxl.xy(22, 2).stroke(getColor s 3).noAntiAlias()
            pxl.xy(23, 2).stroke(getColor s 4).noAntiAlias()

            pxl.xy(23, 1).stroke(getColor s 4).noAntiAlias()

        for s in 8..(min 21 second) do
            pxl.xy(19, s - 3).stroke(getColor s 0).noAntiAlias()
            pxl.xy(20, s - 3).stroke(getColor s 1).noAntiAlias()
            pxl.xy(21, s - 3).stroke(getColor s 2).noAntiAlias()
            pxl.xy(22, s - 3).stroke(getColor s 3).noAntiAlias()
            pxl.xy(23, s - 3).stroke(getColor s 4).noAntiAlias()
        for s in 22..(min 22 second) do
            pxl.xy(19, 19).stroke(getColor s 0).noAntiAlias()
            pxl.xy(20, 20).stroke(getColor s 1).noAntiAlias()
            pxl.xy(21, 21).stroke(getColor s 2).noAntiAlias()
            pxl.xy(22, 22).stroke(getColor s 3).noAntiAlias()
            pxl.xy(23, 23).stroke(getColor s 4).noAntiAlias()

            pxl.xy(20, 19).stroke(getColor s 1).noAntiAlias()
            pxl.xy(21, 19).stroke(getColor s 2).noAntiAlias()
            pxl.xy(22, 19).stroke(getColor s 3).noAntiAlias()
            pxl.xy(23, 19).stroke(getColor s 4).noAntiAlias()

            pxl.xy(21, 20).stroke(getColor s 2).noAntiAlias()
            pxl.xy(22, 20).stroke(getColor s 3).noAntiAlias()
            pxl.xy(23, 20).stroke(getColor s 4).noAntiAlias()

            pxl.xy(22, 21).stroke(getColor s 3).noAntiAlias()
            pxl.xy(23, 21).stroke(getColor s 4).noAntiAlias()

            pxl.xy(23, 22).stroke(getColor s 4).noAntiAlias()
        for s in 23..(min 23 second) do
            pxl.xy(19, 20).stroke(getColor s 1).noAntiAlias()
            pxl.xy(19, 21).stroke(getColor s 2).noAntiAlias()
            pxl.xy(19, 22).stroke(getColor s 3).noAntiAlias()
            pxl.xy(19, 23).stroke(getColor s 4).noAntiAlias()

            pxl.xy(20, 21).stroke(getColor s 2).noAntiAlias()
            pxl.xy(20, 22).stroke(getColor s 3).noAntiAlias()
            pxl.xy(20, 23).stroke(getColor s 4).noAntiAlias()

            pxl.xy(21, 22).stroke(getColor s 3).noAntiAlias()
            pxl.xy(21, 23).stroke(getColor s 4).noAntiAlias()

            pxl.xy(22, 23).stroke(getColor s 4).noAntiAlias()
        for s in 23..(min 36 second) do
            pxl.xy(41 - s, 19).stroke(getColor s 0).noAntiAlias()
            pxl.xy(41 - s, 20).stroke(getColor s 1).noAntiAlias()
            pxl.xy(41 - s, 21).stroke(getColor s 2).noAntiAlias()
            pxl.xy(41 - s, 22).stroke(getColor s 3).noAntiAlias()
            pxl.xy(41 - s, 23).stroke(getColor s 4).noAntiAlias()
        for s in 37..(min 37 second) do
            pxl.xy(4, 19).stroke(getColor s 0).noAntiAlias()
            pxl.xy(3, 20).stroke(getColor s 1).noAntiAlias()
            pxl.xy(2, 21).stroke(getColor s 2).noAntiAlias()
            pxl.xy(1, 22).stroke(getColor s 3).noAntiAlias()
            pxl.xy(0, 23).stroke(getColor s 4).noAntiAlias()

            pxl.xy(4, 20).stroke(getColor s 1).noAntiAlias()
            pxl.xy(4, 21).stroke(getColor s 2).noAntiAlias()
            pxl.xy(4, 22).stroke(getColor s 3).noAntiAlias()
            pxl.xy(4, 23).stroke(getColor s 4).noAntiAlias()

            pxl.xy(3, 21).stroke(getColor s 2).noAntiAlias()
            pxl.xy(3, 22).stroke(getColor s 3).noAntiAlias()
            pxl.xy(3, 23).stroke(getColor s 4).noAntiAlias()

            pxl.xy(2, 22).stroke(getColor s 3).noAntiAlias()
            pxl.xy(2, 23).stroke(getColor s 4).noAntiAlias()

            pxl.xy(1, 23).stroke(getColor s 4).noAntiAlias()
        for s in 38..(min 38 second) do
            pxl.xy(3, 19).stroke(getColor s 1).noAntiAlias()
            pxl.xy(2, 19).stroke(getColor s 2).noAntiAlias()
            pxl.xy(1, 19).stroke(getColor s 3).noAntiAlias()
            pxl.xy(0, 19).stroke(getColor s 4).noAntiAlias()

            pxl.xy(2, 20).stroke(getColor s 2).noAntiAlias()
            pxl.xy(1, 20).stroke(getColor s 3).noAntiAlias()
            pxl.xy(0, 20).stroke(getColor s 4).noAntiAlias()

            pxl.xy(1, 21).stroke(getColor s 3).noAntiAlias()
            pxl.xy(0, 21).stroke(getColor s 4).noAntiAlias()

            pxl.xy(0, 22).stroke(getColor s 4).noAntiAlias()
        for s in 38..(min 51 second) do
            pxl.xy(4, 56 - s).stroke(getColor s 0).noAntiAlias()
            pxl.xy(3, 56 - s).stroke(getColor s 1).noAntiAlias()
            pxl.xy(2, 56 - s).stroke(getColor s 2).noAntiAlias()
            pxl.xy(1, 56 - s).stroke(getColor s 3).noAntiAlias()
            pxl.xy(0, 56 - s).stroke(getColor s 4).noAntiAlias()
        for s in 52..(min 52 second) do
            pxl.xy(4, 4).stroke(getColor s 0).noAntiAlias()
            pxl.xy(3, 3).stroke(getColor s 1).noAntiAlias()
            pxl.xy(2, 2).stroke(getColor s 2).noAntiAlias()
            pxl.xy(1, 1).stroke(getColor s 3).noAntiAlias()
            pxl.xy(0, 0).stroke(getColor s 4).noAntiAlias()

            pxl.xy(3, 4).stroke(getColor s 1).noAntiAlias()
            pxl.xy(2, 4).stroke(getColor s 2).noAntiAlias()
            pxl.xy(1, 4).stroke(getColor s 3).noAntiAlias()
            pxl.xy(0, 4).stroke(getColor s 4).noAntiAlias()

            pxl.xy(2, 3).stroke(getColor s 2).noAntiAlias()
            pxl.xy(1, 3).stroke(getColor s 3).noAntiAlias()
            pxl.xy(0, 3).stroke(getColor s 4).noAntiAlias()

            pxl.xy(1, 2).stroke(getColor s 3).noAntiAlias()
            pxl.xy(0, 2).stroke(getColor s 4).noAntiAlias()

            pxl.xy(0, 1).stroke(getColor s 4).noAntiAlias()
        for s in 53..(min 53 second) do
            pxl.xy(4, 3).stroke(getColor s 1).noAntiAlias()
            pxl.xy(4, 2).stroke(getColor s 2).noAntiAlias()
            pxl.xy(4, 1).stroke(getColor s 3).noAntiAlias()
            pxl.xy(4, 0).stroke(getColor s 4).noAntiAlias()

            pxl.xy(3, 2).stroke(getColor s 2).noAntiAlias()
            pxl.xy(3, 1).stroke(getColor s 3).noAntiAlias()
            pxl.xy(3, 0).stroke(getColor s 4).noAntiAlias()

            pxl.xy(2, 1).stroke(getColor s 3).noAntiAlias()
            pxl.xy(2, 0).stroke(getColor s 4).noAntiAlias()

            pxl.xy(1, 0).stroke(getColor s 4).noAntiAlias()
        for s in 53..(min 59 second) do
            pxl.xy(s - 48, 4).stroke(getColor s 0).noAntiAlias()
            pxl.xy(s - 48, 3).stroke(getColor s 1).noAntiAlias()
            pxl.xy(s - 48, 2).stroke(getColor s 2).noAntiAlias()
            pxl.xy(s - 48, 1).stroke(getColor s 3).noAntiAlias()
            pxl.xy(s - 48, 0).stroke(getColor s 4).noAntiAlias()
    }

let all =
    scene {
        bg.color(hsva 195.0 0.9 0.2 1.0)
        let! ctx = getCtx ()
        seconds ctx.now.Minute ctx.now.Second
        time ctx.now.Hour ctx.now.Minute
    }

all |> Simulator.start

(*
Simulator.stop ()
*)