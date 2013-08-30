module NobleParser

    open Lib
    open AST
    open Common

    let rec SRoot (state: Noble) = function
            | line when IsMatch "^\d+: .*$" line -> SEvent state line
            | Regex "^(\w[\w\s'-_]+) \[(\d+)\]$" [name; id] ->
                FSM(SRoot, { state with id = int id; name = name })
            | Regex "^\s+Location:\s+.+ in \w[\w\s'-_]+ \[([a-z]{2}\d{2})\], .+$" [coords] ->
                FSM(SRoot, { state with coords = parseCoords(coords) })
            | Regex "^\s+Location:\s+\w[\w\s'-_]+ \[([a-z]{2}\d{2})\], .+$" [coords] ->
                FSM(SRoot, { state with coords = parseCoords(coords) })
            | _ -> FSM(SRoot, state)

    and SEvent (state: Noble) = function
        | Regex "^(\d+): (.+\.)$" [day; text] ->
            let event = { day = int day; text = text }
            FSM(SEvent, { state with events = event::state.events })
        | Regex "(\d+): (.*)" [day; text] ->
            let event = { day = int day; text = text }
            FSM(SEventContinue, { state with events = event::state.events })
        | _ -> FSM(SRoot, state)

    and SEventContinue (state: Noble) = function
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
        let inital = FSM(SRoot,  { Noble.id = 0; name = ""; coords = ("", 0); events = [] })
        match (List.fold (fun state input -> match state with FSM(fn, st) -> fn st input) inital lines) with FSM(_, state) -> state