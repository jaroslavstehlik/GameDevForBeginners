[Main page](../../readme.md)

# Camera
Without a proper camera in our game we would be unable to see the player or the world.  
A camera is trying to mimic a physical camera in real world.
It has a position, orientation, aspect ratio and field of view.

Most common camera behaviours in games
# Fixed camera

***Resident evil 2***\
<img src="../../img/resident_evil_2.webp" alt="resident_evil_2" height="400"/>

You just place your camera in the scene without any script

# Follow camera

***Super Mario Bros 3***\
<img src="../../img/super_mario_bros_3.gif" alt="super_mario_bros_3" height="400"/>

Simplest camera which just follows the target with slight delay

[source code | FollowCamera.cs](../Unity/Assets/Camera/FollowCamera.cs)

# First person camera

***CS GO***\
<img src="../../img/cs_go.gif" alt="cs_go" height="400"/>

Slightly advanced camera which allows the player to look around

[source code | FirstPersonCamera.cs](../Unity/Assets/Camera/FirstPersonCamera.cs)

# Third person camera

***Red Dead Redemption***\
<img src="../../img/red_dead_redemption_horse.webp" alt="red_dead_redemption_horse" height="400"/>

Advanced camera which adds certain offset from player.
The movement of the camera is called camera orbit as it orbits around the player.
When the camera detects collision between player and camera it comes closer to prevent occlusion.
It allows custom distance from player by using mouse scroll wheel.

[source code | ThirdPersonCamera.cs](../Unity/Assets/Camera/ThirdPersonCamera.cs)