# spawners
Spawners are areas which spawn specific units. Those units can be enemies, loot, npcs basically everything in the game which needs to respawn at a certain time is considered spawner. 

***Minecraft monster spawner***\
![minecraft](../../img/minecraft_spawners.webp)
## Object to spawn
We have to spawn something, otherwise it would be not considered a spawner.
That something is usually a game element or a unit.

## Event
The object has to spawn at a specific time called event.
We usually spawn things at a specific interval, for example once in a minute.
Or we spawn things when we kill an enemy, another enemy is spawned, therefore
the number of enemies remains constant.

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