DISCLAIMER

Dragon Ball, Dragon Ball Z, Dragon Ball Super, and all related characters, concepts, and assets are the property of Akira Toriyama, Shueisha, Toei Animation, and their respective rights holders.

This project is a non commercial fan work created for entertainment and educational purposes only. No copyright infringement is intended.

All original code and systems in this repository are the work of the project contributors unless otherwise credited.

AI assistance was used during early development to help bootstrap a functional baseline. The project is actively being learned and developed further by the author.

----------------------------------------------------------------

# Systems Preview Video
https://youtu.be/Z_VvuwaMq64

----------------------------------------------------------------

# DragonBallAscension (tModLoader) | WIP DBZ Style Terraria Mod

A Dragon Ball inspired Terraria mod focused on:

- Ki resource system and Ki charging
- Transformations with Ki drain and mastery hooks
- Flight and Instant Transmission movement systems
- Future systems including mentors, quests, form unlock progression, Ki attacks and weapons, and full balance pass

----------------------------------------------------------------

## Current Status (Functional Baseline)

This repository is currently in an early v0.5 baseline state. Core systems function, but content and balance are placeholders.

----------------------------------------------------------------

## Design Philosophy

- Ki is an active resource. You must charge to recover it. No passive regeneration.
- Transformations trade power for sustained Ki drain.
- Flight is a core mobility system, not vanilla wings, and will require training.
- Progression is mastery and training driven, not instant unlocks.
- Instant Transmission will require training, with upgrades reducing Ki cost and extending range.

----------------------------------------------------------------

### Planned Milestones

- v0.6 Basic Ki attacks and stat scaling
- v0.7 Mastery system and early progression
- v0.8 Mentor NPC framework
- v0.9 Balance pass and UI polish
- v1.0 Full playable progression to Wall of Flesh

----------------------------------------------------------------

## Works Right Now

- Ki meter using a charge only system with no passive regeneration
- Ki charging via keybind
- Transform and Power Down via keybinds
- Flight toggle that drains Ki
- Instant Transmission to cursor with Ki cost
- Race and Trait switching through testing NPC, effects not yet implemented
- Debug and testing tools for development only, not part of intended gameplay

----------------------------------------------------------------

## Not Implemented Yet (Planned)

- Stat bonuses from races and forms
- Ki attacks, weapons, armor, and accessories
- Mastery progression and unlock requirements
- Mentor NPCs such as Elder Kai, Beerus, and Whis, including questlines
- Transformation tree and UI menu
- Dragon Balls and wish systems
- Full progression content through Wall of Flesh and beyond

----------------------------------------------------------------

## Roadmap (High Level)

Goal for 1.0: A fully playable progression experience through Wall of Flesh with meaningful unlocks, balance, and a functional set of Ki based attacks and items.

Later goals include Mechanical Bosses, Plantera, Golem, and Moon Lord progression paths, mentor arcs, Dragon Balls, and deeper mastery systems.

----------------------------------------------------------------

## Help Wanted

If you would like to contribute, the project is currently looking for:

### Coders (C# / tModLoader)

- Implement race effects and form stat scaling
- Create Ki attacks including blasts, beams, and charge attacks
- Build mastery system based on time in form and combat usage
- Implement mentor NPC logic and quest triggers
- Improve multiplayer safety and system synchronization

### UI and UX Designers

- Design transformation tree and menu UI
- Improve Ki meter polish and accessibility
- Add simple in game tutorial prompts and tooltips

### Balance Testers

- Tune Ki costs and drain rates
- Evaluate form scaling against vanilla progression
- Test rage unlock conditions from pre Hardmode into Hardmode

----------------------------------------------------------------

## Starter Tasks (Good First Contributions)

These are smaller, self contained tasks suitable for new contributors or first pull requests.

- Replace ClampKi usage with Math.Clamp cleanup where appropriate
- Implement a basic Ki blast projectile
- Implement simple stat scaling framework for transformations
- Set up basic transformation buffs with custom buff icons
- Implement trait bonuses and basic race stat effects
- Add initial form buffs usable by both player and mentor NPCs
- Basic Ki value synchronization for multiplayer stability
- Add simple Ki meter UI polish improvements
- Create first pass transformation activation visual feedback
- Implement simple form drain tuning constants for testing

----------------------------------------------------------------

## Installation and Development Setup (Local)

1. Install tModLoader
2. Clone or download this repository into:
   Documents/My Games/Terraria/tModLoader/ModSources/DragonBallAscension
3. Open tModLoader, go to Workshop or Development, then Build and Reload
4. Enable only this mod for clean testing

----------------------------------------------------------------

## Contribution Notes

- Keep changes focused. One feature per pull request when possible.
- Comment tuning constants and explain why changes were made.
- Document any new keybinds added.
- No copyrighted Dragon Ball assets will be included in this repository. Code and original assets only.

----------------------------------------------------------------

## License

MIT License

Copyright (c) 2026 DusttheRegulus

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files, the Software, to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED AS IS, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE, AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES, OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT, OR OTHERWISE, ARISING FROM, OUT OF, OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
