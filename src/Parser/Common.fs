module Common

    open Lib
    open System.Text.RegularExpressions

    let parseCoords (coords: string) =
        let m = Regex.Match(coords, "([a-z]{2})(\d{2})")
        ( m.Groups.[1].Value, int m.Groups.[2].Value)

    let parseTime (time: string) =
        match time with
            | "impassable" -> -1
            | Regex "(\d+) days?" [days] -> int days