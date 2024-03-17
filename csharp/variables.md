## Variables

![variables](../img/variables.webp)

Just as constants are akin to the staple ingredients in our culinary analogy, variables in programming are like the versatile ingredients found in a kitchen, such as vegetables, meats, or spices, whose quantities and types might change depending on the dish being prepared. In cooking, a chef might adjust these ingredients based on taste preference, the number of servings, or even the ingredients available at the moment. This flexibility allows for a wide range of dishes to be created from a basic set of ingredients, each tailored to the occasion.

Similarly, variables in a programming context are named storage locations that can hold different values over time. Just like adjusting the amount of carrots in a stew or the type of cheese in a pasta dish can dramatically alter the flavor profile, changing the values stored in variables affects the behavior and output of a program. Variables provide the flexibility to process and manipulate data dynamically, responding to user input, external data, or the program's internal state changes. This adaptability is essential for creating responsive, efficient, and complex software systems.
	
Variable is always defined by:

	type name = value;

Variables can be assigned

```csharp
int integerNumber = 5;
float floatingPointNumber = 5.5f;
double doubleFloatingPointNumber = 5.5;
bool boolean = true;
char singleCharacter = 's';
string text = "this is a plain text";
```

Variables can be also unassigned

```csharp
int integerNumber; // default value is 0
float floatingPointNumber;  // default value is 0f
double doubleFloatingPointNumber;  // default value is 0.0
bool boolean; // default value is false
char singleCharacter;  // default value is '\0'
string text;  // default value is null
```

Unassigned variables are always assigned by the compiler with its default value specified by the type. Default value of any type can be obtained by calling:

```csharp
default(int); // 0
default(float); // 0f
default(double); // 0.0
default(bool); // false
default(char); // '\0'
default(string); // null
```

Variable names

	1variable ❌ do not start names with a number
	kočička$#. ❌ do not use special symbols
	my variable ❌ do not use spaces
	MyVariable ❌ do not start variable name with upper case
	my_variable ✅ you can use underscores instead of spaces
	myVariable ✅ use camel case instead of spaces

[Continue with types](types.md)

[Variables on Microsoft.com](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/variables)