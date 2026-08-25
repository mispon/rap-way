---
name: unity-mobile-ui-toolkit
description: Builds, reviews, and validates mobile-first runtime UI Toolkit screens for Rap Way. Use when creating or changing UXML, USS, UI layout, responsive mobile behavior, HUDs, menus, modals, or UI Builder previews.
---

# Rap Way Mobile UI Toolkit

## Scope

Use UI Toolkit as a mobile application UI system: UXML is semantic structure, USS is layout and appearance, and C# only binds state and emits intents.

## Layout contract

- Author from Portrait `390 × 844`; validate `360 × 800`, `390 × 844`, and `412 × 915`.
- An ordinary screen fits one viewport with no vertical scrolling. Split, page, or drill down long content instead of shrinking touch targets or overflowing.
- Use one full-screen root, then explicit top chrome, flexible content, and fixed bottom actions/navigation. Only overlays and intentional decorations use absolute positioning.
- Keep a single layout owner for each axis. Do not combine percentage widths with additive margins, competing `flex-grow`, or runtime size patches.
- Use `48px` minimum touch targets. Assume Russian and English may wrap; use concise labels and allow body copy to wrap.

## Styling contract

- Reuse `DesignTokens.uss`; use semantic `--rw-*` variables and shared classes before adding literals.
- Every runtime UXML declares its token USS followed by its screen USS through `<Style>` entries, so UI Builder previews the real appearance without Play mode.
- Do not use unsupported USS `gap`; express spacing through deterministic margins, padding, borders, or dedicated layout elements.
- Do not set ordinary visual styling through C#. C# may apply dynamic state classes and safe-area padding only.
- Keep UXML player text localizable. Preview-only data must remain editor-only and never become a runtime fallback.

## Workflow

1. Inspect the UXML hierarchy, USS classes, runtime-generated elements, and localization text lengths.
2. Define the screen's vertical budget before editing: fixed zones first, then one flexible content region.
3. Implement a coherent screen layout rather than patching individual overlapping elements.
4. Open the UXML in UI Builder at the reference viewport and confirm stylesheet order, hierarchy, and visible geometry.
5. Validate a real runtime flow for localized and dynamically generated content. Use UI Debugger if Builder and runtime differ.
6. Import/compile with Unity, inspect new Console errors, and verify all three mobile sizes before handoff.

## Review checklist

- No clipped or overlapping visible controls.
- No horizontal overflow.
- Every action is reachable and at least `48px` high.
- Primary action is visually dominant; fixed navigation does not compete with content.
- A UXML opened in UI Builder retains the real screen styling without runtime C#.
