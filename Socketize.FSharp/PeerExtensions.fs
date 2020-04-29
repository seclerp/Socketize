namespace Socketize.FSharp

open Microsoft.Extensions.Logging
open Socketize

[<RequireQualifiedAccess>]
module Client =
    let create (options: ClientOptions) (logger: ILogger<Client>) (schemaBuilder: FuncSchema) =
        let processingService = FuncProcessingService schemaBuilder
        Client(processingService, logger, options)

[<RequireQualifiedAccess>]
module Server =
    let create (options: ServerOptions) (logger: ILogger<Server>) (schemaBuilder: FuncSchema) =
        let processingService = FuncProcessingService schemaBuilder
        Server(processingService, logger, options)