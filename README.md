# LiquidSnow
The LiquidSnow is a collection of .NET assemblies that you can use to make your .NET development work easy. Although it's still in development and is highly unstable, it is super conveneint for various tasks that trouble the .NET developer even in it's current state. Below is a preview of what each of the assemblies do:

## 1. Core
Core is a collection of exteremly fundamental and useful utilities and extensions that are necessary across of .NET development platforms. For example it provides interfaces to get properties, by name from objects, corrects the failures of System.IO.Path.Combine() by adding a better and perhaps more philosophically correct approach, plus a bunch of other super cool stuffs. Perhaps this is the most important, most of the other assemblies actively depend on it.

## 2. Enigma
Perhaps my favourite, it is just basically Encryption. Enigma allows for fanatics like myself an opportunity to cipher, decipher and hack classical encryption methods like the playfair, ceaser and more, and most importantly it provides a mechanism for file encryption and protection. It does this by providing the EnigmaFile and EnigmaContainer, the file basically provides a way to encrypt and decrypt single files securely as a developer, if you use a lot of serialization like me and save the string to file, you'll find this very handy as it can automatically convert a C# object and store it to a file. The EnigmaContainer, on the other hand provides an interface for developers to store collections of data, essentially it manages several instances of EnigmaFiles in what can be thought of as a 'safe'. The project is .NET Standard so anywhere .NET can run, it can run.

## 3. Goro
A super interesting one, one that came about by my need of creating nice UIs that are standardized. Goro is the WPF developers pie, it supports themes(accent colors, darkmode/lightmode), contains custom controls and has several useful utilities, such as those that convert Bitmaps to BitmapImages, and so many more. It provides skinning and styles for several wpf controls, and provides basic colors to get you started. This can only be used, unfortunately, in a .NET WPF project, it's for designing so duh? Also also, Goro is 'Drawing' in my native language, if you ever wonder where the name came from.

## 4. Mercury
Just like the Roman god Mercury, this assembly is useful for those intending to send messages. It provides a Server Client infrastructure that the dotnet developer can use to make his life very easy. The Mercury.Azure.Blobs provides an interface for blob transfer, which is secure and very safe for use. It provides a way to enqueue downloads/uploads to an Azure Blob Storage account. The project is mainly useful in scenarios where the .NET developer has decided to use the SAS approach.

## 5. Pluto
Basically, if you're developing software that has anything Financial or Accounting in it, Pluto is the one for you. The assembly provides interface and logic for dealing with Financial Accounting information and data, using accepted international standards. It is .NET Standard, so chill, it will be easy to intergrate it to the stuff you already have.

## 6. Lansique
Basically again, if you're developing software that has anything legal, lawyerish in it, then this will help you. Although still under construction, it's ultimate goal is to provide a way to organize legal documents, scan and obtain them from the web and allow the dotnet developer to present such data to the user meaningfully.

## 8. Sia
Basically again again, Sia is for those who are organizing libraries and such, in it's complete form, it is intended to be fully able to manage libraries, and provide interfaces that the dotnet developer can use to display such library information to the user, such as loading pdf documents, organizing files, viewing ebups etc. Sia is still in very early developemt and its definate path is yet to be shaped, I'm a solo dev, remeber....these things drain :)

## 7. Anubis
Still far early in development, I intend to migrate my Steganography codework here, so that anyone can potentially use the fun libaries for hiding information in images. The development for this is still far off though, when compared to other assemblies in the collection

