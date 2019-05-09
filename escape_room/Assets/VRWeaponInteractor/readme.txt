VR Weapon Interactor is comprehensive package of editor extensions and tools for turning your 3D weapon
models into working interactable weapons.

This package requires VRInteraction as a dependency.

Included in this package are all the scripts needed to make a pistol, assault rifle, shotgun or rifle ready to shoot, all the editor scripts
to setup and customize  your own weapon with sounds and effects, an example scene with shootable targets and
a fully setup example weapon along with a setup guide for reference when setting up your own weapon.

Please email me if you find a bug or something doesn't do what you expected: sam@massgames.co.uk
Join the Discord here: https://discord.gg/x4MtDhe

Whats new:
2.4:
-Added increased recoil each consecutive shot.
-Fixed issue with slide locking back when lock back was toggled off.
2.3:
-All 22 Chamferzone weapons now pre-setup if you own the weapon model in integrations
-Improvements to the second held methods
-Added attach and detach attachment events
2.2:
-Implemented new Chamferzone weapon in integrations
-Remove rigidbodies when items are frozen (replaced with marker)
-Moved attachment overrides to specific attachment position so they can be different per weapon
2.1:
-Added FinalIK integration with hand poses
-Added audio source variable
-Improved left and right thumbstick action for Oculus
-Fixed missing CanAcceptMethod check
2.0:
-Re-designed base VR Interaction
-Added full attachment system
-Improved handling on all weapons (Environment collision while held)
-Improved all editors and added tooltips
-Added an ammo tray
1.3.2:
-Updated OVR
1.3.1:
-Fixed unlinked held position bug
-Improved bullet ejection
-Added delay to bullet ejection after firing
1.3:
-Oculus Native support added
-Added Revolver weapon type
-Weapon recoil added
-Improved second held position
-Added burst shot mode
When updating you may find the hover no longer works on weapons. Fix this by going to the
weapon objects (gun mesh, slide etc) find the hover section that should have a 0 under it,
change this to the amount of mesh renderers you would like to use as hovers and drag them in.
Check the example scene weapons for reference on how this should look.
1.2.2:
-Added Oculus Controls Support
-Separated VRInput from VRInteractor
-Added script descriptions
1.2.1:
-Updated to SteamVR 1.2
-Fixed Auto Load
-Added old SteamVR Event system
1.2:
-Inventory system
-Knife throwing
-Second held position works with slide (for shotguns)
-Completely redone controls system
-Added centre pad button to pad keys
-Auto magazine loading
-Added AKM from http://chamferzone.com/ and pistol from http://tf3dm.com/3d-model/m9-39791.html
-Custom decal tag system
-Fixed bug that stopped slide from working on some models
-Loading a magazine will load a bullet and snap the slide
-Listeners disabled when controller is disabled
-Bullet force added

1.1
-Customisable button mappings
-Cone firing
-Reference weapon for assigning weapon to controller at startup (disable pickup/drop buttons to lock it to controller permanently)
-Magazine ejection (with gravity)
-Redone magazine receiver to make setting up easier 
-Multiple holding points on weapons 
-Slide now stays back when there is no bullet 
-Sword sounds added 
-Bullet wizard requires reference from in scene 
-Lots of bug fixes