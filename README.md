# SlimResult
SlimResult, inspired by [language-ext](https://github.com/louthy/language-ext) and [FluentResult](https://github.com/altmann/FluentResults), is a lightweight and fast .NET 6+ library designed for implementing the Result pattern. It enhances code clarity and simplifies error handling, making your applications more robust and maintainable without sacrificing performance.

## Features
- ```Result<T>``` and ```Result``` are used to hold operation results that might be successful or not. The generic version represents results that hold values.
- ```Option<T>``` declares variables that either have a value or are empty, eliminating nulls and their problems.

## Installation
SlimResult package is available on [NuGet](https://www.nuget.org/packages/FarzanHajian.SlimResult/).

## Usage
Start by adding the following lines:
```csharp
using FarzanHajian.SlimResult;
using static FarzanHajian.SlimResult.SlimResultHelper;      // You need it only if you want to use its helper methods
```
Consider the following method that takes two values and returns their division. The result could be either a successful result that holds the actual division result or a failed result that has an error object that in turn, contains the error message:
```csharp
Result<float> Divide(float a, float b)
{
    if (b == 0) return new Result<float>(new Error("The divisor cannot be 0"));
    return new Result<float>(a / b);
}
```

Here is a piece of code that shows how the method and its result object can be used in the simplest way:
```csharp
float a = 12.5f, b = 45.25f;
Result<float> result = Divide(a, b);
if (result.IsSuccess)
    Console.WriteLine($"The result is {result.Value}");
else
    Console.WriteLine($"An error has been occured: {result.Error.Message}");
```

It's also possible to take a more functional approach:
```csharp
float a = 12.5f, b = 45.25f;
Divide(a, b)
    .Match(
        r => Console.WriteLine($"The result is {r}"),
        err => Console.WriteLine($"An error has been occured: {err.Message}")
    );
```

The next code snippet demonstrates other available methods:
```csharp
float a = 12.5f, b = 45.25f;

// Using "IfSuccess" to chaing results
var res1 = Divide(a, b).IfSuccess(r => new Result<float>(r * 2));
res1 = Divide(a, b).IfSuccess(r => Success(r * 2));     // Using the "Success" helper method defined in SlimResultHelper
res1 = Divide(a, b).IfSuccess(r => r * 2);              // Using the implicit capability of the "Result" class

// Using "IfFailure" this time
var res2 = Divide(a, b)
    .IfFailure(err =>
        {
            Console.WriteLine(err.Message);
            return Failure<float>(err);                 // Using the "Failure" helper method defined in SlimResultHelper
        });

Console.WriteLine(Divide(a, b).ValueOrDefault(-1));     // Prints "-1" in case of any problem

// Creating a guarding against possible exceptions
var res3 = Try(
    () => Console.WriteLine(a / b),     // This might throw an exception
    exp =>
    {
        Console.WriteLine(exp.Message);
        return Failure(exp);            // Exceptions can be impicitly converted to instances of Error
    }
);
```

The "Divide" method can be simplified using the helper methods defined in SlimResultHelper and implicit data type conversions that SlimResult classes support.
In this case, "Failure" and "Success" methods are used to create Result objects, and the error message is automatically converted to an Error object:
```csharp
Result<float> DivideSimplified(float a, float b)
{
    if (b == 0) return Failure<float>("The divisor cannot be 0");
    return Success(a / b);
}
```

Removing ```Success``` is also possible as "a / b" can be implicitly converted to Result:
```csharp
Result<float> DivideSimplified(float a, float b)
{
    if (b == 0) return Failure<float>("The divisor cannot be 0");
    return (a / b);
}
```

And finally:
```csharp
Result<float> DivideSimplified2(float a, float b)
{
    return b == 0 ? Failure<float>("The divisor cannot be 0") : (a / b);
}
```

Keep in mind that a non-generic version of ```Result``` is also available. It is suitable for operations (methods) that do not have any returning value but need to be tracked for their success/failure.
In addition, there are asynchronous versions of the methods you saw above.

Besides ```Result``` there is another type that acts as a container that MIGHT have a value. It helps avoid nulls.
```csharp
Option<float> a = new Option<float>(12);
var hasValue = a.IsSome;
var isEmpty = a.IsNone;
a.Set(120);             // Changing the value
a.Unset();              // Clearing the value
Console.WriteLine(a.ValueOrDefault(0));    // Prints "0" if empty
a.Match(
    v => Console.WriteLine($"The value is {v}"),
    () => Console.WriteLine("Sorry but there nothing there!")
);

// Other ways to create an instance
Option<float> b = Some(14.35f);     // Using the "Some" helper method
Option<float> c = None<float>();    // Using the "None" helper method
Option<float> d = 14.35f;           // Using the impicit cast
```
