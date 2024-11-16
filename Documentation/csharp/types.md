## Types

![types](../img/types.webp)

Every ingredient possesses a unique flavor profile, akin to how in programming, each data type has distinct characteristics. Consider the nutritional content of our cooking ingredients: some are rich in fats, others in proteins. The ratio of fats to proteins can significantly alter the flavor of a dish, leading us to categorize ingredients based on their nutritional makeup.

Among our culinary repertoire, certain ingredients serve as the foundation of countless dishes. Take, for instance, tomatoes, eggs, and salmon. Each of these can be seen as a fundamental building block in the culinary world.

When we combine these base ingredients, such as in a salad, we're not just mixing flavors; we're creating something new from the foundational elements. A salad represents a harmonious blend of these basic components.

This culinary concept mirrors the structure of programming languages. In programming, we have fundamental elements known as primitive data types. These are the basic ingredients from which we start - akin to our tomatoes, eggs, and salmon. And just as combining these ingredients can create a salad, in programming, when we combine primitive types, we create what are known as composite types.

Primitive data types are the simple, indivisible types that form the basis of programming. These include types like integers, booleans, and characters - each serving a unique purpose and representing different kinds of data.

Composite types, on the other hand, are akin to our salads. They're formed by combining primitive types or other composite types into more complex structures. Examples include arrays, structures, and classes. Just as a salad brings together various flavors into a cohesive dish, composite types combine simpler data types to create more intricate and versatile data structures.
### primitive types
bool | boolean | binary true or false
```csharp
bool positive = true;
bool negative = false;
```

char | character | single letter value
```csharp
char letterA = 'a';
char letterB = 'b';
```

int | integer | whole number	
```csharp
int positiveNumber = 24;
int negativeNumber = -126;
```
		
float | float | floating point number
```csharp
float positiveFloatingNumber = 3.14f;
float negativeFloatingNumber = -126.0f;
```

double | double | double precision floating point number
```csharp
double positiveFloatingNumber = 3.14;
double negativeFloatingNumber = -126.0;
```

enum | enumeration | list of options represented by a whole number
```csharp
enum myEnum {
	firstEnum,
	secondEnum
	thirdEnum
}
```
		
### composite types
[] | array | multiple elements
```csharp
bool[] arrayOfBooleans;
char[] arrayOfCharacters;
int[] arrayOfIntegers;
float[] arrayOfFloats;
double[] arrayOfDoubles;
```

string | string | array of characters, text
```csharp
string myString = "this is my first string";
```

struct | structure | container containing multiple variables
```csharp
struct MyStruct {
	// body of MyStruct
	bool[] myArrayOfBools;
	int myInteger;
	float myFloat;
	string myString;
}
```

class | class | container containing multiple variables and functions
```csharp
class MyClass {
	// body of MyClass
	bool[] myArrayOfBools;
	int myInteger;
	float myFloat;
	string myString;

	void MyFunction()
	{
		// body of MyFunction
	}
}
```

[Continue with scope](scope.md)

[Types on Microsoft.com](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/types)