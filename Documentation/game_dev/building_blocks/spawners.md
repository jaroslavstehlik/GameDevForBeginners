[Main page](../../../readme.md)

# spawners
Spawners are areas which spawn specific units. Those units can be enemies, loot, npcs basically everything in the game which needs to respawn at a certain time is considered spawner. 

***Minecraft monster spawner***\
<img src="../../img/minecraft_spawners.webp" alt="minecraft" height="400"/>
## Object to spawn
We have to spawn something, otherwise it would be not considered a spawner.
That something is usually a game element or a unit.

## Event
The object has to spawn at a specific time called event.
We usually spawn things at a specific interval, for example once in a minute.
Or we spawn things when we kill an enemy, another enemy is spawned, therefore
the number of enemies remains constant.

## Instance Spawner

[![OpenScriptButton](https://img.shields.io/badge/Open%20script-4287f5?style=for-the-badge 'OpenScriptButton')](http://localhost:8081/?actionType=loadScript&value=Runtime/Spawner/InstanceSpawner.cs)

In combination with a timer we clone the spawned game object every duration we set. 
This implementation has few issues however.

- The longer the spawner exists the more it spawns objects which might in best case decrease our game frame rate and in worst case crash our game. This can be resolved by a another timer which destroys the spawned objects
- Spawning GameObjects can create hiccups in our game, which will affect smoothness of our gameplay.

So what can we do to make this safer? Lets use a memory pool.
A memory pool is just an array of GameObjects which are instantiated when needed. Individual objects are disabled and when the spawner is about to spawn an object it just moves the object to the spawn location, resets all its states and turns its visibility on.

## Memory pool
Usually we don't want to spawn many new elements at once because they very often cause hiccups. A hiccup in a game is caused by some operation which takes too much time. If we create elements when the game loads instead ahead of time, we still get some hiccup but it is during level loading which is more acceptable. Then during spawning we just enable and disable those elements, which creates the same effect as spawning but without the hiccup.

# Implementation
Spawner can be implemented as:
- **Array**, an array of elements which represents our memory pool.
- **Integer**, representing the current index of an element we want to spawn.
- **Spawn function**, which enables our element in scene and increments the spawn index.
- **Spawn event**
	- Timer, when our timer finishes, we spawn another element.
	- Game event, when our enemy is killed, we spawn another enemy.

# Memory Pool Spawner

[![OpenSceneButton](https://img.shields.io/badge/Open%20scene-4287f5?style=for-the-badge 'OpenSceneButton')](http://localhost:8081/?actionType=loadScene&value=Spawner.unity) [![OpenScriptButton](https://img.shields.io/badge/Open%20script-4287f5?style=for-the-badge 'OpenScriptButton')](http://localhost:8081/?actionType=loadScript&value=Runtime/Spawner/MemoryPoolSpawner.cs)

While this implementation is much more efficient, the crucial portion of this implementation is the resetting of state every time we need to spawn the object. 
If the Object has complex states, for example animations, physics or game logic that also needs to be reset whenever we need to spawn an object. 
Keep this in mind, the more complex the state, the more complex the reset becomes as well. In a complex reset state might be actually simpler to use the creation and destruction as this always resets the state to its beginning.