# ClassicTriTip

Restores the Tri-Tip Dagger's bleed as it was before two nerfs:

- **15% bleed chance per dagger** (+15% per stack), as before the Anniversary Update; vanilla now gives 10%. The item's pickup text and description show the configured chance.
- **Its own roll alongside Shatterspleen**, as before Survivors of the Void: when a Shatterspleen crit bleeds an enemy, the dagger still rolls and can add a second Bleed stack on the same hit. Vanilla now inflicts only one.

Shatterspleen itself, bleed damage (240% base damage over 3 s × proc coefficient), duration and stacking are unchanged.

## Installation
Install with a Thunderstore-compatible mod manager, or copy the `plugins/ClassicTriTip` folder of the release zip into `BepInEx/plugins/`. Requires BepInEx, RoR2BepInExPack and HookGenPatcher.

## Multiplayer
Host-side: only the host needs the mod. Other players can join with or without it; without it, their item text keeps showing 10%.

## Configuration
`BepInEx/config/com.hugocacciatori.ClassicTriTip.cfg`, section `TriTipDagger`:
- `ChancePerStack` (default 15): bleed chance in percent each dagger gives; 10 is vanilla.
- `RollWithShatterspleen` (default true): the dagger's own roll on Shatterspleen crits.

Changes apply while playing.
