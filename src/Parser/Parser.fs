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
            | IsMatch "^\w[\w\s]* \[[a-z]{2}\d+\]$" -> SectFaction
            | IsMatch "^\w[\w\s]* \[\d+\]$" -> SectNoble
            | IsMatch "^\w[\w\s]* \[[a-z]\d+\].* in .* \[.+\].*$" -> SectInnerLocation
            | IsMatch "^\w[\w\s]* \[[a-z]{2}\d{2}\].* in .*" -> SectProvince
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
            |> Seq.map (fun (key, values) -> (key, values |> Seq.toList))
            |> Map.ofSeq

        let provinces = sections.[SectProvince] |> List.map (fun x -> ProvinceParser.parse x.lines)
        let (turn, faction) = FactionParser.parse (sections.[SectFaction] |> List.head).lines

        {
            AST.faction = faction;
            AST.turn = turn;
            AST.password = "";
            AST.provinces = provinces;
        }