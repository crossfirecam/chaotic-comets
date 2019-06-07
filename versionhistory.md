Chaotic Comets Version History:
The top section of this document only details the most important changes. Full changelogs are further down this document.

2019-06-07 Build 1.2 - UFO, Store, and Saving
- UFOs are more interesting to fight. Retreats at low health. Bonus credits for finishing off after they retreat.
- Asteroids and UFOs now have more kick when they're hit. Avoid bumping into things.
- Game auto-saves at the end of levels. Data is only removed once a new game is started, or the saved game reaches Game Over.
- A shop is available between levels. Top speed, brake efficiency, fire rate, and shot speed can be upgraded.
- Ton of bug fixes & testing.

2019-05-21 Build 1.1 - Particles, Music & Bug Fixes
- Player ship's top speed reduced.
- UFOs that are not destroyed now teleport away at the end of levels.
- Music added. Can be disabled from main menu.
- Particle effects added to player thrusters, teleport. Also added to UFO teleport. Bullets are also particles now.
- Player controls have been fully separated, and rebinding menu in the launcher was made easier to understand.

2019-03-18 Build 1.0 - Public release of Chaotic Comets
- Prior versions of the game were created for a design show in November 2018. This patch brought the game to the wider public and mainly focused on UI changes.
- Main menu greatly improved. Dialogs pop up that allow for difficulty and control options.
- All UI elements are compatible with either gamepad or mouse.


DETAILED CHANGELOGS

1.2 Detailed Changelog
UFO changes
- UFO's base speed and shot speed have increased
- While shield is up, bullets will reflect off of UFO shields
- UFO now goes transparent during teleport
Player changes
- Impact with an asteroid or UFO will slightly push back the player. UFOs exert additional pushback.
- Hard mode no longer removes the brake. It's only half as effective, however.
Gameplay improvements
- The end of each level will now auto-save, and give players the option of shopping for upgrades. As of now, the store has limited features. Later patches will add more options!
-- Upgrades to ship top speed, brake efficiency, fire rate, and shot speed are possible with enough credits. The price increases with each tier.
-- Shop was extensively tested to work with keyboard, controller, or any swapping between the two. Mouse input is blocked, and any clicks of the mouse also reset the UI and don't break it.
- The game has save detection. If a game is quit, the last autosave is kept on file until the next time the game is attempted to be started. The player's given an option of resuming or starting a new game.
-- That autosave won't be deleted until the setup for a new game to replace it has completed.
Bonus item improvements
- There is a limit of UFOs/canisters per level, that changes depending on level number. Asteroids have no limit yet.
- UFOs, canisters, and asteroids can enter from any edge of the screen.
- There can be multiple UFOs or canisters onscreen.
- UFOs and canisters have unique explosions. Discourages canister shooting.
- UFOs have a tracking sound effect added.
- Retro Thrusters renamed to Auto-Brake.
UI improvements
- Music and SFX volume can be individually changed from main menu.
- Fade transitions occur during certain scene changes.
- Main menu has bigger dialogs.
Bug fixes from 1.1
- Player bullets no longer stack with munition shots. Eg, triple shot used to shoot two bullets from main cannon.
- Canisters & UFOs will no longer spawn with 0 asteroids onscreen
- Picking up a canister during end-of-level shield regen and being given full shields no longer cancels the canister's reward shields.
- A lot of code examination and bug fixes along the way were done to help ensure there will be little problems with new features.

1.1 Detailed Changelog
Player ship changes:
- Top speed has been greatly reduced for better handling
- Teleport ability has visual cues and invulnerability time
- Thrusters have visual and sound effects
- Can no longer teleport once all asteroids are destroyed, this prevents early vulnerability on next level
Alien ship changes:
- UFO no longer spawns during end level transition, and teleports away if not killed at the end of levels
- UFO cannot grant any more points by the player shooting in its dying state
Other changes:
- Music added, can be disabled from main menu
- All bullets are fading particles instead of static sprites
- Player button controls have been separated. Eg. Shooting is no longer set off by both Space AND controller A button
- Code optimisation - particularly with less strain being placed on per-frame updates
- Player UI layout changed
- Main/Controls/About menu layouts changed
- A note has been added to controls screen. Due to key rollover on some keyboards, players 1 & 2 (both playing on keyboard) cannot shoot, thrust and rotate all at the same time. The default controls were kept the same due to the low chance of that specific scenario happening, and that all other attempts to change the controls felt inconvenient to play with.

1.0 Detailed Changelog
- Changed Main Menu UI, made keyboard/gamepad selection easier
- Changed all UI, compatible with both controller and keyboard
- Difficulty settings added before each game starts. Easy: Auto braking, Normal: Typical braking, Hard: No braking
- Control test screen changed to be in the center of screen and has a pause menu
- Tweaks to shot speed, canteen size, and how ability meter is displayed
- Help/About menu layouts changed

PRE-GAMEJOLT VERSIONS
These builds are not available on GameJolt. Build 1.0 is the foundation for public updates.

Early December 2018 v1.1 - Post-show Online version
- Native Xbox 360 controller support
- Spaceship can brake, and Retro Thruster powerup now stops the ship immediately
- Powerups more common, aliens less common.
- Shield recharges between levels. Every 10,000 points = extra life.

Late November 2018 v1.0 - Show Demo
- Version of the game demonstrated at a design show
- 5-level demo
