module Arc

open System.IO


let arcBasePath = @"C:\Users\HLWei\source\diverse\"

let getArcPath arcName = arcBasePath + @"\" + arcName

let listArcs () = 
    DirectoryInfo(arcBasePath).GetDirectories() |> Array.map (fun a -> a.Name)

let tryGetAssayFilePaths arcPath = 
    try Directory.GetDirectories(Path.Combine(arcPath,"assays")) |> Some with
    | _ -> None   
    |> Option.map (Array.choose (fun d -> 
        Directory.GetFiles d
        |> Array.tryPick (fun p -> if p.Contains "isa.assay.xlsx" then Some p else None)
    ))

let tryGetInvestigationFilePath arcPath = 
    Directory.GetFiles(arcPath)
    |> Array.tryPick (fun p -> if p.Contains "isa.investigation.xlsx" then Some p else None)
        