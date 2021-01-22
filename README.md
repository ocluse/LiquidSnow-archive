# LiquidSnow
The LiquidSnow is a collection of .NET assemblies that you can use to make your .NET development work easy. Although it's still in development and is highly unstable, it is super conveneint for various tasks that trouble the .NET developer even in it's current state. Below is a preview of what each of the assemblies do:

## ___*Aretha*___
If you want to see some of the cool things you could do with LiquidSnow, build Aretha and run it. It is a .NET 5 Console Application, therefore multiplatform. The longterm intention is for Aretha to be used as a mini LiquidSnow tool, currently you could use it to perform various Classical ciphers performed by `Thismaker.Enigma` and also encrypt and decrypt files from the same library, it also allows you to experiment with the code in `Thismaker.Anubis` by injecting data into bitmap images and ejecting data thereof. If you want a quick example, build and run aretha then type `summon Anubis bitmap inject` and follow through the prompts.

## 1. Core
Core is a collection of exteremly fundamental and useful utilities and extensions that are necessary across of .NET development platforms. For example it provides interfaces to get properties, by name from objects, corrects the failures of `System.IO.Path.Combine()` by adding a better and perhaps more philosophically correct approach, plus a bunch of other super cool stuffs. Perhaps this is the most important, most of the other assemblies actively depend on it.

## 2. Enigma
Perhaps my favourite, it is just basically Encryption. Enigma allows for fanatics like myself an opportunity to cipher, decipher and hack classical encryption methods like the playfair, ceaser and more, and most importantly it provides a mechanism for file encryption and protection. It does this by providing the `EnigmaFile` and `EnigmaContainer`, the file basically provides a way to encrypt and decrypt single files securely as a developer, if you use a lot of serialization like me and save the string to file, you'll find this very handy as it can automatically convert a C#  generic `object` and store it to a file. The `EnigmaContainer`, on the other hand provides an interface for developers to store collections of data, essentially it manages several instances of EnigmaFiles in what can be thought of as a 'safe'. The project is .NET Standard so anywhere .NET can run, it can run.

## 3. Goro
A super interesting one, one that came about by my need of creating nice UIs that are standardized. Goro is the WPF developers pie, it supports themes(accent colors, darkmode/lightmode), contains custom controls and has several useful utilities, such as those that convert `Bitmaps` to `BitmapImages`, and so many more. It provides skinning and styles for several wpf controls, and provides basic colors to get you started. This can only be used, unfortunately, in a .NET WPF project, it's for designing so duh? Also also, Goro is 'Drawing' in my native language, if you ever wonder where the name came from.

## 4. Mercury
Just like the Roman god Mercury, this assembly is useful for those intending to send messages. It provides a Server-Client infrastructure that the dotnet developer can use to make his/her life very easy. This project is currently in cooldown mode as I focus on other projects in the framework. Expect a load of updates. It's usefulness is also limited, in part, by the existence of the greater Aba.

## 5. Pluto
Basically, if you're developing software that has anything Financial or Accounting in it, Pluto is the one for you. The assembly provides interface and logic for dealing with Financial Accounting information and data, using accepted international standards. It is .NET Standard, so chill, it will be easy to intergrate it to the stuff you already have.

## 6. Lansique
Basically again, if you're developing software that has anything legal, lawyerish in it, then this will help you. Although still under construction, it's ultimate goal is to provide a way to organize legal documents, parse them from the web and allow the dotnet developer to present such data to the user ***meaningfully.***

## 8. Sia
Basically again again, Sia is for those who are organizing libraries and as such, in it's complete form, it is intended to be fully able to manage libraries, and provide interfaces that the dotnet developer can use to display such library information to the user, such as loading pdf documents, organizing files, viewing ebups etc. Sia is still in very early developemt and its definite path is yet to be shaped, I'm a solo dev, remeber....these things drain :)

## 7. Anubis
A library dedicated to Steganography(not sure if I spelled that right). Currently, there is a `BitmapJector` class that allows one to inject data into bitmaps and retrieve it later. I could say that Anubis is the most exciting of all the libraries, I've always had a thing for secrets, and it provides a way to hide things so..... Anyway, using Anubis and Enigma will make the dotnet developer unstoppable when it comes to security. Currently I am looking to create a way to hide data in audio files, starting with the WAV files as it is the simplest, ya'll should wish me luck :)

## 8. Aba
After noticing overwhelming similarities among several Server-Client based applications I was creating, I decided to condense the code into a single-shareable library that can be used easily to get such applications running. Aba is exteremly useful, only rivaled by perhaps Core, Enigma and Goro. Aba comes in three parts: AbaClient, which is responsible for handling client side states, such as requesting for Access Tokens, making HTTP requests to a WebApi and more, AbaServer, which has utility for generating custom JWT tokens, server-side authentication, and more core server infrstructure. Finally there is AbaCommon, which provides the common ground for the server and the client to work on.

