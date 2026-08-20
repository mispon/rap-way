# Simulation Kernel

Last updated: 2026-08-20

This document records the implemented Stage 1 contracts. Product rules remain in the GDD and architecture policy remains in the TDD.

## Authoritative state and execution

- One `SimulationSession` owns one authoritative `GameState`.
- Callers receive deep snapshots, never the mutable state owned by the session.
- Every mutation enters through a typed command and one explicit handler.
- A handler works on a deep working copy. Rejection, handler exceptions, or postcondition failures discard that copy and publish nothing.
- A successful command validates the working state, increments its revision, commits an isolated copy that shares no mutable references with the handler, and only then publishes an ordered read-only event list.
- A publication failure cannot roll back an already committed simulation result. The Unity adapter must observe and log such failures.

The initial whole-state copy is intentionally simple. Replace it with scoped change sets only after profiling proves that copies are material and equivalent rollback tests exist.

## Stable identifiers

`StableId` accepts 1-64 characters, begins with a lowercase ASCII letter or digit, and then permits lowercase ASCII letters, digits, `.`, `-`, and `_`. Default/empty IDs are invalid.

IDs are technical contracts and must not be generated from localized or mutable player-facing text.

## Time

- `CalendarState.TotalHours` is the authoritative integer clock.
- `GameDate` is derived with Gregorian calendar rules at whole-hour precision.
- Time advances only through explicit positive-duration commands.
- The current technical cap is `5,000,000` elapsed hours. Reaching it is an expected typed command rejection, not overflow.
- Unity frame time, wall-clock time, and application suspension never advance the simulation.

## Randomness

- `RandomState` owns the master seed and mutable state for every materialized named stream.
- A stream's initial state derives only from the master seed and stable stream name, so consuming one stream cannot perturb another.
- Algorithm version 1 uses FNV-1a name hashing, a SplitMix64-style finalizer, and xorshift64* generation.
- The test suite contains a golden sequence. Changing it is a save-compatibility decision requiring a new algorithm version and migration policy.
- Certain probability outcomes (`0/N` and `N/N`) do not consume random state.

## Numeric contracts

- `Money` stores signed `long` minor units and is capped to `+/-9,000,000,000,000,000` minor units.
- `BasisPoints` stores signed fixed-point multipliers where `10,000` is 100%, capped to `+/-1,000,000` basis points.
- `FixedMath.MultiplyDivide` performs checked integer arithmetic with explicit toward-zero, away-from-zero, or nearest-away-from-zero rounding.
- Technical overflow throws as an invariant failure; gameplay caps reject invalid values before they enter authoritative state.
- Floating-point values are not used by authoritative kernel state or calculations.

## Events and adapters

- Domain events are immutable past-tense facts.
- `ICommittedEventSink` is the only Stage 1 output boundary for independent subscribers.
- Domain and Application contain no MessagePipe, UniRx, Unity, or other vendor types.
- `MessagePipeCommittedEventSink` is implemented in Infrastructure, registered as scoped in the game-session composition scope, and receives only already committed event batches.
- MessagePipe Core and VContainer `1.8.2` are pinned to official commit `58516c36d4465a7b6396b7850a4ad7e03326998c`; `GlobalMessagePipe` is not configured.

## Required regression coverage

Keep tests for stable IDs, calendar boundaries, numeric caps and rounding, PRNG golden vectors and stream isolation, rejection rollback, exception rollback, revision changes, committed event order, publication timing, and Domain/Application dependency boundaries.
