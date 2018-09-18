# Tactile RPG Engine

An XML-based engine for low-resolution explorative role-playing games.

[TOC]

## Introduction

The Tactile RPG Engine has been developed for a university project at the TU Dresden in order to create a role-playing game for tactile displays. These displays offer a mechanical low-resolution displays where pixels are device-driven. Using a tactile display, vision-impaired users can read text (Braille), discover images and even play games.

The most important software interface to the mechanical device is a Boolean matrix where `false` denotes a lowered pin and `true` a raised pin. Passing such a matrix to the device once per frame at a frame rate of about 20 FPS will create a close-to-smooth output on the screen.

## BrailleIO Framework

This project is built on top of the BrailleIO Framework developed at the TU Dresden. The framework gives access to both device input (Buttons, Touch) and output (Sound, Screen). 

In order to manage content on different pages, the BrailleIO offers the `BrailleIOScreen`class. Screens can easily be swapped to, for example, navigate between different  User Interface pages. Each screen can have multiple views of base type `BrailleIOViewRange` to add elements such as buttons and text to your UI. The `BrailleIOViewRange` class also provides the `SetMatrix(bool[,] pixels)` method which can be used to set the pixels of a UI element manually.

## Basic Structure

The following section will give a short overview of the basic structure of the Tactile RPG Engine project

### Game Concept

The Tactile RPG Engine allows users to create simple low-resolution explorative RPGs. The main inspiration comes from the Pokémon-Series made for Nintendo's GameBoy. The players control a character, which always remains in the center of the screen, while the world changes while the character moves around.

![Screen1](E:\Documents\Tactile Game Documentation\Images\Screen1.png)

The user can press a button to interact with the game world and trigger events, which drive the story forward. Changes to the game world and player knowledge will then activate different events. 

### Classes

This section will give you an overview of the most important classes used in the Tactile RPG Engine.

#### InteractionScreen.cs

The abstract `InteractionScreen` inherits directly from the `BrailleIOScreen`. It provides a range of convenience methods to the child classes and enables them to react to screen events (`OnHide` and `OnShow`).  A screen handles most of the game logic and updates the views. The data is held in model classes which can be shared between screens.

#### Game.cs

The game class initializes and holds all the different `BrailleIOScreen` instances and manages callbacks from the menu screens. The screens used with this project are the following:

- Start Screen - a screen shown upon game launch
- Main Menu Screen - a screen showing the main menu
- Load/Save Screens - two screens for loading and saving respectively
- Question Screen - a screen shown whenever the player needs to answer a NPC question
- Game Screen - the screen handling the main game loop and world rendering



Additionally, it hooks into the device input and passes the input events to the active `BrailleIOScreen` instance.

```C#
// <summary>
/// Handles the keyStateChanged event of the adapter control.
/// Occurs when device buttons gets pressed or released.
/// </summary>
/// <param name="sender">The source of the event.</param>
/// <param name="e">The <see cref="BrailleIO_KeyStateChanged_EventArgs"/> instance containing the event data.</param>
void adapter_keyStateChanged(object sender, BrailleIO_KeyStateChanged_EventArgs e)
{
   if (currentScreen != null)
   {
      Task t = new Task(new Action(() => 
      	{ currentScreen.HandleInteraction(e.keyCode); }));
      t.Start();
   }
}
```



*NOTE: It currently holds the global knowledge dictionary, which should be refactored to a Model in the future.*

#### GameScreen.cs

Among different menu screens there is the most important child class of the `InteractionScreen` is the  `GameScreen`. The `GameScreen` holds the main game loop that gets started when the screen gets shown and stopped whenever the screen gets hidden:

```C#
/// <summary>
/// Called when this screen is shown
/// </summary>
protected override void OnShow()
{
   detailRegion.SetVisibility(false);

   gameThread = new Task(new Action(() => gameLoop()));
   gameThread.Start();
}

/// <summary>
/// Called when this screen is hidden
/// </summary>
protected override void OnHide()
{
   gameRunning = false;
}

/// <summary>
/// The magic while loop running the game
/// </summary>
private void gameLoop()
{
   gameRunning = true;

   while (gameRunning)
   {
      Tick();
      Thread.Sleep(50);
   }
}
```

The `Tick()`method does the Update and Draw call for the running game at 20 Ticks per second. It uses two separate `BooleanCanvas` instances, a front and a back buffer in order to reduce flickering artifacts. 

Additionally, the `GameScreen` class holds all the model and controller instances needed for the game. The models can then be passed to other screens (e.g. the question answering screen)

*NOTE: The models could be initiated externally and then passed to the  GameScreen instance*



The Models held by the `GameScreen` are the following:

- CharacterModel (Player avatar)
- LevelModel (Level data and events)
- GameInput (Input data)
- DialogueModel (Current game dialogue)

The `GameScreen` also holds different sub-controller instances to manage specific tasks.

- GameInputController (Updates the input data)
- MovementController (Moves the character around)
- LevelController (Manages interaction with the environment)
- DialogueController (Manages game dialogues)

The `GameScreen` then use the Model data to render to the active `BooleanCanvas` instance and then passes the produced Boolean matrix to a connected `BrailleIOViewRange` instance.

This is the main rendering method in the `GameScreen` class:

```c#
/// <summary>
/// Draws all the world objects to the canvas and swaps the buffers
/// </summary>
private void DrawToCanvas()
{
    buffers[bufferIndex].Clear();

    buffers[bufferIndex].X = (levelModel.Avatar.X + levelModel.Avatar.Width / 2) - 
       buffers[bufferIndex].Width / 2;
    buffers[bufferIndex].Y = (levelModel.Avatar.Y + levelModel.Avatar.Height / 2) - 
       buffers[bufferIndex].Height / 2;

    foreach (WorldObject obj in levelModel.level.Objects)
    {
        if (!obj.isHidden)
        {
            buffers[bufferIndex].Draw(obj);
        }
    }

    buffers[bufferIndex].Draw(levelModel.Avatar);
    mainRegion.SetMatrix(buffers[bufferIndex].Data);

    bufferIndex = (bufferIndex + 1) % 2;
}
```



#### GameInputController.cs

The `GameInputController` is used by the main game loop and translates the event-based UI events passed from the BrailleIO Framework to a polling-based input structure (`GameInput` class). This is necessary to detect buttons being held down by the user.

#### BooleanCanvas.cs

A  `BooleanCanvas` can be created using a `width` and `height` parameter and manages a Boolean matrix, which can be passed to a  `BrailleIOViewRange`.  It provides the `Draw(WorldObject worldObject)`method, which can be used to render a `WorldObject` to the canvas (See WorldObject.cs).

#### LevelLoader.cs

The Level Loader manages both saving and loading of levels and game states. All data is loaded from XML-files. The XML-Syntax for the Tactile RPG Engine is described here.

#### Level.cs

The Levels of the Tactile RPG Engine are grid based. Each grid tile uses 16x16 pixels by default, this can be adjusted to any other size. The level holds a list of `WorldObject` instances, a list of `EventBase` instances (level events) and a list of `EventTrigger` instances. 

An `EventTrigger ` can for instance be an area on the ground, that triggers an event once the player enters.

#### WorldObject.cs

Every object that can be rendered to the screen inherits from the `WorldObject` class. These objects are

- Avatar
- Static Objects
- Doors
- Items
- Containers
- NPCs

All objects in a level are interactable and offer a textual description to further immerse the player. 

Furthermore, each object has a visual representation (pattern) encoded as a **Byte** matrix instead of a Boolean matrix. You can learn more about this concept in the [Graphics](Graphics) section

#### EventBase.cs

The `EventBase` class describes a general game event. `Event` and `EventGroup` both inherit from `EventBase`.  `Event` is a single event that holds a list of `ActionBase`instances that are executed sequentially, once the event fires. `EventGroup` groups multiple instances of `EventBase` together and executes them sequentially once fired.



This is the current implementation of `EventBase`:

```c#
/// <summary>
/// An event in the game world
/// </summary>
abstract class EventBase
{
    public string[] conditions;

    public string[] inverseConditions;

    public string id;

    public abstract void Update(LevelController levelController);

    public abstract bool IsComplete();

    public abstract void Reset();

    public bool IsAvailable()
    {
        foreach (string condition in conditions)
        {
            if (!Game.HasKnowledge(condition))
            {
                return false;
            }
        }

        foreach (string condition in inverseConditions)
        {
            if (Game.HasKnowledge(condition))
            {
                return false;
            }
        }

        return true;
    }
}
```



Before an event gets executed, the conditions and inverse conditions are checked in `IsAvailable`. Conditions and inverse conditions are simple strings held in a global dictionary (see [Facts](Facts)). All of the conditions and none of the inverse conditions have to be met for the event to execute.

This is the implementation of the `Update(InteractionController interactionController)` of the `Event` child class:

```c#
public override void Update(LevelController levelController)
{
    ActionBase action = actions[index];

    if (action.IsComplete())
    {
        index++;
    }
    else
    {
        action.Update(interactionController);
    }
}
```

The `Event` class holds a list of `ActionBase` and executes them sequentially. The event is over once all the actions are complete.

#### ActionBase.cs

All changes to the game world  that are not directly related to the player input (e.g. a cut-scene) can be made with Actions.  There are already a number of Actions implemented, such as actions for dialogue (phrases, questions, answers), actions for movement (relative movement, absolute movement, teleport, go to another level) or general actions like triggering another event or finishing the game.

*NOTE: To create a new action, simply create a class, that inherits from `ActionBase` and describe the action behaviour in the `Update()` method.*

A full list of currently implemented actions can be found in the [XML section](Actions)



### Structure and Flow

The execution flow starts in the `Game` class. The User Interface is set up and the initial screen is shown. You can then navigate to the main menu and start or load a game from there. The user input is processed by the currently active screen.

Once a game starts, the `GameScreen` instance gets shown and the game loop starts. The `GameScreen` updates the sub-controllers (e.g. Movement controller) which translates the user input to game world changes (e.g. your character moves around)

Switching between free player movement and events is done using a state variable. Once the game state is set to event mode, the sub-controllers block the respective user input.

Since actions are basically an extended dynamic controller, they may also process player input. This is implemented in the `DialogueAction` to skip through game dialogue with a button press.

The player moves his avatar and interacts with the game world. This may trigger events and actions that change the global facts dictionary. Changes to the global facts dictionary render more and more events triggerable, thus creating progress in the game.



## Facts

Facts are the main indicator of progress in an Tactile RPG Engine game.  All facts, represented by strings, are held in a global dictionary (`Dictionary<string, bool>`) to efficiently decide whether a fact has been discovered yet.  The Boolean value indicates whether a fact is known, facts not listed in the dictionary are considered unknown.

There is no global list of known and unknown facts. You can create the required facts as you create the levels and events.

Saving the game simply saves all known facts together with the position and level name of the avatar.

Events will compare their execution conditions with the global facts dictionary to check, whether they can be fired. Actions can set or clear facts. (see [Definition Section](Definition Section))

## XML-Syntax

### Levels

All the levels and game content of the Tactile RPG engine is written in XML. The following section describes the syntax used for level setup. 

#### Example Level

This is a slightly modified version of the Tutorial for our Game Project. It contains the most important XML-tags used in the engine.

```xml
<?xml version="1.0" encoding="UTF-8"?>
<level width="50" height="30" onload="tutorial_load">
    <definition>
    	<objects>
      		<object id="wand" name="wand" desc="Eine Wand mit Holzvertäfelung." 					img="bmps/wall.bmp" block="True" />
      		<object id="pavement" name="pavement" desc="Der Boden ist hier sehr rau."
                img="bmps/pavement_WE.bmp" block="False" />
      		<object id="door" name="door" desc="Eine Tür aus Holz." 								img="bmps/door_south.bmp" block="True" />
      		<object id="stool" name="stool" desc="Ein etwas alter Barhocker." 						img="bmps/stool.bmp" block="True" />
      		<object id="franzl" name="Franzl" desc="Eine Person im Tutorial" 						img="bmps/avatar_2.bmp" block="true" />
      		<object id="busch" name="busch" desc="Eine Topfpflanze aus Plastik." 					img="bmps/bush.bmp" block="True" />
    	</objects>
    	<events>
      		<event id="tutorial_load">
        		<phrase>Versuche, ans Ende des Korridors nach unten zu laufen.</phrase>
      		</event>
      		<event id="walk_complete" not="walk_once">
                <phrase set="walk_once">Sehr gut!</phrase>
                <phrase>Du kannst deine Umgebung jederzeit untersuchen.</phrase>
                <phrase>Versuche nun, dir den Hocker genauer anzusehen.</phrase>
            </event>

            <eventgroup id="interaction_complete">
                <event if="barhocker">
                    <phrase>Das ist nur ein alter Barhocker.</phrase>
                </event>
                <event not="barhocker">
                    <phrase set="barhocker">Gut gemacht! Ein Barhocker!</phrase>
                    <phrase>Gehe weiter den Korridor entlang!</phrase>
                </event>
            </eventgroup>

      		<event id="walk2_complete" not="rough_terrain">
        		<phrase set="rough_terrain">Warte! Über dir ist eine Unebenheit im 						Boden!</phrase>
      		</event>

            <eventgroup id="talk_complete">
                <event if="talked_to_franzl">
                    <phrase>Der Franzl ist fröhlich und winkt.</phrase>
                </event>
                <event not="talked_to_franzl">
                    <phrase set="talked_to_franzl">Sehr gut! Du hast den Franzl 							gefunden!</phrase>
                    <trigger trigger="questions" />
                </event>
            </eventgroup>

            <event id="questions">
                <phrase>Manchmal stellen dir Spielfiguren Fragen.</phrase>
                <phrase>Du kannst dann aus mehreren Antworten auswählen.</phrase>
                <phrase>Mit der Taste hoch und der Taste runter kannst du zwischen den 						Antworten wechseln.</phrase>
                <phrase>Mit Bestätigen bestätigst du deine Antwort.</phrase>
                <question>
                    <text>Franzl: Hallo, hast du alles soweit verstanden?</text>
                    <answer trigger="questions">Nein, bitte erkläre es nochmal!								</answer>
                    <answer trigger="questions_complete">Natürlich, das war 								kinderleicht!</answer>
                </question>
            </event>

            <event id="todo1" not="barhocker">
                <phrase>Finde zuerst den Barhocker und untersuche ihn!</phrase>
                <move>Left</move>
            </event>

            <event id="todo2" not="talked_to_franzl">
                <phrase>Finde zuerst den Franzl und rede mit ihm!</phrase>
                <move>Left</move>
            </event>

            <event id="questions_complete">
                <phrase>Wunderbar! Dann hast du die Spielsteuerung verstanden!</phrase>
                <phrase>Finde nun die Tür rechts von dir!</phrase>
            </event>

            <event id="tutorial_complete">
                <phrase>Hier ist die Tür! Jetzt aber raus hier!</phrase>
                <gameover></gameover>
            </event>
        </events>
    </definition>
  
    <triggers>
        <trigger x="-1" y="7" width="3" height="1" event="walk_complete"/>
        <trigger x="2" y="7" width="1" height="2" event="todo1"/>
    </triggers>

    <decos>
        <deco obj="busch" x="-1" y="5" r="0" />
        <deco obj="busch" x="-1" y="2" r="0" />
        <deco obj="wand" x="2" y="4" r="0" />
        <deco obj="wand" x="2" y="3" r="0" />
        <deco obj="wand" x="2" y="2" r="0" />
        <deco obj="wand" x="2" y="1" r="0" />
        <deco obj="pavement" x="6" y="4" r="0" />
        <deco obj="pavement" x="8" y="4" r="0" />
        <deco obj="pavement" x="7" y="4" r="0" />
        <deco obj="wand" x="17" y="-3" r="0" />
        <deco obj="wand" x="13" y="-3" r="0" />
        <deco obj="wand" x="12" y="-3" r="0" />
        <deco obj="wand" x="11" y="-3" r="0" />
        <deco obj="door" x="17" y="-2" r="1" trigger="tutorial_complete"/>
        <deco obj="wand" x="14" y="-3" r="0" />
        <deco obj="wand" x="14" y="-1" r="0" />
        <deco obj="wand" x="14" y="0" r="0" />
        <deco obj="wand" x="14" y="2" r="0" />
        <deco obj="stool" x="-1" y="9" r="0" trigger="interaction_complete" />
    </decos>
    <doors>
    </doors>
    <npcs>
        <npc id="erniebert" obj="franzl" x="7" y="-1" r="0" trigger="talk_complete"/>
    </npcs>
</level>
```

#### Definition Section

In the definition section contains a list of objects and a list of events used in the level. 

##### Objects

An object definition does *not* place an object in the level yet. It only defines how an object behaves and looks like 

```xml
<object id="wall" name="wand" desc="A wall." img="bmps/wall.bmp" block="True" />
```

An object must have the following attributes 

- **id** - a unique identifier
- **name** - a name
- **desc** - a description, which is displayed upon interaction
- **img** - the visual representation of the object as a .bmp (see [Graphics](Graphics) section)
- **block** - a Boolean value, denoting whether the object blocks the characters path or not



##### Events

You can find further information on Actions and Events in the [Classes](Classes) section.

This is a basic event. An event may have a unique **id** attribute and may have an **if** and **not** attribute. The **id** attribute is required if you want to call the event by a trigger.

Facts listed in the **if** attribute have to be *known* in order to execute the event. All facts listed in the **not** attribute have to be *unknown* in order to execute the event (see [Facts](Facts)).

An event may contain one or multiple actions which are executed from top to bottom once the event fires. Any action can have a **set** attribute which holds one or multiple facts (separated by whitespace). All listed facts will be set to *known* once the action gets executed.

```xml
<event id="event1" if="fact1" not="fact2 fact3">
    <phrase set="fact4 fact5">A message.</phrase>
    <trigger trigger="event2" />
</event>
```

**Example:** The example event called *event1* checks, whether *fact1* is known and both *fact2* and *fact3* are unknown. If this conditions is met, a chat message will show the string  *"A message."* while setting *fact4* and *fact5* to known. Subsequently, the event with id *event2* will be fired.

##### Event Group

An event group has the same attribute setup as the basic event. An event group can group multiple sub-events together. This is useful when you want call multiple events with a single trigger. Setting the conditions properly also enables if-else-like statements.

```xml
<eventgroup id="event3">
    <event if="fact6">
        <phrase>Hello there.</phrase>
    </event>
    <event not="fact6">
        <phrase set="fact6">Setting the condition!</phrase>
        <question>
            <text>How are you doing?</text>
            <answer trigger="event4">Great!</answer>
            <answer trigger="event5">Meh..</answer>
        </question>
    </event>
</eventgroup>
```

**Example:** The example code shows an event group *event3*. Once called, the second event (line 5) will be executed (assuming that *fact6* is unknown). A chat message will be shown and *fact6* will be set. A question dialogue will then appear, depending on the user input *event4* or *event5* will be triggered (line 7-11). On the next execution of *event3*, the first event will be called since *fact6* is now definitely known. Note that the child events of the event group do not require their own unique id, since they are not called directly.



The EventGroup class could be extended to support different settings for sub-event execution such as:

- **first_match** - only first executable event gets run
- **last_match** - only last executable event gets run
- **sequence** - current implementation, all executable events get run sequentially



##### Actions

There are a number of actions that can be added to events. All actions described in XML have their C# counterpart. The `LevelLoader` then parses the XML into the respective class.

Currently the following actions are implemented:

- **Phrase** - shows a simple chat message

  ```xml
  <phrase>Hello there.</phrase>
  ```

- **Question** - starts a question event. A question must have a child object with a *text* tag and may have 1-4 child objects with an *answer* tag.

  ```xaml
  <question>
      <text>How are you doing?</text>
      <answer>Great!</answer>
      <answer>Meh..</answer>
  </question>
  ```

- **GoTo** - moves a target object to a set position. *(This is a very basic implementation and could be extended by A-star path-finding)* . The tag requires an *x* and *y* coordinate to specify the grid position. It may contain a *target* attribute to specify the character to move. If no target is specified, the player character is selected.

  ```xml
  <goto target="julia" x="2" y="1" /> <!-- selects character julia -->
  <goto x="2" y="1" /> <!-- selects the player character -->
  ```

- **RelGoTo** - same as **GoTo**, moves the object relative to its current position

  ```xml
  <relgoto target="julia" x="1" y="0" /> <!-- moves julia one tile to the right -->
  ```

- **SetPos** - teleports and object to a specified position. Target selection works as stated above.

  ```xml
  <setpos target="julia" x="2" y="1" /> <!-- telports julia to (2/1) -->
  <setpos x="2" y="1" /> <!-- teleports the player character to (2/1) -->
  ```

- **RelSetPos** - same as **SetPos**, teleports to a position relative to the object's current position.

  ````xml
  <relsetpos target="julia" x="1" y="0" /> <!-- teleports julia o ne tile to the right -->
  ````

- **GoToLevel** - will teleport the player character to a specified level at a specified position.

  ```xml
  <gotolevel target="village" x="0" y="0" />
  ```

- **Move** -  moves the target object into a specified direction. If no target is specified, the player character is selected. The direction can be one of *Left*, *Right*, *Up*, *Down*. (This could be refactored to an xml attribute)

  ```xml
  <move>Right</move> <!-- moves the player character to the right -->
  ```

- **Turn** - turns the target object into a specified direction. If no target is specified, the player character is selected. The direction can be one of *Left*, *Right*, *Up*, *Down*. (This could be refactored to an xml attribute)

  ```xml
  <turn>Right</turn> <!-- turns the player character to the right -->
  ```

- **PickItem** - picks an item up to the inventory. The inventory feature has not been used in our game project. A more extended inventory system might require an inventory UI in the future.

- **Interact** - forces the player character to interact with a specified target

  ```xml
  <interact target="julia" />
  ```

- **Trigger** - triggers an event with a specified name

  ````xml
  <trigger trigger="event1" />
  ````

- **GameOver** - ends the game

  ```xml
  <gameover />
  ```



#### Triggers 

There are multiple things, that can trigger an event in a level. It can be triggered by another event, upon interaction with a world object or by walking into a trigger-area. These trigger-areas are set up in the trigger region

```xml
<triggers>
	<trigger x="26" y="-14" width="3" height="4" event="event1"/>
</triggers>
```

All trigger nodes need to be attached to the `<trigger>` node. The trigger area is defined by x, y, width and height, the event attribute points to the identifier of the target event. 

#### Instances

After all objects and events have been described in the definition section, instances of the defined objects have to be created and connected with the events via triggers. An instance node must have at least the following attributes:

- **obj** - the identifier of a defined object to use its description and display image
- **id** - an object identifier
- **if** / **not** - just as events can declare conditions to be fired, an object can declare conditions to be displayed.
- **x** / **y** - the coordinates of the instance on the level grid
- **r** - the rotation of the object as integer. The rotation of the object will be **r** * 90&deg;, rotating clockwise. (A value of 3 will rotate the object by 270&deg; clockwise)

All instance type must be grouped by their type and attached to their respective parent node.

##### Decos 

A deco is a static object in the level (walls, furniture, etc.). All deco instances must be attached to the `<decos>` node. A deco object only uses the attributes listed above.

```xml
<deco obj="plant" x="2" y="2" r="1" />
<deco obj="plant" x="2" y="0" r="0" if="fact1" /> <!-- this object only gets displayed, if fact1 is known -->
```

##### Doors

A door is a static object, that will take the player to another level upon interaction.  It specifies the target level and coordinates.

```xml
<door obj="door" x="4" y="1" target="village1" targetX="-7" targetY="-5" />
```

##### NPCs

An NPC node creates a non-player character. A character will turn towards the player upon interaction.

```xaml
<npc id="miriam" obj="miri" x="12" y="-10" r="3" />
```

##### Items

An item that can be picked up. The current implementation creates a special fact in the facts dictionary, once the item has been picked up, thus the required id attribute

```xml
<item obj="apple" x="-3" y="-10" r="1" id="apple1" />
```

##### Containers

Can contain items that are picked up upon interaction. This was a planned but did not make it into the final version. There is however a `Container.cs` class, which needs to be created in the level loader.

#### Ranges

In order to create less nodes in general, you can make use of ranges. A range creates a multiple nodes in your level on each grid tile of a specified rectangle (x, y, width, height).

```xml
<range x="0" y="0" width="10" height="1">
	<deco obj="tree" r="0" />
</range>
```

This example will create 10 decos of type tree at (0, 0), (1, 0), (2,0), ..., (9,0)

### Game State and Serialization

As stated above, the game progress is driven by facts. In order to save a game, it is sufficient to save all discovered facts along with the current position and level identifier of the character.

```xml
<?xml version="1.0" encoding="UTF-8"?>
<save>
    <level name="level1" x="9" y="-11" />
    <investigation>
        <fact id="fact1">True</fact>
        <fact id="fact2">True</fact>
        <fact id="fact3">True</fact>
    </investigation>
</save>
```

*NOTE: The term "investigation" should be refactored to "facts"*

### Shortcomings

The usage of a fact driven game progress and the simple XML-syntax cover a wide range of game scenarios.  However there are a few drawbacks:

- Moving characters around with movement events will not update their position. The movement event only updates the position of objects in the current game. Once the game gets saved and reloaded, the position is reset to the position defined in the level. It is possible to get around this issue, by creating two instances of the same object at different positions and toggle their visibility using facts. 
  *However, it would be much easier to serialize the position of dynamic objects along with the facts.*
- All events and actions are currently run sequentially. This could be extended to support parallel execution of actions. However, the space on the screen is quite limited, and multiple actions might be challenging to register for vision impaired players.



## Graphics

Each object definition points to a bitmap file which describes the looks of an object. The `BooleanCanvas` produces a Boolean matrix, that gets then passed to the output device. During the render phase, all currently visible objects are drawn to the canvas. For a Boolean pixel in the canvas there are three things that can happen:

- A pixel is set from `false` to `true`
- A pixel is set from `true` to `false`
- A pixel remains unchanged

To encode all three cases, a simple black and white bitmap did not suffice. Therefore we are using colored bitmaps, where `rgb(1, 0, 0)` describes the first, `rgb(1, 1, 0)` the second and `rgb(1, 1, 1)` the third case.

![Bush](Images\Bush.png)

The example graphics shows what is supposed to be a bush (16x16 pixel). The white pixels will not change the canvas, the red pixels will set the respective canvas pixels to `true`, the yellow pixels will set the respective canvas pixels to `false`.

![Car](Images\Car.png)

The Bitmaps must be multiples of the grid size. The dimension of the bitmap implicitly defines the grid width and height of  an object. The bitmap of this 16x32 pixel car graphic will result in a car object covering 1x2 tiles (using the default grid size of 16)



![Screen2](Images\Screen2.png)

The screenshot shows a simple scene rendered with the BrailleIO emulator.



## WYSIWYG Editor

In addition to the game project, I created a simple WYSIWYG Level editor in the form of a Unity Editor Project.  The implementation is very basic and could be embedded into the Unity Engine via custom User Interface Elements even further. It allows users to define objects and create instances, which then can be arranged in the scene view. The definition and level setup can be exported to the XML-format required by the Tactile RPG Engine.

The WYSIWYG Editor can be found [here](https://github.com/holycrab13/TactileGame/blob/master/Tactile%20WYSIWYG.zip)

## Conclusion

The Tactile RPG Engine enables users to create simple games with XML. The `GameScreen` class creates the well-known basic game loop, updating and rendering is done once per frame while the `GameInput` instance provides key inputs for input polling, thus bringing the `BrailleIO` framework closer to the game developer world. Text output is done in separate views or screens.

The Project can be extended by deriving from the base classes `WorldObject`, `ActionBase` and `EventBase`. 

Incase new objects or events get implemented, the `LevelLoader` class needs to be extended accordingly.

To all the low-resolution, black-and-white explorative RPG enthusiasts out there: Enjoy!











