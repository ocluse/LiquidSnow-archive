# Utilities
These are classes that provide some additional functionality that is not normally found in .NET. There are currently three utilities that are provided, these are the `DateTimeUtility`, the `EnumUtility`, and the `IOUtility`. Each of them is discussed below:

## DateTimeUtility
This utility provides the `FirstDateOfWeek(int week,int weekOfYear)` function that you may find very useful. From the name, this function returns a `DateTime` that represents the first date of the `int week` of the `int year` assuming that the a week always begins on a Monday on the Gregorian Calendar.

## EnumUtility
Using this utility, it is possible to extract all the values of an enum definiton as an `IEnumerable<T>`. This often comes in handy if you wish to enumerate or loop through several enum values. See the example below.

``` cs
using System;
using System.Collections.Generic;
using Thismaker.Core.Utilities;

class Program
{
   static Main(string[] args)
   {
     var fruits=EnumUtility.GetValues<Fruits>();
     foreach(var fruit in fruits)
     {
        Console.WriteLine(fruit.ToString());
     }
   }
}

enum Fruits{Banana, Mango, Apple, Orange, Coconut}
```
## IOUtility
This allows you to combine two or three file/directory paths. Although `System.IO.Path.Combine()` already does this, there are certain scenarios where this utility become useful. Take for example where the user wishes to combine two paths: `C:\Thismaker\` and `\LiquidSnow\Hafser.exe`. Using `System.IO.Path.Combine(string, string)` provides an interesting result, it returns the latter path as is without any modification. This is because Microsoft argues that the path, since it starts with the `\` is an absolute path, and as such the former path is ignored. However in most situations, you will find yourself just merely needing to combine the path to `C:\Thismaker\LiquidSnow\Hafser.exe` and that is where the `IOUtility.CombinePath(string, string)` method really shines. Note that IOUtility is not meant to replace the predefined version, it is meant to substitute in scenarios where the predefined method is limited.
