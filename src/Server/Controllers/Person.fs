namespace Controllers

open ISADotNet.XLSX
open Saturn

module Person = 

    let getPersons arcPath = 
        let invPersons = Arc.tryGetInvestigationFilePath arcPath |> Option.bind (Investigation.fromFile >> fun i -> i.Contacts) |> Option.defaultValue []
        let assayPersons = 
            Arc.tryGetAssayFilePaths arcPath
            |> Option.map (List.ofArray >> List.collect (AssayFile.AssayFile.fromFile >> (fun (_,_,p,_) -> p))) 
            |> Option.defaultValue []
        assayPersons @ invPersons
        |> List.distinct

    let getAssayPersons (assayName : string) arcPath = 
        let assayPersons = 
            Arc.tryGetAssayFilePaths arcPath
            |> Option.bind (fun fps -> fps |> Array.tryPick (fun fp -> if fp.Contains assayName then Some fp else None))
            |> Option.map (AssayFile.AssayFile.fromFile >> (fun (_,_,p,_) -> p)) 
            |> Option.defaultValue []
        assayPersons

    //let getStudyPersons (studyName : string) arcPath = 
    //    let invPersons = Arc.tryGetInvestigationFilePath arcPath |> Option.bind (Investigation.fromFile >> fun i -> i.Contacts) |> Option.defaultValue []
    //    let assayPersons = 
    //        Arc.tryGetAssayFilePaths arcPath
    //        |> Option.map (List.ofArray >> List.collect (AssayFile.AssayFile.fromFile >> (fun (_,_,p,_) -> p))) 
    //        |> Option.defaultValue []
    //    assayPersons @ invPersons
    //    |> List.distinct

    let tryPickPersonByFullName (fullName : string) (persons : ISADotNet.Person list) =
        persons
        |> List.tryFind (fun p ->
            let fn,mn,ln = (Option.defaultValue "" p.FirstName).ToLower(),(Option.defaultValue "" p.MidInitials).ToLower(),(Option.defaultValue "" p.LastName).ToLower()
            let fullName = fullName.ToLower()
            fullName = fn + mn + ln || fullName = fn + ln || fullName = fn + "_" + ln || fullName = ln + fn || fullName = ln + mn + fn
        )

    let personController (arcName : string) = controller {
        index (fun ctx -> 
            let arcPath = Arc.getArcPath arcName
            let persons = getPersons arcPath
            persons
            |> ISA.Json.vizualizeJson
            |> Controller.text ctx
        )
        show (fun ctx personName -> 
            let arcPath = Arc.getArcPath arcName
            let persons = getPersons arcPath
            persons
            |> tryPickPersonByFullName personName
            |> ISA.Json.vizualizeJson
            |> Controller.text ctx
        )
    }

    let studyPersonController studyName = controller {
        index (fun ctx -> (sprintf "Person Index handler for study %i" studyName) |> Controller.text ctx)
        show (fun ctx personName -> (sprintf "Show Person %s handler for study %i" personName studyName ) |> Controller.text ctx)
    }

    let assayPersonController (arcName : string) assay = controller {
        index (fun ctx -> 
            let arcPath = Arc.getArcPath arcName
            let persons = getAssayPersons assay arcPath
            persons
            |> ISA.Json.vizualizeJson
            |> Controller.text ctx
        )
        show (fun ctx personName -> 
            let arcPath = Arc.getArcPath arcName
            let persons = getAssayPersons assay arcPath
            persons
            |> tryPickPersonByFullName personName
            |> ISA.Json.vizualizeJson
            |> Controller.text ctx
        )
    }

    //let assayPersonController2 (assay : ISADotNet.Assay) = controller {
    //    index (fun ctx -> (sprintf "Person Index handler for assay %s" assay.FileName.Value) |> Controller.text ctx)
    //    show (fun ctx personName -> (sprintf "Show Person %s handler for assay %s" personName assay.FileName.Value ) |> Controller.text ctx)
    //}
