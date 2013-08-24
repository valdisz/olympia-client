module AST

    type Route = {
        direction: string;
        province: string;
        region: string;
        coords: string * int;
        time: int
    }

    type InnerLocation = {
        name: string;
        id: string;
        kind: string;
        time: int;
        safe: bool
    }

    type Event = {
        day: int;
        text: string
    }

    type Province = {
        name: string;
        coords: string * int;
        terrain: string;
        region: string;
        safe: bool;
        civ: int;   // will hold civ * 2 to avoid floating point numbers
        rain: bool
        routes: Route list;
        locations: InnerLocation list;
        events: Event list;
        seen: string list;
    }