module Server

open Giraffe
open Saturn


module Routers = 
  
    //let arcRouter = router {
    //    //forward "/assays" Controllers.Assay.assayController
    //    forward "/arc" Controllers.Arc.arcController
    //}

    let apiRouter = router {
        not_found_handler (setStatusCode 404 >=> text "Api 404")
        forward "/arcs" Controllers.Arc.arcController
    }

    let router = router {
        forward "/api" apiRouter
    }

let app =
    application {
        url "http://0.0.0.0:8085"
        use_router Routers.router
        memory_cache
        use_static "public"
        use_gzip
    }

run app
