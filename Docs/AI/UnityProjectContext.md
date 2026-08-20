# Unity Project Context

<!-- unity-onboarding:generated:start -->

## Project Summary

- Project root: `D:\Projects\Rap Way`
- Project: Rap Way, a mobile-first rap artist career simulator
- Last analyzed: 2026-08-20
- Last analyzed commit: `c73d2628` on branch `v3`

## Confirmed Environment

- Unity version: 6000.5.9f1 (revision b57deb96f08d)
- Render pipeline: Built-in Render Pipeline (no SRP package or assigned render-pipeline asset detected)
- Input system: Unity Input System (`activeInputHandler: 1`)
- UI system: uGUI/Canvas prefabs; no UXML or USS assets detected
- Confirmed target: Android (`com.Meepson.RapWay`, min SDK 25, target SDK 34, IL2CPP)
- Additional target intent: PC is planned but not yet validated in project settings or builds

## Important Packages And Frameworks

| Area | Finding | Confidence | Evidence |
| --- | --- | --- | --- |
| Dependency injection | VContainer from GitHub | Confirmed | `Packages/manifest.json`, `Assets/_Project/Code/App/Scopes/` |
| Async | Cysharp UniTask from GitHub | Confirmed | `Packages/manifest.json`, first-party services |
| UI | Unity uGUI 2.5.0 with prefab-based windows | Confirmed | `Packages/manifest.json`, `Assets/_Project/Prefabs/UI/` |
| Input | Unity Input System 1.20.0 | Confirmed | `Packages/manifest.json`, `ProjectSettings/ProjectSettings.asset` |
| Serialization | Newtonsoft Json 3.2.2 plus Unity serialization | Confirmed | `Packages/manifest.json`, save-system code |
| Ads | Clever Ads Solutions Unity package | Confirmed | `Packages/manifest.json` |

## Directory Structure

| Path | Purpose | Confidence | Evidence |
| --- | --- | --- | --- |
| `Assets/_Project/Code/App` | Composition roots, installers, startup flow | Confirmed | VContainer scopes and entry points |
| `Assets/_Project/Code/Core` | Shared runtime services: UI, save, time, audio, scenes | Confirmed | Representative service files |
| `Assets/_Project/Code/Domain` | Domain models, interfaces, events | Confirmed | Folder contents |
| `Assets/_Project/Code/Features` | Feature-specific code; currently prototype/test UI | Confirmed | Folder contents |
| `Assets/_Project/Code/UI` | Window base classes, HUD, widgets, safe-area helpers | Confirmed | Folder contents |
| `Assets/_Project/Data` | ScriptableObject configuration assets | Confirmed | UI and time configuration assets |
| `Assets/_Project/Prefabs/UI` | Canvas, HUD, buttons, and window prefabs | Confirmed | Prefab inventory |
| `Assets/_Project/Scenes` | Boot, MainMenu, Game, and Debug scenes | Confirmed | Scene inventory and Build Settings |

## Assembly Boundaries

| Assembly | Responsibility | Key references | Notes |
| --- | --- | --- | --- |
| `RapWay.Domain` | Engine-independent rules and facts | .NET Standard only | Compiled by both Unity and `DotNet/RapWay.Domain` |
| `RapWay.Application` | Engine-independent use cases and transaction coordination | `RapWay.Domain` | Compiled by both Unity and `DotNet/RapWay.Application` |
| `RapWay.Infrastructure` | Engine-independent external adapters | Domain, Application | Empty marker boundary introduced for incremental migration |
| `RapWay.Presentation.Unity` | Unity views, presenters, and visual adapters | Domain, Application | Empty marker boundary introduced for incremental migration |
| `RapWay.Composition.Unity` | Unity/VContainer composition | All target layers | Empty marker boundary introduced for incremental migration |
| `RapWay` | Legacy prototype compatibility | Target assemblies plus VContainer, UniTask, uGUI, project plugins | Temporary assembly retained to preserve serialized prototype scripts |

## Scenes And Startup Flow

- Enabled build scenes: `Boot`, `MainMenu`, `Game`
- Development-only scene: `Debug` (not enabled in Build Settings)
- Startup: `BootLifetimeScope` starts `BootController`; after a placeholder delay it loads `MainMenu`
- Game startup: `GameLifetimeScope` starts `GameStartup`, which opens `HUDWindow`
- MainMenu-to-Game transition: unknown from the representative code inspected

## Architecture

| Pattern | Finding | Confidence | Evidence |
| --- | --- | --- | --- |
| Composition | VContainer lifetime scopes and installer extensions | Confirmed | `App/Scopes`, `App/Installers` |
| UI navigation | Singleton `UIService` instantiates registered `BaseWindow` prefabs and maintains screen history | Confirmed | `Core/UI/UIService.cs`, `Data/UIConfig.cs` |
| Async flow | UniTask-based scene loading, UI transitions, save/load, and autosave | Confirmed | Core and App services |
| Configuration | ScriptableObject configs registered through DI | Confirmed | `TimeConfig`, `UIConfig`, `RootLifetimeScope` |
| Persistence | Save/load service over an abstract storage service, currently file-backed | Confirmed | `Core/SaveSystem` |
| Presentation separation | Early-stage window/view and presenter direction | Likely | `Features/Test/UI`, current window abstractions |

## Coding Conventions

- Namespace root: `RapWay`; one legacy `_Project` namespace reference remains
- Private fields: `_camelCase`; serialized fields commonly `camelCase`
- Formatting: 4 spaces, CRLF, braces required, explicit types preferred
- Async: UniTask/UniTaskVoid with `.Forget()` at entry-point boundaries
- Comments/docs: sparse; XML comments only where behavior benefits from explanation

## Testing And Validation

- Pure C# test entry point: `dotnet test RapWay.slnx --configuration Release`
- SDK: .NET `10.0.400`, pinned by `global.json`
- Test stack: NUnit `4.6.1`, NUnit3TestAdapter `6.2.0`, Microsoft.NET.Test.Sdk `18.8.1`
- Current pure test count: 41 architecture/kernel tests; all passed on 2026-08-20
- Domain and Application projects link the same source compiled by Unity; gameplay code is not duplicated
- Unity EditMode and PlayMode tests: none currently detected
- Files under `Features/Test` are UI prototypes, not automated tests

## Available Unity Tooling

| Capability | Status | Evidence |
| --- | --- | --- |
| Unity MCP bridge | available | Official Unity CLI `1.0.0-beta.5`, `unity mcp`, and `com.unity.pipeline` `0.5.0-exp.1` |
| Unity Editor connection | available | Editor `6000.5.9f1` reported `ready`; Pipeline server reachable locally |
| Repository editing | available | Local project is open in Codex |
| Console/scene/prefab live inspection | available | 142 Pipeline commands discovered; initial Console error check returned zero errors |
| Git over SSH | available | `origin` fetch validated on 2026-08-19 |

## Important Constraints

- Mobile-first UI must respect safe areas, variable aspect ratios, touch targets, and limited memory.
- Preserve the existing VContainer composition root and UniTask async conventions unless a migration is explicitly approved.
- Prefer Editor-safe scene and prefab edits through the connected Unity Pipeline/MCP tools.
- `game.keystore` exists at repository root. Treat signing material as sensitive and do not expose its contents.
- The project currently depends heavily on prefab-authored uGUI, which is harder to edit and validate reliably without Unity Editor automation.

## Unknowns And Confidence

- iOS configuration and build health are unverified.
- Windows/PC build configuration and input behavior are unverified.
- Unity 6000.5.9f1 imports and compiles the new target asmdefs. A 2026-08-20 smoke test reached `MainMenu` from `Boot` and completed the prototype save call.
- CharacterCreator2D currently throws an initialization exception because `CharacterUtility.Init` receives a missing shader. Treat URP 2D/material compatibility as unverified and resolve it at the Stage 4 presentation gate.
- Dynamic Batching emits a deprecation warning. Scene/prefab references beyond the smoke path, player builds, and device behavior still require dedicated validation.
- The intended visual design system, supported device matrix, localization scope, analytics, and monetization requirements are not documented.
- The best UI authoring path (continue uGUI versus incremental UI Toolkit adoption) requires one representative screen prototype and device validation.

## Source Files Inspected

- `README.md`, `.editorconfig`
- `ProjectSettings/ProjectVersion.txt`
- `ProjectSettings/ProjectSettings.asset`
- `ProjectSettings/EditorBuildSettings.asset`
- `ProjectSettings/GraphicsSettings.asset`
- `ProjectSettings/QualitySettings.asset`
- `Packages/manifest.json`
- `Assets/_Project/Code/RapWay.asmdef`
- `Assets/_Project/Code/Domain/RapWay.Domain.asmdef`
- `Assets/_Project/Code/Application/RapWay.Application.asmdef`
- `Assets/_Project/Code/Infrastructure/RapWay.Infrastructure.asmdef`
- `Assets/_Project/Code/Presentation.Unity/RapWay.Presentation.Unity.asmdef`
- `Assets/_Project/Code/Composition.Unity/RapWay.Composition.Unity.asmdef`
- `RapWay.slnx`, `global.json`, and projects/tests under `DotNet/`
- Representative files under `Assets/_Project/Code/App`, `Core`, `Data`, and `UI`
- First-party scene, prefab, and UI-asset inventories

<!-- unity-onboarding:generated:end -->
