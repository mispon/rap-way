# Rap Way Agent Guide

## Product Direction

- Rap Way is an indie, mobile-first rap career simulator with a possible PC release.
- Optimize for a small team: simple systems, fast iteration, reusable content, and low maintenance cost.
- Treat product feel, readable UI, short feedback loops, and reliable saves as higher priority than speculative abstractions.
- Ask before changing product direction, adding a paid/service dependency, performing a broad migration, or deleting content.

## Sources Of Truth

- Use Unity `6000.5.9f1`; do not upgrade the Editor or packages incidentally.
- Treat `GAME_DESIGN_DOCUMENT.md` as the product source of truth for agreed gameplay mechanics, scope, and design constraints.
- Treat `TECHNICAL_DESIGN_DOCUMENT.md` as the source of truth for architecture, technology choices, dependency boundaries, and migration direction.
- Treat `ROADMAP.md` as the source of truth for implementation order. Keep exactly one numbered stage `In Progress`; do not begin a later system unless its dependencies and the active stage gate are satisfied.
- Read `Docs/AI/UnityProjectContext.md` when onboarding or when architecture context is needed.
- First-party code and content live under `Assets/_Project/`. Treat other top-level `Assets/` folders as vendor/imported unless verified otherwise.
- Preserve user changes. Check `git status` before and after work; never reset, clean, or overwrite unrelated files.

## Required Workflow

1. Inspect the relevant code, assets, scene/prefab wiring, and existing conventions before editing.
2. Check the active `ROADMAP.md` stage and keep the change within its goal, dependencies, and acceptance criteria unless the user explicitly reprioritizes it.
3. Prefer the smallest coherent change that completes the requested behavior.
4. Use the connected Unity Pipeline/MCP for scenes, prefabs, GameObjects, serialized references, Console, tests, and validation.
5. Do not hand-edit Unity YAML (`.unity`, `.prefab`, `.asset`, `.mat`, `.controller`) unless Editor-safe tooling cannot do the job and the IDs/format are understood.
6. After C# changes, trigger compilation, wait for completion, and inspect Unity Console errors.
7. Validate the affected flow at mobile aspect ratios. Run relevant tests; add a regression test for a fixed logic bug when practical.
8. Report what changed, what was validated, and any remaining uncertainty.

## Architecture

- Keep `MonoBehaviour` views thin. `RapWay.Domain` and `RapWay.Application` must not reference Unity or Unity-specific packages and must be testable with `dotnet test`.
- Use VContainer constructor injection. Register dependencies in scopes/installers; avoid new global singletons, service locators, and hidden `Find` calls.
- Keep state-changing simulation synchronous. Use standard `Task`/`ValueTask` in engine-independent ports, Unity `Awaitable` for simple Unity-native operations, and UniTask only in Unity-facing integrations that justify it, including MessagePipe and selected DOTween workflows.
- Pass cancellation tied to application, session, scene, or screen lifetime. Fire-and-forget is allowed only at deliberate entry-point boundaries where failures are observed and logged.
- Store gameplay definitions in validated JSON with stable IDs. Limit ScriptableObjects to Unity asset references and presentation/import configuration; never use them for mutable runtime state.
- Follow the directed assemblies in `TECHNICAL_DESIGN_DOCUMENT.md`: Domain, Application, Infrastructure, Presentation.Unity, and Composition.Unity. Group related code by feature inside those layers.
- Use explicit commands for intent and ordered domain events for committed facts. PubSub is not a command or query mechanism; prefer direct calls inside one cohesive feature.

## C# Style

- Follow `.editorconfig`: four spaces, braces, explicit accessibility, and sorted `using` directives.
- Use namespace root `RapWay`. Code identifiers and technical comments are English; player-facing text must be localizable.
- Store player-facing text in feature-scoped Unity Localization String Table Collections. Russian is the source locale and English is required for release; never place translated prose directly in gameplay JSON.
- Unity String Table assets under `Assets/_Project/Localization/` are the sole source of truth for localization keys and translations. Do not hand-edit their YAML; use Unity Localization tools or `Rap Way/Localization` editor commands.
- Create one collection per feature (`UI.Common`, `Activities`, `News`, `Events`, etc.), not a project-wide text table. Use English collection names and lower-case semantic `snake_case` entry keys.
- UI code may use the generated `GameLocalizationKeys.cs` API, but never edit it manually. Gameplay JSON carries ordinary `{ "table", "key" }` references. Do not add a hand-maintained C# key list, translation seed data, or hard-coded fallback prose.
- Use stable semantic English keys and named Smart String arguments with typed contracts. Do not use positional placeholders, derive keys from source text, or assemble sentences from localized fragments.
- For every new text: add the key in the appropriate collection, fill Russian and English text, run `Rap Way/Localization/Validate Catalog`, and regenerate the key API before handoff. Missing or empty required-locale entries are defects.
- Use `PascalCase` for types/members, `camelCase` for locals/parameters, `_camelCase` for private fields, and `IName` for interfaces.
- Prefer `[SerializeField] private` over public fields. Preserve serialized names or use `FormerlySerializedAs` when renaming.
- Keep one primary type per file and match file/type names. Avoid regions, clever abstractions, and comments that repeat the code.
- Avoid per-frame allocations, LINQ, reflection, scene searches, and repeated component lookups in hot paths. Optimize measured paths, not hypothetical ones.

## UI And UX

- New screen-heavy UI should use UI Toolkit (`UXML` structure, `USS` styling, C# behavior) unless an existing uGUI screen is being maintained or a required effect is unsupported.
- Do not mix UI Toolkit and uGUI inside one screen without a documented reason. Replace the legacy uGUI prototype after the approved UI Toolkit shell and vertical slice are functional.
- Build reusable primitives and shared design tokens before duplicating styling. Avoid inline styles when a reusable USS class is appropriate.
- Design mobile-first: safe areas, adaptive layouts, readable type, touch targets of roughly 44-48 px minimum, and no hover-only interaction.
- Verify narrow/tall and wide mobile layouts, then mouse/keyboard behavior for PC. Keep navigation usable with touch, mouse, keyboard, and Back/Escape.
- Validate UI with pseudo-localization. Missing keys, stale required translations, placeholder mismatches, clipping, and missing Cyrillic/Latin glyphs are release-blocking defects.
- Separate presentation from logic: views expose UI events/state; presenters/controllers coordinate services and domain models.
- Use DOTween or UI Toolkit transitions intentionally; animations must be interruptible and must not block core interactions.

## Tests And Validation

- Add fast NUnit tests through `dotnet test` for domain rules, calculations, state transitions, persistence migrations, and deterministic services. Use Unity EditMode tests only when Unity APIs or assembly integration are required.
- Add PlayMode tests sparingly for scene wiring, navigation, prefab integration, and critical end-to-end flows.
- Tests must be deterministic and independent of execution order, network access, real time, and production save data.
- A bug fix should reproduce the failure first when feasible. Do not weaken assertions merely to make a test pass.
- Completion requires zero new Console errors. Warnings introduced by the change must be resolved or explicitly justified.

## Assets, Data, And Performance

- Always keep Unity `.meta` files with their assets. Never regenerate GUIDs or move/rename referenced assets casually.
- Do not commit `Library`, `Temp`, `Obj`, `Logs`, `UserSettings`, generated IDE files, or local builds.
- Treat `game.keystore`, credentials, signing data, tokens, and service configuration as sensitive; never print or copy their contents.
- Ask before installing/upgrading packages. Record why a dependency is needed and prefer maintained, narrowly scoped packages.
- For mobile, watch texture/audio import settings, memory, overdraw, Canvas/UI rebuilds, startup time, and save durability.

## Git And Delivery

- Keep commits focused and reviewable. Do not commit, push, create branches, or open PRs unless explicitly requested.
- Do not mix formatting churn, generated changes, package upgrades, and gameplay work in one change.
- Before handoff, inspect the diff and ensure only intended project files changed.
- Never claim a scene, prefab, test, build, or device flow works without actual validation evidence.
