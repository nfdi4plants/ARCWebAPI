#r "nuget: FSharp.Data"
#r "nuget: ISADotNet, 0.3.1-preview.1"

open System.IO
open FSharp.Data

/// Read .xlsx files to Byte []. Need to adapt path
// let xlsxBytes = File.ReadAllBytes (Path.GetFullPath @"C:\Users\Freym\Desktop\Book1.xlsx")

/// This was used to check if Byte [] can be parsed back to .xlsx without problems
// let ms = new MemoryStream(xlsxBytes)
// File.WriteAllBytes (Path.GetFullPath @"C:\Users\Freym\Desktop\Book2.xlsx", xlsxBytes)

/// This string needs to contain the basic path for the target api 
/// Hint: Check "src/Shared/Shared.fs" for Routbuilder function and for the api-type
[<LiteralAttribute>]
let CommonAPIStr = @"http://localhost:8085/api/"

/// I use this line (https://github.com/nfdi4plants/Swate/blob/developer/src/Server/Server.fs#L280) for the most simple version of a get request.
/// The following function can be used to check server connection. Url needs to be adapted
// Http.RequestString("https://localhost:3000/test/test1")

// // The following can be any apis you want to adress directly.
// /// Post request with Byte [] -> string
let personsJson = $"{CommonAPIStr}getPersonsJson"
// /// Post request with int -> string
let personsXLSX = $"{CommonAPIStr}getPersonsXlsx"


Http.RequestString(
    personsJson,
    httpMethod = "GET"
)

Http.RequestString(
    personsXLSX,
    httpMethod = "GET"
)

Http.RequestString(
    CommonAPIStr,
    httpMethod = "DELETE"
)


Http.RequestString(
    $"{CommonAPIStr}arcs/sampleArc/assays/lelelelel/persons/lukasweil",
    httpMethod = "GET"
)
