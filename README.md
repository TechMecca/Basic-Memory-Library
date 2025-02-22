# Memory Manipulation Library (x86)

## Overview

This library provides low-level memory manipulation functionalities for x86 applications. It allows reading, writing, and modifying memory addresses dynamically, making it useful for debugging, reverse engineering, and other advanced use cases.

## Features

- Read and write memory of x86 processes
- Apply patches to modify memory content dynamically
- Read null-terminated ASCII strings from memory
- Hook and detour functions in memory
- Handle access violations gracefully

## Installation

Clone the repository and include the library in your C# project:

```sh
$ git clone https://github.com/TechMecca/Basic-Memory-Library.git
```

Add the necessary references to your C# project and compile it.

## Usage

### Reading and Writing Memory

```csharp
IntPtr address = new IntPtr(0x12345678);
int value = MemoryManager.Read<int>(address);
MemoryManager.Write(address, 42);
```

### Reading Strings

```csharp
string str = MemoryManager.ReadString(new IntPtr(0x12345678));
Console.WriteLine(str);
```

### Patching Memory

```csharp
byte[] patchBytes = { 0x90, 0x90, 0x90 }; // NOP instruction patch
Patch patch = new Patch(new IntPtr(0x12345678), patchBytes, "Example Patch");
patch.Apply();
```

### Using Detours

Detours allow you to hook and redirect function calls dynamically.

```csharp
Delegate originalFunction = (Delegate) Marshal.GetDelegateForFunctionPointer(
    (IntPtr) SomeFunction, typeof(TestDelegate));

if (test == null || !test.IsApplied)
{
    test = new Detour(originalFunction, new TestDelegate(CustomFunctionHandler), "TestDetour");
    test.Apply();
}
```

This will redirect calls from `originalFunction` to `CustomFunctionHandler`.

To restore the original function:

```csharp
test.Remove();
```

#### Example of a Detoured Function

```csharp
int CustomFunctionHandler(int arg1, int arg2)
{
    // Call the original function before or after modification
    int result = (int) test.CallOriginal(arg1, arg2);
    
    Console.WriteLine("Function Detoured!");
    return result;
}
```

## Error Handling

This library catches `AccessViolationException` when attempting to read invalid memory locations and logs the issue instead of crashing the application.

## Compatibility

- x86 architecture
- Windows operating systems
- Requires .NET Framework or .NET Core

## Disclaimer

This library is intended for educational and debugging purposes. Use it responsibly and only on systems you have permission to modify.

## License

MIT License. See `LICENSE` file for details.

