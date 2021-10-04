namespace Controllers

open ISADotNet.XLSX
open Saturn

module Protocol = 

    let getProtocols arcPath = 
        let invProtocols = 
            Arc.tryGetInvestigationFilePath arcPath 
            |> Option.bind (fun p ->
                Investigation.fromFile p
                |> ISADotNet.API.Investigation.getStudies
                |> Option.map (List.collect (fun s -> s.Protocols |> Option.defaultValue []))
            )
            |> Option.defaultValue []

        let assayProtocols = 
            Arc.tryGetAssayFilePaths arcPath
            |> Option.map (List.ofArray >> List.collect (AssayFile.AssayFile.fromFile >> (fun (_,p,_,_) -> p))) 
            |> Option.defaultValue []
        assayProtocols @ invProtocols
        |> List.distinct

    let getAssayProtocols (assayName : string) arcPath = 
        let assayPersons = 
            Arc.tryGetAssayFilePaths arcPath
            |> Option.bind (fun fps -> fps |> Array.tryPick (fun fp -> if fp.Contains assayName then Some fp else None))
            |> Option.map (AssayFile.AssayFile.fromFile >> (fun (_,p,_,_) -> p)) 
            |> Option.defaultValue []
        assayPersons
        

    let protocolController (arcName : string) = controller {
        index (fun ctx -> 
            let arcPath = Arc.getArcPath arcName
            let protocols = getProtocols arcPath
            protocols
            |> ISA.Json.vizualizeJson
            |> Controller.text ctx
        )
        show (fun ctx protocolName -> 
            let arcPath = Arc.getArcPath arcName
            let protocols = getProtocols arcPath
            protocols
            |> ISADotNet.API.Protocol.tryGetByName protocolName
            |> ISA.Json.vizualizeJson
            |> Controller.text ctx
        )
    }

    let studyProtocolController studyName = controller {
        index (fun ctx -> (sprintf "Person Index handler for study %i" studyName) |> Controller.text ctx)
        show (fun ctx personName -> (sprintf "Show Person %s handler for study %i" personName studyName ) |> Controller.text ctx)
    }

    let assayProtocolController (arcName : string) assay = controller {
        index (fun ctx -> 
            let arcPath = Arc.getArcPath arcName
            let persons = getAssayProtocols assay arcPath
            persons
            |> ISA.Json.vizualizeJson
            |> Controller.text ctx
        )
        show (fun ctx protocolName -> 
            let arcPath = Arc.getArcPath arcName
            let persons = getAssayProtocols assay arcPath
            persons
            |> ISADotNet.API.Protocol.tryGetByName protocolName
            |> ISA.Json.vizualizeJson
            |> Controller.text ctx
        )
    }

    //let assayPersonController2 (assay : ISADotNet.Assay) = controller {
    //    index (fun ctx -> (sprintf "Person Index handler for assay %s" assay.FileName.Value) |> Controller.text ctx)
    //    show (fun ctx personName -> (sprintf "Show Person %s handler for assay %s" personName assay.FileName.Value ) |> Controller.text ctx)
    //}
