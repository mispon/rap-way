# Rap Way Implementation Roadmap

Version: 0.1

Status: Living document

Last updated: 2026-08-20

## 1. Purpose

This document is the dependency-ordered implementation path for Rap Way. It answers one question: **which system should be built next, and what must be true before moving on?**

- `GAME_DESIGN_DOCUMENT.md` defines product behavior and scope.
- `TECHNICAL_DESIGN_DOCUMENT.md` defines architecture and technology boundaries.
- `ROADMAP.md` defines implementation order and readiness gates.
- `AGENTS.md` defines day-to-day contribution rules.

This is a rolling-wave roadmap. The entire route stays visible, the active stage is detailed, the next few stages have enough definition to expose dependencies, and distant stages remain intentionally coarse until they approach.

## 2. Operating rules

- There are no calendar promises in this document.
- Exactly one numbered implementation stage is `In Progress` at a time.
- Finish a thin but complete vertical capability before starting the next system.
- "Complete" includes domain rules, content/data, persistence, UI, localization, tests, and Unity/device validation when those concerns apply.
- Build the minimum durable core needed now. Expand an earlier system later only through an explicit roadmap item or a requirement of the active stage.
- Keep the project compiling and runnable throughout migration.
- Do not extend replaced prototype systems. Remove old code and assets after their replacement passes its gate.
- Update the active and next stages after every stage completion. Do not prematurely decompose the distant tail into code tasks.
- Consequential product or architecture changes must first update the GDD or TDD; the roadmap follows those sources rather than overriding them.

## 3. Status legend

- `In Progress`: the single active implementation stage.
- `Planned`: ordered but not active.
- `Blocked`: cannot proceed; the blocking condition and required decision must be written into the stage.
- `Done`: all acceptance criteria were validated, with evidence recorded in the completing change.
- `Gate`: a required decision or checkpoint rather than a standalone gameplay system.

## 4. Route overview

### Foundation and production shell

0. Architecture boundaries and test harness.
1. Deterministic simulation kernel.
2. Persistence foundation.
3. Visual direction gate, which may be prepared alongside engine-independent foundation work and must pass before Stage 4.
4. Unity presentation foundation.
5. Localization foundation.
6. Gameplay content pipeline.

### Playable life and music loop

7. Career bootstrap, identity, and appearance.
8. Character resources and action loop.
9. Effects, conditions, and event foundation.
10. Global map and locations.
11. Survival economy, lifestyle, and ordinary jobs.
12. Home, equipment, shops, and project services.
13. Skills, form, progression, and talents.
14. NPC industry and relationships foundation.
15. Track creation pipeline.
16. Release, audience, hype, reputation, and catalog economy.
17. News hub and charts.
18. Concert preparation and resolution.

**Checkpoint A:** first vertical slice — “from courier to a filled local club.”

### Full sandbox breadth

19. Random-event depth and authoring scale.
20. Clips, albums, and connected projects.
21. Social media and public image.
22. Team, delegation, and project services depth.
23. External labels, contracts, and rights.
24. Player-owned label.
25. Districts, cities, and world expansion.
26. Personal life, health, dependence, and recovery.
27. Aging, retirement, and generational continuation.

**Checkpoint B:** complete career sandbox from unknown artist to retirement or legacy.

### Product completion

28. Content scale, balance, onboarding, and accessibility.
29. Monetization and mobile platform services.
30. Android release hardening.
31. Optional PC adaptation.

## 5. Completed foundation stage

### Stage 0 — Architecture boundaries and test harness

**Status:** Done

**Goal:** Create the engine-independent structure in which all later game rules can be implemented and tested without Unity.

**Why now:** The current prototype has one broad runtime assembly, Unity-coupled services, no first-party automated tests, and architectural patterns that conflict with the accepted TDD. Every later system depends on correcting this boundary first.

**Dependencies:**

- Unity `6000.5.9f1` project imports and compiles.
- Current repository state and vendor boundaries are documented.
- GDD and TDD decisions are accepted.

**Must deliver:**

- Engine-independent `RapWay.Domain` and `RapWay.Application` projects/assemblies with directed references matching the TDD.
- Unity-facing `RapWay.Infrastructure`, `RapWay.Presentation.Unity`, and `RapWay.Composition.Unity` boundaries, introduced with the smallest viable wiring.
- A `dotnet test` solution using NUnit for pure C# tests.
- Shared test conventions and one representative test proving Domain executes without Unity assemblies.
- A minimal Unity composition path that keeps the existing project runnable while the new architecture is introduced.
- An explicit inventory mapping prototype systems to their planned replacement/removal stage.

**Should deliver:**

- Dependency-direction validation that fails when Domain or Application references Unity/vendor assemblies.
- Clear folder/namespace templates for the first feature without creating empty speculative layers per feature.

**Acceptance criteria:**

- `dotnet test` succeeds from the repository root.
- Unity completes script compilation with zero new errors.
- `RapWay.Domain` and `RapWay.Application` reference no Unity or vendor runtime packages.
- The game still enters its current runnable prototype flow.
- No unrelated vendor assets, scenes, prefabs, or serialized GUIDs changed.
- The prototype removal inventory names an owning future stage for every temporary architecture component.

**Validation evidence (2026-08-20):**

- `dotnet test RapWay.slnx --configuration Release` built Domain/Application for `netstandard2.1` and passed 3/3 tests under .NET SDK `10.0.400`.
- Architecture tests rejected Unity and vendor references from Domain/Application and proved a Domain event executes outside Unity.
- Unity `6000.5.9f1` imported all five target asmdefs and generated their `.meta` files; Pipeline reported the Editor `ready` after successful script compilation.
- A Pipeline smoke test entered Play Mode from `Boot`, logged the boot sequence and save, and reached the `MainMenu` scene.
- One pre-existing CharacterCreator2D initialization exception (`Material` constructed with a missing shader) remains an explicit Stage 4 URP 2D compatibility risk; it is unrelated to the new assembly boundary and was not hidden or expanded.
- Diff inspection confirmed that no vendor scene, prefab, asset, or serialized GUID change is retained from this stage.

**Deferred:**

- Actual `GameState`, time, random, commands, saves, and gameplay rules.
- Broad code movement or formatting unrelated to establishing assembly boundaries.
- Removal of the old runtime assembly before replacement paths compile.

**Sources:** TDD sections 3-4, 20, and 25; GDD sections 2-5.

## 6. Detailed active and near-term stages

### Stage 1 — Deterministic simulation kernel

**Status:** Done

**Goal:** Establish the small, synchronous core through which all state-changing gameplay actions run.

**Depends on:** Stage 0.

**Why now:** Every later save, resource, NPC, content, and UI feature needs one deterministic way to mutate authoritative state and explain committed results.

**Must deliver:**

- Stable ID, capped/fixed-point numeric, game date/time, named deterministic random-stream, command-result, and ordered domain-event primitives.
- A minimal authoritative root `GameState` with no speculative feature state.
- A synchronous Application transaction that validates intent, prevents partial mutation, commits once, and exposes facts only after commit.
- Pure C# tests for determinism, event order, overflow/bounds, validation failure, and failure atomicity.

**Should deliver:**

- A small committed-event port ready for a later MessagePipe adapter without installing or coupling MessagePipe to the core yet.
- Removal or containment notes for any prototype time/event code made obsolete by the kernel.

**Progress evidence (2026-08-20):**

- Implemented minimal `GameState`, `SimulationSession`, stable IDs, Gregorian integer-hour calendar, capped money/basis points, checked fixed math, versioned named PRNG streams, typed commands/results, atomic working-copy execution, ordered domain events, and the vendor-free `ICommittedEventSink` boundary.
- `dotnet test RapWay.slnx --configuration Release --no-restore` passes 41/41 tests, including PRNG golden vectors, stream isolation, numeric overflow/caps/rounding, calendar boundaries, rejection and exception rollback, mutable-reference isolation, state revision, event order, post-commit publication, and architecture boundaries.
- Unity `6000.5.9f1` imported and compiled the shared Domain/Application sources with Pipeline `ready` and an empty Console.
- MessagePipe Core/VContainer `1.8.2` is pinned to official commit `58516c36d4465a7b6396b7850a4ad7e03326998c`; the adapter is scoped to `GameLifetimeScope`, `GlobalMessagePipe` is unused, and Domain/Application remain vendor-free.
- First-party prototype UniRx broker registration and unused time-fact publication were removed. A `Game` scene Play Mode smoke test built the VContainer/MessagePipe scope without integration errors; the only runtime exception was the separately tracked pre-existing CharacterCreator2D missing-shader issue.

**Vertical result:** A test can create a minimal `GameState`, execute an explicit command, advance action-driven time, consume a named random stream, commit ordered domain events, and either apply all changes or none.

**Acceptance criteria:**

- Stable IDs, fixed-point/capped numeric helpers, game date/time, named deterministic random streams, command results, and ordered domain events exist as pure C#.
- One authoritative root `GameState` owns mutable session state.
- Application transaction validation prevents partial mutation and publishes facts only after commit.
- Boundary, overflow, determinism, failure-atomicity, and event-order tests pass under `dotnet test`.
- MessagePipe is pinned and connected only at the committed-event boundary; prototype UniRx messaging is not expanded.

**Deferred:** Feature-specific resources, economy, NPCs, and content definitions.

**Sources:** TDD sections 5-10 and 22; GDD sections 5-6.

### Stage 2 — Persistence foundation

**Status:** In Progress

**Goal:** Make the authoritative state durable before real careers accumulate valuable progress.

**Depends on:** Stage 1.

**Why now:** Careers will quickly become valuable player data. Persistence must become durable and migratable before feature state expands beyond the minimal kernel.

**Must deliver:**

- Explicit snapshot DTOs for the current `GameState`, calendar, and materialized random streams without Unity/vendor types or implicit type metadata.
- Sequential schema migrations, deterministic DTO mapping, checksum verification, temporary writes, atomic replacement, two rotating backups, and recovery reporting.
- Pure tests for round trips, corrupt/interrupted writes, backup recovery, schema migration, unsupported versions, and numeric/random boundaries.
- A narrow Unity lifecycle adapter for pause/quit save requests that snapshots synchronously and observes asynchronous write failures.

**Should deliver:**

- Debounced autosave scheduling after meaningful committed commands without coupling persistence to Domain or the game clock.
- Removal of the replaced `Dictionary<string, object>`, `TypeNameHandling`, distributed `ISaveable`, and legacy save registrations after compatibility wiring is verified.

**Vertical result:** A minimal game session can be saved, closed, loaded, verified, and recovered from a deliberately corrupted primary file.

**Acceptance criteria:**

- Explicit versioned snapshot DTOs map to and from `GameState` without Unity/vendor types.
- Sequential migrations, checksum verification, atomic replacement, two rotating backups, and recovery reporting are implemented.
- Tests cover round-trip stability, interrupted/corrupt writes, migration fixtures, numeric boundaries, and unsupported schema handling.
- Unity lifecycle adapters save safely on mobile pause/quit boundaries without making the domain asynchronous.
- Replaced `Dictionary<string, object>` and distributed `ISaveable` paths are removed when no longer referenced.

**Deferred:** Cloud saves, compression without evidence, and DTO fields for unimplemented systems.

**Sources:** TDD section 13; GDD sections 6.3 and 20.

### Stage 3 — Visual direction gate

**Status:** Gate — Open. This decision gate does not count as a second active implementation stage: it may be designed while Stage 0 remains active, but must be approved before Stage 4 production UI work.

**Goal:** Turn the intended tone of Rap Way into concrete constraints from which UI and world presentation can be built.

**Depends on:** Product direction in the GDD; no code dependency.

**Deliverables:**

- Root `VISUAL_DESIGN_GUIDE.md` covering mood, references, palette, typography, shape language, icons, illustration/map/character treatment, spacing, touch targets, and motion.
- One approved reference mobile screen that combines resource HUD, navigation, and a content card at a realistic aspect ratio.
- A small initial token list derived from the reference rather than an abstract component library.

**Acceptance criteria:**

- The reference remains readable on representative narrow/tall and wide mobile layouts.
- Cyrillic and Latin font coverage is confirmed.
- The direction is feasible with URP 2D, UI Toolkit, CharacterCreator2D, and available assets.
- The user explicitly approves the guide and reference screen.

**Deferred:** Final art for every location, exhaustive component states, and production asset creation.

**Sources:** GDD sections 3, 4, 9, and 25; TDD sections 16-17.

### Stage 4 — Unity presentation foundation

**Status:** Planned

**Goal:** Create the persistent mobile-first shell used by every later vertical system.

**Depends on:** Stages 0 and 3. Stage 2 must be available before real session navigation is finalized.

**Vertical result:** The app can move through bootstrap, main menu, new/load career, game session, HUD, modal, and Back/Escape flows using UI Toolkit in one persistent presentation shell.

**Acceptance criteria:**

- URP 2D is configured with an explicit mobile baseline and CharacterCreator2D shader compatibility is validated.
- UI Toolkit navigation, screen lifetime, safe area, modal layering, input routing, focus, and cancellation are implemented.
- Approved tokens and only the primitives required by real shell screens exist.
- Touch, mouse, keyboard, and Back/Escape paths work at representative mobile aspect ratios.
- The shell uses presenters/read models and contains no gameplay rules.
- Equivalent legacy uGUI shell code/assets are removed after replacement validation.

**Deferred:** Feature-specific screens, decorative polish, and general Addressables migration.

**Sources:** TDD sections 14-17 and 25.

### Stage 5 — Localization foundation

**Status:** Planned

**Goal:** Make all subsequent player-facing text safe to author, translate, validate, and render before text volume grows.

**Depends on:** Stage 4 and the approved typography from Stage 3.

**Vertical result:** Shell text is authored once in Russian, translated to English, rendered through typed named arguments, tested with pseudo-localization, and rejected by release validation when incomplete or stale.

**Acceptance criteria:**

- A Unity-compatible pinned Localization package and its Addressables dependency are recorded in the lockfile.
- Russian source and required English locale use feature-scoped String Table Collections.
- The catalog tool can add/update keys, track translation state, validate placeholders/completeness, report usage, and generate typed key references.
- Missing-key fallback, `Draft`/`Reviewed`/`Needs Review`, glossary, pseudo-localization, and Cyrillic/Latin glyph validation work.
- No shell player-facing prose remains hard-coded.

**Deferred:** Remote catalogs, CDN updates, RTL/CJK, and external spreadsheet synchronization until a concrete workflow needs it.

**Sources:** TDD sections 12.1, 17.2, and 22.

### Stage 6 — Gameplay content pipeline

**Status:** Planned

**Goal:** Provide validated, engine-independent authoring for the definitions used by every gameplay system.

**Depends on:** Stages 0-2; integrates with Stage 5 for text keys.

**Vertical result:** A small JSON catalog with stable IDs loads into immutable definitions, resolves localization/asset keys, and produces clear validation failures for broken content.

**Acceptance criteria:**

- Explicit JSON DTOs, schema/version policy, catalog loading, stable IDs, references, and validation are implemented.
- Duplicate IDs, broken references, invalid ranges, unknown fields, and forbidden cycles fail development/release validation as appropriate.
- Gameplay JSON contains no translated prose or Unity object references.
- One representative content type is covered by pure C# loader and validator tests.

**Deferred:** Bespoke editors until real authoring friction justifies them.

**Sources:** TDD section 12.

## 7. Playable life and music loop

The following stages are ordered, but their internals should be refined only as they become near-term.

### Stage 7 — Career bootstrap, identity, and appearance

**Status:** Planned

Create/new/load career flow, stable player identity, irreversible start-template choice, source data for age/background, engine-independent `AppearanceSpec`, and lazy CharacterCreator2D assembly for the visible protagonist. Start with the default template plus a small set including a hard or humorous “rock bottom” start.

**Depends on:** Stages 2, 4-6. **Sources:** GDD sections 20-21; TDD section 17.2.

### Stage 8 — Character resources and action loop

**Status:** Planned

Implement money, energy, satiety, motivation, action costs, discrete time advancement, daily boundaries, readable causes of change, and the first rest/eat/work debug-to-production flow. All values use explicit caps and safe numeric types and survive save/load.

**Depends on:** Stage 7. **Sources:** GDD sections 5-8; TDD sections 8 and 10.

### Stage 9 — Effects, conditions, and event foundation

**Status:** Planned

Implement timed states, buffs/debuffs, condition checks, weighted deterministic events, cooldowns, consequences, and explainable modifier breakdowns. Build only enough event authoring for the next survival systems; deeper libraries arrive in Stage 19.

**Depends on:** Stage 8. **Sources:** GDD sections 7.7, 7.8, and 22.

### Stage 10 — Global map and locations

**Status:** Planned

Implement the first city/district map, home, job, shop, studio, and venue points; location availability; travel actions that consume time/resources; and stable navigation without scene-per-screen architecture.

**Depends on:** Stages 4, 8, and 9. **Sources:** GDD section 9; TDD section 16.3.

### Stage 11 — Survival economy, lifestyle, and ordinary jobs

**Status:** Planned

Implement ordinary work as time/energy-for-money actions, contextual events, food and rest quality, recurring lifestyle categories, affordability fallback, hunger/deprivation consequences, and recovery from poverty without mini-games.

**Depends on:** Stages 8-10. **Sources:** GDD sections 8.1, 10, and 23.

### Stage 12 — Home, equipment, shops, and project services

**Status:** Planned

Implement the static home-as-base progression, room/capability gates, equipment ownership and quality, shop purchases, and the generic price/quality/reputation contract for one-off project services. Furniture placement remains out of scope.

**Depends on:** Stages 10-11. **Sources:** GDD sections 8.2-8.3 and 18.2.

### Stage 13 — Skills, form, progression, and talents

**Status:** Planned

Implement learn-by-doing skills, challenge suitability, diminishing progression speed, recent-practice form, qualitative UI feedback, and circumstance-based talents that alter options or checks. Start with lyric writing, performance, production, and charisma.

**Depends on:** Stages 8-9 and 12. **Sources:** GDD sections 7.4-7.6.

### Stage 14 — NPC industry and relationships foundation

**Status:** Planned

Implement the unified NPC identity/career model, small authored cast, dynamic simulation detail, relationship and multilayer reputation foundations, scheduled autonomous actions, and lazy visible NPC assembly. Avoid ECS; measure before optimizing.

**Depends on:** Stages 1, 6-9, and 13. **Sources:** GDD sections 7.3 and 16; TDD section 11.

### Stage 15 — Track creation pipeline

**Status:** Planned

Implement a track project from concept through beat, lyrics, recording, mixing, and completion; choices, skills, equipment, services, collaborators, time, energy, and motivation contribute to hidden multidimensional quality. The player receives qualitative feedback, not a single exposed score.

**Depends on:** Stages 12-14. **Sources:** GDD section 11.

### Stage 16 — Release, audience, hype, reputation, and catalog economy

**Status:** Planned

Implement release preparation, quality-versus-expectation outcomes, fast-decaying hype, fan segments, public/industry/peer reputation, finite attention, discoverability, finite royalties, catalog tails, and negative amplification of disappointing high-hype releases.

**Depends on:** Stages 14-15. **Sources:** GDD sections 7.2-7.3 and 13.

### Stage 17 — News hub and charts

**Status:** Planned

Implement persisted news items from domain facts, deterministic localized text variants, relevance selection, basic local chart computation, visible NPC activity, and explanations that connect player actions to world consequences.

**Depends on:** Stages 14-16. **Sources:** GDD sections 13.4 and 16.4; TDD section 12.1.

### Stage 18 — Concert preparation and resolution

**Status:** Planned

Implement booking, venue size, date, ticket/pricing assumptions, set list, rehearsal, promotion, costs, staff/services, condition, stimulants, attendance, performance resolution, profit/loss, reputation, fans, hype, and post-event news. No rhythm mini-game.

**Depends on:** Stages 9, 12-17. **Sources:** GDD section 14.

### Checkpoint A — First vertical slice

Validate the complete story “from courier to a filled local club” on a representative Android device. The checkpoint passes only when a fresh player can understand tradeoffs, create and release a cared-about track, attempt a concert, recover from ordinary failure, save/load reliably, and want to start another career. Albums, labels, generational play, broad personal life, and full ads remain deferred.

**Sources:** GDD section 25.

## 8. Full sandbox breadth

### Stage 19 — Random-event depth and authoring scale

**Status:** Planned

Expand event chains, positive/negative contextual events, stateful sequences, rarity/cooldown controls, content validation, authoring reports, and variety metrics across work, home, music, relationships, and the industry.

### Stage 20 — Clips, albums, and connected projects

**Status:** Planned

Add clips tied to tracks, album planning and track selection, connected-project amplification, budgets/services, release timing, catalog behavior, and concert demand generated by strong releases.

**Sources:** GDD section 12.

### Stage 21 — Social media and public image

**Status:** Planned

Add intent/tone-based posting, channel fit, relationship/reputation consequences, hype interaction, public mistakes, automation boundaries, and later PR-manager delegation without requiring free-form text entry.

**Sources:** GDD section 15.

### Stage 22 — Team, delegation, and project services depth

**Status:** Planned

Add the reduced permanent staff roster, including manager, PR manager, security, beatmaker, and ghostwriter where still justified by UI testing; staff quality, cost, trust, automation, and bounded autonomy; and deeper one-off service selection for projects.

**Sources:** GDD section 18.

### Stage 23 — External labels, contracts, and rights

**Status:** Planned

Add label reputation layers, offers, negotiation, advances, budgets, obligations, ownership, royalty splits, contract duration, breach/exit, and relationship-dependent opportunities. Validate that contracts change strategy rather than merely multiply income.

**Sources:** GDD sections 11.3 and 19.1.

### Stage 24 — Player-owned label

**Status:** Planned

Add a legally/financially separate company, label reputation and rating, autonomous signed artists, contracts, staff/services as needed, company cash flow, owner salary/dividends, and limits preventing free withdrawal of the company budget.

**Depends on:** Stages 14, 22, and 23. **Sources:** GDD section 19.2.

### Stage 25 — Districts, cities, and world expansion

**Status:** Planned

Add career-gated districts, differentiated opportunities/costs/audiences, travel or relocation decisions, city-level markets, and content streaming/loading only if profiling justifies it. Expansion must deepen choices, not create cosmetic duplicate maps.

**Sources:** GDD section 9.

### Stage 26 — Personal life, health, dependence, and recovery

**Status:** Planned

Add optional personal-life events and relationships, muse/inspiration interactions, injuries and health consequences, deterministic stimulant escalation into crash/tolerance/dependence/withdrawal, treatment/recovery paths, and meaningful routes back from poverty or disgrace.

**Sources:** GDD sections 7.8, 17, and 23.

### Stage 27 — Aging, retirement, and generational continuation

**Status:** Planned

Add aging for player and NPCs, career phases, retirement/death rules once separately designed, irreversible transfer of control, former protagonists becoming autonomous NPCs, successor/start-new-save flow, milestone saves, and world population regeneration.

**Depends on:** Mature NPC, relationship, economy, and save migrations. **Sources:** GDD section 20.

### Checkpoint B — Complete career sandbox

Validate a long-running world in which the player can rise, fail, recover, sign or found a label, age, retire, and continue through a new creator while the industry changes around them. Run soak simulations, multi-generation save migrations, economy checks, and repeated-career playtests before calling the systemic breadth complete.

## 9. Product completion

### Stage 28 — Content scale, balance, onboarding, and accessibility

**Status:** Planned

Scale authored/procedural NPCs, events, news, items, services, talents, starts, and locations; tune pacing and economy with reproducible simulations; add onboarding and contextual explanations; finish RU/EN review; validate pseudo-localization, text scaling, contrast, touch size, content controls, and representative device UX.

### Stage 29 — Monetization and mobile platform services

**Status:** Planned

Implement provider adapters, consent/privacy flow, strict placement policy, rare natural-boundary interstitials, clearly marked inbox/news cards, no-ads purchase, offline-safe failure behavior, and platform-specific separation. Ads never alter simulation outcomes.

**Sources:** GDD section 24; TDD section 18.

### Stage 30 — Android release hardening

**Status:** Planned

Finalize supported devices and performance budgets; profile startup, memory, overdraw, UI, CharacterCreator2D, saves, and long simulations; validate IL2CPP/AAB, suspend/resume, low storage, interrupted writes, clean install/upgrade, store compliance, crash diagnostics, signing hygiene, and release smoke tests.

**Sources:** TDD sections 19-21 and 26.

### Stage 31 — Optional PC adaptation

**Status:** Planned, contingent on a separate go/no-go decision after the mobile game is proven.

Adapt responsive layouts, keyboard/mouse/navigation, window and resolution behavior, save paths/cloud decisions, performance profiles, storefront/platform services, and monetization removal. Do not compromise the mobile-first architecture in anticipation of this stage.

## 10. Roadmap maintenance

When a stage is completed:

1. Record validation evidence in the completing change and set the stage to `Done`.
2. Move exactly one next stage to `In Progress`.
3. Expand that stage to the level currently used by Stage 0.
4. Refine only the next two or three stages when new dependencies or product learning require it.
5. Update GDD/TDD first if implementation taught us that an accepted product or technical decision must change.
6. Commit the roadmap update with the work that caused it when practical.

When a stage is blocked, name the concrete blocking condition. Uncertainty, difficulty, or a desire for more polish is not itself a blocker.
