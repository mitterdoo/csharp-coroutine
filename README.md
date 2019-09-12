# csharp-coroutine
A small C# class that allows for use of Coroutines, similar to Lua

### Similarities to Lua
Coroutines in Lua are relatively simple. You create a coroutine that uses a function. Inside the function, you may call `coroutine.yield(...)` to return any values to the `coroutine.resume(...)` invocation of it. Likewise, `coroutine.resume(...)` sends any values in the parenthesis into the resumed coroutine, and has them returned in the `coroutine.yield(...)` call that suspended the coroutine.

Unlike Lua, this C# implementation will *not* catch errors, but it *will* provide a stacktrace that includes the coroutine function!

## Usage
Your coroutine function must follow the signature:
```csharp
IEnumerable<OutType> myCoroutine(Getter<InType> input)
```
where `OutType` is the type of the value returned when yielding within the coroutine, and `InType` is the type of the value fed into the coroutine at the point where it resumed. You must use `Getter.Value` to get the actual value sent inside, as the `ref` keyword cannot be used for this library.
Additionally, `yield return ...` is used to suspend execution of the coroutine, sending the return value to the invocation of `Resume`.

After the coroutine function has been defined, you must create the wrapper object for it, in order to execute the code:
```csharp
Coroutine<InType, OutType> co = new Coroutine<InType, OutType>(myCoroutine);
```
Finally, after creating a wrapper, you may call `Coroutine.Resume` as many times as needed, whenever you want. Be warned, a coroutine cannot be resumed once it has finished! Check `Coroutine.Alive` to tell whether it may be resumed again.

## Example
```csharp
IEnumerable<int> myCoroutine(Getter<int> input)
{
	// Print the value that was fed in first
	Console.WriteLine(input.Value);
	
	// Send `input + 1` to the Resume that invoked the coroutine, and wait to be resumed again
	yield return input.Value + 1;

	// input now has a different value!
	// Square it
	yield return input.Value * input.Value;

}

void Main()
{
	Coroutine<int,int> co = new Coroutine<int,int>(myCoroutine);

	int Value = co.Resume(1); 	// prints 1
	Console.WriteLine(Value); 	// prints 2

	Value = co.Resume(6);		// doesn't print
	Console.WriteLine(Value);	// prints 36

	Console.WriteLine(co.Alive); // prints false, since coroutine has finished
	
}
```
