#load "../../.deps/PxlLocalDevShadow.fsx"
open PxlLocalDevShadow

open System
open Pxl
open Pxl.Ui

(*

Idea and Design: Urs Enzler
Programming: Urs Enzler
Color optimizations: Urs Enzler

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
            .color Colors.white
    }

let interpolate start ``end`` step steps =
    start + ((``end`` - start) * ((float step) / (float steps)))

let lines hour  =
    scene {
        let hTop, hBottom, saturationTop, saturationBottom, valueTop, valueBottom =
            match hour with
            | h when h < 7 -> 200.0, 200.0, 0.3, 0.5, 0.0, 0.4
            | h when h < 8 -> 216.0, 375.0, 0.42, 0.17, 0.76, 0.83
            | h when h < 9 -> 216.0, 213.0, 0.22, 0.27, 0.76, 0.83
            | h when h < 18 -> 216.0, 200.0, 0.51, 0.0, 0.78, 0.85
            | h when h < 22 -> 216.0, 375.0, 0.42, 0.17, (interpolate 0.78 0.0 (h-18) 4), (interpolate 0.83 0.4 (h-18) 4)
            | _ -> 200.0, 200.0, 0.3, 0.5, 0.0, 0.4

        for l in 0..19 do
            let step = l + 1
            line.p1p2(0, l, 24,l).stroke(
                hsv
                    (interpolate hTop hBottom step 20)
                    (interpolate saturationTop saturationBottom step 20)
                    (interpolate valueTop valueBottom step 20)
            ).noAntiAlias()
    }



let mythen =
    image(PxlLocalDev.loadAsset "submissions\ursenzler_Mythen.png", 0, 0)

let all =
    scene {
        let! ctx = getCtx ()
        lines ctx.now.Hour
        mythen
        time ctx.now.Hour ctx.now.Minute
    }

all |> Simulator.start

(*
Simulator.stop ()
*)