## Functions

![functions](../img/functions.webp)

Functions in programming are akin to transformative processes in cooking; they represent a change or a series of changes. Consider the action of slicing tomatoes within a recipe. Here, tomatoes serve as our "ingredients"—the data, if you will—and slicing represents the "function" that transforms these ingredients.

The function of slicing requires inputs: whole tomatoes and the desired size of the slices. The output of this function is the sliced tomatoes, ready for further use in the recipe. This process illustrates how a function takes initial inputs, performs defined operations (in this case, slicing to a specific size), and produces outputs.

Similarly, an oven can be thought of as another type of function in our culinary analogy. The inputs for this "oven function" include the uncooked food, the cooking time duration, and the set temperature. The output, much like the result of a programming function, is the transformed state of the input: cooked food, ready to be served.

This approach to understanding functions emphasizes the concept that actions—or functions—in programming receive inputs, undergo a specific set of operations or transformations, and then yield outputs. This parallels cooking actions, where ingredients are transformed through methods such as slicing or heating, resulting in a new state or product.

```csharp
// takes an int as an input value
// outputs an int as an output value
int OurFunction(int value)
{
	return 5;
}

// does not take any input
// outputs an int
int OurFunction()
{
	return 5;
}

// does not take any input
// does not output any output
void OurFunction()
{
	// body of a function
}
```

```csharp
// Previous function would be useles to call.
// Lets make it do something.

// We can modify for example variables which were defined in parent scope.
int myNumber;
void OurFunction()
{
	myNumber = 5;
}

// We can also call another function from a function.
void OurOtherFunction()
{
	// which assignes the value of the variable myNumber to number 5
	OurFunction();	
}

// If we call our self from within a function we cause an infinite recursion.
// So our game hangs indefinitelly.
void OurHangingFunction()
{
	OurHangingFunction();
}
```

Function names

	1myFunction ❌ do not start names with a number
	kočička$#. ❌ do not use special symbols
	my variable ❌ do not use spaces
	My_Function ❌ do not use underscore instead of space in function names
	MyFunction ✅ do start function name with upper case

[Continue with operators](operators.md)

[Functions on Microsoft.com](https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/local-functions)