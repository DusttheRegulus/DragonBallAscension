# DragonBallAscension (tModLoader) — WIP DBZ-Style Terraria Mod

A Dragon Ball inspired Terraria mod focused on:
- Ki resource system + Ki charging
- Transformations with Ki drain and mastery hooks
- Flight + Instant Transmission movement tech
- Future: mentors, quests, form unlock progression, ki attacks/weapons, balance pass

## Current Status (Functional Baseline)
This repo is in an early “v0.5 baseline” state: core systems work, content and balancing are placeholders.

**Works right now:**
- Ki meter (charge + drain)
- Charging Ki (keybind)
- Transform + Power Down (keybinds)
- Flight (toggle, drains Ki)
- Instant Transmission (teleport to cursor, costs Ki)
- Race + Trait switching via testing NPC (effects not implemented yet)
- Debug/testing tools (temporary; will be removed/locked down later)

**Not implemented yet (planned):**
- Stat bonuses from races/forms
- Ki attacks/weapons, armor, accessories
- Mastery progression + unlock requirements
- Mentors (Elder Kai / Beerus / Whis / etc) and questlines
- Transformation tree UI/menu
- Dragon Balls / wishes systems
- Full progression content to Wall of Flesh and beyond

## Roadmap (High-Level)
**Goal for 1.0:** Playable progression through Wall of Flesh with meaningful unlocks, balance, and at least a small set of ki attacks/items.

**Later goals:** Mechanical bosses → Plantera → Golem → Moon Lord form paths, mentor arcs, Dragon Balls, and deep mastery systems.

## Help Wanted
If you’d like to contribute, I’m specifically looking for:

### Coders (C# / tModLoader)
- Implement race effects + form stat scaling
- Ki attacks (basic blast, beam, charge attacks)
- Mastery system (time in form + combat usage)
- Mentor NPC logic + quest triggers
- Multiplayer safety (syncing key systems)

### UI / UX Designers
- Transformation tree/menu UI
- Ki meter polish + accessibility
- Simple in-game tutorial prompts/tooltips

### Balance Testers
- Ki costs/drain rates
- Form scaling vs vanilla progression
- “Rage unlock” conditions testing (pre-HM → HM)

## Installation / Dev Setup (Local)
1) Install tModLoader
2) Clone/download this repo into:
   `Documents/My Games/Terraria/tModLoader/ModSources/DragonBallAscension`
3) Open tModLoader → Workshop/Dev → Build + Reload
4) Enable only this mod for clean testing

## Contribution Notes
- Keep changes focused: one feature per PR if possible
- Comment tuning constants and explain why they changed
- If you add new keybinds, document them here
- No copyrighted DBZ assets will be included in the repo (code-only / original assets only)

## License
(TODO: choose a license)
