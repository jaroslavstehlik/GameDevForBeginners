[Main page](../../readme.md)

# Timers
Timers are essential blocks for timing certain events. Certain portions of gameplay can be restricted by a timer. Super Mario Bros has a level timer by which the player has to finish the level, otherwise he dies. Super Smash Bros has a match timer which limits the duration of a match during which players have to defeat one another.

Timers play an essential role in many aspects of game play. Spawner has a timer which tells when a certain object or enemy spawns. Spells have very often cooldowns a timer which tells for how long does the player have to wait until they can activate the spell again.

***Bomberman bomb explosion***\
<img src="../../img/bomberman.webp" alt="bomberman" height="400"/>
## Duration
Timers have duration in seconds at which the timer has to trigger an event.

# Cycles
Timers can repeat them selves or they can be a one time event.
They can be specified by the number of cycles.

# Events
The event which has to be triggered when the timer ends.

# Implementation
- Duration in seconds
- Coroutine which waits for duration before it executes event.
- Event, trigger an even when the timer ends.

## Basic Timer

[source code | BasicTimer.cs](../Unity/Assets/Timer/BasicTimer.cs)

## Cyclic Timer

[source code | CyclicTimer.cs](../Unity/Assets/Timer/CyclicTimer.cs)