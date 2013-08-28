module FactionParser

    open Lib
    open AST
    open Common

    let SRoot state = function
        | Regex "Olympia G3 turn (\d+)" [turn] ->
            match state with (turn, faction) -> (int turn, faction)
        | Regex "Report for (\w+) \[([a-z]{2}\d+)\]\." [name; id] ->
            match state with (turn, faction) -> (turn, { Faction.id = id; Faction.name = name})
        | _ -> state

    let parse lines =
        let inital = ( 0, { Faction.id = ""; Faction.name = "" } )
        List.fold SRoot inital lines
