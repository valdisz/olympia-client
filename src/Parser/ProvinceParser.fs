module ProvinceParser

    open Lib
    open AST
    open Common

    let rec SRoot state = function
            | "Inner locations:" -> FSM(SLocations, state)
            | "Seen here:" -> FSM(SSeen, state)
            | "It is raining." -> FSM(SRoot, { state with rain = true })
            | line when IsMatch "^\d+: .*$" line -> SEvent state line
            | IsMatch "^Routes leaving .+:$" -> FSM(SRoutes, state)
            | Regex "^(\w[\w\s]*) \[(\w{2}\d{2})\], (\w[\w\s]*), in (\w[\w\s]*), (?:(\w[\w\s]*), )?(?:civ-(\d+)|wilderness)$" [name; coords; terrain; region; safe; civ] ->
                FSM(SRoot, { state with
                                    name = name;
                                    coords = parseCoords coords;
                                    terrain = terrain;
                                    region = region;
                                    safe = (safe = "safe haven");
                                    civ = match civ with
                                            | "" -> 0
                                            | _ -> int civ })
            | _ -> FSM(SRoot, state)

    and SRoutes state = function
        | Regex "^\s+(\w[\w\s]*), to (\w[\w\s]*) \[(\w{2}\d{2})\], (\d+ days?|impassable)$" [dir; province; coords; time] ->
            let route = { direction = dir; province = province; region = ""; coords = parseCoords coords; time = parseTime time }
            FSM(SRoutes, { state with routes = route::state.routes })
        | Regex "^\s+(\w[\w\s]*), to (\w[\w\s]*) \[(\w{2}\d{2})\], (\w[\w\s]*), (\d+ days?|impassable)$" [dir; province; coords; region; time] ->
            let route = { direction = dir; province = province; region = region; coords = parseCoords coords; time = parseTime time }
            FSM(SRoutes, { state with routes = route::state.routes })
        | _ -> FSM(SRoot, state)

    and SLocations state = function
        | Regex "(\w[\w\s]*) \[(\w\d+)\], (\w[\w\s]*)(?:, (safe haven))?, (\d+) days?" [name; id; kind; safe; time] ->
            let loc = { id = id; name = name; kind = kind; safe = (safe = "safe haven"); time = int time }
            FSM(SLocations, { state with locations = loc::state.locations })
        | _ -> FSM(SRoot, state)

    and SSeen state = function
        | Regex "^\s+(\S.+)$" [unit] -> FSM(SSeen, { state with seen = unit::state.seen })
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
        let inital = FSM(SRoot,  { name = ""; coords = ("",-1); terrain = ""; region= ""; safe = false; civ = 0; rain = false; routes = []; locations = []; events = []; seen = [] })
        let province = match (List.fold (fun state input -> match state with FSM(fn, st) -> fn st input) inital lines) with FSM(_, state) -> state

        { province with events = List.rev province.events }