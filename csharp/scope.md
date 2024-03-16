## Scope

![scope vs cooking](../img/scope.webp)

Think of programming scope like the workspace in a kitchen. In a kitchen, you have different areas designated for specific tasks—there's a spot for preparing vegetables, another for mixing ingredients, and perhaps a station for plating. Each area has its tools and ingredients, accessible only within that space. In programming, scope functions similarly: it defines where variables and functions are accessible. Variables defined within a function (like tools in our vegetable prep area) are available only in that specific function, not elsewhere in the program. Just as you wouldn't use a salad fork to flip a steak on the grill, a variable defined within one function can't be used directly in another, unless it's made available through specific mechanisms, akin to moving an ingredient from one part of the kitchen to another. This organization ensures that our cooking (and coding) environment remains orderly, efficient, and free of unexpected interactions.

Each class, struct and function has a body.
Where we define our variables affects the accessibility of those variables.
	
variable is accessible only by the body of the class
	
```csharp
class MyClass 
{
	int myVariable;
}
```

variable is accessible only by the body of the function

```csharp
class MyClass 
{
	void MyFunction() 
	{
		int myVariable;
	}
}
```

function is accessible only by the body of the class

```csharp
class MyClass 
{
	void MyFunction() 
	{
	}
}
```

if we want to make a function or a variable visible outside of the class
we need to define in the body of the class and make it public.

```csharp
class MyClass 
{
	public void MyFunction() 
	{
	}
}
```

if we want to make a class accessible to other classes.
we have to make it public as well.

```csharp
public class MyClass 
{
	public void MyFunction() 
	{
	}
}
```