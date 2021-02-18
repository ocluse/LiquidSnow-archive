# Extensions

There are several extensions to various classes provided by Core. They are therefore divided into the general namespaces within which they fall This also makes it easy to use them without cluttering your code, as they only become available once you refer to a library that they target. Currently, there are:
1. [Global Extensions](#global-extensions)
2. [Generic Extensions](#generic-extensions)
3. [ObjectModel Extensions](#objectmode-extensions)
4. [IO Extensions](#io-extensions) 

# Global Extensions
These are extensions that do not fall into any specific namespace.

See [Global Extensions Page](./GlobalExtensions.md)

# Generic Extensions
These are extensions for the `System.Collections.Generic` namespace

See [Generic Extensions Page](./GenericExtensions.md)

# ObjectModel Extensions
Are extensions for `System.Collections.ObjectModel` namespace

See [ObjectModel Extensions Page](./ObjectModelExtensions.md)

# IO Extensions
Are extensions targeting `System.IO` namespace.

Core offers the `ReadAlBytes()` extension to all types of `Stream` that returns all the bytes of the stream, from beginning to end. Although a similar effect can be achieved differently from different `Stream` derivers, for example `MemoryStream.ToArray()` or `File.ReadAllBytes()`, the fact that the provided methods are not constant can sometimes be problem. Consider a scenario where you are developing an API that requires type `Stream` as one of the inputs, you will have to cycle through the known types of stream to be able to find a way to real all the bytes of that particular stream. However, using this, you can simply call the method and it will return all the bytes from that stream, provided that the stream in question is __seekable__. If not, an error will be thrown. This therefore means that the extension cannot work in, for example, `NetworkStream` which is not seekable.