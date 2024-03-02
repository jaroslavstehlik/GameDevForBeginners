## Scope

Each class, struct and function has a body.
Where we define our variables affects the accessibility of those variables.
	
variable is accessible only by the body of the class
	
```csharp
	class MyClass {
		int myVariable;
	}
```

variable is accessible only by the body of the function

```csharp
	class MyClass {
		void MyFunction() {
			int myVariable;
		}
	}
```

function is accessible only by the body of the class

```csharp
	class MyClass {
		void MyFunction() {
		}
	}
```

if we want to make a function or a variable visible outside of the class
we need to define in the body of the class and make it public.

```csharp
	class MyClass {
		public void MyFunction() {
		}
	}
```

if we want to make a class accessible to other classes.
we have to make it public as well.

```csharp
	public class MyClass {
		public void MyFunction() {
		}
	}
```