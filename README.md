# LiquidSnow
The LiquidSnow is a collection of .NET assemblies that you can use to make your .NET development work easy. It covers mutitudes of bases, from fundamental concepts to those that I just added for fun. While most of the API is generally frozen, there may still be breaking changes in the future, so keep this in mind. If you have any issues, don't hesitate to ask for assistance in the issues tab. Also, check the Wiki for documentation on how to use the Framework.

LiquidSnow Assemblies:
 * [Aretha](#aretha)
 * [Core](#core)
 * [Horus](#horus)
 * [Goro](#goro)
 * [Mercury](#mercury)
 * [Pluto](#pluto)
 * [Anubis](#anubis)
 * [Aba](#aba)


<a id="aretha"></a>
## ___*Aretha*___
If you want to see some of the cool things you could do with LiquidSnow, clone the repository, build Aretha and run it. It is a .NET Core Console App, therefore you can build it on Linux, Windows and Mac. Once you've run it, you should see: `Aretha: Ready to summon`. Type `summon anubis bitmap inject` and follow through the prompts. This should kickstart a process that will allow you to hide a file of your chosing inside an image of your chosing. Be sure to check out the Wiki to see all the things you should be able to do with Aretha. 

<a id="core"></a>
## 1. Core
Core is a collection of exteremly fundamental and useful utilities and extensions that are necessary across of .NET development platforms. For example it provides interfaces to get properties, by name from objects, corrects the failures of `System.IO.Path.Combine()` by adding a better and perhaps more philosophically correct approach, plus a bunch of other super cool stuffs. All the other assemblies in the framework actively depend on it. Check out the Wiki for how to use the methods and functions defined in Core. I'm certain you shall find one of them useful. See the [docs](https://github.com/thismaker/LiquidSnow/blob/main/docs/Core/Core.md) for the documentation.

<a id="horus"></a>
## 2. Horus
Extensive library with a main focus on encryption. Currently it handles 2 main categories: classical algorithms, such as playfair and vigenere, and symmetric algorithms, such as AES and Rijndael. On top of that, it provides an IO structure for safely and securely storing files by means of encryption. It does this by providing the `CryptoFile`(single files) and the `CryptoContainer`(multiple files) objects. Horus can also simulate the EnigmaMachine, allows you to define your own EnigmaMachine or use one of the predefined ones based on real models.

<a id="goro"></a>
## 3. Goro
A super interesting one, one that came about by my need of creating nice UIs that are standardized. Goro is the WPF developers pie, it supports themes(accent colors, darkmode/lightmode), contains custom controls and has several useful utilities, such as those that convert `Bitmaps` to `BitmapImages`, and so many more. It provides skinning and styles for several fundamental wpf controls, and provides basic colors to get you started. This can only be used, unfortunately, in a .NET WPF project, it's for designing so duh? Also also, Goro is 'Drawing' in my native language, if you ever wonder where the name came from. Check the Wiki for more info.

<a id="mercury"></a>
## 4. Mercury
Just like the Roman god Mercury, this assembly is useful for those intending to send messages. It provides a Server-Client infrastructure for sending message over Net Sockets (not __WebSockets!__). As such, it is exteremely barebones. For a better approach, utilise `Thismaker.Aba.Client` and `Thismaker.Aba.Server` to create superior client-server applications.

<a id="pluto"></a>
## 5. Pluto
It provides a crude framework for developing financial management and accounting based applications.

<a id="anubis"></a>
## 7. Anubis
A library dedicated to Steganography(not sure if I spelled that right). It allows you to hide files and data in images and WAV  audio files. See [docs](https://github.com/thismaker/LiquidSnow/blob/main/docs/Anubis/Anubis.md) for the documentation.

<a id="aba"></a>
## 8. Aba
After noticing overwhelming similarities among several Server-Client based applications I was creating, I decided to condense the code into a single-shareable library that can be used easily to get such applications running. Aba is exteremly useful, only rivaled by perhaps Core, Enigma and Goro. Aba comes in three parts: AbaClient, which is responsible for handling client side states, such as requesting for Access Tokens, making HTTP requests to a WebApi and more, AbaServer, which has utility for generating custom JWT tokens, server-side authentication, and more core server infrstructure. Finally there is AbaCommon, which provides the common ground for the server and the client to work on.



----

### Code of Conduct

While contributing to the project, please maintain a healthy environment for everyone. TO do this, kindly adhere to the stipulated [Code of Conduct](.github/CODE_OF_CONDUCT.md)


----

### Security
Read the [Security Policy](.github/SECURITY.md) document to see the security policy and how to help in case of a vulnerability.


----

### 📜 License

This project is Copyright (c) Thismaker and licensed under the terms of the [Apache License Version 2.0](http://www.apache.org/licenses/LICENSE-2.0)

