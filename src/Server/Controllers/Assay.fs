namespace Controllers

open ISADotNet.XLSX
open Saturn

module Assay =

    let getAssays arcPath = 
        let invAssays = 
            Arc.tryGetInvestigationFilePath arcPath 
            |> Option.bind (
                Investigation.fromFile 
                >> 
                (
                    fun i ->    
                        i.Studies 
                        |> Option.map (fun ss -> ss |> List.collect (fun s -> s.Assays |> Option.defaultValue []))
                )
            ) 
            |> Option.defaultValue []
        let assays = 
            Arc.tryGetAssayFilePaths arcPath
            |> Option.map (List.ofArray >> List.map (AssayFile.AssayFile.fromFile >> (fun (_,_,_,a) -> a))) 
            |> Option.defaultValue []
        assays @ invAssays
        |> List.distinct
    
    let tryPickAssayByName (fullName : string) (assays : ISADotNet.Assay list) =
        assays
        |> List.tryFind (fun a ->

            Option.defaultValue "" a.FileName
            |> fun a -> a.Contains fullName
        )

    let assayController arcName = controller {

        subController "/persons" (Person.assayPersonController arcName)

        index (fun ctx ->
            let arcPath = Arc.getArcPath arcName
            let assays = getAssays arcPath
            assays
            |> ISA.Persons.vizualizeJson
            |> Controller.text ctx
        )
        show (fun ctx assayName -> 
            let arcPath = Arc.getArcPath arcName
            let assays = getAssays arcPath
            assays
            |> tryPickAssayByName assayName
            |> ISA.Persons.vizualizeJson
            |> Controller.text ctx
        )
    }