namespace Controllers

open Saturn

module Arc =

    let arcController = controller {

        subController "/assays" Assay.assayController
        //subController "/persons" Person.personController

        index (fun ctx -> 
            Arc.arcBasePath
            |> System.IO.Directory.GetDirectories
            |> Array.reduce (fun a b -> a + "\n" + b)        
            |> Controller.text ctx
        )
        show (fun ctx arcName -> 
            let arcPath = Arc.getArcPath arcName
            let inv = Arc.tryGetInvestigationFilePath arcPath
            match inv with
            | Some p -> 
                ISADotNet.XLSX.Investigation.fromFile p          
                |> ISA.Persons.vizualizeJson
                |> Controller.text ctx
            | None -> failwithf "arc %s has no investigation" arcPath
        )
    }