[Main page](../../../readme.md)

# Teleports or portals
Teleports and portals can save the player some time or they can transport the player to a different place on the map, which can cause disorientation and excitement. The most common use case of a teleport is when player dies.
When the player dies he needs to spawn at a certain location called spawn point.

***Portal, infinite portal***\
<img src="../../img/portal.gif" alt="portal" height="400"/>
## Trigger
We first need a trigger which detects the player and maybe our objects as well.

## Teleportation point
Then we need a specific place on the map to which the teleport or portal will transport us.

# Implementation
- Trigger, which detects that player or objects has entered the teleport
- Position, which tells where to teleport our player or object
- Move player to a specific position

## Teleport Transform

[![OpenScriptButton](https://img.shields.io/badge/Open%20script-4287f5?style=for-the-badge 'OpenScriptButton')](http://localhost:8081/?actionType=loadScript&value=Runtime/Teleport/TeleportTransform.cs)

This is the simplest teleport we can make. However, it has a major flaw.
If our object contains a RigidBody the teleportation might fail due to collision detection.
When we move RigidBodies around, they still have to detect collisions and if there is a wall in the way, the player might get stuck. Lets make this teleport compatible with RigidBodies then.

## Teleport Rigidbody

[![OpenSceneButton](https://img.shields.io/badge/Open%20scene-4287f5?style=for-the-badge 'OpenSceneButton')](http://localhost:8081/?actionType=loadScene&value=Teleport.unity) [![OpenScriptButton](https://img.shields.io/badge/Open%20script-4287f5?style=for-the-badge 'OpenScriptButton')](http://localhost:8081/?actionType=loadScript&value=Runtime/Teleport/TeleportRigidbody.cs)

Now our teleport actually supports moving RigidBodies around. But we might want to filter only specific objects to be able to teleport. We can use object tag as a filter.