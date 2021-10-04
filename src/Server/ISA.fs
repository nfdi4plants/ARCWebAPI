module ISA

open ISADotNet
open ISADotNet.XLSX

type SparseMatrixExtensions =

    static member toTable (matrix : SparseMatrix,?prefix,?separator) =
        let prefix = match prefix with | Some p -> p + " " | None -> ""
        let separator = Option.defaultValue '\t' separator |> string
        seq {
            for key in matrix.Keys do
                (prefix + key :: List.init matrix.Length (fun i -> matrix.TryGetValueDefault("",(key,i))))
                |> List.reduce (fun a b -> a + separator + b)
            for key in matrix.CommentKeys do
                (Comment.wrapCommentKey key :: List.init matrix.Length (fun i -> matrix.TryGetValueDefault("",(key,i))))
                |> List.reduce (fun a b -> a + separator + b)
        }
        |> Seq.reduce (fun a b -> a + "\n" + b) 


module Json = 


    let vizualizeJson persons =
        System.Text.Json.JsonSerializer.Serialize(persons,options = ISADotNet.JsonExtensions.options)

    let vizualizeXLSX persons =
        persons
        |> Contacts.toSparseMatrix
        |> SparseMatrixExtensions.toTable

    //let getPersonsJson () = getPersons Arc.arcPath |> vizualizeJson
    //let getPersonsXlsx () = getPersons Arc.arcPath |> vizualizeXLSX