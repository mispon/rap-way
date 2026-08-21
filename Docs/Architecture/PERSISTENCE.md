# Persistence Foundation

## Authority and boundaries

- Rap Way persists explicit snapshots of `GameState`; domain events are not a permanent source of truth.
- `RapWay.Application.Persistence.IGameSaveStore` is the engine-independent port.
- DTOs, JSON, checksums, and physical file handling live in `RapWay.Infrastructure.Persistence`.
- Unity lifecycle and MessagePipe autosave adapters live in `RapWay.Composition.Unity.Persistence`.
- Domain and Application contain no Newtonsoft.Json, Unity, MessagePipe, file-system, or platform-path types.

## Pre-release format

- The save envelope contains `checksumAlgorithm`, `checksum`, and `payload` only.
- The payload contains an ISO-8601 UTC timestamp and explicit DTOs for revision, calendar, random master seed, and materialized random streams.
- Unsigned 64-bit random values are invariant decimal strings so JSON consumers cannot round them through floating-point numbers.
- Materialized streams are serialized in stable-ID order for deterministic output.
- Unknown fields are rejected instead of silently discarded; this makes an old development save intentionally incompatible after a DTO change.
- `TypeNameHandling` is always `None`; `$type` metadata and arbitrary object dictionaries are prohibited.

## Integrity and development cutover

- SHA-256 covers the compact JSON representation of the unmodified payload.
- JSON date auto-parsing is disabled before checksum verification so ISO strings remain canonical.
- There is deliberately no schema version or migration pipeline before release.
- An incompatible existing development save creates a new bootstrap career and is immediately replaced by a current snapshot.
- Save-format versioning and migrations are a release gate, when player careers become durable.

## Durable file layout

- Primary: `career.save.json`.
- Backups: `career.save.backup1.json` and `career.save.backup2.json`.
- Temporary write: `career.save.tmp` in the same directory.
- A captured immutable state copy is serialized before background I/O.
- The temporary file is flushed to disk, then replaces the primary in the same directory; the previous primary becomes backup 1 and the prior backup 1 becomes backup 2.
- A valid primary wins. If it is invalid, the newest valid backup is loaded and the selected recovery source is reported.
- An interrupted temporary file is never considered a load candidate.

## Unity lifecycle

- `GameSessionCoordinator` loads a valid career before exposing snapshots. Missing saves create a temporary bootstrap state; an unrecoverable development save is replaced with one instead of blocking the game.
- `GameSaveLifecycleAdapter` captures synchronously and requests observed asynchronous writes on mobile pause and best-effort application quit.
- `CommittedEventAutosaveScheduler` listens only to committed facts and debounces autosave requests by two seconds. It does not use the domain clock.
- The bootstrap date is temporary composition data until the career-creation stage owns start date and seed selection.

## Legacy cutover

The old `SaveLoadService`, `Dictionary<string, object>`, distributed `ISaveable`, and `TypeNameHandling.Auto` path was removed after the new vertical slice passed its pure .NET and Unity integration checks. `AutoSaveManager` was removed from `RootLifetimeScope.prefab` through Unity Editor tooling before its script was deleted. Prototype continuous time and scene loading no longer initiate hidden saves. Existing legacy `savegame.json` files in a player's platform data directory are intentionally left untouched as recoverable archives; schema-v1 career files use different names and never overwrite them.

## Validation

- Pure .NET tests cover round trips, `long`/`ulong` boundaries, deterministic stream order, checksum corruption, incompatible legacy fields, missing saves, interrupted temp files, both backup levels, and total corruption.
- Unity compilation and a Game-scene Play Mode smoke test validate VContainer construction and lifecycle-object creation.
- Android suspend/resume and platform file-replacement behavior still require a device smoke test before release; this is release validation, not a second persistence implementation path.
