#load "../../.deps/PxlLocalDevShadow.fsx"
open PxlLocalDevShadow

open System
open Pxl
open Pxl.Ui

module FusistorCore =
    open System
    open System.Collections.Generic

    [<RequireQualifiedAccess>]
    type Color =
        | Black
        | Brown
        | Red
        | Orange
        | Yellow
        | Green
        | Blue
        | Violet
        | Grey
        | White
        | Gold
        | Silver
        | Pink

        member this.AsDigitOrExponent() =
            match this with
            | Black -> 0.0
            | Brown -> 1.0
            | Red -> 2.0
            | Orange -> 3.0
            | Yellow -> 4.0
            | Green -> 5.0
            | Blue -> 6.0
            | Violet -> 7.0
            | Grey -> 8.0
            | White -> 9.0
            | Gold -> -1.0
            | Silver -> -2.0
            | Pink -> -3.0

        member this.AsTolerance() =
            match this with
            | Brown -> 0.01
            | Red -> 0.02
            | Orange -> 0.0005
            | Yellow -> 0.0002
            | Green -> 0.005
            | Blue -> 0.0025
            | Violet -> 0.001
            | Grey -> 0.0001
            | Gold -> 0.05
            | Silver -> 0.1
            | _ -> failwith "Invalid tolerance color"

        member this.AsTcr() =
            match this with
            | Black -> 250
            | Brown -> 100
            | Red -> 50
            | Orange -> 15
            | Yellow -> 25
            | Green -> 20
            | Blue -> 10
            | Violet -> 5
            | Grey -> 1
            | _ -> failwith "Invalid TCR color"

        static member FromTolerance(tolerance: float) =
            match tolerance with
            | 1.0 -> Brown
            | 2.0 -> Red
            | 0.05 -> Orange
            | 0.02 -> Yellow
            | 0.5 -> Green
            | 0.25 -> Blue
            | 0.1 -> Violet
            | 0.01 -> Grey
            | 5.0 -> Gold
            | 10.0 -> Silver
            | _ -> failwithf "Invalid tolerance value %f" tolerance

        static member FromTcr(tcr: int) =
            match tcr with
            | 250 -> Black
            | 100 -> Brown
            | 50 -> Red
            | 15 -> Orange
            | 25 -> Yellow
            | 20 -> Green
            | 10 -> Blue
            | 5 -> Violet
            | 1 -> Grey
            | _ -> failwith "Invalid TCR value"

        static member FromInt(value: int) =
            match value with
            | 0 -> Black
            | 1 -> Brown
            | 2 -> Red
            | 3 -> Orange
            | 4 -> Yellow
            | 5 -> Green
            | 6 -> Blue
            | 7 -> Violet
            | 8 -> Grey
            | 9 -> White
            | -1 -> Gold
            | -2 -> Silver
            | -3 -> Pink
            | _ -> failwithf "Invalid color value %d" value

    type Resistor =
        | ZeroOhm
        | ThreeBand of Color * Color * Color
        | FourBand of Color * Color * Color * Color
        | FiveBand of Color * Color * Color * Color * Color
        | SixBand of Color * Color * Color * Color * Color * Color

        member this.Bands() =
            match this with
            | ZeroOhm -> [ Color.Black ]
            | ThreeBand(b1, b2, b3) -> [ b1; b2; b3 ]
            | FourBand(b1, b2, b3, b4) -> [ b1; b2; b3; b4 ]
            | FiveBand(b1, b2, b3, b4, b5) -> [ b1; b2; b3; b4; b5 ]
            | SixBand(b1, b2, b3, b4, b5, b6) -> [ b1; b2; b3; b4; b5; b6 ]

        member this.Calc() =
            match this with
            | ZeroOhm -> (0.0, 0.0, 0.0, None)
            | ThreeBand(band1, band2, band3) ->
                let digit1 = band1.AsDigitOrExponent()
                let digit2 = band2.AsDigitOrExponent()
                let exponent = band3.AsDigitOrExponent()
                let tolerance = 0.2
                let multiplier = Math.Pow(10.0, exponent)
                let ohm = (digit1 * 10.0 + digit2) * multiplier
                let toleranceOhm = ohm * tolerance
                let min = ohm - toleranceOhm
                let max = ohm + toleranceOhm
                (ohm, min, max, None)
            | FourBand(band1, band2, band3, band4) ->
                let digit1 = band1.AsDigitOrExponent()
                let digit2 = band2.AsDigitOrExponent()
                let exponent = band3.AsDigitOrExponent()
                let tolerance = band4.AsTolerance()
                let multiplier = Math.Pow(10.0, exponent)
                let ohm = (digit1 * 10.0 + digit2) * multiplier
                let toleranceOhm = ohm * tolerance
                let min = ohm - toleranceOhm
                let max = ohm + toleranceOhm
                (ohm, min, max, None)
            | FiveBand(band1, band2, band3, band4, band5) ->
                let digit1 = band1.AsDigitOrExponent()
                let digit2 = band2.AsDigitOrExponent()
                let digit3 = band3.AsDigitOrExponent()
                let exponent = band4.AsDigitOrExponent()
                let tolerance = band5.AsTolerance()
                let multiplier = Math.Pow(10.0, exponent)
                let ohm = (digit1 * 100.0 + digit2 * 10.0 + digit3) * multiplier
                let toleranceOhm = ohm * tolerance
                let min = ohm - toleranceOhm
                let max = ohm + toleranceOhm
                (ohm, min, max, None)
            | SixBand(band1, band2, band3, band4, band5, band6) ->
                let digit1 = band1.AsDigitOrExponent()
                let digit2 = band2.AsDigitOrExponent()
                let digit3 = band3.AsDigitOrExponent()
                let exponent = band4.AsDigitOrExponent()
                let tolerance = band5.AsTolerance()
                let tcr = band6.AsTcr()
                let multiplier = Math.Pow(10.0, exponent)
                let ohm = (digit1 * 100.0 + digit2 * 10.0 + digit3) * multiplier
                let toleranceOhm = ohm * tolerance
                let min = ohm - toleranceOhm
                let max = ohm + toleranceOhm
                (ohm, min, max, Some tcr)

    module Resistor =

        let isValidColorInBand (color: Color) (bandPosition: int) (bandCount: int) : bool =
            let validConfigs = HashSet<(Color * int * int)>()
            // Zero-ohm resistor
            validConfigs.Add(Color.Black, 1, 1) |> ignore
            // Band 1 in 3-band resistor
            for c in
                [ Color.Brown
                  Color.Red
                  Color.Orange
                  Color.Yellow
                  Color.Green
                  Color.Blue
                  Color.Violet
                  Color.Grey
                  Color.White ] do
                validConfigs.Add(c, 1, 3) |> ignore
            // Band 2 in 3-band resistor
            for c in
                [ Color.Black
                  Color.Brown
                  Color.Red
                  Color.Orange
                  Color.Yellow
                  Color.Green
                  Color.Blue
                  Color.Violet
                  Color.Grey
                  Color.White ] do
                validConfigs.Add(c, 2, 3) |> ignore
            // Band 3 in 3-band resistor
            for c in
                [ Color.Black
                  Color.Brown
                  Color.Red
                  Color.Orange
                  Color.Yellow
                  Color.Green
                  Color.Blue
                  Color.Violet
                  Color.Grey
                  Color.White
                  Color.Gold
                  Color.Silver
                  Color.Pink ] do
                validConfigs.Add(c, 3, 3) |> ignore
            // Band 1 in 4-band resistor
            for c in
                [ Color.Brown
                  Color.Red
                  Color.Orange
                  Color.Yellow
                  Color.Green
                  Color.Blue
                  Color.Violet
                  Color.Grey
                  Color.White ] do
                validConfigs.Add(c, 1, 4) |> ignore
            // Band 2 in 4-band resistor
            for c in
                [ Color.Black
                  Color.Brown
                  Color.Red
                  Color.Orange
                  Color.Yellow
                  Color.Green
                  Color.Blue
                  Color.Violet
                  Color.Grey
                  Color.White ] do
                validConfigs.Add(c, 2, 4) |> ignore
            // Band 3 in 4-band resistor
            for c in
                [ Color.Black
                  Color.Brown
                  Color.Red
                  Color.Orange
                  Color.Yellow
                  Color.Green
                  Color.Blue
                  Color.Violet
                  Color.Grey
                  Color.White
                  Color.Gold
                  Color.Silver
                  Color.Pink ] do
                validConfigs.Add(c, 3, 4) |> ignore
            // Band 4 in 4-band resistor
            for c in
                [ Color.Brown
                  Color.Red
                  Color.Orange
                  Color.Yellow
                  Color.Green
                  Color.Blue
                  Color.Violet
                  Color.Grey
                  Color.Gold
                  Color.Silver ] do
                validConfigs.Add(c, 4, 4) |> ignore
            // band 1 in 5-band resistor
            for c in
                [ Color.Brown
                  Color.Red
                  Color.Orange
                  Color.Yellow
                  Color.Green
                  Color.Blue
                  Color.Violet
                  Color.Grey
                  Color.White ] do
                validConfigs.Add(c, 1, 5) |> ignore

            // band 2 in 5-band resistor
            for c in
                [ Color.Black
                  Color.Brown
                  Color.Red
                  Color.Orange
                  Color.Yellow
                  Color.Green
                  Color.Blue
                  Color.Violet
                  Color.Grey
                  Color.White ] do
                validConfigs.Add(c, 2, 5) |> ignore
            // band 3 in 5-band resistor
            for c in
                [ Color.Black
                  Color.Brown
                  Color.Red
                  Color.Orange
                  Color.Yellow
                  Color.Green
                  Color.Blue
                  Color.Violet
                  Color.Grey
                  Color.White ] do
                validConfigs.Add(c, 3, 5) |> ignore
            // band 4 in 5-band resistor
            for c in
                [ Color.Black
                  Color.Brown
                  Color.Red
                  Color.Orange
                  Color.Yellow
                  Color.Green
                  Color.Blue
                  Color.Violet
                  Color.Grey
                  Color.White
                  Color.Gold
                  Color.Silver
                  Color.Pink ] do
                validConfigs.Add(c, 4, 5) |> ignore
            // band 5 in 5-band resistor
            for c in
                [ Color.Brown
                  Color.Red
                  Color.Orange
                  Color.Yellow
                  Color.Green
                  Color.Blue
                  Color.Violet
                  Color.Grey
                  Color.Gold
                  Color.Silver ] do
                validConfigs.Add(c, 5, 5) |> ignore

            // band 1 in 6-band resistor
            for c in
                [ Color.Brown
                  Color.Red
                  Color.Orange
                  Color.Yellow
                  Color.Green
                  Color.Blue
                  Color.Violet
                  Color.Grey
                  Color.White ] do
                validConfigs.Add(c, 1, 6) |> ignore
            // band 2 in 6-band resistor
            for c in
                [ Color.Black
                  Color.Brown
                  Color.Red
                  Color.Orange
                  Color.Yellow
                  Color.Green
                  Color.Blue
                  Color.Violet
                  Color.Grey
                  Color.White ] do
                validConfigs.Add(c, 2, 6) |> ignore
            // band 3 in 6-band resistor
            for c in
                [ Color.Black
                  Color.Brown
                  Color.Red
                  Color.Orange
                  Color.Yellow
                  Color.Green
                  Color.Blue
                  Color.Violet
                  Color.Grey
                  Color.White ] do
                validConfigs.Add(c, 3, 6) |> ignore
            // band 4 in 6-band resistor
            for c in
                [ Color.Black
                  Color.Brown
                  Color.Red
                  Color.Orange
                  Color.Yellow
                  Color.Green
                  Color.Blue
                  Color.Violet
                  Color.Grey
                  Color.White
                  Color.Gold
                  Color.Silver
                  Color.Pink ] do
                validConfigs.Add(c, 4, 6) |> ignore
            // band 5 in 6-band resistor
            for c in
                [ Color.Brown
                  Color.Red
                  Color.Orange
                  Color.Yellow
                  Color.Green
                  Color.Blue
                  Color.Violet
                  Color.Grey
                  Color.Gold
                  Color.Silver ] do
                validConfigs.Add(c, 5, 6) |> ignore
            // band 6 in 6-band resistor
            for c in
                [ Color.Black
                  Color.Brown
                  Color.Red
                  Color.Orange
                  Color.Yellow
                  Color.Green
                  Color.Blue
                  Color.Violet
                  Color.Grey ] do
                validConfigs.Add(c, 6, 6) |> ignore

            validConfigs.Contains(color, bandPosition, bandCount)

        let tryCreate bands =
            match bands with
            | [ color ] when color = Color.Black -> Some ZeroOhm
            | [ band1; band2; band3 ] when
                isValidColorInBand band1 1 3
                && isValidColorInBand band2 2 3
                && isValidColorInBand band3 3 3
                ->
                Some(ThreeBand(band1, band2, band3))
            | [ band1; band2; band3; band4 ] when
                isValidColorInBand band1 1 4
                && isValidColorInBand band2 2 4
                && isValidColorInBand band3 3 4
                && isValidColorInBand band4 4 4
                ->
                Some(FourBand(band1, band2, band3, band4))
            | [ band1; band2; band3; band4; band5 ] when
                isValidColorInBand band1 1 5
                && isValidColorInBand band2 2 5
                && isValidColorInBand band3 3 5
                && isValidColorInBand band4 4 5
                && isValidColorInBand band5 5 5
                ->
                Some(FiveBand(band1, band2, band3, band4, band5))
            | [ band1; band2; band3; band4; band5; band6 ] when
                isValidColorInBand band1 1 6
                && isValidColorInBand band2 2 6
                && isValidColorInBand band3 3 6
                && isValidColorInBand band4 4 6
                && isValidColorInBand band5 5 6
                && isValidColorInBand band6 6 6
                ->
                Some(SixBand(band1, band2, band3, band4, band5, band6))
            | _ -> None

        let try_create_1_band (band: Color) : Result<Resistor, String> =
            if isValidColorInBand band 1 1 then
                Ok(Resistor.ZeroOhm)
            else
                Error("no valid 1-band resistor")

        let try_create_3_band (band1: Color, band2: Color, band3: Color) : Result<Resistor, String> =
            if
                isValidColorInBand band1 1 3
                && isValidColorInBand band2 2 3
                && isValidColorInBand band3 3 3
            then
                Ok(Resistor.ThreeBand(band1, band2, band3))
            else
                Error("no valid 3-band resistor")

        let try_create_4_band (band1: Color, band2: Color, band3: Color, band4: Color) : Result<Resistor, String> =
            if
                isValidColorInBand band1 1 4
                && isValidColorInBand band2 2 4
                && isValidColorInBand band3 3 4
                && isValidColorInBand band4 4 4
            then
                Ok(Resistor.FourBand(band1, band2, band3, band4))
            else
                Error("no valid 4-band resistor")

        let try_create_5_band
            (band1: Color, band2: Color, band3: Color, band4: Color, band5: Color)
            : Result<Resistor, String> =
            if
                isValidColorInBand band1 1 5
                && isValidColorInBand band2 2 5
                && isValidColorInBand band3 3 5
                && isValidColorInBand band4 4 5
                && isValidColorInBand band5 5 5
            then
                Ok(Resistor.FiveBand(band1, band2, band3, band4, band5))
            else
                Error("no valid 4-band resistor")

        let try_create_6_band
            (band1: Color, band2: Color, band3: Color, band4: Color, band5: Color, band6: Color)
            : Result<Resistor, String> =
            if
                isValidColorInBand band1 1 6
                && isValidColorInBand band2 2 6
                && isValidColorInBand band3 3 6
                && isValidColorInBand band4 4 6
                && isValidColorInBand band5 5 6
                && isValidColorInBand band6 6 6
            then
                Ok(Resistor.SixBand(band1, band2, band3, band4, band5, band6))
            else
                Error("no valid 4-band resistor")

        let determineDigits (ohm: float) =
            let mutable exponent = 0
            let mutable s = ohm.ToString()
            let s2 = s.Replace(".", "").Trim([| '0' |])

            if s2.Length > 3 then
                None
            else
                while s.Contains(".") do
                    let idx = s.IndexOf('.')
                    exponent <- exponent - 1
                    s <- s.Remove(idx, 1).Insert(idx + 1, ".")
                    let n = Double.Parse(s)
                    s <- n.ToString()

                if s.Length = 1 then
                    s <- s.Insert(1, "0")
                    exponent <- exponent - 1

                while s.Length > 3 || (ohm <= 99000000000.0 && s.Length = 3 && s.[2] = '0') do
                    exponent <- exponent + 1
                    s <- s.Substring(0, s.Length - 1)

                let digits = s.ToCharArray() |> Array.map (fun c -> int c - int '0')
                Some(digits, exponent)

        let determine (resistance: float, tolerance: Option<float>, tcr: Option<int>) =
            match determineDigits resistance with
            | Some(digits, e) ->
                match digits.Length, tolerance, tcr with
                | 2, None, None -> Some(ThreeBand(Color.FromInt digits.[0], Color.FromInt digits.[1], Color.FromInt e))
                | 2, Some tol, None ->
                    Some(
                        FourBand(
                            Color.FromInt digits.[0],
                            Color.FromInt digits.[1],
                            Color.FromInt e,
                            Color.FromTolerance tol
                        )
                    )
                | 3, Some tol, None ->
                    Some(
                        FiveBand(
                            Color.FromInt digits.[0],
                            Color.FromInt digits.[1],
                            Color.FromInt digits.[2],
                            Color.FromInt e,
                            Color.FromTolerance tol
                        )
                    )
                | 3, Some tol, Some tcr ->
                    Some(
                        SixBand(
                            Color.FromInt digits.[0],
                            Color.FromInt digits.[1],
                            Color.FromInt digits.[2],
                            Color.FromInt e,
                            Color.FromTolerance tol,
                            Color.FromTcr tcr
                        )
                    )
                | 3, None, None -> None
                | _ -> None
            | None -> None

// -------------------------------------------------------------

let fusitorColorToPxlColor color =
    match color with
    | FusistorCore.Color.Black -> Pxl.Colors.black
    | FusistorCore.Color.Brown -> Pxl.Colors.brown
    | FusistorCore.Color.Red -> Pxl.Colors.red
    | FusistorCore.Color.Orange -> Pxl.Colors.orange
    | FusistorCore.Color.Yellow -> Pxl.Colors.yellow
    | FusistorCore.Color.Green -> Pxl.Colors.green
    | FusistorCore.Color.Blue -> Pxl.Colors.blue
    | FusistorCore.Color.Violet -> Pxl.Colors.violet
    | FusistorCore.Color.Grey -> Pxl.Colors.gray
    | FusistorCore.Color.White -> Pxl.Colors.white
    | FusistorCore.Color.Gold -> Pxl.Colors.gold
    | FusistorCore.Color.Silver -> Pxl.Colors.silver
    | FusistorCore.Color.Pink -> Pxl.Colors.pink

let drawResistor (colors: Color list) = scene {
    for (i, c) in List.indexed colors do
        rect.xywh(i * 3, 0, 2, 10).fill(c)
}

scene {
    let! ctx = getCtx()

    let resistor = FusistorCore.Resistor.determine(250.0, Some 10.0, None)
    let bands = resistor.Value.Bands() |> List.map fusitorColorToPxlColor
    drawResistor bands
    text.var4x5("250", 0, 12)
    text.var4x5("Ohm", 0, 18)
}
|> Simulator.start

// let t = async {
//     do! Async.Sleep 30000
//     Simulator.stop ()
// }
// Async.Start t
