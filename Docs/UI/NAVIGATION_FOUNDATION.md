# UI Navigation Foundation

## Purpose

Rap Way uses one mobile-first, in-session navigation history. It supports Football Manager-style links between related pages without turning each game page into a Unity scene or relying on visual modal overlays.

## Route contract

`UiRouteRegistry` is the sole registry of route behavior. Every route declares:

- its stable route ID;
- its mode: `Push`, `Replace`, `Dialog`, or `Home`;
- the typed context contract it accepts;
- whether its own layout renders an in-UI Back affordance.

Routes never infer their behavior from names, UXML files, or callers. A screen view receives an already validated route entry and emits intents; it does not manipulate history directly.

The initial vertical slice follows this contract end to end: `Home → ActivitySelection → ActivityConfirmation → ActivitySession → ActivityResult → Home`. The selection and confirmation use `Push`; session and result use `Replace`; Close from the result intentionally clears to Home. Its renderer may refresh progress from simulation state, but it never chooses the next route itself.

## History behavior

- `Home` is the only in-game root.
- Map, Career, Inbox, Actions, and all later sections are equal routes. Entering one from any other page uses the same global history.
- `Push` adds a new entry.
- `Replace` updates the current non-root entry without increasing depth. If Home is current, the navigator adds the target after Home so the root remains intact.
- Home action clears history to a new Home entry and resets Home's transient view state.
- The default maximum depth is 50, configured in the Unity-authored presentation settings. On overflow, the oldest entry after Home is removed; Home is never removed.
- Route entries retain transient local state (for example, a selected Inbox category, filter, card, or scroll position) until popped. This state is not part of a save file.

## Contexts

The initial reusable context family is deliberately small:

- `EmptyContext` for pages with no input;
- `EntityContext` for an entity kind and stable ID;
- `ActivityContext` for a selected activity and optional session ID;
- `DialogContext` for localized title/body references and explicit actions.

New context contracts are created only when these cannot accurately represent a new data shape. Raw prose and `map[string]string`-style parameter bags are prohibited.

## Dialog pages

A dialog is a full-screen page on a phone, but logically transient:

- it does not enter history;
- it retains its origin route entry and origin local state;
- Close, Cancel, or an acknowledged result returns to the origin;
- an unavailable link opens the universal informational dialog page rather than becoming a dead end;
- a dialog action that must open another page explicitly closes/replaces the dialog before navigating.

## Back and input

Android Back and Escape first close an active dialog, then pop a navigable route when the stack has an entry above Home. Individual layouts may omit a visible Back button; that controls only the screen composition, never the system Back behavior.

## Screen ownership

Each route owns its top chrome, content, and contextual bottom navigation. Bottom navigation is not a permanent tab bar. A compact Home action may be offered in the top chrome; activating it clears history to fresh Home.
