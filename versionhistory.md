Chaotic Comets Version History:
The top section of this document only details the most important changes. Full changelogs are further down this document.

Version 1.3 - UFO AI, Tutorial, Gameplay tweaks (2020 June)
- New UFO type added, and both type's AI have been improved greatly.
- High Score board added to main menu. Tracks top 10 scores for each game mode.
- The game's gotten more difficult. Player ship has reduced base stats. Asteroids travel faster.
- Interactive Tutorial added. Teaches the player game concepts with keyboard or Xbox controller prompts.
- Two auto-save slots, one for each game mode. Cheat mode added to Options.
- Lots of QOL changes, bug fixes, and backend code improvements.

Version 1.2 - UFO, Store, and Saving (2019 June)
- UFOs are more interesting to fight. Retreats at low health. Bonus credits for finishing off after they retreat.
- Asteroids and UFOs now have more kick when they're hit. Avoid bumping into things.
- Game auto-saves at the end of levels. Data is only removed once a new game is started, or the saved game reaches Game Over.
- A shop is available between levels. Top speed, brake efficiency, fire rate, and shot speed can be upgraded.
- Ton of bug fixes & testing.

Version 1.1 - Particles, Music & Bug Fixes (2019 May)
- Particle effects added to player thrusters, teleport. Also added to UFO teleport. Bullets are also particles now.
- Music added. Can be disabled from main menu.
- UFOs that are not destroyed now teleport away at the end of levels.

Version 1.0 - Public release of Chaotic Comets (2019 March)
- Prior versions of the game were created for a design show in November 2018. This patch brought the game to the wider public and mainly focused on UI changes.
- Main menu greatly improved. Dialogs pop up that allow for difficulty and control options.
- All UI elements are compatible with either gamepad or mouse.


DETAILED CHANGELOGS

### Version 1.3 Detailed Changelog (2020-06-05)
UFO changes
- Red UFO added. Flies from left to right, only once, then disappears. Features heavier weaponry.
    - Red UFO deviates in direction up and down as well. Never goes offscreen.
- Green UFO AI improved. No longer rams the player, maintains some distance. Follows players through edges of the screen.
- Shield is also used during collisions to give more realism to the UFOs. Flicks on and off when hitting a large object.
- UFO spawn cap changed. 0 on level 1, 1 on levels 2-3, 2 on levels 4-7, and all others 3 UFOs. Doubles in two-player mode.

Player changes
- Most aspects of the player have been changed. Base fire rate down, base thrust speed up, base shot speed up.
- Knockback from collisions reduced, and collisions no longer spin the player out of rotational control.
- Triple shot has a 12 degree cone instead of 20 degrees.
- Auto-Brake and manual brake will bring the ship to a complete stop.
    - Auto-Brake now shows a particle effect when stopping the ship fast enough.
- Invulnerability time during shield charge-up is reduced.

Gameplay changes
- High Score system added. Can be filtered by game mode and reset. Last used nickname in each mode is saved for the next time.
- Control Test section removed. Replaced with an interactive tutorial.
    - Teaches the player most gameplay aspects, with a choice of P1/P2 keyboard or Xbox controller prompts.
- Cheat mode in Options for spawning props and granting powerups/lives/credits. Disables saving and high scores.
- Shop upgrades are cheaper. (Previous cost + 1000) per level, down from (P cost + 2000).
- Extra lives come every 5000 points, down from 10,000.
- Bullet size shrunk by half. Emits light.
- Player and UFO slightly shrunk.
- All asteroids are faster.

Bonus item improvements
- Powerup canister gives items more fairly. Less receiving of the same few powerups.
- Powerup canister expires faster. Collecting it is urgent. Countdown before disappearing is much more brief.

Sound / UI improvements
- Removed all prompts for controller choices. The newly added Rewired allows for both to control the player at the same time.
- Fullscreen / Windowed toggle added.
- Pause screen pauses all SFX and music, resumes immediately.
- Shop, About, and Player UI updated considerably. All UI has sound effects now.
- Previously loud noises are reduced in volume. UFO noises pitched down to sound different.
- Mouse cursor disappears whenever other control methods are used.

Bug fixes from 1.2
- Player now earns lives based on total credits, instead of current credits that could be spent and not contribute to extra lives.
- UFO attempting to retreat now will always retreat in the opposite direction. No more randomly ramming forward into the player.
- UFO attempting to retreat won't teleport while offscreen anymore. Checks to see if their location is in a certain range, then tries to teleport.
- Player upgrades aren't transferred from one game to another under certain conditions
- UFO bullets killing a player now plays the ship explosion sound.
- Dead spaceships no longer collide with objects.
- Canteens no longer clip into asteroids.
- UFOs can no longer be instantly killed without going through the retreat phase.



### Version 1.2 Detailed Changelog (2019-06-07)
UFO changes
- UFO's base speed and shot speed have increased
- While shield is up, bullets will reflect off of UFO shields
- UFO now goes transparent during teleport

Player changes
- Impact with an asteroid or UFO will slightly push back the player. UFOs exert additional pushback.
- Hard mode no longer removes the brake. It's only half as effective, however.

Gameplay improvements
- The end of each level will now auto-save, and give players the option of shopping for upgrades. As of now, the store has limited features. Later patches will add more options.
    - Upgrades to ship top speed, brake efficiency, fire rate, and shot speed are possible.
    - The price increases with each tier.
    - Shop was extensively tested to work with keyboard, controller, or any swapping between the two. Mouse input is blocked, and any clicks of the mouse also reset the UI and don't break it.
- The game has save detection. If a game is quit, the last autosave is kept on file until the next time the game is attempted to be started. The player's given an option of resuming or starting a new game.
    - That autosave won't be deleted until the setup for a new game to replace it has completed.

Bonus item improvements
- There is a limit of UFOs/canisters per level, that changes depending on level number. Asteroids have no limit.
- UFOs, canisters, and asteroids can enter from any edge of the screen.
- There can be multiple UFOs or canisters onscreen.
- UFOs and canisters have unique explosions. Discourages canister shooting.
- UFOs have a tracking sound effect added.

UI improvements
- Music and SFX volume can be individually changed from main menu.
- Fade transitions occur during certain scene changes.
- Main menu has bigger dialogs.

Bug fixes from 1.1
- Player bullets no longer stack with munition shots. Eg, triple shot used to shoot two bullets from main cannon.
- Canisters & UFOs will no longer spawn with 0 asteroids onscreen
- Picking up a canister during end-of-level shield regen and being given full shields no longer cancels the canister's reward shields.
- A lot of code examination and bug fixes along the way were done to help ensure there will be little problems with new features.



### Version 1.1 Detailed Changelog (2019-05-21)
Player ship changes:
- Top speed has been greatly reduced for better handling.
- Teleport ability has visual cues and invulnerability time.
- Thrusters have visual and sound effects.
- Can no longer teleport once all asteroids are destroyed, this prevents early vulnerability on next level.

Alien ship changes:
- UFO no longer spawns during end level transition, and teleports away if not killed at the end of levels.
- UFO cannot grant any more points by the player shooting in its dying state.

Other changes:
- Music added, can be disabled from main menu.
- All bullets are fading particles instead of static sprites.
- Player button controls have been separated. Eg. Shooting is no longer set off by both Space AND controller A button.
- Code optimisation - particularly with less strain being placed on per-frame updates.
- Player UI layout changed.
- Main/Controls/About menu layouts changed.
- A note has been added to controls screen. Due to key rollover on some keyboards, players 1 & 2 (both playing on keyboard) cannot shoot, thrust and rotate all at the same time. The default controls were kept the same due to the low chance of that specific scenario happening, and that all other attempts to change the controls felt inconvenient to play with.

### Version 1.0 Detailed Changelog (2019-03-18)
- Changed Main Menu UI, made keyboard/gamepad selection easier.
- Changed all UI, compatible with both controller and keyboard.
- Difficulty settings added before each game starts. Easy: Auto braking, Normal: Typical braking, Hard: No braking.
- Control test screen changed to be in the center of screen and has a pause menu.
- Tweaks to shot speed, canteen size, and how ability meter is displayed.
- Help/About menu layouts changed.

### PRE-GAMEJOLT VERSIONS
These builds are not available publicly. Version 1.0 is the foundation for public updates.

Early December 2018 b1.1 - Online Portfolio version
- Native Xbox 360 controller support.
- Spaceship can brake, and Auto-Brake powerup now stops the ship immediately.
- Powerups more common, aliens less common.
- Shield recharges between levels. Every 10,000 points = extra life.

Late November 2018 b1.0 - Design Show Demo
- Version of the game demonstrated at a design show.
- Basic showcase of gameplay loop. 5-level demo.
- Xbox controller support achieved using third-party rebind software
