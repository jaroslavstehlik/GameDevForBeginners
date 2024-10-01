## Operators

![operators](../img/operators.webp)

Operators in programming are like the tools and techniques used in cooking. Just as different kitchen tools or techniques combine and transform ingredients into a dish, operators manipulate data to produce a result.

Assignment operators are like setting your cooking station before you begin preparing a meal. Just as you would assign specific tools and ingredients to their spots on your counter, in programming, you assign values to variables for later use. For example, setting `x = 5` is like placing five tomatoes on your counter, ready to be used in your dish.

```csharp
void Start()
{	
	// assign a to 5
	float a = 5f;

	// assign the value of a to 6
	a = 6;

	// the value of a is 6
	Debug.Log(a);

	// assign b to 3
	float b = 3f;

	// assign the value of b to the value of a
	b = a;

	// the value of b is 6
	Debug.Log(b);
}
```

Consider basic arithmetic operators, such as addition, subtraction, multiplication, and division. Using these operators is akin to measuring ingredients. For example, adding two numbers (`+`) is like combining two different ingredients in a bowl. Subtracting one number from another (`-`) can be compared to removing an ingredient from a mixture, such as taking a bit of salt out if you've added too much. Multiplication (`*`) could be likened to duplicating a recipe for more servings, effectively increasing each ingredient proportionally. Division (`/`) is akin to dividing a recipe into smaller portions, reducing each ingredient accordingly.

```csharp
void Start()
{	
	float a = 5f;
	float b = 6f;

	float resultOfAddition = a + b;
	Debug.Log($"5 + 6 = {resultOfAddition}");

	float resultOfSubtraction = a - b;
	Debug.Log($"5 - 6 = {resultOfSubtraction}");

	float resultOfMultiplication = a * b;
	Debug.Log($"5 * 6 = {resultOfMultiplication}");

	float resultOfDivision = a / b;
	Debug.Log($"5 / 6 = {resultOfDivision}");
}
```

This example shows how we can modify an original value using a shorthand operator instead.

```csharp
void Start()
{	
	float number = 0;
	Debug.Log($"{number} = 0");

	// add 10 to number
	number += 10f;
	Debug.Log($"{number} = 10");

	// subtract 5 from number
	number -= 5f;
	Debug.Log($"{number} = 5");

	// multiply number by 2
	number *= 2f;
	Debug.Log($"{number} = 10");

	// divide number by 2
	number /= 2f;
	Debug.Log($"{number} = 5");
}
```

[Continue with conditions](conditions.md)

[Operators on Microsoft.com](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/)