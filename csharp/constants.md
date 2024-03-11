## Constants

![constants](../img/constants.webp)

Drawing the parallel between programming concepts and cooking further, let's explore the idea of constants. In the culinary world, constants can be likened to the essential, unchanging elements of a recipe. Consider salt: its role and taste remain consistent across dishes. Whether you're baking a cake or seasoning a stew, the salt's fundamental properties—its flavor and its chemical composition—do not vary. It serves as a constant, providing a reliable foundation upon which the complexity of a dish can be built.

In programming, constants hold a similar place of importance. They are the unalterable values that a program relies on throughout its execution. Just as a pinch of salt consistently adds the same flavor enhancement to a variety of dishes, a constant in a program provides a steady, immutable reference point. Whether it's the maximum size of a data structure, the URL of a web service, or a literal value used for comparison, constants ensure that certain critical values remain unchanged, no matter what operations or functions the program performs. This immutability makes constants a crucial tool in maintaining the integrity and predictability of a program's behavior, akin to how staple ingredients like salt help anchor and define the flavor profile of a dish.

```csharp
5; // is a whole number
5.5f; // is a floating point number
5.5; // is a double precision floating point number
's' // is a single character
"this is a plain text"; // is a string
```

While constants might be assigned to a variable 
they appear on the right side from the = equal symbol.
```csharp
int fiveInt = 5;
float fiveFloat = 5.5f;
double fiveDouble = 5.5;
char plainChar = 's';
string plainString = "this is a plain text";
```

They can also appear directly as a parameter of a function.
```csharp
MyFunctionTakesInt(5);
MyFunctionTakesFloat(5.5f);
MyFunctionTakesDouble(5.5);
MyFunctionTakesChar('s');
MyFunctionTakesString("this is a plain text");
```
