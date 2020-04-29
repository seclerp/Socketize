namespace Socketize.FSharp

open Socketize

type MessageHandler = Context -> byte[] option -> unit

type Route = {
    route: string
    handler: Context -> byte[] option -> unit
}

type Hub = {
    route: string
    subRoutes: Route list
}

type FuncSchema = {
    routes: Route list
    hubs: Hub list
}

module FuncSchemaBuilder =
    type SchemaPart =
        | Hub of Hub
        | Route of Route

    let route (route: string) (handler: MessageHandler) =
        { route = route; handler = handler }
        |> SchemaPart.Route

    let subRoute (route: string) (handler: MessageHandler) =
        { route = route; handler = handler }

    let hub (route: string) (subRoutes: Route list) =
        { route = route; subRoutes = subRoutes }
        |> SchemaPart.Hub

    let schema (parts: SchemaPart list) =
        let initialState : Hub seq * Route seq = Seq.empty, Seq.empty
        let folder (hubs, routes) current =
            match current with
            | Hub hub -> (seq { yield! hubs; yield hub }, routes)
            | Route route -> (hubs, seq { yield! routes; yield route })

        let (hubs, routes) = parts |> List.fold folder initialState
        {
            hubs = hubs |> List.ofSeq
            routes = routes |> List.ofSeq
        }