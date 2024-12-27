#load "../../.deps/PxlLocalDevShadow.fsx"

open PxlLocalDevShadow

open System
open Pxl
open Pxl.Ui

let time (now: DateTime) =
    scene {
        let! ctx = getCtx ()

        let timeText =
            text.var4x5($"{now.Hour}:{now.Minute}").color (Colors.white)

        let textWidth = timeText.measure ()
        let marginLeft = (ctx.width - textWidth) / 2

        let marginTop =
            (ctx.height - (int timeText._data.fontSize) - 1)
            / 2

        timeText
            .xy(marginLeft, marginTop)
            .color (Colors.white)
    }

let colors =
    [ Color.rgb (230, 69, 69)
      Color.rgb (230, 101, 69)
      Color.rgb (230, 133, 69)
      Color.rgb (230, 165, 69)
      Color.rgb (230, 197, 69)
      Color.rgb (230, 230, 69)
      Color.rgb (197, 230, 69)
      Color.rgb (165, 230, 69)
      Color.rgb (133, 230, 69)
      Color.rgb (101, 230, 69)
      Color.rgb (69, 230, 69)
      Color.rgb (69, 230, 101)
      Color.rgb (69, 230, 133)
      Color.rgb (69, 230, 165)
      Color.rgb (69, 230, 197)
      Color.rgb (69, 230, 230)
      Color.rgb (69, 197, 230)
      Color.rgb (69, 165, 230)
      Color.rgb (69, 133, 230)
      Color.rgb (69, 101, 230)
      Color.rgb (69, 69, 230)
      Color.rgb (101, 69, 230)
      Color.rgb (133, 69, 230)
      Color.rgb (165, 69, 230)
      Color.rgb (197, 69, 230)
      Color.rgb (230, 69, 230)
      Color.rgb (230, 69, 197)
      Color.rgb (230, 69, 165)
      Color.rgb (230, 69, 133)
      Color.rgb (230, 69, 101) ]

type Point = { X: float; Y: float }

type Data =
    { Points: Point list
      vX: float
      vY: float }

let getRandomVelocity () =
    let vX = Random.Shared.NextDouble() * 1.9
    let vY = Math.Sqrt (4.0 - (vX * vX))
    vX, vY

let getRandomStart width height (now: DateTime) =
    let vX, vY = getRandomVelocity ()
    { Points =
          [ { X = (Random.Shared.NextDouble() * width) % width
              Y = (Random.Shared.NextDouble() * height) % height } ]
      vX = vX
      vY = vY }

let worm width height =
    scene {
        let! now = getNow ()
        let! data = useState { getRandomStart width height now }

        let! secChanged = Trigger.valueChanged (now.Millisecond / 100)
        let! minChanged = Trigger.valueChanged now.Minute

        let current = data.value

        let next =
            if minChanged then
                getRandomStart width height now
            else if secChanged then
                let x = current.Points.Head.X
                let y = current.Points.Head.Y
                let xCandidate = x + current.vX
                let yCandidate = y + current.vY

                let x, vX =
                    if xCandidate < 0 then
                        x, -current.vX
                    else if xCandidate >= width then
                        x, -current.vX
                    else
                        xCandidate, current.vX

                let y, vY =
                    if yCandidate < 0 then
                        y, -current.vY
                    else if yCandidate >= height then
                        y, -current.vY
                    else
                        yCandidate, current.vY

                { current with
                      Points = { X = x; Y = y } :: current.Points
                      vX = vX
                      vY = vY }
            else
                current

        let pairs =
            next.Points
            |> List.take (min 60 next.Points.Length)
            |> List.pairwise
            |> List.rev
            |> List.mapi (fun i pair -> i, pair)

        for i, (a, b) in pairs do
            let color = colors[(current.Points.Length + i) % colors.Length]
            let c = Color.argb(255.0- (float i), color.r, color.g, color.b)
            line
                .p1p2(int a.X, int a.Y, int b.X, int b.Y)
                .strokeThickness(2.0)
                .useAntiAlias()
                .stroke (c)

        do data.value <- next
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

let all =
    scene {
        let! ctx = getCtx ()
        worm ctx.width ctx.height
        diffuser
        time ctx.now
    }

all |> Simulator.start
(*
Simulator.stop ()
*)

