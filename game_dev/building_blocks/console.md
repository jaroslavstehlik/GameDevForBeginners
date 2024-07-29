# Console
Every game, every program and everything on your PC starts with a console.  
It is the simplest form of user input and visual feedback a computer can represent.  
Consoles in games are heavily used for script debugging, collecting bread crumbs,  
making sure that certain parts of your program did run properly and also  
for logging errors.

Therefore a console is the biggest friend of any developer.

```csharp
using UnityEngine;

public class Console : MonoBehaviour
{
    public void Log(string message)
    {
        Debug.Log(message);
    }
}
```

This simple script will provide us a public function which can be used in the
unity editor to debug custom messages or events. 
It is the simplest program we can write and it is often called
Hello World!