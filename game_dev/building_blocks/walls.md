# Walls
Primary building block of most games are walls or barriers. They are the main game design element which gives the player a clear feedback about where they can or cant go.
The most typical scenario would be a maze game. Maze games have very clear functionality without the need to use any script. The two openings of the maze clearly define start and finish.

***Maze***\
<img src="https://media0.giphy.com/media/v1.Y2lkPTc5MGI3NjExNWJ4NmNzcGpvMnhmODJhdXloa2EwZzlidmR2enlqeGZkOWdscTN4byZlcD12MV9pbnRlcm5hbF9naWZfYnlfaWQmY3Q9Zw/DdmrOg4Z4tndBRAtM3/giphy.gif" width="50%" height="50%" />

Colliders also serve us a walking platform. Many platforming games fine tune their colliders meticulously to achieve the perfect platform for jumping, running, wall running etc.

Typical representations are:
## Grid
Grid structure can very easily create a clear path which is walkable.
It clearly presents it self to player and is very predictable. 
Because each block has the same size, it is very simple to design levels with grids.
The main disadvantage of grids is a very blocky character of the whole level.

***The Legend of Zelda: Link's Awakening***\
<img src="https://images.nintendolife.com/screenshots/19687/large.jpg" width="50%" height="50%" />
## Colliders
Physics engines use colliders to define certain volumes which are walkable platforms or un walkable walls. They are expressed as primitive geometric objects with certain volume.
Most common shapes are boxes, spheres, cylinders, capsules.

***The Last of Us***\
<img src="https://lh5.googleusercontent.com/EcW3377sylNzYr3YjvJ2uIZek2iVGj6I8w1Hd6mJXTQkYV5KdW4Biwizj9WR0dEHtOs6NrAurCplXyyz3LYgAszBWMui8jFjqy5fGjYeMF6_EBKE4PcbklLmdVYcJl6YCbeSrpt0?width=535&auto=webp&quality=80&disable=upscale" width="*100%" height="100%" />