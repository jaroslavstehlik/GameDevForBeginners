## Conditions

![conditions](../img/conditions.webp)

Conditions in programming are like following a recipe that adjusts based on what ingredients you have on hand. Imagine you're making a soup. The recipe might say, "If you have carrots, add two cups; otherwise, add potatoes." This conditional instruction changes the course of your cooking based on the availability of ingredients, leading to different variations of the soup.

In programming, conditions work similarly through the use of "if-else" statements. These statements allow a program to execute different code blocks "branches" based on whether a condition is true or false. For example, if a user is over 18 (condition is true), they can view certain content; otherwise (condition is false), they are redirected to a different page.

Just as in cooking, where you might adjust the heat based on how quickly something is cooking ("If the water is boiling too rapidly, lower the heat"), in programming, you might adjust the flow of a program based on user input, the state of data, or other conditions. This flexibility allows for dynamic responses and customized outcomes, much like how a skilled chef can adapt a recipe to achieve the best possible dish under the given circumstances.

We can compare two numbers if one number is great than the other

```csharp
void Start()
{
	int age = 18;

	// compare if age is greater or equal to 18
	if(age >= 18) {
		Debug.Log("You are allright!");
	} else {
		Debug.Log("You have to be older than 18!");		
	}

	// is the same as writing
	// compare if age is less than 18
	if(age < 18) {
		Debug.Log("You have to be older than 18!");		
	} else {		
		Debug.Log("You are allright!");
	}
}
```

We can also compare specific words using equality operator ==

```csharp
void Start()
{
	string name = "john";

	// does the name match john?
	if(name == "john") {
		Debug.Log("We found John!");
	} else {
		Debug.Log("This is not the person we are looking for!");
	}

	// Not comparison! but variable assignment!
	// One of the most common mistake programmers make.
	if(name = "john) {
		Debug.Log("We found John!");
	} else {
		Debug.Log("This is not the person we are looking for!");
	}
}
```

If we need to match multiple words we can write it as this

```csharp
void Start()
{
	string name = "john";

	// does the name match john?
	if(name == "john") {
		Debug.Log("We found John!");
	} else if(name == "jack") {
		Debug.Log("We found Jack!");
	} else if(name == "perry") {
		Debug.Log("We found Perry!");
	} else {
		Debug.Log("This is not the person we are looking for!");
	}
}
```

If our conditions branch in many different options we can use switch instead

```csharp
void Start()
{
	string name = "john";

	// pass the value to switch
	switch(name)
	{
		// when we match the value
		case "john":
			Debug.Log("We found John!");
			break;
		case "jack":
			Debug.Log("We found Jack!");
			break;
		case "perry":
			Debug.Log("We found Perry!");
			break;
		// when we do not match any value
		default:
			Debug.Log("This is not the person we are looking for!");
			break;
	}	
}
```

[Continue with loops](loops.md)

[Conditions on Microsoft.com](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/statements/selection-statements)