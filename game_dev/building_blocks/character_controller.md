# Character controller
Is one of the most important element in any game. It can be as simple as a dot navigating on a grid and as complex as Assassin's Creed character which climbs and runs on walls or Spider man character which can swing from building to building. The main objective is design. We need to limit our character abilities to fit our level design.

***Super mario bros character controller***\
<img src="../../img/supermariobros.webp" alt="supermariobros" height="400"/>
## Controller Input
- Gamepad
- Keyboard/mouse
- Motion controller

## Controller navigation
- ### Grid
	- **Position**, coordinates on map
	- **Orientation**, south, north, west, east
	- Non-walkable tiles, are obstacles
- ### Physics
	- **Position**, xyz coordinates in world
	- **Orientation**, degrees
	- **Velocity**, at which speed and direction the character walks
	- **Colliders**, are obstacles

## Controller modifiers
- Climbing
	- Walls, ladders, boulders, mountains
- Jumping
	- single, double, triple
- Flying
	- jetpack, superpower, glide
- Riding
	- horse, car, boat, bike, creature
- Swimming
	- Swim above water level, dive below water level
- Sliding
	- Wall slide, ground slide
- Swinging

## World modifiers
- Materials
	- Rubber, bouncy
	- Dirt, slow
	- Ice, sliding
- Forces
	- Gravity
	- Wind
	- Water
	- Force fields

## Interactions
- **Action**
	- Press button, push lever, talk with NPCs, open doors
- **Attack**
	- Melee, weapon, magic
	- block, dodge, parry