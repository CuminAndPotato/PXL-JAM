#load "fsi/PxlLocalDev.fsx"

open System.IO

module PxlLocalDev =
    let loadAsset (assetPath: string) =
        let path = Path.Combine(__SOURCE_DIRECTORY__, "../PxlApps/assets", assetPath)
        let content = File.ReadAllBytes(path)
        new MemoryStream(content)

let createCanvas () = Pxl.CanvasProxy.create Pxl.CanvasProxy.Channel.Tcp "localhost"

module Simulator =
    let start scene = Pxl.Ui.Fsi.Eval.start createCanvas scene
    let stop () = Pxl.Ui.Fsi.Eval.stop ()

// Shadowing parts of the Pxl assembly (bugfixes, new features, etc.)
module Color =
    open Pxl

    // 🙏 Urs :)
    /// Converts HSV to RGB.
    /// h: Hue in degrees (0-360)
    /// s: Saturation (0.0-1.0)
    /// v: Value (0.0-1.0)
    let hsv (h: float) (s: float) (v: float) =
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

        (r, g, b) |> Color.rgb |> fun c -> c.opacity(0.7)
