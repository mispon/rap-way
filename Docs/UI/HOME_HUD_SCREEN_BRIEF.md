# Home HUD Screen Brief

Status: Approved interaction and visual direction; static UI Toolkit composition implemented. Runtime visual validation pending.

## Purpose

The Home HUD is the player's default orientation screen. In one glance it answers:

- What time is it and how is the character doing?
- Can the character afford and sustain the next action?
- Is there an important opportunity or inbox item to consider?
- Where can the player go next?

It is not a task feed, an exhaustive statistics page, or a second copy of the Career screen.

## Viewport and hierarchy

The canonical composition is Portrait `390 × 844` logical pixels. It has three stable zones:

1. **Top status chrome**: date/time, Money, Fans, compact Character State, and active-condition strip.
2. **Character tableau**: current home/location background, CharacterCreator2D character in idle animation, and at most one contextual opportunity card.
3. **Action chrome**: a calm, dominant Act button above persistent bottom navigation.

The Home HUD fits one viewport. Detail is opened through sheets or destination screens, never by vertically scrolling the shell.

## Top status chrome

Always visible:

- Date and time.
- Money and Fans.
- Energy, Satiety, and Motivation as compact bounded indicators.
- Active buff/debuff icons.

Tapping Character State opens the detailed state view. Tapping the condition strip opens the full condition sheet with source, duration, and exact effects.

Condition icons are ordered deterministically by gameplay importance, modifier strength, then stable ID. Four icons are visible; remaining effects collapse into a `+N` affordance. Colour supports sign recognition, but every state also has a distinct icon and label in the detail sheet.

## Character tableau

The tableau establishes place and character identity rather than simulating an open world.

- Initial presentation: a CharacterCreator2D idle animation over a static location background.
- It reflects current location, outfit, and major visual condition only when assets support it.
- It has no weather or time-of-day variants.
- It displays no more than one contextual opportunity card. The card leads to an Inbox item or an appropriate action category.

Tapping the character is reserved for the future Character Hub: detailed state, skills, personal timeline, and milestones. The personal timeline is an immutable audit of committed events. It may support NPC/world reactions later, but the first version is read-only.

## Action and navigation chrome

`Act` is the dominant entry point for time-spending actions. Its first-level categories are:

- Work
- Creativity
- Recovery
- Contacts

Concrete actions depend on location, time, money, conditions, relationships, and unlocked content.

`Act` signals the next normal step, never a warning or irreversible commitment. It uses the semantic `--rw-color-action-primary` token: muted terracotta, a thin light inner border, restrained type, and a height around `60–64px` at the canonical viewport. Urgency is communicated by the relevant opportunity or Inbox badge, not by turning the persistent primary action into an alarm.

Persistent navigation has four destinations:

- Home
- Map
- Career
- Inbox

Inbox contains messages, requests requiring decisions, public news, and charts. It uses badges by destination: a count for unread material and a stronger attention marker for an item requiring a decision. Routine events never interrupt play. A modal is reserved for a result or deadline that cannot be meaningfully deferred.

## Design and implementation rules

- Use the approved `DesignTokens.uss` palette and the visual language in `VISUAL_DESIGN_GUIDE.md`. Reserve coral for brand or exceptional emphasis; use the semantic action token for the persistent primary action.
- Maintain 48px minimum touch targets.
- Use localized text only; validate Russian and English wrapping.
- Build the static approved layout in UI Builder before wiring runtime state.
- Validate `360 × 800`, `390 × 844`, and `412 × 915` before promoting the HUD layout as reusable.

## Next design step

Open the UXML in UI Builder and validate the implemented composition at `360 × 800`, `390 × 844`, and `412 × 915`; then run the HUD in Play Mode. Do not add new HUD behavior until this composition is visually accepted.
