module Lib

    open System.Text.RegularExpressions

    type FSM<'state, 'event> = FSM of ('state -> 'event -> FSM<'state, 'event>) * 'state

    let (|Regex|_|) (pattern: string) (input: string) =
        let m = Regex.Match(input, pattern)
        if m.Success then Some(List.tail [ for g in m.Groups -> g.Value ])
        else None
    
    let (|IsMatch|_|) (pattern: string) (input: string) =
        if (Regex.IsMatch(input, pattern)) then Some IsMatch else None 
    
    let IsMatch (pattern: string) (input: string) = Regex.IsMatch(input, pattern)