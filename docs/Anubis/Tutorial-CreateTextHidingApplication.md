# Tutorial: Use Anubis to create a C# Console Steganography Application

In this tutorial, we are going to create a simple C# console program to hide text provided by the user into an image. To understand how this works, check out the [Introduction](./Anubis.md#how-it-works).

In the course of the tutorial, we shall overview all the different aspects the `Jector`.

## Step 1: Creating the Project
First, you need to create a C# console application. Depending on your enviroment and IDE, the steps may differ. 

If you are using __Microsoft Visual Studio__ then head over to **File > New > Project > Console Application (.NET Core)** then click create. You will need to give the project a name, for example: _StegaApplication_ or some other name that you wish. Click Create once more to create your project.

If you are using **Visual Stuido Code** the steps followed are slightly different. Start Visual Studio Code, then select **File > Open Folder** then navigate to a folder of your choice where you **Create a new Folder** that you can name _StegaApplication_ or other name you wish. Click **Select Folder** and once that is done, open the **Terminal** and in the **Terminal** enter the following command:
```
dotnet new console
```

Irrespective of the method used, you should have the usual template `Program.cs` file shown below:

```cs
using System;
namespace StegaApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }
    }
}
```

## Step 2: References
Next, we need to add a reference to Anubis for our project to be able to use it. There are three ways to do this, i.e. referencing a nuget package, referencing the _Thismaker.Anubis.csproj_ file, or copying the actual .dll files and pasting and referencing them from the project. For now, I will describe the nuget method, as it is the simplest and best.

Anubis has been packaged into a referancable nuget package that can be found at https://www.nuget.org/packages/Thismaker.Anubis/. To reference this package:

If you are using **Microsoft Visual Studio** head over to **Project>Manage Nuget Packages** and select the **Browse** tab. Search _Thismaker.Anubis_, select the latest version(usually selected by default) and click **Install**.

If you are using **Visual Studio Code**, open the _StegaApplication.csproj_ file or *YOUR_PROJECT.csproj* and add the following line:
```xml
<ItemGroup>
    <PackageReference Include="Thismaker.Anubis" Version="1.0.2" />
</ItemGroup>
``` 

By the end of this step, you should have a reference to _Thismaker.Anubis_ in your project in some form. Next, we will use these libraries to hide text.

## Step 3: Hiding Text
Modify `Program.cs` to read as follows:
```cs
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Thismaker.Anubis.Imaging;

namespace StegaApplication
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Enter the path to an image: ");
            string inputImagePath = Console.ReadLine();

            Console.WriteLine("Enter text to hide:");
            string hideText = Console.ReadLine();
            byte[] hideData = hideText.GetBytes<UTF8Encoding>();

            Console.WriteLine("Enter path to save output image: ");
            string outputImagePath = Console.ReadLine();



            using (FileStream fileInput = File.OpenRead(inputImagePath))
            using (FileStream fileOutput = File.OpenWrite(outputImagePath))
            using (MemoryStream streamData = new MemoryStream(hideData))
            {
                BitmapJector jector = new BitmapJector
                {
                    EOF = "\u0004", //Setting the End of File
                };

                await jector.InjectAsync(fileInput, fileOutput, streamData);
            }

            Console.WriteLine("Data successfully hidden, check the output image file");
        }
    }
}
```

Once that is done, build and run the project. On **Microsoft Visual Studio**, you can click **Debug > Start Debugging** as that will first build the project and start an instance.

On **Visual Studio Code**, you can follow the [instuctions](https://docs.microsoft.com/en-us/dotnet/core/tutorials/debugging-with-visual-studio-code#:~:text=Open%20the%20Debug%20view%20by,Start%20Debugging%20from%20the%20menu.) by Microsoft Docs on how to debug a Console Application in Code.

If you run the program correctly, you should experience a similar interface to the one below:
```
Enter the path to an image:
C:\Thismaker\Volatile\x1.png
Enter text to hide:
Where is my mother
Enter path to save output image:
C:\Thismaker\Volatile\output.png
Data successfully hidden, check the output image file
```

### Understanding the Code

As you can see, we have added additional `using` statements, e.g. `using Thismaker.Anubis.Imaging` that will allow us to access the functionality we need. After that, we made the `Main()` method become an asynchronous method by replacing the `void` with the `Task`. This will allow us to properly use the `InjectAsync` method as you shall see soon.

```cs
Console.WriteLine("Enter the path to an image: ");
string inputImagePath = Console.ReadLine();
```

The above two lines are responsible for obtaining a _path_ to the file containing the original image, that is, the image without any data hidden inside.

```cs
Console.WriteLine("Enter text to hide:");
string hideText = Console.ReadLine();
byte[] hideData = hideText.GetBytes<UTF8Encoding>();
```
The above line obtains the text that the user wishes to hide in the image. Of key is the line `byte[] hideData = hideText.GetBytes<UTF8Encoding>();`. If you have read about how Anubis works, you'll notice that it hides _bytes_ inside the image. This means that we need to convert this text to bytes. You do that by using an `Encoding`. The `GetBytes<Encoding>` is an extension method that is provided by `Thismaker.Core`. To see other _Core_ extensions and utilities, see the page on [Core](../Core/Core.md).

```cs
using (FileStream fileInput = File.OpenRead(inputImagePath))
using (FileStream fileOutput = File.OpenWrite(outputImagePath))
using (MemoryStream streamData = new MemoryStream(hideData))
{
    BitmapJector jector = new BitmapJector
    {
        EOF = "\u0004", //Setting the End of File
    };

    await jector.InjectAsync(fileInput, fileOutput, streamData);
}
```

This is the section that actually does the work of writing the text into the image. The first using statements are just for obtaining auto-disposed streams of the data. `fileInput` is the image where the data should be hidden. `fileOutput` is the image which has the data hidden within, and `streamData` is a `MemoryStream` that contains the data to be hidden. Notice that in the initialization of `jector`, we provide the text `EOF="\u0004`. This sets the End of File. A marker that will be used to indicate to the the end of text. Without providing a _EOF_, then the data extracter will simply read through to the end of the image, returning **everything**, which will absolutely be gibberish.

## Step 4: Retrieving Hidden Text

Modify `Task.Main(string` to look like follows:

```cs
static async Task Main(string[] args)
{
    Console.WriteLine("Enter the path to image with hidden text: ");
    string inputImagePath = Console.ReadLine();

    string hiddenText = "";
    using (FileStream fileInput = File.OpenRead(inputImagePath))
    using (MemoryStream streamData = new MemoryStream())
    {
        BitmapJector jector = new BitmapJector
        {
            EOF = "\u0004", //Setting the End of File
        };
        await jector.EjectAsync(fileInput, streamData);
        byte[] hiddenData = streamData.ToArray();
        hiddenText = hiddenData.GetString<UTF8Encoding>();
    }

    Console.WriteLine($"Hidden Text: {hiddenText}");           
}
```

Build and run the program, follow through the prompts, providing the image output in Step 3 as the input image for step 4. If done correctly, you should experience an output as follows:
```
Enter the path to image with hidden text: 
C:\Thismaker\Volatile\output.png
Hidden Data: Where is my mother
```
### Understanding the code
Most of the code for extracting the hidden data is similar to code used in putting the data there in the first place.

```cs
Console.WriteLine("Enter the path to image with hidden text: ");
string inputImagePath = Console.ReadLine();
```

This line obtains a path to the image **containing** the data. Remember that this time the input is actually the output image of the previous step, as we are now extracting the data from that output.

```cs
string hiddenText = "";
using (FileStream fileInput = File.OpenRead(inputImagePath))
using (MemoryStream streamData = new MemoryStream())
{
    BitmapJector jector = new BitmapJector
    {
        EOF = "\u0004", //Setting the End of File
    };
    await jector.EjectAsync(fileInput, streamData);
    byte[] hiddenData = streamData.ToArray();
    hiddenText = hiddenData.GetString<UTF8Encoding>();
}
```
This block obtains the data hidden in the image. As usual, we create a `FileStream` with read permissions to the path of the steganographic image. We also create a `MemoryStream` where the data extracted will be written. The line `await jector.EjectAsync(fileInput, streamData);` extracts the data in the steganographic image and writes it to the `streamData`.

Like in the previous step, the line `hiddenText = hiddenData.GetString<UTF8Encoding>();` makes use of a _Core_ extension that converts bytes to string. This string is what is later displayed.

### Expermient!
Remember what we said above the `EOF` and the consequences as a result of the lack of it? Now try to change the line assigning an `EOF` to be `EOF= null` and run the program again. I wish you luck understanding what you get, and if you do get something legible, then your image must have already had some form of text hidden.

## Step 5: Combining Writing and Retrieval

Below is a program that asks the user what they wish to do. It can either hide text, or retrieve hidden text. An example of this program has been made available in the samples section of the repository.

```cs
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Thismaker.Anubis.Imaging;

namespace StegaApplication
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Would you like to insert data or retrieve data? You can type either: \nINSERT \nEXTRACT");
            string operation=Console.ReadLine();

            if (operation.ToUpper() == "INSERT")
            {
                await Inject();
            }
            else
            {
                await Eject();
            }
        }

        static async Task Inject()
        {
            Console.WriteLine("Enter the path to an image: ");
            string inputImagePath = Console.ReadLine();

            Console.WriteLine("Enter text to hide:");
            string hideText = Console.ReadLine();
            byte[] hideData = hideText.GetBytes<UTF8Encoding>();

            Console.WriteLine("Enter path to save output image: ");
            string outputImagePath = Console.ReadLine();



            using (FileStream fileInput = File.OpenRead(inputImagePath))
            using (FileStream fileOutput = File.OpenWrite(outputImagePath))
            using (MemoryStream streamData = new MemoryStream(hideData))
            {
                BitmapJector jector = new BitmapJector
                {
                    EOF = "\u0004", //Setting the End of File
                };

                await jector.InjectAsync(fileInput, fileOutput, streamData);
            }

            Console.WriteLine("Data successfully hidden, check the output image file");
        }

        static async Task Eject()
        {
            Console.WriteLine("Enter the path to image with hidden text: ");
            string inputImagePath = Console.ReadLine();

            string hiddenText = "";
            using (FileStream fileInput = File.OpenRead(inputImagePath))
            using (MemoryStream streamData = new MemoryStream())
            {
                BitmapJector jector = new BitmapJector
                {
                    EOF = "\u0004", //Setting the End of File
                };
                await jector.EjectAsync(fileInput, streamData);
                byte[] hiddenData = streamData.ToArray();
                hiddenText = hiddenData.GetString<UTF8Encoding>();
            }

            Console.WriteLine($"Hidden Data: {hiddenText}");
        }
    }
}
```

## Further Steps
Where should I go from here? You may be wondering. Well, for one, you can try to modify the application above to hide other files, not just text. Next, you can try to read more about how Anubis works by looking through the source code. Admittedly I have not completed writing the docs, but I'm trying, the source is well commented though. Keep your eye on the lookout for when the next docs drop. Oh, and if you came from the [Anubis](./Anubis.md) introduction page, there was a question I poised on which image contained the hidden data. You can try to see if you are able to obtain that data using the above program. However, note that the default EOF was used, therefore, do not assign an EOF.

 Thanks :) 

By Thismaker.