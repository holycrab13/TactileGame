DotNet_LanguageLocalization
=========

## Intension:
Localization in speech and other parts is not an easy task even when it should be doable without recompiling .Net projects. So this little project allows for a definition of language overlays for a basic translations in a key value manner. The translations are organized in XML files that can be translated easily by external translates. 

The phrases itself can contain placeholders width specific a order, so the translation can be filled in with an unlimited number of parameters.


## How to use:


--	TODO: build a small workflow

tud.mci.LanguageLocalization.LL

### Example for a localization file

```
<?xml version="1.0" encoding="utf-16" standalone="yes" ?>
<localization>

  <locale language="default">
     <trans id="tangram.lector.oo_observer.selected_elements">{0} elements selected.</trans>  
  </locale>

  <locale language="de">
    <trans id="tangram.lector.oo_observer.selected_elements">{0} Elemente gewählt.</trans>  
  </locale>

</localization>
```

Add this file for example as a text file resource [Language.txt]. You can rename it to .xml if you want. **Attention:** afterwards you have to fix the Resources.resx file manually.

### Example

#### Attention: the resources are only accessible through the given namespace. The namespace of the instancing class must be the same as the “standardnamespace” in the project properties. 
Afterwards you can add this file into the constructor of your LL class instance
```
LL ll = new LL(Properties.Resources.Language);
```

The language to use will be the default definition instead of the locale of the system is set to an existing language key in the definition file.

To get a translated key request the local instance:
```
ll.GetTrans(String key, params string[] strs)
```

If no localized text is available the default text is returned. If no matching key exists an empty string will be returned.


### Configuration

To define the standard culture of the translation to use, use the app.config file of your programm and add e.g.:

```
	<appSettings>
		<add key="DefaultCulture" value="en-US" />
	</appSettings>
```

## You want to know more?

--	TODO: build help from code doc

For getting a very detailed overview use the [code documentaion section](/Help/index.html) of this project.

