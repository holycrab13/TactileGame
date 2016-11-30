DotNet_AudioRenderer
=========
Small AudioRenderer for an easy access to the text to speech engine of windows and .Net.

## Intension:
This small piece of software allows for a fast and easy text to speech output as well as playing sounds through the .Net api interfaces.


## How to use:

--	TODO: build a small workflow

To define the standard culture of the voice to use, use the app.config file of your program and add e.g.:

``` XML 
	<appSettings>
		<add key="DefaultCulture" value="en-US" />
	</appSettings>
```

The standard voice to use can be definded in the same way:

``` XML 
	<appSettings>
		<add key="StandardVoice" value="ScanSoft Steffi_Full_22kHz, ScanSoft Steffi_Dri40_16kHz" />
	</appSettings>
```

**ATTENTION:** The standard culture will override the standard voice if it don't fit together.

**ATTENTION:** The project is only able to find voices of the same target platform type. This means if you compile it as x86 it is only able to find 32bit voices. If you compile it as x64 it is only able to identify 64bit voices.
	
### Example


Because the AudioRenderer is a singleton, you can define a static global variable for it in your class.

``` C# 
/// <summary>
/// The singleton instance of the global available audio renderer
/// </summary>
static AudioRenderer audio = AudioRenderer.Instance;
```

#### Text to Speech

To play a text string via TTS simply call

``` C# 
audio.PlaySound("welcome");
```

If a currently ongoing audio-output should be aborted and the new one should be played call

``` C# 
audio.PlaySoundImmediately("good by");
```

The voice to use can be adapted for every call as well

``` C# 
audio.PlaySound("welcome everybody", "ScanSoft Steffi_Full_22kHz");
```

#### Play Sound Files

To play sound wav files you can call

``` C#
audio.PlayWave("path/to/a/file.wav");
```

If you want to play a standard windows sound, you may want to use some of the default defined sounds in the `StandardSounds` enum

``` C# 
audio.PlayWave(StandardSounds.Ping);
```

Of course, you can stop all ongoing audio-outputs to play the wav immediately 

``` C# 
audio.PlayWaveImmediately(StandardSounds.Critical);
```

## ATTENTION!

This project is configured as a 32bit project. It can easily been compiled as x64 or mixed - but after doing so, the project is no longer able to find installed 32bit voices. So handle with care if you want to use your 32bit voices.


## You want to know more?

--	TODO: build help from code doc

For getting a very detailed overview use the [code documentation section](/Help/index.html) of this project.

