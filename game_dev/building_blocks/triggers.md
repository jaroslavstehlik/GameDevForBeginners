# Triggers
Triggers are very similar to colliders but instead of being a barrier which cant be walked in to, it is a detector which main purpose is to be walked in to or detect that something has entered it. It can detect players, npcs, objects.

Triggers are the most primitive game element we can introduce in a game. From a simple switch which opens some door, to a very complicated puzzles or systems which can very easily keep us occupied for several hours.

<img src="https://i.imgur.com/u4CcL.gif" width="100%" height="100%" />


What makes a trigger to detect stuff?
# Grid
On a grid based game, we can identify a trigger with a specific trigger tile. If we can identify which tile on the grid is for walking and which is not, we can also create another class of a tile which is meant for triggering.

# Colliders
Primitive shapes which describe a specific volume can be also used as triggers in most physics engines. We usually have to use some kind of flag on the collider to make it act as a trigger, otherwise it would be functioning as a wall.

# Events
Every trigger if it detects something has to trigger an event. An event is simply a link to a specific function. That function is called when we activate the trigger.