# Rap Way Visual Design Guide

Version: 0.1

Status: Draft — pending approval of the first reference screen

Last updated: 2026-08-20

## 1. Purpose and authority

This document defines the visual language and production constraints for Rap Way. It governs UI, illustration, character presentation, locations, the global map, iconography, typography, color, and motion.

- `GAME_DESIGN_DOCUMENT.md` remains authoritative for product behavior.
- `TECHNICAL_DESIGN_DOCUMENT.md` remains authoritative for implementation boundaries.
- This guide is authoritative for visual decisions after the Stage 3 gate is approved.
- Generated reference images establish direction and composition, not pixel-perfect UI specifications.

The goal is a coherent style that a solo developer can produce and maintain without sacrificing the readability required by a text-heavy management sandbox.

## 2. Visual north star

**A contemporary music editorial and street-zine interface with warm analog echoes of the late 1990s and early 2000s.**

Rap Way should feel like a living combination of:

- A local music magazine.
- A concert poster wall.
- An artist's notebook and home studio.
- A modern mobile career-management application.
- A warm city evening carried by low-key hip-hop and R&B ambience.

The setting is not historical. Smartphones, social media, streaming, and the contemporary music industry exist. Nostalgia comes from color, print texture, typography, composition, and sound rather than obsolete mechanics or constant VHS effects.

## 3. Visual principles

### 3.1 Music editorial, not a hip-hop stereotype

Use condensed poster type, strong crops, paper layers, subtle halftone, tape, stickers, and image-led headlines. Do not default to graffiti, chains, luxury gold, guns, neon alleys, or gangster caricature.

### 3.2 Readability is part of the style

Rap Way contains long news, events, contracts, explanations, and numerical decisions. Decoration must create hierarchy, never compete with copy. Functional surfaces use clean edges and predictable spacing even when decorative frames imply torn paper.

### 3.3 Contemporary with analog warmth

Modern information architecture sits inside a tactile visual world. Paper grain and print defects are quiet background signals, not full-screen filters.

### 3.4 Characters belong to the same publication

CharacterCreator2D's clean comic outlines and flat shading are treated as illustrated magazine cutouts. Backgrounds remain slightly softer, simpler, and lower contrast so characters retain focus.

### 3.5 Progress lives in the world

The application shell stays consistent. Success and failure appear through clothing, posture, home and equipment tiers, venue scale, media placement, crowds, collaborators, and the quality of surrounding environments.

### 3.6 Motion punctuates decisions

Ordinary navigation is fast. Large releases, concerts, career collapses, and breakthroughs earn longer sequences. Motion never hides rules or holds input hostage.

## 4. Reference images

### 4.1 Mood and world tone

![Rap Way home-studio mood art](Docs/Visual/References/mood-home-studio-v1.png)

Use this image for atmosphere, palette, comic/editorial compatibility, and the balance of ambition with modest beginnings. It is more detailed than an ordinary gameplay location should be.

### 4.2 First mobile reference screen

![Rap Way early home reference screen](Docs/Visual/References/home-screen-reference-v1.png)

Use this image for hierarchy and composition:

- Compact six-resource HUD.
- Clear location identity.
- Character integrated into a static tableau.
- One contextual editorial card.
- Three immediate actions.
- Four persistent navigation destinations.
- Dark shell surrounding warm paper surfaces.

Do not trace generated text, spacing, icons, or component geometry literally. Production UI is rebuilt in UI Toolkit with responsive layout, localized text, accessibility states, and validated touch targets.

## 5. Color system

The initial palette is intentionally small. Values are provisional until validated in the UI Toolkit reference implementation on representative devices.

### 5.1 Core colors

- `Graphite 950` — `#181A1B`: application background and deepest shell.
- `Charcoal 850` — `#242729`: raised dark surfaces and navigation.
- `Paper 100` — `#F2E7D5`: primary light content surface and dark-shell text.
- `Paper Shadow 250` — `#D4C1A4`: borders, dividers, and quiet metadata on dark surfaces.
- `Ink 900` — `#202223`: primary text and icons on paper.
- `Coral 500` — `#F04F4A`: brand accent, active navigation, primary action, and attention.
- `Olive 500` — `#78815A`: grounded secondary accent and satiety/lifestyle cues.
- `Amber 500` — `#D8943B`: warmth, energy, opportunity, and warning cues.
- `Burgundy 650` — `#7A3541`: reputation, consequence, and deeper editorial accent.
- `Danger 650` — `#B83243`: destructive/error state, kept distinct from brand coral.

### 5.2 Contrast rules

Current calculated contrast targets include:

- Paper on Graphite: approximately `14.28:1`.
- Paper on Charcoal: approximately `12.29:1`.
- Ink on Paper: approximately `13.06:1`.
- Coral on Graphite: approximately `4.93:1`.
- Amber on Graphite: approximately `6.84:1`.
- Paper on Danger: approximately `4.80:1`.

Coral and Olive are not body-text colors on Paper. Use Ink for copy and pair semantic colors with an icon, label, pattern, or sign; never communicate state by hue alone.

### 5.3 Usage balance

- Graphite/Charcoal establish the shell.
- Paper carries reading-heavy content.
- Coral is sparse and reserved for the current destination, the primary action, or urgent editorial focus.
- Olive, Amber, and Burgundy support category distinction and atmosphere.
- Avoid rainbow resource bars. Shape and label remain primary identifiers.

## 6. Typography

### 6.1 Roles

- **Display:** condensed, heavy, poster-like type for screen titles, districts, chart positions, dates, and large outcome numbers.
- **Body:** modern, calm sans serif with excellent Cyrillic/Latin legibility for news, events, explanations, contracts, and controls.
- **Utility:** tabular numerals from the body family for resources, money, time, and comparisons.
- **Accent:** a marker or stamped style may appear only in authored decorative art, never in required functional copy.

### 6.2 Initial candidates

- Display candidate: **Oswald**, weights 500-700.
- Body candidate: **Manrope**, weights 400-700.

Both candidates are distributed under the SIL Open Font License in the Google Fonts repository and include Cyrillic support. They are not project dependencies until imported, licensed files are retained, and Unity font atlases are validated.

### 6.3 Rules

- Headlines may use uppercase when short; sentences and long labels do not.
- Never use condensed display type for paragraphs.
- Do not use graffiti lettering for navigation or system labels.
- Prefer real typographic hierarchy over outlines, glows, and stacked shadows.
- Support text expansion and pseudo-localization without reducing body text below the approved mobile minimum.
- Use tabular numerals where changing values would otherwise shift the HUD.

## 7. Layout and surfaces

### 7.1 Grid

- Base spacing unit: `4` logical pixels.
- Common gaps: `8`, `12`, `16`, `24`, and `32`.
- Primary screen gutters begin at `16` plus safe-area insets.
- Interactive targets are at least `48 x 48` logical pixels unless device testing establishes a larger project minimum.
- Decorative asymmetry may cross the visual grid; controls and reading columns may not.

### 7.2 Shape language

- Functional dark panels: straight or subtly rounded corners, thin warm-gray outline.
- Paper cards: mostly rectangular, small corner radius, optional decorative torn outer layer.
- Primary buttons: strong rectangular silhouette, coral fill on dark or coral edge/action block on paper.
- Chips/tags: compact, high-contrast, never used as the primary reading container.
- Avoid universally pill-shaped controls and excessive floating cards.

### 7.3 Information density

- One screen answers one primary question but may expose several related actions.
- Keep resources, date, current location, and immediate opportunity visible when relevant.
- Reveal formulas, modifier sources, and history on demand.
- The main action is visually dominant; secondary actions remain easy to find but quieter.
- Bottom navigation contains persistent destinations only.

### 7.4 Mobile viewport rule

- The canonical authoring viewport is `390 × 844` logical pixels in Portrait orientation.
- Ordinary gameplay screens fit in one vertical mobile viewport; they do not use vertical scrolling as an escape hatch for weak hierarchy.
- Persistent actions and navigation stay anchored outside the content area. Content changes through explicit buttons, tabs, focused screens, sheets, pagination, or drill-down views.
- Lists that cannot remain readable within one viewport must be split into purpose-specific screens or paged; they are not compressed below the approved touch and text minimums.
- Validate the shell at 360 × 800, 390 × 844, and 412 × 915 logical-pixel viewports before treating a layout as reusable.

## 8. Core UI composition

### 8.1 Application shell

- Dark graphite/charcoal base.
- Safe-area-aware top status region.
- Persistent bottom navigation with no more than four or five destinations.
- Content surface may switch between tableau, paper feed, list, chart, or focused project view.
- Modal sheets rise from the bottom on mobile and retain Back/Escape behavior for PC.

### 8.2 Resource HUD

- Show Energy, Satiety, Motivation, Money, Fans, and Hype with stable positions when all are relevant.
- Renewable needs may use bounded value bars.
- Accumulated resources emphasize numeric value and trend rather than fake bounded bars.
- Every resource has icon, localized label, number, and optional trend/state.
- Tapping opens sources, drains, and recent changes.
- Large gains/losses use a short color/sign pulse and a textual cause.

### 8.3 Home HUD direction

The first production-ready HUD composition is specified in `Docs/UI/HOME_HUD_SCREEN_BRIEF.md`.

- Home uses three stable zones: top status chrome, character tableau, and action/navigation chrome.
- Money, Fans, date/time, compact renewable state, and active conditions remain visible; deeper breakdowns open on demand.
- The tableau contains the current location, idle character, and no more than one contextual opportunity card.
- `Act` is the dominant action entry. Home, Map, Career, and Inbox are the only persistent destinations.
- Inbox groups messages, decisions, news, and charts. Routine events use badges rather than interrupting the player.

### 8.4 Cards

- News/event cards use Paper and Ink with one category accent.
- Preserve a strict title → context → consequence/action hierarchy.
- Use portraits only when a character is relevant to the item.
- Do not fill feeds with unrelated decoration or unique card layouts.

## 9. Character presentation

- Full body: character creation, home/location tableaux, meetings, concerts, and major event staging.
- Bust portrait: news, relationships, team, dialogue, offers, and artist cards.
- Avatar: charts, notifications, dense lists, and compact comparison rows.
- Background NPCs appear only when their presence carries information or atmosphere.
- Separate characters from backgrounds with a restrained paper edge, rim light, or soft shadow.
- Preserve CharacterCreator2D identity; do not regenerate the same NPC independently per scene.
- Generated key art may use original illustrative characters, but gameplay identity remains assembled from `AppearanceSpec` through CharacterCreator2D.

## 10. Global map and travel

The global map is a static fast-travel navigation hub, not an open-world simulation.

- Districts are large stylized regions with recognizable silhouettes.
- Points of interest use consistent printed icons or small facade illustrations.
- Links communicate available travel, time cost, and access rather than street-level geography.
- Locked districts remain visible as muted silhouettes.
- No map weather, time-of-day variants, walking avatar, or animated route.
- Selecting a destination opens a concise confirmation with time, cost, requirements, and available activity categories.
- Confirmed travel uses a brief poster/card/transport transition and resolves immediately.

## 11. Location tableaux

- Each location has one strong illustrated background at first.
- CharacterCreator2D figures and context props are layered over the background.
- Actions are explicit in a lower action area; do not rely on pixel hunting.
- Two to four subtle loops may animate light, signage, curtains, equipment indicators, smoke, or ambient particles.
- Home, studio, and venue progression swaps a few large readable elements or a complete tier background; no furniture placement.
- Generated backgrounds contain no final UI, readable signage required for gameplay, or baked player character.
- Ordinary gameplay backgrounds are 20-30% simpler and lower contrast than key art.

## 12. Iconography and badges

- Core system icons use one-color screen-print geometry with a controlled mix of outline and fill.
- Icons remain recognizable at the smallest HUD size and are tested in monochrome.
- Pair icons with labels in critical contexts.
- Emoji and photorealistic images are not system icons.
- Talents, rare states, achievements, and major career moments may use richer illustrated badges.
- Add icons only for implemented systems; do not build a speculative master library.

## 13. Motion

### 13.1 Ordinary interactions

- Tap/press feedback: approximately `80-140 ms`.
- Card or screen entry: approximately `150-250 ms`.
- Resource change feedback: approximately `200-350 ms`.
- Use decisive ease-out motion; avoid elastic bounce and continuous pulsing.
- All transitions are interruptible and never block an already valid next input.

### 13.2 Major moments

Releases, chart breakthroughs, concert results, major contracts, scandals, retirement, and collapse may use `400-900 ms` staged editorial sequences, sound accents, and stronger composition changes. Repeated sequences become skippable.

### 13.3 Reduced motion

- Remove scale punches, parallax, and nonessential ambient loops.
- Replace movement with opacity/color/state changes.
- Preserve timing clarity and event order.

## 14. Career progression

Keep the shell stable. Show progression through:

- Clothing, grooming, posture, and character condition.
- Home tiers, room capability, equipment, and studio quality.
- Venue scale, crowd treatment, transport, and collaborators.
- The prestige and visual scale of media coverage.
- More prominent chart, poster, cover, and billboard placement.

Failure reverses material context without degrading usability. Poverty, exhaustion, injury, and damaged reputation may change tableau details, portraits, and editorial tone; they do not hide information or make controls intentionally unpleasant.

## 15. AI-assisted art production

### 15.1 Ownership by tool

- **Image generation/editing:** concepts, location backgrounds, map art, loading/key art, posters, album/track placeholder art, textures, and controlled variants.
- **CharacterCreator2D:** persistent player/NPC identity, clothing, portraits, and visible gameplay characters.
- **UI Toolkit:** exact layout, localized text, reusable components, charts, buttons, focus, accessibility, and responsive behavior.
- **Unity/DOTween/URP 2D:** layering, lightweight animation, transitions, particles, lighting accents, and final device rendering.

### 15.2 Production sequence

1. Define gameplay purpose, visible actions, upgrade tiers, and character slots.
2. Select approved mood/reference images and the shared palette.
3. Generate low-cost composition drafts without UI or baked characters.
4. Select one composition and edit one variable at a time.
5. Produce a clean production background with reserved negative space for characters/actions.
6. Separate only the few layers that need animation.
7. Integrate CharacterCreator2D figures and deterministic UI in Unity.
8. Validate crop, contrast, memory, compression, aspect ratios, and touch readability on device.
9. Record prompt, references, generator, date, edits, and final asset path in the provenance log.

### 15.3 Prompt anchor

Every location prompt should preserve these anchors unless an approved exception is documented:

> Polished 2D comic illustration, clean contour, flat color blocks, selective soft shadows, modern music editorial / street-zine composition, restrained halftone and paper grain, warm contemporary city with subtle late-1990s/early-2000s analog flow, graphite/paper/coral/olive/amber palette, gameplay-readable negative space, no UI, no readable text, no logos, no watermark, no photorealism, no 3D, no cyberpunk neon, no luxury-gold stereotype, no copied real-world brand or game imagery.

## 16. Accessibility and localization

- Never encode meaning by color alone.
- Validate body text, icons, focus, and states against appropriate contrast targets.
- Support RU/EN expansion, pseudo-localization, and Cyrillic/Latin glyph coverage.
- Avoid baking functional text into raster art.
- Support touch, mouse, keyboard, Back/Escape, and visible focus.
- Respect safe areas and test narrow/tall and wide mobile layouts.
- Provide reduced motion and do not require hover.
- Keep decorative texture out of text interiors when it harms recognition.

## 17. Explicit anti-style

Rap Way is not:

- A photorealistic crime drama.
- A neon cyberpunk game.
- A luxury black-and-gold interface.
- A graffiti font showcase.
- A casino-like free-to-play HUD.
- A permanent VHS filter.
- A literal copy of GTA or any real artist's branding.
- A sterile corporate dashboard.
- A cute bubbly mobile farm UI.

## 18. Gate acceptance

Stage 3 may be marked complete when:

- The user approves this guide and the first home reference screen.
- Palette and typography direction are accepted.
- The screen remains feasible in UI Toolkit and CharacterCreator2D.
- Cyrillic/Latin support is confirmed for selected font candidates.
- The production pipeline is accepted as sustainable for a solo developer.

### 18.1 UI Toolkit token source

`Assets/_Project/Resources/UI/DesignTokens.uss` is the executable source of truth for the approved palette, shared spacing, border, touch-target, and typography-size tokens.

- UI styles use semantic `--rw-*` custom properties instead of duplicating literal colors or common sizes.
- Screen-specific USS files may introduce layout rules, but must not redefine a core palette token.
- Font asset slots are added to this same file only when the approved Oswald/Manrope assets are imported and their Cyrillic/Latin atlases are validated.
- Changes to a core token require mobile-aspect validation because they intentionally affect every UI Toolkit screen.

Exact component tokens remain provisional until Stage 4 rebuilds the reference screen in UI Toolkit and validates it on representative mobile aspect ratios.

## 19. References

- [Oswald in Google Fonts](https://github.com/google/fonts/tree/main/ofl/oswald)
- [Manrope in Google Fonts](https://github.com/google/fonts/tree/main/ofl/manrope)
- [OpenAI image generation guide](https://developers.openai.com/api/docs/guides/image-generation)
- `Assets/CharacterCreator2D/Preview/Daily/Preview.png` — character rendering compatibility reference only.
- `Docs/Visual/References/mood-home-studio-v1.png` — accepted mood reference.
- `Docs/Visual/References/home-screen-reference-v1.png` — first UI composition reference.
