#load "../../.deps/PxlLocalDevShadow.fsx"

open PxlLocalDevShadow

open System
open Pxl
open Pxl.Ui

let time (now: DateTime) =
    scene {
        let! ctx = getCtx ()

        let timeText =
            text.var4x5($"%02d{now.Hour}:%02d{now.Minute}").color (Colors.white)

        let textWidth = timeText.measure ()
        let marginLeft = (ctx.width - textWidth) / 2

        timeText
            .xy(marginLeft, 4)
            .color Colors.white
    }

let image =
    [
        "                               yyyyyyyy           g                                 "
        "                                   yyyyyyyy    ggggg                                "
        "                               yyyyyyyy       gggggggg                              "
        "                                   yyyyyyyy     ggg ggg                             "
        "                                    y y          gg  g                              "
        "                                    y y     e e                                     "
        "                                    y y   e eeee                                    "
        "                                    y y  eeeeeeee                                   "
        "                                          eeeeee                                    "
        "                                         eeeeeeee                                   "
        "                                          e be e                                    "
        "                                            bb      lll lll                         "
        "                                            bb      lfl lfl                         "
        "                                            bb      lfl lfl                         "
        "                                            bb      ddddddd                         "
        "                                            bb      dcdddcd                         "
        "                                            bb      ddddddd                         "
        "                                            bb      ddcccdd                         "
        "                                            bb       ddddd                          "
        "                                            bb       ddddd                          "
        "                                    tdddddddddddddddddddddd                         "
        "                                   ttdddddddddddddddddddddd                         "
        "                                   t dd dd  bb        dd dd                         "
        "                                     dd dd  bb        dd dd                         "


    ]

let shift i =
    let shiftLine (line: string) =
        line.AsSpan().Slice(line.Length - 1 - i - 24, 24).ToString()
    image
    |> List.map shiftLine

let dog (minute: int) (second: int) =
    scene {
        //let! trigger = Trigger.valueChanged(second)
        pxl.xy(second % 24, 1).stroke(Colors.blue)

        //if trigger then
        let current = shift second

        let pixels =
            [|0..23|]
            |> Array.collect (fun l ->
                current[l]
                |> Seq.toArray
                |> Array.mapi (fun i pixel -> l, i, pixel))

        for l, c, pixel in pixels do
            let color =
                match pixel with
                | 'b' -> Colors.brown
                | 'y' -> Colors.yellow
                | 'd' -> Colors.saddleBrown
                | 'e' -> Colors.green
                | 'g' -> Colors.gray
                | 't' -> Colors.rosyBrown
                | 'l' -> Colors.saddleBrown
                | 'f' -> Colors.sandyBrown
                | 'c' -> Colors.bisque
                | _ when l <= 20 -> Colors.blue
                | _ -> Colors.beige
            pxl.xy(c,l).stroke(color)
    }

let diffuser =
    scene {
        rect
            .xywh(0, 2, 24, 9)
            .fill(Color.argb (80, 0, 0, 0))
            .useAntiAlias ()

        rect
            .xywh(0, 3, 24, 7)
            .fill(Color.argb (80, 0, 0, 0))
            .useAntiAlias ()
    }

let all =
    scene {
        let! ctx = getCtx ()
        dog ctx.now.Minute ctx.now.Second
        diffuser
        time ctx.now
    }

all |> Simulator.start
(*
Simulator.stop ()
*)

