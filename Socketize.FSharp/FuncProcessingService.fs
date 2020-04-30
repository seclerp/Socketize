namespace Socketize.FSharp

open Lidgren.Network

open Socketize
open Socketize.Exceptions
open Socketize.Routing
open Socketize.FSharp

type FuncProcessingService(schema: FuncSchema) =
    let (FuncSchema schema) = schema
    let handlers =
        schema
        |> List.map (fun route -> route.route, route.handler)
        |> dict

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