Chaotic Comets v1.5.1

Developed by CrossfireCam (https://crossfirecam.itch.io)

This is a space-shooter game inspired by Maelstrom and Asterax. Clear levels of asteroids to progress.
There's a two player mode, shop for upgrades, and various temporary powerups.

The game can be played with keyboard or controller. Check 'Options' in-game to rebind controls.
Online multiplayer can be achieved using Parsec: https://parsec.app/

To uninstall this game, delete the entire 'Chaotic Comets v1.5.1' folder.

------------------
OS Specific Instructions
------------------
Windows - Smartscreen may block the game from opening. Click 'More info' and 'Run anyway'.

Linux - The game may not run as an executable.
• For Ubuntu, go to the executable's 'Properties, Permissions' and check the box for 'Allow executing file as program'.
• For other distros, you could run "chmod +x" on the executable to mark it as an executable.
• If the game crashes immediately, check "~/.config/unity3d/CrossfireCam/Chaotic Comets" for an error log

------------------
Changelog
------------------
v1.5 (2023 Jun)
• Game is now playable online through WebGL
• Difficulty: Changes are applied in Options menu. Reworked into 'Normal' & 'Hard'. Both are more difficult
• Ship: Handling improved. Triple shot & rapid shot buffed
• Shop: Shield/Life purchasing removed, most upgrades nerfed
• Misc: SFX changes, greatly reduced UI text, level transitions & respawning is faster
• v1.5.1 Bugfix: Patched Unity vulnerability CVE-2025-59489

v1.4 (2021 Apr)
• Shop: Looks better. More upgrades are available
• Difficulty: Now affects asteroid speed and UFO aggressiveness. Added 'Insane' difficulty
• UI: Game UI takes up less room. Cheat mode UI improved. Add 'bonus timer' that awards extra credits
• Ship: Weapons rebalanced. Manual-fire is faster
• Controller support improved
• v1.4.1 Bug Fix: Tutorial bullet range no longer too short

v1.3 (2020 Jun)
• Red UFO type added. Flies across screen, more damaging shots
• High Score board added
• Interactive tutorial added
• Cheat mode added

v1.2 (2019 Jun)
• UFOs retreat at low health. Bonus credits awarded for finishing them off
• Auto-save added
• Shop added. Player's ship can be upgraded

v1.1 (2019 May)
• Particle effects added
• Music and extra SFX added

v1.0 (2019 Mar)
• Public game release, focused on UI changes
• Main menu improved. Difficulty and control selection dialogs added

b1.0 (2018 Nov)
• Created for a university design show
• Game is compatible with gamepad & mouse

------------------
Troubleshooting
------------------
CONTROLLER NOT WORKING?
- Check the full list of compatible controllers: https://guavaman.com/projects/rewired/docs/SupportedControllers.html
- If a controller's stick is experiencing drift, visit 'Options > Rebind Controls > Calibrate'
- Nintendo Switch Pro Controller won't work over USB - connect via Bluetooth
- If both players are using the same keyboard, then some inputs may be dropped due to limitations on most keyboards

SOMETHING ELSE?
- If key presses in other apps are causing inputs in Chaotic Comets, click back and forth between the windows to fix it
- If Parsec isn't accepting connecting player's keyboard or gamepad, visit https://support.parsec.app/hc/en-us/articles/115002702731
- If you find any bugs, leave a comment at: https://crossfirecam.itch.io/chaotic-comets
- The game may malfunction after upgrading from v1.4 to v1.5. Try visiting Options & clicking 'Reset Game Data'

------------------
Attribution
------------------
Unity Asset Store Credits: Controller Support - Rewired, SFX - Ultimate Sound FX, Music - Ultimate Game Music Collection, Particles - Unity Particle Pack

Shop UI improved with assistance from these guides (PastelStudios & eron82):
- http://blog.pastelstudios.com/2015/09/07/unity-tips-tricks-multiple-event-systems-single-scene-unity-5-1/
- https://forum.unity.com/threads/solved-multiple-eventsystem.512695/

Foundation of this project (first 2 weeks of development, game has changed greatly since then):
- https://www.youtube.com/playlist?list=PLa5_l08N9jzMKA8gNHG42Mv3idfIkztBU

Asteroids and Powerups spawn code assistance (metalted and GlassesGuy):
- https://answers.unity.com/questions/1646067/

Scaling of camera (non-16x9 resolutions) acheived with AspectRatioEnforcer script (Eric Haines / Eric5h5):
- http://wiki.unity3d.com/index.php?title=AspectRatioEnforcer

Fix for an issue where two buttons could be highlighted at once (daterre):
- https://forum.unity.com/threads/button-keyboard-and-mouse-highlighting.294147/

Debugging tool for finding Missing Reference errors in Unity (Lior Tal of Gamasutra):
- https://www.gamasutra.com/blogs/LiorTal/20141208/231690/Finding_Missing_Object_References_in_Unity.php