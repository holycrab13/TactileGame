DotNet_Extensibility
=========

## Intension:
Copy a compiled extension dll to a specific folder. The application to extend will be look inside this directory for specific extension objects and will load them and initialize through several functions. No Recompilation for function extension get necessary.


## How to use:

You only have to ask the ExtensionLoader for all available objects of a certain type in a certain directory ... that’s it

### How to build an extension

#### ATTENTION: 
Don’t let the compiler add local copies of resources of your extension that will be sent to the extension by the framework. This will lead to problems in checking the wright object types in initialization function.

#### ATTENTION: 
If you only use the compiled dll of this project, make sure you disable the option for including interop-types. This project does not provide an interface!

If you build extension in independent projects, make sure you copy them after compilation to the extension directory the main application is looking for. So the extension project should include his folders into the output directory via xcopy.

E.g., add the xcopy as "post build process"

XCOPY "$(SolutionDir)EXT" "$(TargetDir)Extensions" /H /S /Y /C /I /Q /R


### Example


Include namespace 

``` C#
using tud.mci.extensibility;
```

Define an extension directory to look for extensions to load

``` C#
const string EXTENSION_DIR = @"./Ext";
```

Load all extensions in the subdirectories

``` C#
private List<Type> LoadExtension()
{
    var extTypes = new List<Type>();
    var dir = new DirectoryInfo(EXTENSION_DIR);

    if (dir.Exists)
    {
        var dirs = dir.GetDirectories();
        // check every subdirectory
        foreach (var directoryInfo in dirs)
        {
            // let the Extension loader fetch all dlls (classes) that implement a certain type or interface
            // in this example the type to search for is FooBar

            var extension = ExtensionLoader.LoadAllExtensions(typeof(FooBarClass), directoryInfo.FullName).SelectMany(x => x.Value);
            extTypes.AddRange(extension);
        }
    }

    // now we can instantiate the single identified extension classes
    foreach (var extType in extTypes)
    {
        try
        {
            // you can also add constructor parameters such as the example object someConstrParam
            var adapter = Activator.CreateInstance(extType, someConstrParam);
            // TODO: do whatever you want with your loaded and instantiated Object
        }
        catch (Exception e) { }
    }

    return extTypes;
}
```
