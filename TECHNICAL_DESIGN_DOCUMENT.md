# Rap Way Technical Design Document

Version: 0.1

Status: Living document

Last updated: 2026-08-20

## 1. Purpose and authority

This document defines the target architecture, engineering constraints, technology choices, and validation strategy for Rap Way.

- `GAME_DESIGN_DOCUMENT.md` is authoritative for product behavior and game mechanics.
- This document is authoritative for technical structure and implementation boundaries.
- `AGENTS.md` defines day-to-day contribution rules.
- `Docs/AI/UnityProjectContext.md` records the observed state of the repository and may lag behind this target design.

When the implementation and this document disagree, the mismatch must be called out. Existing prototype code does not override an accepted decision in this document.

## 2. Engineering goals

Rap Way should be:

- Deterministic enough to reproduce simulation failures from state, random streams, and commands.
- Testable without starting Unity for the majority of game rules.
- Reliable under save/load, application suspension, interrupted writes, and schema evolution.
- Mobile-first without embedding mobile or Unity concepts into game rules.
- Simple enough for a very small team to understand and change safely.
- Modular without becoming a collection of frameworks and micro-assemblies.

The architecture optimizes for correctness, iteration speed, readable control flow, and low maintenance cost. It does not optimize for hypothetical scale.

## 3. System boundary

Unity is the rendering, input, audio, animation, scene, and platform shell. The game simulation is engine-independent C#.

The following layers must not reference Unity or Unity-specific packages:

- `RapWay.Domain`
- `RapWay.Application`

This includes a ban on `UnityEngine`, `MonoBehaviour`, `ScriptableObject`, `Vector2`, `Mathf`, Unity serialization, VContainer, MessagePipe, UniTask, DOTween, and CharacterCreator2D in those layers.

Domain and Application must compile and run in a normal .NET test process.

## 4. Architectural shape

Rap Way is a modular monolith with five main runtime assemblies.

### 4.1 RapWay.Domain

Contains:

- Authoritative game state and its modules.
- Entities, stable IDs, value objects, and invariants.
- Domain rules and calculations.
- Domain events describing facts that already occurred.
- Deterministic time, numeric, and random abstractions that are part of the simulation model.

Dependencies: standard .NET APIs allowed by the shared target only.

### 4.2 RapWay.Application

Contains:

- Commands and command handlers.
- Use-case orchestration.
- Simulation-step transaction boundary.
- Typed results and expected failure reasons.
- Ports for persistence, committed-event publication, logging, and other external capabilities.

Dependencies: Domain only.

### 4.3 RapWay.Infrastructure

Contains:

- Save DTOs, JSON serialization, migrations, checksums, and file storage.
- Content loading and validation.
- MessagePipe adapters.
- Local diagnostics and platform-independent infrastructure implementations.

Dependencies: Domain, Application, and narrowly scoped non-Unity libraries such as Newtonsoft.Json.

Infrastructure code that requires Unity must be placed in a Unity-specific assembly or namespace rather than leaking Unity references into the engine-independent assembly.

### 4.4 RapWay.Presentation.Unity

Contains:

- UI Toolkit views and presenters.
- Unity input, scene, audio, and visual adapters.
- DOTween animations.
- CharacterCreator2D integration.
- URP-specific visual behavior.
- Unity-side advertisement integration.

Dependencies: Application, approved Infrastructure contracts, Unity APIs, and presentation-side packages.

### 4.5 RapWay.Composition.Unity

Contains:

- VContainer lifetime scopes and registrations.
- Unity startup and shutdown wiring.
- Adapter selection and environment configuration.

It must not contain game rules or become a service locator.

### 4.6 Feature organization

Within a layer, code is grouped by gameplay feature where useful, for example `Career`, `Character`, `Music`, `World`, `Economy`, and `Industry`.

Do not create an assembly, DI module, repository, or base-class hierarchy for every feature by default. New physical boundaries require a demonstrated ownership, compile-time, reuse, or dependency benefit.

## 5. Authoritative state

There is one authoritative `GameState` per active game session. It is divided into explicit state modules such as:

- `WorldState`
- `PlayerState`
- `IndustryState`
- `EconomyState`
- `CalendarState`
- `RandomState`
- `ScheduledEventState`

Exact modules may evolve, but ownership rules do not:

- Gameplay state is serializable and belongs to `GameState`.
- Services must not hide authoritative gameplay data in singleton fields.
- Relationships between entities use stable IDs, not Unity references or direct references into presentation objects.
- State changes occur only through an Application command transaction.
- Read models and UI models are derived data, not alternate authorities.
- Only one `GameState` is active at a time.

Rap Way uses snapshot persistence, not event sourcing. Domain events are not the permanent source of truth.

## 6. Commands, results, and events

The standard flow is:

```text
Player or system intent
    -> typed Command
    -> one Command Handler
    -> validation and state mutation
    -> typed Result
    -> ordered Domain Events
    -> commit
    -> publication of committed facts
    -> presentation and external side effects
```

### 6.1 Commands

- Commands are imperative requests such as `ReleaseTrack`, `WorkShift`, or `TravelToDistrict`.
- A command has exactly one handler.
- Commands are not sent through PubSub.
- Expected rejection is returned as a typed result, not thrown as an exception.
- Command handlers are explicit and discoverable; a mediator framework is not required.

### 6.2 Domain events

- Events are immutable facts named in the past tense, such as `TrackReleased` or `WorkShiftCompleted`.
- Events are created only after their underlying state transition succeeds.
- State-changing handlers run synchronously in a stable order inside the simulation step.
- Presentation and external side effects observe events only after commit.
- Queries and request/response workflows do not use the event bus.

### 6.3 MessagePipe

MessagePipe is the Unity/runtime adapter for distributing committed facts to independent subscribers.

- MessagePipe types do not appear in Domain or Application.
- The broker is scoped to the active game session.
- `GlobalMessagePipe` is prohibited.
- Subscriptions must be owned by a lifetime scope or explicitly disposed with their view.
- Async subscribers cannot retroactively change a committed simulation result.

## 7. Simulation transaction

A simulation command is synchronous and atomic in memory.

The target execution sequence is:

1. Receive a typed command.
2. Check preconditions against the current `GameState` and immutable content catalog.
3. Execute domain changes using deterministic time, random, and numeric services.
4. Collect resulting domain events in order.
5. Validate critical postconditions.
6. Commit the new state.
7. Publish committed events.
8. Schedule optional external work such as autosave, audio, animation, or ads.

If the command is rejected, no partial state change or committed event may remain.

Copying the entire state for every command is not required. Atomicity may be implemented through disciplined mutation, scoped change sets, or feature-specific transactions, provided rollback and failure behavior are covered by tests.

## 8. Time and scheduling

Simulation time is action-driven and discrete.

- The base unit is one in-game hour stored as an integer `TotalHours` value.
- Calendar dates use Gregorian rules at whole-hour precision; the initial technical ceiling is `5,000,000` elapsed hours.
- Days, weeks, months, years, dates, and ages derive from calendar rules.
- Actions have integer-hour durations.
- The world advances only when a command explicitly advances time.
- The simulation processes needs, deadlines, scheduled work, and events chronologically across the interval.
- Items scheduled for the same time use a stable secondary order.
- Closing or suspending the application freezes the world.
- Unity `Update`, `Tick`, `deltaTime`, and wall-clock time do not advance the simulation.

Unity may animate the visual transition between two committed times, but that animation is not the clock.

## 9. Deterministic randomness

All authoritative randomness is explicit.

- Domain code receives an `IRandom` abstraction.
- Random seed and generator state are stored in `GameState`.
- Separate named streams isolate systems such as world events, NPC decisions, release reception, and health risks.
- Tests can provide fixed or scripted random sequences.
- A resolved outcome is saved and is not rerolled after loading.
- `UnityEngine.Random`, `System.Random.Shared`, time-derived seeds, and hidden global random state are prohibited in simulation code.

Changing random algorithms is a save-compatibility decision and requires a migration or stream-version strategy.

Algorithm version 1 derives each stream from the master seed plus its stable name using FNV-1a and a SplitMix64-style finalizer, then generates values with xorshift64*. A committed golden-vector test protects this contract.

## 10. Numeric model and overflow safety

Authoritative simulation state does not use `float` or `double`.

- Money uses a `Money` value object backed by `long` minor units and is technically capped to `+/-9,000,000,000,000,000`; lower feature-specific caps may be introduced by gameplay rules.
- Percentages and multipliers use fixed-point integers such as basis points, where `10_000` represents 100%. The shared value is technically capped to `+/-1,000,000`; calculations specify toward-zero, away-from-zero, or nearest-away-from-zero rounding.
- Needs and bounded resources use explicit normalized integer ranges.
- Skills store durable XP; displayed level and progress derive from a progression rule.
- Probabilities use integer weights.
- Time uses integer hours.

Safety rules:

- Arithmetic in domain value objects is checked.
- Technical overflow is an invariant failure, never silent wraparound.
- Gameplay resources have explicit, documented caps.
- Central `FixedMath` helpers implement overflow-safe multiplication and division with specified rounding.
- Bounded resources change through methods such as `Gain`, `Spend`, and `Clamp`, not unrestricted field mutation.
- Boundary, cap, rounding, and overflow tests are mandatory for numeric value objects.

`BigInteger` and custom infinite-number systems are out of scope. Formatting abbreviated values belongs to Presentation.

## 11. NPC simulation

NPCs use ordinary C# data and systems, not Unity ECS.

- All NPCs use a compatible core data model.
- Simulation detail is dynamic rather than represented by different incompatible entity types.
- Active and significant NPCs receive detailed updates when affected by a command or scheduled event.
- Background NPCs are updated in deterministic daily or weekly batches where appropriate.
- Market populations may remain aggregated until an individual needs to be materialized.
- Indexes, scheduling, and explicit per-step budgets are preferred before multithreading or ECS.

Unity Jobs, Burst, and ECS require profiler evidence and a representative failing workload before adoption.

## 12. Gameplay content

Gameplay definitions are engine-independent JSON.

Examples include:

- Skills and progression rules.
- Activities, jobs, and costs.
- Events, conditions, weights, cooldowns, and outcomes.
- Districts, locations, housing, and services.
- Effects, items, contracts, and economy parameters.
- NPC archetypes and market definitions.

Content rules:

- Every definition has a stable string ID independent of file name and Unity GUID.
- JSON maps into explicit pure C# DTOs and then validated immutable definitions.
- Startup builds one read-only `GameContentCatalog`.
- Automated validation rejects duplicate IDs, broken references, invalid ranges, impossible conditions, and cycles where forbidden.
- Unknown fields and schema mismatches must be surfaced rather than silently ignored in development.
- ScriptableObjects are restricted to Unity asset references, import/render configuration, and presentation settings.
- Unity assets are referenced from gameplay data by stable keys resolved by a presentation asset catalog.

CSV is not part of the gameplay-content stack. Gameplay JSON may reference localization keys, but it never contains translated player-facing prose.

### 12.1 Localization

Unity Localization String Table Collections are the canonical store for player-facing text. Collections are split by feature, for example `UI.Common`, `UI.MainMenu`, `News`, and `Events`, rather than accumulated in one global table.

#### Ownership and boundaries

- Russian is the source locale. English is a required locale for the first release.
- Collection names, stable semantic keys, and placeholder names are English.
- Domain and Application emit facts and typed data; they never format localized sentences.
- Presentation resolves a localization key with a typed argument contract.
- Gameplay definitions may own variant keys, conditions, and weights. Localization owns only wording.
- The deterministic simulation selects and persists the variant key and its source data so loading a save or changing locale cannot reroll an event.

#### Templates and grammar

- Named Smart String arguments such as `{artistName}` and `{fanCount}` are required. Positional placeholders such as `{0}` are prohibited.
- Localized sentences are complete templates; they are not assembled from translated fragments.
- Smart Strings handle supported plural, number, and simple gender choices.
- Materially different grammar uses explicit complete variants.
- Arbitrary artist names are not automatically declined. Copy should keep names in safe grammatical forms where practical.
- Every template's placeholder set and argument types are validated across all required locales.

#### Translation workflow

- Translation states are `Draft`, `Reviewed`, and `Needs Review`.
- Changing Russian source copy marks existing translations `Needs Review` through project-owned revision metadata.
- AI-assisted English drafts are allowed but remain `Draft` until reviewed.
- Onboarding, navigation, monetization, and major event copy receive line-by-line review. Repetitive news/event variants additionally use automated checks and sampled human review.
- A versioned project glossary defines required translations for game terms, resources, roles, and recurring UI verbs.
- Development renders a conspicuous marker such as `[MISSING:News.Release.Success]` for an unresolved key.
- Production falls back to Russian rather than rendering an empty string.
- Release validation fails for missing keys, missing required-locale text, stale required translations, placeholder mismatches, or unsupported glyphs.

#### Authoring and tooling

A project-owned `LocalizationCatalogTool` uses Unity Localization editor APIs; table YAML is never edited manually. It should support:

- Adding or updating a shared key once with source text, feature ownership, context, and translator comments.
- Searching key usage and reporting unused or broken references.
- Generating typed key references such as `LocKeys.g.cs`.
- Validating completeness, translation state, named placeholders, argument contracts, duplicate intent, and forbidden positional placeholders.
- Producing a readable review report for source changes and release readiness.
- Optional Google Sheets or XLIFF exchange later; neither is the canonical runtime source.

Pseudo-localization is part of UI validation and deliberately expands/wraps strings to expose clipping and hard-coded text. The first release guarantees Cyrillic and Latin font coverage. RTL and CJK require a separate font, layout, input, and device-validation decision.

## 13. Persistence

Rap Way stores explicit versioned JSON snapshots.

### 13.1 Format

- `GameSaveDto` is explicit and does not contain arbitrary `object` dictionaries.
- `SchemaVersion` is independent from the application version.
- Save DTOs contain no Unity object references or vendor types.
- Newtonsoft.Json is the current serializer implementation.
- `TypeNameHandling` and implicit polymorphic type-name persistence are prohibited.
- Production compression is introduced only after measuring a real size or load-time problem.

### 13.2 Migrations

- Migrations are sequential and explicit: `v1 -> v2 -> v3`.
- An old DTO is migrated before conversion into current `GameState`.
- Migration tests use committed fixture saves for every supported schema.
- A schema change is incomplete until its migration and compatibility test exist.

### 13.3 Durability

- Save to a temporary file, flush, and atomically replace the main file.
- Maintain the primary save and two rotating backups.
- Store and verify a checksum to detect corruption.
- Store timestamps in UTC with an unambiguous representation.
- Recovery selects the newest valid backup and reports what happened.
- Autosave occurs after meaningful committed commands with debounce; it is not triggered from the domain clock.
- Save writes use immutable snapshots so a background write cannot observe concurrent mutation.

## 14. Dependency injection and lifetimes

VContainer is the composition technology. Constructor injection is the default for plain C# objects.

### 14.1 ApplicationScope

Lives for the process lifetime and owns:

- Immutable content catalog.
- Save storage and platform paths.
- Local diagnostics.
- Platform services and global settings.

### 14.2 GameSessionScope

Created by New Game or Load Game and destroyed on return to the main menu. It owns:

- One `GameState`.
- Command handlers and simulation coordinator.
- Calendar scheduler and random streams.
- Session-scoped MessagePipe.
- Session-specific read models and presenters where appropriate.

### 14.3 SceneScope

Owns only Unity objects associated with a loaded presentation scene.

Views and presenters are transient or explicitly controlled by the UI navigator. They are not global singletons.

Prohibited patterns:

- Static service locators.
- `GlobalMessagePipe`.
- Arbitrary `Container.Resolve()` calls outside composition/factory boundaries.
- Hidden scene searches such as repeated `FindObjectOfType`.
- Field injection into plain C# objects.

## 15. Async and concurrency

The simulation and state-changing command handlers are synchronous.

- Domain never exposes async APIs.
- Application ports use standard `Task`, `ValueTask`, and `CancellationToken` when I/O is genuinely asynchronous.
- Unity's `Awaitable` is preferred for simple Unity-native operations.
- UniTask remains a Unity-side infrastructure dependency because MessagePipe requires it and it provides useful selected integrations.
- UniTask types do not cross into Domain or Application.
- Long-running Unity work is cancelled with application, session, scene, or screen lifetime.
- Fire-and-forget is allowed only at an explicit top-level boundary that observes and logs failures.
- Async event reactions cannot mutate the already committed result of a simulation step.

Do not add multithreaded simulation until a measured workload requires it. Save serialization may move off the main thread only from a stable snapshot.

## 16. Unity presentation

### 16.1 UI Toolkit

UI Toolkit is the standard runtime UI system.

- UXML defines structure.
- USS defines styles and shared design tokens.
- C# views bind elements, render screen models, and emit user intents.
- Presenters translate state into screen models and intents into Application commands.
- Views do not query or mutate Domain directly.
- Explicit binding is preferred over reflection-heavy automatic binding.
- Mobile safe areas, touch targets, aspect ratios, and Back/Escape behavior are mandatory.

The presentation flow is one-way:

```text
GameState -> read/screen model -> View -> user intent -> Command -> GameState
```

### 16.2 uGUI removal

The existing uGUI code is a disposable prototype.

Removal sequence:

1. Build a minimal UI Toolkit application shell.
2. Implement a working Main Menu to Game Session to HUD vertical slice.
3. Validate mobile layouts and navigation.
4. Remove legacy uGUI prefabs, windows, widgets, services, and configuration.
5. Remove `com.unity.ugui` only after dependency inspection confirms no remaining consumer.

Do not maintain two permanent UI frameworks.

### 16.3 Navigation and scenes

- A minimal Bootstrap scene creates the Application scope and persistent UI Toolkit shell.
- Main Menu is a UI screen, not a separate scene.
- Starting or loading a game creates GameSessionScope.
- An additive Game scene contains cameras, backgrounds, visible characters, and other world presentation.
- UI screens are navigator-managed views, not scenes.
- A development-only Debug scene may remain independent.
- Scene reload must not destroy authoritative GameState.

### 16.4 Animation

DOTween is presentation-only.

- Tweens never determine simulation completion or game outcome.
- Screen-owned tweens are cancelled or killed on detach/disposal.
- Animations must be interruptible and must not block essential navigation.
- Reduced-motion support should be possible through centralized animation policy.

### 16.5 Input

Unity Input System is the input technology.

- Input actions are interpreted in Presentation and converted into UI intents or Application commands.
- Touch is primary; mouse and keyboard are supported for PC and Editor workflows.
- Back/Escape is routed through the UI navigator.
- Domain and Application never read Unity input devices.

## 17. Rendering and character visuals

### 17.1 Render pipeline

The target pipeline is URP with the 2D Renderer.

- Default sprite rendering is Unlit.
- HDR, depth/stencil, opaque texture, shadows, post-processing, and renderer features stay disabled until a concrete visual requirement justifies them.
- 2D lighting is introduced selectively.
- Mobile configuration is the baseline.
- A PC quality profile is created only with a validated PC build target.

CharacterCreator2D's six Shader Graph assets already target URP Sprite Lit/Unlit or Universal Unlit. No other project-specific shader or material conflict was found during the decision audit.

### 17.2 CharacterCreator2D boundary

CharacterCreator2D is the production character customization and assembly system, but it is not a domain model.

- Domain stores an engine-independent `AppearanceSpec` using stable part IDs, colors, and parameters.
- A shared `CharacterVisualService` assembles the player or any visible NPC.
- Character views are pooled and reused.
- Background NPCs have no Unity GameObject or loaded visual representation.
- Ordinary scenes target a small number of fully assembled visible characters.
- Concert crowds and other mass scenes use cheaper baked/decorative representations.
- Portrait rendering uses a bounded cache.
- The plugin's `CC2D_RES` lazy resource mode is the first loading strategy to validate.
- Unused fantasy content, examples, creator UI, and production-irrelevant assets should be excluded from the build after an explicit usage audit.

Unity Localization uses Addressables for locale and table loading, so Addressables enter the initial stack behind that package boundary. They are not the general-purpose asset-loading standard: CharacterCreator2D continues to use its native `CC2D_RES` lazy loading until device profiling proves it insufficient. Remote catalogs, CDN delivery, and a wholesale project-asset migration are out of scope.

## 18. Ads and platform services

Clever Ads Solutions is the current advertisement provider behind project-owned abstractions.

- `IAdService` hides the provider SDK.
- `AdPolicy` owns placement eligibility, cooldowns, and frequency caps.
- Placements are typed, for example app entry, map transition, news feed, and inbox.
- Network failure, timeout, or unavailable inventory never blocks gameplay or navigation.
- Ads are presentation-side effects and do not modify gameplay state.
- Rewarded gameplay advantages are prohibited.
- A no-ads entitlement is stored separately from the game save.
- Development and tests use a fake provider.
- Consent and store compliance remain at the platform boundary.

The simulation has no backend dependency and remains fully playable offline apart from optional ad delivery.

## 19. Local diagnostics

Rap Way uses local diagnostics only.

- Application-level logging goes through a project-owned logging port.
- Development builds write categorized logs to Unity Console and a bounded rotating local file.
- Expected command rejection is not logged as an error.
- Invariant failure, save corruption, content failure, and unhandled exceptions are errors.
- A bounded ring buffer may capture recent commands, committed events, schema version, simulation time, and random-stream identifiers for reproduction.
- Secrets, signing data, advertising IDs, and free-form user content must not be logged.
- Performance markers cover simulation steps, save/load, content initialization, and character assembly.

No Sentry, Firebase telemetry, custom analytics backend, or remote log collection is planned. Production crash visibility is limited to the platform information supplied by Google Play Console.

## 20. Testing strategy

### 20.1 .NET unit tests

Domain and Application tests run through `dotnet test` without Unity.

- NUnit is the standard test framework.
- Handwritten fakes, stubs, deterministic clocks, and scripted random sources are preferred over a mocking framework.
- Tests cover rules, transitions, calculations, event order, failure atomicity, numeric boundaries, migrations, and content validation.
- Scenario builders make complex GameState setup readable.
- Tests are deterministic, isolated, and independent of wall-clock time or network access.

Target project setup:

- Domain and Application compile against the Unity-compatible shared .NET API surface, initially `netstandard2.1`.
- Test projects target the pinned repository .NET SDK.
- `global.json` will pin the SDK; the current development machine has .NET SDK 10.0.400.
- Project-owned `.csproj` and solution files must be explicitly allowed by `.gitignore`; Unity-generated project files remain ignored.

### 20.2 Unity tests

- EditMode tests cover Unity adapters, serialization compatibility, and assembly wiring where Unity is required.
- PlayMode tests are limited to critical startup, navigation, scene, save/load, and prefab/document integration.
- Android device smoke validation remains required for rendering, memory, safe areas, input, suspend/resume, and IL2CPP behavior.

## 21. Build and validation

Initial GitHub CI is intentionally lightweight:

- Build engine-independent projects.
- Run `dotnet test`.
- Validate gameplay JSON and cross-references.
- Run pure C# formatting/static checks when configured.

Unity CI is deferred. Unity compilation, Unity tests, and Android smoke builds run locally through one reproducible validation command using Unity CLI/Pipeline tooling.

Android release baseline:

- IL2CPP.
- AAB output.
- Development Build disabled.
- Signing material and passwords remain outside Git.
- Mobile performance is validated on a representative lower-end device.

PC build and storefront pipelines are deferred until a Windows vertical slice exists.

## 22. Dependency register

### 22.1 Installed and retained

- Unity Editor `6000.5.9f1`: rendering and platform shell.
- .NET SDK `10.0.400`: pinned by `global.json` for engine-independent builds and tests.
- NUnit `4.6.1`, NUnit3TestAdapter `6.2.0`, and Microsoft.NET.Test.Sdk `18.8.1`: pinned pure .NET test toolchain.
- VContainer `1.19.0` currently resolved: Unity composition and lifetime scopes.
- MessagePipe Core and VContainer `1.8.2`, pinned to official commit `58516c36d4465a7b6396b7850a4ad7e03326998c`: scoped publication of committed domain facts only.
- UniTask `2.5.11` currently resolved: limited Unity-side async support and MessagePipe requirement.
- Newtonsoft Json Unity package `3.2.2` / Json.NET `13.0.2`: save and content infrastructure.
- Unity Input System `1.20.0`: touch, mouse, keyboard, and platform input.
- DOTween `1.2.815` generation or newer imported asset: presentation animation. Exact imported version must be verified and recorded before the next upgrade.
- CharacterCreator2D: character visual assembly. Imported asset version must be recorded from its source/license record.
- Clever Ads Solutions `4.7.4` currently resolved: advertisement provider behind `IAdService`.
- Unity Pipeline `0.5.0-exp.1`: editor automation and validation tooling, not a runtime architecture dependency.

### 22.2 Planned

- URP version verified for Unity `6000.5.9f1`: 2D Renderer and CharacterCreator2D shader support.
- Unity Localization version verified for Unity `6000.5.9f1`: String Table Collections, Smart Strings, pseudo-localization, and editor APIs. Its resolved Addressables dependency is pinned in the package lock and initially contained to localization.

### 22.3 Planned removal or containment

- UniRx: remove after replacing the prototype `IMessageBroker` use with the accepted event flow.
- uGUI: remove after the UI Toolkit vertical slice is functional and package consumers are audited.
- Continuous `GameTimeService.Tick`: replace with action-driven discrete simulation time.
- Distributed `ISaveable` plus `Dictionary<string, object>` save state: replace with explicit snapshot DTOs and migrations.

## 23. Dependency policy

- Git packages use exact release tags or commit hashes; floating default branches are prohibited.
- `Packages/packages-lock.json` is committed.
- Package updates are isolated, reviewable changes.
- An update requires .NET tests, Unity compilation, relevant Unity tests, and Android smoke validation proportional to impact.
- Vendor asset code is not reformatted or casually modified.
- New dependencies require a concrete current use case, ownership layer, removal condition, license check, and platform compatibility check.
- Prefer adapters around vendor APIs that would otherwise spread across project code.

## 24. Explicit non-goals

The initial architecture does not include:

- Event sourcing.
- Unity ECS, Jobs, or Burst for simulation.
- General-purpose Addressables asset migration, remote catalogs, or CDN content delivery beyond the dependency used by Unity Localization.
- A gameplay backend or offline progression server.
- Remote configuration.
- External telemetry or analytics.
- Unity CI.
- A separate scene for every screen or location.
- A generic repository abstraction for every entity.
- A mediator/CQRS framework.
- A permanent uGUI/UI Toolkit hybrid.
- Runtime gameplay dependence on ScriptableObjects.

## 25. Migration sequence from the prototype

The target should be reached incrementally while keeping the project runnable. `ROADMAP.md` owns the detailed current order; this section records the architectural dependency sequence it must preserve.

1. Scaffold engine-independent assemblies and the `dotnet test` solution.
2. Introduce core value objects, authoritative `GameState`, command results, the Application transaction, deterministic time/random streams, numeric helpers, and the committed-event port with tests.
3. Add pinned MessagePipe packages at the committed-event boundary without expanding prototype UniRx usage.
4. Implement explicit snapshot saves, migrations, atomic writes, backup recovery, and mobile lifecycle adapters against the minimal real `GameState`.
5. Complete the approved visual direction gate, migrate to URP 2D, and build the UI Toolkit application shell and navigation slice.
6. Remove replaced legacy uGUI runtime content and UniRx after dependency checks.
7. Install the pinned Unity Localization package, establish source/required locales, and add catalog generation and release validators before large-scale text authoring.
8. Add gameplay content catalogs and validation before feature definition volume grows.
9. Replace continuous `GameTimeService` and distributed `ISaveable` behavior as their new vertical systems take ownership.
10. Validate CharacterCreator2D materials and assembly on Android, then enable and profile its lazy resource loading.

Each migration step should have a narrow acceptance criterion and should not combine unrelated gameplay work.

## 26. Open technical decisions

- Exact Android device support and performance budgets.
- Exact URP package/configuration validated against Unity 6000.5.9f1.
- CharacterCreator2D asset version and final build-content curation.
- Whether profiling ever justifies expanding Addressables beyond localization.
- PC build, storefront, input, and save-path requirements.
- Whether cloud saves are ever required; they are not assumed.

## 27. Decision maintenance

This is a living document, not a historical transcript.

- Update it when an accepted architecture decision changes.
- Record the reason and migration impact for consequential reversals.
- Keep package versions aligned with the lock file after validated upgrades.
- Move detailed implementation instructions into feature documentation when they no longer belong at architecture level.
- Do not weaken boundaries silently to make a single feature faster to implement.

## 28. Primary references

- [Unity UI system comparison](https://docs.unity3d.com/Manual/UI-system-compare.html)
- [Unity Awaitable](https://docs.unity3d.com/Manual/async-await-support.html)
- [Unity render pipeline feature comparison](https://docs.unity3d.com/Manual/render-pipelines-feature-comparison.html)
- [VContainer](https://github.com/hadashiA/VContainer)
- [MessagePipe](https://github.com/Cysharp/MessagePipe)
- [UniTask](https://github.com/Cysharp/UniTask)
- [Unity Newtonsoft Json package](https://docs.unity3d.com/Packages/com.unity.nuget.newtonsoft-json@3.2/manual/index.html)
