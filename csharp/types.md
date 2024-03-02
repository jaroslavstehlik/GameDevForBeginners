## Types
	
Each variable or a constant has a specific type.
	
#### primitive types
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
		
#### composite types
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

