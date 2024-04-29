# Inventory
An inventory in a game is a virtual space which stores game elements.
Those specific elements are not present in the game world when they are in inventory.
Inventory just holds the information that the element can be brought up to the game if needed.

## Chest
Chests is a very useful game mechanic in games, they gives us a sense of surprise or serve us a safe place where to store our valuables. Chests are usually static, placed somewhere so we can find them and function as a temporary storage room.

## Player inventory
Player inventory can be though of as a backpack. As in real life we store things in our pockets and backpacks. The functionality is exactly the same as with a chest, just the inventory is attached to a moving object, usually us the player. But it can be also on a horse or a car, it depends on the game.

# Implementation
Inventory can be implemented as a single Array.
- **Array**
	- **Continuous**, elements are stored in continuous arrangement. First element is stored at first place in the array, Second element at the second place and so on.
	- **Length**, An array has to specify its length or size so we can tell what is the maximum amount of elements an Array can hold.