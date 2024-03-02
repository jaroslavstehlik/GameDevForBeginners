## Functions

Action which can take in input and return an output

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
