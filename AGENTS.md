# Rap Way Agent Guide

## Product Direction

- Rap Way is an indie, mobile-first rap career simulator with a possible PC release.
- Optimize for a small team: simple systems, fast iteration, reusable content, and low maintenance cost.
- Treat product feel, readable UI, short feedback loops, and reliable saves as higher priority than speculative abstractions.
- Ask before changing product direction, adding a paid/service dependency, performing a broad migration, or deleting content.

## Sources Of Truth

- Use Unity `6000.5.9f1`; do not upgrade the Editor or packages incidentally.
- Treat `GAME_DESIGN_DOCUMENT.md` as the product source of truth for agreed gameplay mechanics, scope, and design constraints.
- Read `Docs/AI/UnityProjectContext.md` when onboarding or when architecture context is needed.
- First-party code and content live under `Assets/_Project/`. Treat other top-level `Assets/` folders as vendor/imported unless verified otherwise.
- Preserve user changes. Check `git status` before and after work; never reset, clean, or overwrite unrelated files.

## Required Workflow

1. Inspect the relevant code, assets, scene/prefab wiring, and existing conventions before editing.
2. Prefer the smallest coherent change that completes the requested behavior.
3. Use the connected Unity Pipeline/MCP for scenes, prefabs, GameObjects, serialized references, Console, tests, and validation.
4. Do not hand-edit Unity YAML (`.unity`, `.prefab`, `.asset`, `.mat`, `.controller`) unless Editor-safe tooling cannot do the job and the IDs/format are understood.
5. After C# changes, trigger compilation, wait for completion, and inspect Unity Console errors.
6. Validate the affected flow at mobile aspect ratios. Run relevant tests; add a regression test for a fixed logic bug when practical.
7. Report what changed, what was validated, and any remaining uncertainty.

## Architecture

- Keep `MonoBehaviour` views thin. Put game rules in plain C# domain/application services that can be tested without Unity scenes.
- Use VContainer constructor injection. Register dependencies in scopes/installers; avoid new global singletons, service locators, and hidden `Find` calls.
- Use UniTask for asynchronous Unity work. Pass cancellation tied to object/application lifetime where work can outlive a frame.
- Use `.Forget()` only at deliberate entry-point boundaries and ensure failures are logged or handled.
- Use ScriptableObjects for authoring/configuration, not mutable runtime state. Keep save DTOs explicit and versionable.
- Organize new gameplay by feature while keeping shared infrastructure in `Core`, composition in `App`, and pure models/contracts in `Domain`.
- Prefer events/messages for cross-feature notifications; prefer direct calls inside one cohesive feature.

## C# Style

- Follow `.editorconfig`: four spaces, braces, explicit accessibility, and sorted `using` directives.
- Use namespace root `RapWay`. Code identifiers and technical comments are English; player-facing text must be localizable.
- Use `PascalCase` for types/members, `camelCase` for locals/parameters, `_camelCase` for private fields, and `IName` for interfaces.
- Prefer `[SerializeField] private` over public fields. Preserve serialized names or use `FormerlySerializedAs` when renaming.
- Keep one primary type per file and match file/type names. Avoid regions, clever abstractions, and comments that repeat the code.
- Avoid per-frame allocations, LINQ, reflection, scene searches, and repeated component lookups in hot paths. Optimize measured paths, not hypothetical ones.

## UI And UX

- New screen-heavy UI should use UI Toolkit (`UXML` structure, `USS` styling, C# behavior) unless an existing uGUI screen is being maintained or a required effect is unsupported.
- Do not mix UI Toolkit and uGUI inside one screen without a documented reason. Migrate existing uGUI incrementally, never wholesale without approval.
- Build reusable primitives and shared design tokens before duplicating styling. Avoid inline styles when a reusable USS class is appropriate.
- Design mobile-first: safe areas, adaptive layouts, readable type, touch targets of roughly 44-48 px minimum, and no hover-only interaction.
- Verify narrow/tall and wide mobile layouts, then mouse/keyboard behavior for PC. Keep navigation usable with touch, mouse, keyboard, and Back/Escape.
- Separate presentation from logic: views expose UI events/state; presenters/controllers coordinate services and domain models.
- Use DOTween or UI Toolkit transitions intentionally; animations must be interruptible and must not block core interactions.

## Tests And Validation

- Add EditMode tests for domain rules, calculations, state transitions, persistence migrations, and deterministic services.
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
