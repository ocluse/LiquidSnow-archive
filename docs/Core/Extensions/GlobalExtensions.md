# Global Extensions

There are several global extensions provided. They include:
1. [GetBytes<T>()](#getbytest) for objects of type string
2. [GetString<T>()](#getstringt) for objects of type byte array(byte[])
3. [GetPropValue<T>(string name)](#getpropvaluetstring-name) for all object types
4. [SubArray<T>(int startIndex, int length)](#subarraytint-startindex-int-length) for all array types
5. [ToBlock()](#toblock) for objects of type string
6. [IsString(string value)](#isstringstring-value)bool IsString(string value) for objects of type char[]
7. [IsPerfectSquare()](#isperfectsquare) for objects of type double.
 
## `GetBytes<T>()`
Returns the bytes of a string for a particular encoding. Consider the example below:
```cs
string name = "Kay";
byte[] nameBytes=name.GetBytes<UTF8Encoding>();
```

As you can see, the return type is a byte array containing the sequence of bytes representing the string. The generic type T must be of type Encoding. Currently, supported encoding types are:
* UTF8Encoding
* UTF7Encoding
* ASCIIEncoding
* UTF32Encoding
* UnicodeEncoding.

## `GetString<T>()`
Returns the string represented by the provided bytes in a specific encoding. A usage scenario, like the one indicated below, is where you are reading from a file using a stream. As `stream.Read(byte[],int,int)` returns the result into a `byte[]`, using this function you can convert that result into it's representative string. For example:
```cs
using (FileStream fs = File.OpenRead("data.txt"))
{
    byte[] buffer=new byte[4];
    fs.Read(buffer, 0, buffer.Length);
    string characters = bufffer.GetString<ASCIIEncoding>();
}
```
See [`GetBytes<T>`](#getbytest) for supported encodings

## `GetPropValue<T>(string name)`
This extension allows you to obtain the property of an object by name. The use case scenarios of this is wide. Consider the example below:
```cs
public class Human:IAnimal
{
    public object Live() 
    {
   //we do some work and return a result
    }
}
```
In the example above, the class `Human` inherits from `IAnimal` which requires inheritors to declare the function `Live` with a return type `object` that is unique for each inheritor. Supposing you were aware that `Human.Live()` returns a type `Lifetime` that has a property `Quality` in it, normally you'd approach it like this:
```cs
Human human=new Human();
Lifetime life=(Lifetime)human.Live();//First you cast the result.
var quality = life.Quality; //Then you access your result.
```
While this might be appropriate in some situations, in some situations you can essentially do this to quickly get the same thing:
```cs
var quality = human.Live().GetPropValue<LifeQuality>("Quality");
```
You pass the type you want the property in, in this case `LifeQuality`, and the name of property, in this case "Quality"

This function has the overload `GetPropValue(string name)` that returns type `object` in case you are unsure of the type of property, but are sure of the name, which is a bit unlikely.

## `SubArray<T>(int startIndex, int length)`

Similar to `string.Substring(int startIndex, int length)` this function allows you to get the sub-array from a bigger array. The start index is the index of the original array where the new array should start, and the length is the number of items from the start index that should be included in the resultant array from the original array.

Note that the value provided for `length` should not exceed the Length of the initial array or else an ArgumentOutOfRangeException will be thrown. See the example below for how one might employ this extension:

```cs
string[] fruits =new string[]{"apple", "mango", "orange", "banana", "dates"};
string[] fav_fruits=fruits.Subarray(2, 3);
```
In the preceeding exampe, the array __fav_fruits__ will be created with the fruits orange, banana and dates.

## `ToBlock()`
This particular extension applies to strings. It transforms an input string to the so called block case. This is where all first letters are capitalized, like in a sentence. For example the string "hello where are you" becomes "Hello where are you".

## `IsPerfectSquare()`
This extension checks where a double is a perfect square. Perfect squares are numbers whose square-root is a full integer. For example: 4, 9, 16, 25, 36, 49, 64 etc. The function returns a true if the double is a perfect square of a certain number.