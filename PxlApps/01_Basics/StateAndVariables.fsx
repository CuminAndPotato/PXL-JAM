#load "../../.deps/PxlLocalDevShadow.fsx"
open PxlLocalDevShadow

open System
open Pxl
open Pxl.Ui



// -------------------------------------------------------------




// A simple counter with a state variable
scene {
    // Here, we declare a variable with initial value 0
    let! count = useState { 0 }

    text.mono6x6($"{count.value}").color(Colors.white)

    // In every frame, we increment the counter
    do count.value <- count.value + 1
}
|> Simulator.start




(*
Simulator.stop ()
*)
