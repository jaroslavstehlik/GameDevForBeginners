## Loops

![functions](../img/loops.webp)

In programming, loops facilitate the repetition of instructions.  
Instead of executing an instruction once and moving on, a loop allows the instruction to repeat itself, typically until a certain condition is met.   

Consider the process of kneading dough as an analogy. You repeatedly pull and stretch the dough until it reaches the desired consistency. This change in the dough's texture serves as the condition to end the kneading loop.  

Similarly, imagine pouring milk into a measuring cup. The action of pouring is the repeated instruction. The loop continues until the milk reaches a specific level in the cup, at which point the condition to terminate the loop is met. 

This process illustrates how loops function in programming, repeating actions efficiently until a defined condition is satisfied.

```csharp
// In math we often represent percents ranging from 0 to 100
// In games we often represent percents as floats ranging from 0 to 1

float doughKneedingProgress = 0.0f;

void Start()
{
	// repeat until progress reaches 1.0
	while(doughKneedingProgress < 1.0f)
	{
		// increase the dough kneeding progress by 1 percent
		doughKneedingProgress += 0.01f;
	
		// output the kneeding progress to console
		Debug.log($"Kneeding dough: {doughKneedingProgress * 100}%");
	}
	
	// output to console that we are done
	Debug.log("Dough is done!");
}
```
  
Imagine a tray filled with cupcakes awaiting frosting. We could design a single function to apply a specific frosting pattern to one cupcake. By executing this function for each cupcake on the tray, effectively repeating the pattern across all cupcakes, we establish a loop. This loop allows us to efficiently apply the same decorative frosting to each cupcake, ensuring consistency and saving time.

```csharp
// tray full of cupcakes
CupCake[] cupCakes = new CupCake[10];

void Start()
{
	// make sure to repeat this until we reach all cupcakes
	for(int i = 0; i < cupCakes.length; i++)
	{
		// add frosting over each cupcake
		CupCakes[i].AddFrosting();
	}
}
```

We can write the same program slightly simpler,
each time we repeat the loop we obtain the correct cupcake.
We lost the information about which cupcake we are right now processing
which depends on the program we are writing.

```csharp
// tray full of cupcakes
CupCake[] cupCakes = new CupCake[10];

void Start()
{
	// make sure to repeat this until we reach all cupcakes
	foreach(CupCake cupCake in cupCakes)
	{
		// add frosting over each cupcake
		cupCake.AddFrosting();
	}
}
```

We can also write a loop as a recursive function.
This method is rarely used but sometimes it can be handy.

```csharp
// tray full of cupcakes
CupCake[] cupCakes = new CupCake[10];

void Start()
{
	AddFrosting(cupCakes, 0);
}

void AddFrosting(CupCake[] cupCakes, int cupCakeIndex)
{
	if(cupCakeIndex < cupCakes.Length)
	{
		cupCakes[cupCakeIndex].AddFrosting();
		AddFrosting(cupCakes, cupCakeIndex + 1);
	}
}
```
