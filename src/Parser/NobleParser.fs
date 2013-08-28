module NobleParser

    open Lib
    open AST
    open Common

    let rec SRoot state = function
            | line when IsMatch "^\d+: .*$" line -> SEvent state line
            | Regex "^(\w[\w\s]+) \[(\d+)\]$" [name; id] ->
                FSM(SRoot, { state with Noble.id = int id; Noble.name = name })
            | _ -> FSM(SRoot, state)

    and SEvent state = function
        | Regex "^(\d+): (.+\.)$" [day; text] ->
            let event = { day = int day; text = text }
            FSM(SEvent, { state with events = event::state.events })
        | Regex "(\d+): (.*)" [day; text] ->
            let event = { day = int day; text = text }
            FSM(SEventContinue, { state with events = event::state.events })
        | _ -> FSM(SRoot, state)

    and SEventContinue state = function
        | Regex "^(\d+): (.+\.)$" [day; text] ->
            let head = List.head state.events
            let tail = List.tail state.events
            let event = { day = int day; text = head.text + " " + text }
            FSM(SEvent, { state with events = event::tail })
        | Regex "(\d+): (.*)" [day; text] ->
            let head = List.head state.events
            let tail = List.tail state.events
            let event = { day = int day; text = head.text + " " + text }
            FSM(SEventContinue, { state with events = event::state.events })
        | _ -> FSM(SRoot, state)

    let parse lines =
        let inital = FSM(SRoot,  { Noble.id = 0; Noble.name = ""; Noble.events = [] })
        match (List.fold (fun state input -> match state with FSM(fn, st) -> fn st input) inital lines) with FSM(_, state) -> state