# App icon concepts

The repository intentionally retains both approved soft-illustration sources:

- `teddy-moon-b-soft.png`: sleeping curled-up teddy; currently used by the app.
- `teddy-moon-c-soft.png`: teddy hugging the crescent; retained as an alternative.

The `prepare-icons.mjs` helper removes the white generation surround, normalizes the artwork to a square navy canvas, and creates `*-production.png` files. `render-comparison.mjs` renders both production variants at 16, 24, 32, 48, and 256 pixels for legibility review. Both scripts require the `sharp` Node.js package.

The active production copy is `src/OpenSleepMusic.App/Resources/AppIcon/appicon.png`. Do not replace either soft source when changing the active variant; regenerate and copy the selected production output instead.
