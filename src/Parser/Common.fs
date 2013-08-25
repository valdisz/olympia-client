module Common

    open Lib
    open System.Text.RegularExpressions

    let parseCoords (coords: string) =
        let m = Regex.Match(coords, "([a-z]{2})(\d{2})")
        ( m.Groups.[1].Value, int m.Groups.[2].Value)

    exception InvalidNumberNameError of string

    let textToNumber = function
        | "zero" | "nothing" -> 0
        | "one" -> 1
        | "two" -> 2
        | "three" -> 3
        | "four" -> 4
        | "five" -> 5
        | "six" -> 6
        | "seven" -> 7
        | "eight" -> 8
        | "nine" -> 9
        | "ten" -> 10
        | "eleven" -> 11
        | "twelve" -> 12
        | name -> raise (InvalidNumberNameError("Provided number name" + name + " does not represent any known number"))

    exception InvalidTimeFormatError of string

    let parseTime (time: string) =
        match time with
            | "impassable" -> -1
            | Regex "(\d+) days?" [days] -> int days
            | Regex "(\w+) days?" [days] -> textToNumber days
            | _ -> raise (InvalidTimeFormatError("Cannot understand provided time " + time))