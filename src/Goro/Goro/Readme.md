# How to use Goro in your .NET Core project
It is important to note that Goro will only work in a .NET Core environment. I chose .NET Core since it appears to be the future of the .NET platform, as .NET Framework appears to have been frozen in version 4.8. Therefore, I recommend anyone just beginning on .NET to start on the latest technology, as there are several improvements that come with it. Developing WPF for .NET Core is not far from developing for .NET Framework, with minor improvements, and in some cases, such as the `app.config`, abolishments with improvements.

## Prerequisites
First, you will need to have created a .NET Core WPF project. You can easily do this in Visual Studio by selecting File>New Project>>.NET (WPF). Ensure that Visual Studio is updated to the latest version for optimum experience. In case you are not using Visual Studio, then you must probably already know how to create a .NET Core WPF project.

## Step 1
You will need to add a reference to Goro in your project. There are multiple ways of doing this, currently, due to the state of the API, I recommend cloning the repository and adding a **project reference** to Goro. The usual way to do that is to add `Goro.csproj` to your solution(if you took the Visual Studio approach) and easily right click your project in solution explorer, then go to Add>Project Reference then tick Goro. To add it manually, go to your project's `.csproj` file and add the following item group:

  
    <ItemGroup>
      <ProjectReference Include="...Path to Goro.csproj"/>
     </ItemGroup>
     
## Step 2
Once you have done that, you will need to expose styles defined in Goro to your project. This is done through a resource dictionary. App wide resource dictionary are defined in `App.xaml` folder. Move to the file and modify/add the following section:

    <Application.Resources>
      <ResourceDictionary>
        <ResourceDictionary.MergerdDictionaries>
            <ResourceDictionary Source="/Thismaker.Goro;component/styles.xaml"/>
            <!--Other resource dictionaries can go here, that you use-->
        </ResourceDictionary.MergerdDictionaries>
      </ResourceDictionary>
    </Application.Resources>
    
## Step 3
Once that has been done, rebuild your solution/project for changes to take place. Immediately you should see changes in your project. You can therefore use other styles also declared in the library, for example the `ListView` component has an additional style called `ModerateList` that you can assign by adding the line: `Style={Dynamic Resource ModerateList}` to your list's xaml. Goro comes with a few predefined themes that you can assign by, anywere in the code writing the lines: `ThemeManager.SetTheme(DefaultTheme.Briliet);` for example. You can also set your own theme by calling the same method using the overload taht takes in `Theme` as the parameter.

Goro further defines other components such as the ButtonIcon which is a button that uses an icon instead. To use it, add the xml namespace `xmlns:goro="http://schemas.thismaker.com/liquidsnow/goro"` to your `MainWindow.xaml` or other page or window xaml definition sections. Once that is done, you can easily access the Goro controls by simply going: `< goro:ButtonIcon />`

# Conclusion
I will work to try and improve the documentation by adding a better Wiki. I hope for now this is sufficient enough to guide you through the first steps.
