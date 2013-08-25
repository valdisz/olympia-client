module Parser

    open Lib
    open System.Text.RegularExpressions

    type sectionType =  
        | SectHeader
        | SectFaction
        | SectNoble
        | SectProvince
        | SectInnerLocation
        | SectNewPlayers
        | SectOrdersTemplate
        | SectLore

    type Section = { st: sectionType; lines: string list }

    exception UnknonwSectionError of string

    let sectionByContent line =
        match line with
            | IsMatch "^Olympia G3 turn \d+$" -> SectHeader
            | IsMatch "^.+\[[a-z]+\d+\]$" -> SectFaction
            | IsMatch "^.+\[\d+\]$" -> SectNoble
            | IsMatch "^.+ \[[a-z]+\d+\], .+ in .+, (?:(?:civ-\d+)|wilderness)$" -> SectProvince
            | IsMatch "^.+ \[[a-z]+\d+\], .+ in .+$" -> SectInnerLocation
            | "New players" -> SectNewPlayers
            | "Order template" -> SectOrdersTemplate
            | "Lore sheets" -> SectLore
            | _ -> raise (UnknonwSectionError("Cannot understand section type by text: " + line))

    let sectionMarker current =
        let headerLine = List.head current.lines
        let section = { st = sectionByContent headerLine; lines = [headerLine] }

        (section, { current with lines = List.tail current.lines })

    let mapSections sections line =
        match sections with
            | [] -> [{ st = SectHeader; lines = [line] }]
            | _ ->
                let current = List.head sections
                let tail = List.tail sections
                match line with
                    | IsMatch "^\-{72}$" ->
                        let (a, b) = sectionMarker current
                        a::b::tail
                    | _ -> { current with lines = line::current.lines }::tail

    let parse (lines: string seq) =
        let sections =
            lines
            |> Seq.fold mapSections []
            |> Seq.map (fun s -> { s with lines = List.rev s.lines })
            |> Seq.groupBy (fun x -> x.st)
            |> Seq.map (fun (key, values) -> (key, values |> Seq.toArray))
            |> Map.ofSeq

        sections.[SectProvince] |> Array.map (fun x -> ProvinceParser.parse x.lines)