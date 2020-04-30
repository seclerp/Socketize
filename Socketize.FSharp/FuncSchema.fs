namespace Socketize.FSharp

open Socketize
open Socketize.Routing

type MessageHandler = Context -> byte[] option -> unit

type Route = {
    route: string
    handler: Context -> byte[] option -> unit
}

type Hub = {
    route: string
    subRoutes: Route list
}

type FuncSchema = FuncSchema of Route list

module FuncSchemaBuilder =
    type SchemaPart =
        | Hub of Hub
        | Route of Route

    let combineRoutes =
        sprintf "%s/%s"

    let route (route: string) (handler: MessageHandler) =
        { route = route; handler = handler }
        |> SchemaPart.Route

    let onConnect (handler: MessageHandler) =
        route SpecialRouteNames.ConnectRoute handler

    let onDisconnect (handler: MessageHandler) =
        route SpecialRouteNames.DisconnectRoute handler

    let subRoute (route: string) (handler: MessageHandler) =
        { route = route; handler = handler }

    let hub (route: string) (subRoutes: Route list) =
        { route = route; subRoutes = subRoutes }
        |> SchemaPart.Hub

    let schema (parts: SchemaPart list) =
        let initialState = Seq.empty<Route>
        let folder routes current =
            match current with
            | Hub hub ->
                let resolvedSubRoutes =
                    hub.subRoutes
                    |> Seq.map (fun route -> { route with route = combineRoutes hub.route route.route })
                seq { yield! routes; yield! resolvedSubRoutes }
            | Route route -> seq { yield! routes; yield route }

        parts
        |> List.fold folder initialState
        |> List.ofSeq
        |> FuncSchema