# How to use Goro in your .NET Core project
It is important to note that Goro will only work in a .NET Core environment. I chose .NET Core since it appears to be the future of the .NET platform, as .NET Framework appears to have been frozen in version 4.8. Therefore, I recommend anyone just beginning on .NET to start on the latest technology, as there are several improvements that come with it. Developing WPF for .NET Core is not far from developing for .NET Framework, with minor improvements, and in some cases, such as the `app.config`, abolishments with improvements.

## Prerequisites
First, you will need to have created a .NET Core WPF project. You can easily do this in Visual Studio by selecting File>New Project>>.NET (WPF). Ensure that Visual Studio is updated to the latest version for optimum experience. In case you are not using Visual Studio, then you must probably already know how to create a .NET Core WPF project.

## Step 1
You will need to add a reference to Goro in your project. There are multiple ways of doing this, currently, due to the state of the API, I recommend cloning the repository and adding a **project reference** to Goro. The usual way to do that is to add `Goro.csproj` to your solution(if you took the Visual Studio approach) and easily right click your project in solution explorer, then go to Add>Project Reference then tick Goro. To add it manually, go to your project's `.csproj` file and modify it to look like this:

  
