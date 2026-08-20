# Rap Way Prototype Migration Inventory

Last updated: 2026-08-20

This inventory assigns every known first-party prototype area to the roadmap stage that replaces or deliberately retains it. It prevents temporary architecture from becoming permanent by accident.

## Boundary strategy

- `RapWay.Domain` and `RapWay.Application` are the engine-independent source of truth for new rules and use cases.
- `RapWay.Infrastructure`, `RapWay.Presentation.Unity`, and `RapWay.Composition.Unity` are introduced as directed target assemblies.
- The existing `RapWay` assembly remains a temporary compatibility boundary for serialized prototype scripts.
- Existing script files are moved only when their replacement stage can preserve or intentionally remove their behavior and serialized references.
- No new gameplay or platform behavior is added to the legacy assembly unless it is required to keep the migration runnable.

## Current prototype ownership

### Legacy root assembly

- Current area: `Assets/_Project/Code/RapWay.asmdef`.
- Current role: compiles all prototype Unity code not yet moved behind a target assembly boundary.
- Replacement: shrinks during Stages 1-18 as vertical systems move; delete when no first-party source remains under it.
- Removal gate: Unity compilation, scene/prefab reference audit, and zero remaining code references.

### Composition and startup

- Current area: `Assets/_Project/Code/App/`, including boot, installers, scopes, and `GameStartup`.
- Problems: Unity/VContainer composition, prototype navigation, time, saves, and UI are coupled in one assembly.
- Replacement owner: Stage 4 for application shell/lifetimes/navigation; Stage 2 for persistence wiring; Stage 1 for committed-event wiring.
- Removal gate: equivalent `RapWay.Composition.Unity` registrations and startup flow validated in Unity.

### Continuous prototype time

- Current area: `Core/TimeSystem/GameTimeService.cs`, `Data/Settings/TimeConfig.cs`, and the now-unused legacy `Domain/Events/TimeEvents.cs` definitions.
- Problems: frame-driven time, Unity delta time in simulation, distributed saving, and fire-and-forget autosaves remain mixed.
- Stage 1 status: first-party UniRx broker registration and unused prototype time-fact publications were removed. New committed facts leave Application only through `ICommittedEventSink` and the scoped MessagePipe adapter.
- Replacement owner: Stage 1 for deterministic time/events and Stage 8 for player-facing action costs/calendar behavior.
- Removal gate: action-driven command transaction publishes committed facts and all old subscribers are migrated.

### Distributed save system

- Current area: `Core/SaveSystem/` and `Domain/Interfaces/ISaveable.cs`.
- Problems: mutable registration, `Dictionary<string, object>`, vendor-shaped restore data, and incomplete durability semantics.
- Replacement owner: Stage 2.
- Removal gate: explicit versioned snapshot round-trip, migrations, checksum, atomic replacement, backups, recovery tests, and mobile lifecycle adapter.

### Scene loading and navigation

- Current area: `Core/Services/SceneLoaderService.cs`, boot/game scenes, and UI-driven scene changes.
- Problems: scene navigation is broader than required by the accepted persistent-shell architecture.
- Replacement owner: Stage 4.
- Removal gate: bootstrap plus persistent UI Toolkit shell enters main menu/game session and Back/Escape works without screen-per-scene navigation.

### uGUI windows and widgets

- Current area: `Core/UI/`, `UI/`, `Data/UIConfig.cs`, and `Prefabs/UI/`.
- Problems: prefab-authored screen registry, hidden scene searches, uGUI-specific views, and prototype-only screens.
- Replacement owner: Stage 4.
- Removal gate: reference shell screens are rebuilt and validated in UI Toolkit; serialized usages and package consumers are audited.

### Audio stub

- Current area: `Core/Audio/AudioService.cs`.
- Current role: placeholder dependency for prototype buttons.
- Replacement owner: Stage 4 for presentation feedback; later content stages may expand it.
- Removal gate: retain only if it becomes the project-owned audio boundary; otherwise replace with the approved Unity adapter.

### Empty domain and feature prototypes

- Current area: `Domain/Models/Track.cs` and `Features/Test/`.
- Problems: placeholder types do not prove architecture or player behavior.
- Replacement owner: Stage 4 removes test UI; Stage 15 replaces the empty Track model with the accepted track-project pipeline.
- Removal gate: no serialized or code references remain, or the stable type is migrated into its owning feature.

### Third-party containment

- UniRx under `Assets/Plugins/UniRx/`: no first-party runtime usages remain after Stage 1. Keep the vendor folder only until a complete serialized/vendor reference audit confirms safe removal.
- uGUI package: remove after Stage 4 replaces all first-party UI and vendor/package consumers are audited.
- UniTask: retain only at accepted Unity-facing integrations; no Domain/Application references.
- VContainer: retain in `RapWay.Composition.Unity`; no Domain/Application references.
- DOTween: retain in Presentation.Unity only after the Stage 4 migration.
- CharacterCreator2D: retain behind the Stage 7/14 visual adapter; never reference it from Domain/Application.

## Update rule

When a legacy file is moved, replaced, or deleted, update this inventory in the same change. A temporary adapter must name its removal stage and gate here or in the active roadmap stage.
