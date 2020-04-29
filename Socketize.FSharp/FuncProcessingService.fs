namespace Socketize.FSharp

open Lidgren.Network
open Socketize
open Socketize.Exceptions
open Socketize.Routing
open Socketize.FSharp

type FuncProcessingService(schema: FuncSchema) =
    let combineRoutes =
        sprintf "%s/%s"

    let handlers =
        seq {
            for route in schema.routes do
                yield route.route, route.handler

            for hub in schema.hubs do
                for route in hub.subRoutes do
                    let fullRoute = combineRoutes hub.route route.route
                    yield fullRoute, route.handler
        } |> dict

    let processMessage (route: string) (ctx: Context) (dtoRaw: byte[] option) : bool =
        if handlers.ContainsKey(route) |> not then
            false
        else
            let handler = handlers.[route]
            handler ctx dtoRaw
            true

    interface IProcessingService with
        member _.ProcessMessage(route: string, message: NetIncomingMessage, failWhenNoHandlers: bool) =
            let messageLength = message.ReadInt32()
            let dtoRaw = if messageLength = 0 then None else message.ReadBytes(messageLength) |> Some
            let ctx = Context message.SenderConnection

            if processMessage route ctx dtoRaw |> not && failWhenNoHandlers then
                SocketizeException(sprintf "Handler for route '%s' not found" route)
                |> raise

        member _.ProcessDisconnected(conn: NetConnection) =
            let ctx = Context conn
            processMessage SpecialRouteNames.DisconnectRoute ctx None
            |> ignore

        member _.ProcessConnected(conn: NetConnection) =
            let ctx = Context conn
            processMessage SpecialRouteNames.ConnectRoute ctx None
            |> ignore