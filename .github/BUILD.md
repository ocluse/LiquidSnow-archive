# Build and Run on Linux

To understand how LiquidSnow works on Linux or to debug an issue, get all the source files, then locally build and run it.

## Obtain the Source Code

First, fork the LiquidSnow repository so that you can make a pull request. Clone the forked repository to your local machine.

```
git clone https://github.com/{username}/LiquidSnow.git
```

**Note** Ensure that you've installed all dependancies and libraries. You can reference them from the [requirements.txt](../requirements.txt) file

---

## Install .NET on Windows, Linux, and macOS
LiquidSnow requires the .NET SDK and .NET Runtime.
To install the, check out [How to Insall](https://docs.microsoft.com/en-gb/dotnet/core/install/) docs on Microsoft's website.

---
---
## Building Specific LiquidSnow Projects/Assemblies on `Linux`.
```
dotnet build [PROJECT] --runtime [DISTRO]
```
Replace [PROJECT] with a specific LiquidSnow project/assembly e.g. Aretha.
Replace [DISTRO] with a specific Linux distro and its architecture e.g. Ubuntu.18.04-x64.

Example build of Aretha for Ubuntu
```
dotnet build Aretha --runtime ubuntu.18.04-x64
```


