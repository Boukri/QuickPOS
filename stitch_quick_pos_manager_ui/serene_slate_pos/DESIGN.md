# Design System Document: The Clinical Sanctuary

## 1. Overview & Creative North Star
The Creative North Star for this design system is **"The Clinical Sanctuary."** 

In the high-end body care industry, the interface must bridge the gap between medical-grade precision and spa-like serenity. We are moving away from the "grid-of-boxes" POS template. Instead, this system utilizes **Tonal Layering** and **Intentional Asymmetry** to create an interface that feels curated rather than manufactured. 

By prioritizing "breathing room" (negative space) and soft transitions over rigid borders, we ensure the POS remains intuitive during high-traffic retail moments while maintaining a premium brand aesthetic. We treat the screen not as a flat surface, but as an editorial layout with depth, utilizing overlapping elements and sophisticated typographic scales to guide the user’s eye.

## 2. Colors & Surface Architecture
The palette is a sophisticated blend of "Deep Spruce" teals, "Slate" grays, and "Alabaster" whites. 

### The "No-Line" Rule
To achieve a high-end editorial feel, **1px solid borders are strictly prohibited for sectioning.** Boundaries must be defined solely through background color shifts.
*   **Surface Hierarchy:** Use `surface-container-low` (#f2f4f6) for the primary background of the application. 
*   **Nesting:** Place `surface-container-lowest` (#ffffff) elements (like product cards) on top of `surface-container-low` (#f2f4f6) zones to create a natural, soft lift.
*   **Active Zones:** Use `surface-container-high` (#e6e8ea) to denote recessed areas, such as an inactive sidebar or a search tray.

### The "Glass & Gradient" Rule
Standard flat buttons are insufficient for a premium experience.
*   **Signature Gradients:** Use a subtle linear gradient (Top-Left to Bottom-Right) transitioning from `primary` (#02645f) to `primary_container` (#2d7d78) for primary action buttons. This provides a tactile "soul" to the interface.
*   **Glassmorphism:** For floating elements, such as "Quick View" modals or the "Active Cart" panel, use a semi-transparent `surface_container_lowest` with a `backdrop-blur` of 20px. This allows the product colors to bleed through, creating an integrated, modern feel.

## 3. Typography
This system employs a dual-typeface strategy to balance clinical clarity with boutique personality.

*   **Display & Headlines (Manrope):** Use Manrope for all `display-` and `headline-` tokens. Its wide stance and modern geometric shapes convey an authoritative yet approachable "editorial" tone. Use `headline-lg` for dashboard metrics to make numbers feel like a design statement.
*   **Utility & Body (Inter):** Use Inter for all `title-`, `body-`, and `label-` tokens. Inter’s high x-height and neutral character ensure that inventory counts, SKU numbers, and prices are legible at a glance, even on smaller POS tablets.
*   **The Power of Scale:** Use `display-lg` (3.5rem) for daily revenue totals on the dashboard. The contrast between this and `label-sm` (0.6875rem) metadata creates a hierarchy that feels intentional and high-end.

## 4. Elevation & Depth
Depth is achieved through tonal stacking rather than heavy shadows.

*   **Tonal Layering Principle:** 
    *   **Level 0 (Background):** `surface` (#f7f9fb)
    *   **Level 1 (Sectioning):** `surface-container-low` (#f2f4f6)
    *   **Level 2 (Interaction Cards):** `surface-container-lowest` (#ffffff)
*   **Ambient Shadows:** When an element must "float" (e.g., a dropdown menu), use a shadow with a 24px blur, 0px spread, and 6% opacity, tinted with the `on_surface` color. Avoid pure black shadows.
*   **The Ghost Border Fallback:** If a container requires further definition for accessibility, use a "Ghost Border": `outline_variant` (#bec9c7) at 15% opacity. This provides a "suggestion" of a boundary without cluttering the visual field.

## 5. Components

### Product Cards
*   **Structure:** No external border. Use `xl` (1.5rem) corner radius.
*   **Styling:** The image container should use `surface-container-highest` (#e0e3e5). The product name uses `title-md` in `on_surface`.
*   **Interaction:** On tap/hover, the card should scale slightly (1.02x) and shift from `surface-container-lowest` to a subtle gradient of `primary_fixed_dim`.

### Metric Cards (Dashboard)
*   **Style:** Bold and asymmetric. Place the metric value (`headline-lg`) in the top-left and the trend indicator (`label-md`) in the bottom-right.
*   **Coloring:** Use `tertiary_container` (#a26045) for alert-based metrics (e.g., "Low Stock") to provide a warm, organic contrast to the cool teal primary palette.

### Data Tables (Inventory)
*   **No Dividers:** Forbid the use of horizontal lines. 
*   **Zebra Striping:** Use alternating rows of `surface-container-low` and `surface-container-lowest`. 
*   **Typography:** Column headers must use `label-sm` in all-caps with 0.05em letter spacing for a professional, "ledger" feel.

### Buttons & Touch Targets
*   **Sizing:** All primary POS buttons must have a minimum height of 56px to ensure accessibility for high-speed touch interactions.
*   **Primary:** Gradient of `primary` to `primary_container` with `on_primary` text.
*   **Secondary:** Ghost style using `surface_container_high` background and `on_secondary_container` text.

### Glass Sidebar (The Cart)
*   The checkout sidebar should use a `surface_container_lowest` at 80% opacity with a heavy backdrop blur. This separates the "transaction" from the "shopping" experience visually.

## 6. Do's and Don'ts

### Do
*   **Do** use `md` (0.75rem) or `lg` (1rem) rounding for most containers to maintain the "Soft Minimalist" aesthetic.
*   **Do** use vertical white space (32px, 48px, or 64px) to separate dashboard modules rather than lines.
*   **Do** utilize `primary_fixed` (#a4f0ea) for subtle highlights in search bars or active filters.

### Don't
*   **Don't** use pure black (#000000) for text. Always use `on_surface` (#191c1e) to maintain a soft, premium contrast.
*   **Don't** use standard "Success Green" or "Warning Orange." Use the system's `primary` for success and `tertiary` for warnings to keep the brand cohesive.
*   **Don't** cram information. If a table feels tight, increase the row height and let the user scroll. High-end design requires space to breathe.

### Accessibility Note
While we prioritize a "no-line" aesthetic, always ensure that the contrast ratio between `surface` tiers meets WCAG AA standards. Use the `outline` token at low opacities if user testing indicates a need for clearer container definition on lower-quality POS tablets.