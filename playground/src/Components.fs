module Components

open Feliz
open Fable.Core
open Fable.Core.JsInterop
open Browser.Dom
open Shared

type jsx = JSX.Html

[<ReactMemoComponent(true)>]
let ToggleThemeButton (theme: string, setTheme: string -> unit) =
    Html.button [
        prop.text "Toggle Theme"
        prop.onClick (fun _ -> 
            if theme = "light" then setTheme "dark"
            else setTheme "light"
        )
    ]

let private useRenderAndDismountLogs (name: string) =
    console.log (sprintf "[%s] onRender" name)

    let dismountLogger: unit -> (unit -> unit) =
        fun () ->
            fun () ->
                console.log (sprintf "[%s] dismount" name)

    React.useEffectOnce(dismountLogger)

let private sectionStyle =
    [
        style.border(1, borderStyle.solid, "#d9d9d9")
        style.borderRadius 8
        style.padding 12
        style.marginBottom 12
    ]

let private mountControls (isMounted: bool) (setIsMounted: bool -> unit) =
    Html.div [
        prop.style [
            style.display.flex
            style.gap 8
            style.marginBottom 8
        ]
        prop.children [
            Html.button [
                prop.text "Mount"
                prop.disabled isMounted
                prop.onClick (fun _ -> setIsMounted true)
            ]
            Html.button [
                prop.text "Dismount"
                prop.disabled (not isMounted)
                prop.onClick (fun _ -> setIsMounted false)
            ]
        ]
    ]

[<ReactComponent>]
let private ExampleUseLayoutEffectUnit (dependencyToken: int) =
    let name = "useLayoutEffect(unit -> unit)"
    useRenderAndDismountLogs name

    React.useLayoutEffect(
        (fun () ->
            console.log (sprintf "[%s] setup dependencyToken=%i" name dependencyToken)
        ),
        [| box dependencyToken |]
    )

    Html.p [
        prop.text (sprintf "Mounted. dependencyToken=%i" dependencyToken)
    ]

[<ReactComponent>]
let private ExampleUseLayoutEffectCleanup (dependencyToken: int) =
    let name = "useLayoutEffect(unit -> (unit -> unit))"
    useRenderAndDismountLogs name

    let cleanupEffectFactory: unit -> (unit -> unit) =
        fun () ->
            console.log (sprintf "[%s] setup dependencyToken=%i" name dependencyToken)
            fun () ->
                console.log (sprintf "[%s] cleanup dependencyToken=%i" name dependencyToken)

    React.useLayoutEffect(cleanupEffectFactory, [| box dependencyToken |])

    Html.p [
        prop.text (sprintf "Mounted. dependencyToken=%i" dependencyToken)
    ]

[<ReactComponent>]
let private ExampleUseLayoutEffectDisposable (dependencyToken: int) =
    let name = "useLayoutEffect(unit -> IDisposable)"
    useRenderAndDismountLogs name

    React.useLayoutEffect(
        (fun () ->
            console.log (sprintf "[%s] setup dependencyToken=%i" name dependencyToken)
            FsReact.createDisposable(fun () ->
                console.log (sprintf "[%s] Dispose dependencyToken=%i" name dependencyToken)
            )
        ),
        [| box dependencyToken |]
    )

    Html.p [
        prop.text (sprintf "Mounted. dependencyToken=%i" dependencyToken)
    ]

[<ReactComponent>]
let private ExampleUseLayoutEffectDisposableOption (dependencyToken: int, createCleanup: bool) =
    let name = "useLayoutEffect(unit -> IDisposable option)"
    useRenderAndDismountLogs name

    let optionalDisposableFactory: unit -> System.IDisposable option =
        fun () ->
            console.log (sprintf "[%s] setup dependencyToken=%i createCleanup=%b" name dependencyToken createCleanup)
            if createCleanup then
                Some (FsReact.createDisposable(fun () ->
                    console.log (sprintf "[%s] Dispose dependencyToken=%i" name dependencyToken)
                ))
            else
                None

    React.useLayoutEffect(optionalDisposableFactory, [| box dependencyToken; box createCleanup |])

    Html.p [
        prop.text (sprintf "Mounted. dependencyToken=%i createCleanup=%b" dependencyToken createCleanup)
    ]

[<ReactComponent>]
let private ExampleUseLayoutEffectOnceUnit () =
    let name = "useLayoutEffectOnce(unit -> unit)"
    useRenderAndDismountLogs name

    React.useLayoutEffectOnce(fun () ->
        console.log (sprintf "[%s] setup" name)
    )

    Html.p [
        prop.text "Mounted. Check console for setup and dismount logs."
    ]

[<ReactComponent>]
let private ExampleUseLayoutEffectOnceCleanup () =
    let name = "useLayoutEffectOnce(unit -> (unit -> unit))"
    useRenderAndDismountLogs name

    let cleanupEffectFactory: unit -> (unit -> unit) =
        fun () ->
            console.log (sprintf "[%s] setup" name)
            fun () ->
                console.log (sprintf "[%s] cleanup" name)

    React.useLayoutEffectOnce(cleanupEffectFactory)

    Html.p [
        prop.text "Mounted. Check console for setup, cleanup, and dismount logs."
    ]

[<ReactComponent>]
let private ExampleUseLayoutEffectOnceDisposable () =
    let name = "useLayoutEffectOnce(unit -> IDisposable)"
    useRenderAndDismountLogs name

    React.useLayoutEffectOnce(fun () ->
        console.log (sprintf "[%s] setup" name)
        FsReact.createDisposable(fun () ->
            console.log (sprintf "[%s] Dispose" name)
        )
    )

    Html.p [
        prop.text "Mounted. Check console for setup, Dispose, and dismount logs."
    ]

[<ReactComponent>]
let private ExampleUseLayoutEffectOnceDisposableOption (createCleanup: bool) =
    let name = "useLayoutEffectOnce(unit -> IDisposable option)"
    useRenderAndDismountLogs name

    let optionalDisposableFactory: unit -> System.IDisposable option =
        fun () ->
            console.log (sprintf "[%s] setup createCleanup=%b" name createCleanup)
            if createCleanup then
                Some (FsReact.createDisposable(fun () ->
                    console.log (sprintf "[%s] Dispose" name)
                ))
            else
                None

    React.useLayoutEffectOnce(optionalDisposableFactory)

    Html.p [
        prop.text (sprintf "Mounted. createCleanup=%b. Remount after toggling to test both paths." createCleanup)
    ]

[<ReactComponent>]
let private SectionUseLayoutEffectUnit () =
    let isMounted, setIsMounted = React.useState true
    let dependencyToken, setDependencyToken = React.useStateWithUpdater 0

    Html.div [
        prop.style sectionStyle
        prop.children [
            Html.h3 [ prop.text "1) useLayoutEffect(unit -> unit)" ]
            mountControls isMounted setIsMounted
            Html.button [
                prop.text "Bump dependency"
                prop.onClick (fun _ -> setDependencyToken(fun value -> value + 1))
            ]
            if isMounted then
                ExampleUseLayoutEffectUnit(dependencyToken)
            else
                Html.p [ prop.text "Component is dismounted." ]
        ]
    ]

[<ReactComponent>]
let private SectionUseLayoutEffectCleanup () =
    let isMounted, setIsMounted = React.useState true
    let dependencyToken, setDependencyToken = React.useStateWithUpdater 0

    Html.div [
        prop.style sectionStyle
        prop.children [
            Html.h3 [ prop.text "2) useLayoutEffect(unit -> (unit -> unit))" ]
            mountControls isMounted setIsMounted
            Html.button [
                prop.text "Bump dependency"
                prop.onClick (fun _ -> setDependencyToken(fun value -> value + 1))
            ]
            if isMounted then
                ExampleUseLayoutEffectCleanup(dependencyToken)
            else
                Html.p [ prop.text "Component is dismounted." ]
        ]
    ]

[<ReactComponent>]
let private SectionUseLayoutEffectDisposable () =
    let isMounted, setIsMounted = React.useState true
    let dependencyToken, setDependencyToken = React.useStateWithUpdater 0

    Html.div [
        prop.style sectionStyle
        prop.children [
            Html.h3 [ prop.text "3) useLayoutEffect(unit -> IDisposable)" ]
            mountControls isMounted setIsMounted
            Html.button [
                prop.text "Bump dependency"
                prop.onClick (fun _ -> setDependencyToken(fun value -> value + 1))
            ]
            if isMounted then
                ExampleUseLayoutEffectDisposable(dependencyToken)
            else
                Html.p [ prop.text "Component is dismounted." ]
        ]
    ]

[<ReactComponent>]
let private SectionUseLayoutEffectDisposableOption () =
    let isMounted, setIsMounted = React.useState true
    let dependencyToken, setDependencyToken = React.useStateWithUpdater 0
    let createCleanup, setCreateCleanup = React.useState true

    Html.div [
        prop.style sectionStyle
        prop.children [
            Html.h3 [ prop.text "4) useLayoutEffect(unit -> IDisposable option)" ]
            mountControls isMounted setIsMounted
            Html.div [
                prop.style [ style.display.flex; style.gap 8; style.marginBottom 8 ]
                prop.children [
                    Html.button [
                        prop.text "Bump dependency"
                        prop.onClick (fun _ -> setDependencyToken(fun value -> value + 1))
                    ]
                    Html.button [
                        prop.text (sprintf "Toggle optional cleanup (now %b)" createCleanup)
                        prop.onClick (fun _ -> setCreateCleanup(not createCleanup))
                    ]
                ]
            ]
            if isMounted then
                ExampleUseLayoutEffectDisposableOption(dependencyToken, createCleanup)
            else
                Html.p [ prop.text "Component is dismounted." ]
        ]
    ]

[<ReactComponent>]
let private SectionUseLayoutEffectOnceUnit () =
    let isMounted, setIsMounted = React.useState true

    Html.div [
        prop.style sectionStyle
        prop.children [
            Html.h3 [ prop.text "5) useLayoutEffectOnce(unit -> unit)" ]
            mountControls isMounted setIsMounted
            if isMounted then
                ExampleUseLayoutEffectOnceUnit()
            else
                Html.p [ prop.text "Component is dismounted." ]
        ]
    ]

[<ReactComponent>]
let private SectionUseLayoutEffectOnceCleanup () =
    let isMounted, setIsMounted = React.useState true

    Html.div [
        prop.style sectionStyle
        prop.children [
            Html.h3 [ prop.text "6) useLayoutEffectOnce(unit -> (unit -> unit))" ]
            mountControls isMounted setIsMounted
            if isMounted then
                ExampleUseLayoutEffectOnceCleanup()
            else
                Html.p [ prop.text "Component is dismounted." ]
        ]
    ]

[<ReactComponent>]
let private SectionUseLayoutEffectOnceDisposable () =
    let isMounted, setIsMounted = React.useState true

    Html.div [
        prop.style sectionStyle
        prop.children [
            Html.h3 [ prop.text "7) useLayoutEffectOnce(unit -> IDisposable)" ]
            mountControls isMounted setIsMounted
            if isMounted then
                ExampleUseLayoutEffectOnceDisposable()
            else
                Html.p [ prop.text "Component is dismounted." ]
        ]
    ]

[<ReactComponent>]
let private SectionUseLayoutEffectOnceDisposableOption () =
    let isMounted, setIsMounted = React.useState true
    let createCleanup, setCreateCleanup = React.useState true

    Html.div [
        prop.style sectionStyle
        prop.children [
            Html.h3 [ prop.text "8) useLayoutEffectOnce(unit -> IDisposable option)" ]
            mountControls isMounted setIsMounted
            Html.button [
                prop.text (sprintf "Toggle optional cleanup (now %b)" createCleanup)
                prop.onClick (fun _ -> setCreateCleanup(not createCleanup))
            ]
            if isMounted then
                ExampleUseLayoutEffectOnceDisposableOption(createCleanup)
            else
                Html.p [ prop.text "Component is dismounted." ]
        ]
    ]

[<ReactComponent>]
let Main () =
    Html.div [
        Html.h2 [
            prop.text "useLayoutEffect overload visual checker"
        ]
        Html.p [
            prop.text "Each section has its own component with Mount and Dismount buttons."
        ]
        Html.p [
            prop.text "Check browser console for onRender and dismount logs."
        ]
        Html.p [
            prop.text "Note: React StrictMode can trigger additional mount/unmount cycles in development."
        ]

        SectionUseLayoutEffectUnit()
        SectionUseLayoutEffectCleanup()
        SectionUseLayoutEffectDisposable()
        SectionUseLayoutEffectDisposableOption()
        SectionUseLayoutEffectOnceUnit()
        SectionUseLayoutEffectOnceCleanup()
        SectionUseLayoutEffectOnceDisposable()
        SectionUseLayoutEffectOnceDisposableOption()
    ]
