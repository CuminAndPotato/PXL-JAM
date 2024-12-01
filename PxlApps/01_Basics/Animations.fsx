#load "../../.deps/PxlLocalDevShadow.fsx"
open PxlLocalDevShadow

open System
open Pxl
open Pxl.Ui



// -------------------------------------------------------------


let finalScene =
    scene {
        let! x = Anim.linear(1.0, 0, 10, repeat = Repeat.Loop)
        pxl.xy(x.value, 0).stroke(Colors.blue)
    }

finalScene |> Simulator.start


(*
Simulator.stop ()
*)
