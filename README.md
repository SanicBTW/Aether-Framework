# Aether Framework

Welcome to Aether Framework *a simple and lightweight modding framework*.

The main purpose of this modding framework is to keep it simple to add modding on different type of projects and applications.

The framework itself its pretty modular and it's built on top of that mindset, allowing the developer to be able to change aspects of the framework without having to change a lot from it.

Now let's get to the framework features, shall we?

# Features

Class overwriting (da best feature), quick and simple event system, mod loading (must have!), and modding engines.

For now the framework only has one modding engine which is the main target of the mod loader, assemblies (.dlls), but you can add one by yourself!

The mod loaders use a mod registry to save up enabled/disabled mods to keep each mod state saved between instances of your application.

Now that we had a quick rundown of features, let's get the usage of the features!

# Usage (Class Overwriting)

Class Overwriting is the biggest feature on this Modding Framework (aside from the Event System I built).

This feature comes from [here](https://github.com/kwfnf/kfunkin/tree/master/source/kfunkin/modding) which uses CPPIA (Haxe propietary).

Anyways, let's get into it!

## Loading Overridable classes

Before getting into Mod Loading and the cool stuff, you need to help the Class Registry get the "Overridable Classes".

The way you do that is extremely easy: 
```cs
ClassRegistry.RegisterOverridableClasses(Assembly.GetExecutingAssembly());
```
once you do that you're ready to go, but what is the Class Registry without the sweet sweet "Overridable Classes"?

The next section will help you with that!

## Marking classes as Overridable

Since C# doesn't have a feature like Haxe Macros, I decided to add an Attribute to mark classes as `Overridable`

The way you use the Attribute in an empty constructor is pretty simple, here have an example:
```cs
[OverridableClass]
public class ExampleClass
{
  // methods
}
```
pretty easy huh? If you want another example, check [this one](https://github.com/SanicBTW/MF-osuframework/blob/master/OFModTest/OFModTest.Game/SpinningBox.cs#L12) 

If the class you want to override has arguments in the constructor, don't worry! The Attribute can help in that:

```cs
[OverridableClass(typeof(string), typeof(int))]
public class ExampleClass
{
  public ExampleClass(string str, int num)
  {
    // do something
  }
}
```
that's somewhat bloaty but it's necessary to [help Reflection](https://github.com/SanicBTW/MF-osuframework/blob/master/ModdingFramework/ClassRegistry.cs#L127) when working with class instances from Overridable Classes,
if you want anoda example, check [this one](https://github.com/SanicBTW/MF-osuframework/blob/master/OFModTest/OFModTest.Game/ModListEntry.cs#L16)

It may be missing a lot of functionality, but for now the essentials are working properly (haven't tested if the `out` or `in` keywords).

## Class Overwritten Event

This event gets [fired](https://github.com/SanicBTW/MF-osuframework/blob/master/ModdingFramework/ClassRegistry.cs#L100) each time a class on the registry gets overwritten, this can be used to refresh anything that is using the Class Registry, being as dynamic as possible.

Check [this example](https://github.com/SanicBTW/MF-osuframework/blob/master/OFModTest/OFModTest.Game/MainScreen.cs#L71) that refreshes the current game screen whenever there is a class overwritten.

## How to use them

Now that you know how to fill up your Class Registry with overridable classes, we are going to learn how to use them.

You can overwrite the Class Registry classes everywhere, in a mod, in the base game, anywhere you think of, this makes it extremely dynamic;

Let's see a simple example:

```cs
[OverridableClass]
public class HelloWorld
{
  public virtual void DoSomething()
  {
    Console.WriteLine("Hello World!");
  }
}
```
now, this class should be in the Class Registry already, let's get to do another class that overwrites that behaviour

```cs
public class HelloHell : HelloWorld
{
  public override void DoSomething()
  {
    Console.WriteLine("Hello Hell!");
  }
}
```
once you have a class that overwrites the base class behaviour, let's rewrite it!

```cs
ClassRegistry.OverwriteClass<HelloWorld, HelloHell>();
```
that's easy! But how do I create an instance of it, that's easy too:
```cs
HelloWorld instance = ClassRegistry.CreateInstance<HelloWorld>();
```
now, calling `DoSomething` should return `Hello Hell!`, let's check it out:
```cs
instance.DoSomething(); // "Hello Hell!"
```
that's amazing and easy isn't it?

### Caveats

Overwriting an already overwritten class can have weird behaviour, you can take a look at Mod Example 2.

This approach makes it that the Overridable Class needs to have a lot of methods in order to make it extremely moddable.

## You're ready to go!

Once you have the Class Registry filled up with your cool classes, we need to get with the next topic, mod loading!

# Usage (Mod Loader)

The usage is really simple, all you have to do is create a [ModLoader](https://github.com/SanicBTW/MF-osuframework/blob/master/ModdingFramework/ModLoader.cs) which handles everything by itself (The mod loading and assemblies, registries, etc)

You can pass a couple of arguments to customize your mod loader, lets get to them
- `folder` basically the folder INSIDE the output directory where the mod loader will scan for mods.
- `filePrefix` a quick filter for which files to load, if you only want to load for example `Indexer.Format.<rest>` you would pass `Indexer.Format`.
- `engine` the modding engine the mod loader is gonna use, defaults to [Assembly Engine](https://github.com/SanicBTW/MF-osuframework/blob/master/ModdingFramework/Engines/AssemblyEngine.cs), the modding engine will handle the `folder` and `filePrefix` arguments properly.
- `config` the configuration provider used by the [ModRegistry](https://github.com/SanicBTW/MF-osuframework/blob/master/ModdingFramework/ModRegistry.cs) to save some needed configuration for the mod loaders, usually a list of enabled/disabled mods.

Once you create a new Mod Loader, it will [automatically apply](https://github.com/SanicBTW/MF-osuframework/blob/master/ModdingFramework/ModLoader.cs#L57) Hot Reload events to the event manager.

You can see an example of some of those arguments being changed in [here](https://github.com/SanicBTW/MF-osuframework/blob/master/OFModTest/OFModTest.Game/OFModTestGameBase.cs#L39).

### Warning

ALWAYS register overridable classes BEFORE creating a mod loader.

### Fun Fact
`RegisterOverridableClasses` is also [executed](https://github.com/SanicBTW/MF-osuframework/blob/master/ModdingFramework/Engines/AssemblyEngine.cs#L30) whenever it loads a mod (with the Assembly Engine)

Maybe you can make cool stuff inside mods when overriding mod classes? I haven't really tried that but it would be crazy amazing ngl

## Ready to go!

Once you have all of this, you're all set, the essentials are done, you might want to make the mod loader a singleton IF you're not thinking of adding more modding engines to your application (for example a custom Wren engine)

· But what about events? You mentioned it before.

Yeah that's the next topic

# Usage (Event Manager)

The [EventManager](https://github.com/SanicBTW/MF-osuframework/blob/master/ModdingFramework/EventManager.cs) is another great feature of this Modding Framework and probably the only thread safe thing in the whole framework.

This event system handles 2 types of events, `global` events and `targeted` events (based on `Intents` most of the time, to know how to use them, go to the `IMod` usage section)

## Fun Fact
When creating a new modding engine, the Mod Registry inside of it, will get [automatically](https://github.com/SanicBTW/MF-osuframework/blob/master/ModdingFramework/ModRegistry.cs#L28) added to the Event Manager. 

## Registering a handler for global events

It's as easy as just doing
```cs
EventManager.Register<EventType>(EventRegistryType.GLOBAL, handler);

private void handler(EventType ev)
{
  // do something
}
```
you can see an example of registering a handler for the `HRUpdateApplicationEvent` ([Hot Reload Application Event](https://github.com/SanicBTW/MF-osuframework/blob/master/ModdingFramework/Events/HRUpdateApplicationEvent.cs)) [here](https://github.com/SanicBTW/MF-osuframework/blob/master/OFModTest/ModExample/SpinningBoxReplacer.cs#L32).

You can register a global handler anywhere in your application, it's not only for Mods, but Targeted Events are, so let's get to them!

## Registering a targeted event handler (Intents)

This one gets only a little bit more harder, but not that hard, it's only adding an intent and nothing else!

The `Intents` will get detailed on the `IMod` usage section, for now it's only to show how to listen for a targeted event.

We will use an existing example for this section, [ScreenFadeMod](https://github.com/SanicBTW/MF-osuframework/blob/master/Mod.Example2/ScreenFadeMod.cs) from ModExample2, [TestEvent](https://github.com/SanicBTW/MF-osuframework/blob/master/OFModTest/OFModTest.Game/MainScreen.cs#L14) from MainScreen and [GameIntents.SCREEN_LOAD_COMPLETE](https://github.com/SanicBTW/MF-osuframework/blob/master/OFModTest/OFModTest.Game/GameIntents.cs#L5) from GameIntents (refer to the Intents section of `IMod` for recommendations)

Lets start with the mod creation

```cs
public class ScreenFadeMod : IMod
{
  public ModManifest Manifest => new()
  {
    // your manifest fields
    Intents = ["SCREEN_LOAD_COMPLETE"]
  };

  // interface implementation
}
```

The intents provided inside the manifest will get used to be able to fire events targeted to THAT intent, once that is done we can go and listen to events targeted to that type of event!

```cs
public void OnEnable()
{
  // Registers a targeted event listener for TestEvent from THIS mod
  EventManager.Register<TestEvent>(EventRegistryType.TARGETED, handler, this);
}
```

And voilà you have a targeted intent event, to fire it its as easy as the following:
```cs
EventManager.TriggerEventByIntent(new TestEvent(this), "SCREEN_LOAD_COMPLETE");
```
it's not really targeted when you think of only dispatching the events to a specific mod but since you cannot specifically access the Mods (with its typing but can only access the interface) the safest and quickest way to dispatch specific events is this one.

## Registering a targeted event handler (IMod)

This is the real targeted events, for this you need a saved instance of the mod you want to target and an event that extends [TargetedEvent](https://github.com/SanicBTW/MF-osuframework/blob/master/ModdingFramework/Events/TargetedEvent.cs)

In the mod handler you just need to do the previous steps:
```cs
EventManager.Register<EventType>(EventRegistryType.TARGETED, handler, this);
```

And in the triggering is easier
```cs
IMod mod = ...targetMod;
// Create a new instance of your event that extends TargetedEvent and pass the target mod to it
EventManager.TriggerTargetedEvent(new EventType(mod, ...));
```

This makes it extra specific to a single mod but that's the real intention of the targeted event handlers.

## Built-in Events

There are only 2 built-in events on the framework to avoid the bloat as much as possible, the 2 built-in events are related to the hot reload:

- [Hot Reload Cache Clear Event](https://github.com/SanicBTW/MF-osuframework/blob/master/ModdingFramework/Events/HRCacheClearEvent.cs), it's fired before .NET applies the hot reload changes through the next event
- [Hot Reload Application Update Event](https://github.com/SanicBTW/MF-osuframework/blob/master/ModdingFramework/Events/HRUpdateApplicationEvent.cs), it's fired when the new hot reload changes are applied

these 2 events pass the types that changed since the hot reload, you can see an example of manually updating overwritten classes whenever there is an update application event that passes the mod types [here](https://github.com/SanicBTW/MF-osuframework/blob/master/OFModTest/ModExample/SpinningBoxReplacer.cs#L58)

## Event Behaviour

The event behaviour can be modified by custom events, it also includes an `Adjust` function to be able to change any parameter of the event, this can be useful for moments where the mod can change the default values of an event and the game will use the values of the event for something, have an example:
```cs
public class DamageEvent : Event
{
  public int DamageAmount { get; private set; }
    
  public DamageEvent(int damageAmount)
  {
    DamageAmount = damageAmount;
  }

  public override void Adjust()
  {
    // Adjust the damage amount, for example by applying a global multiplier
    DamageAmount = (int)(DamageAmount * 0.9); // Apply a 10% reduction
  }
}

// To see how the IMod interface works, look at the IMod usage section
public class DamageReductionMod : IMod
{
  // implement interface...
  public void OnEnable()
  {
    EventManager.Register<DamageEvent>(EventRegistryType.GLOBAL, OnDamageEvent);
  }

  private void OnDamageEvent(DamageEvent e)
  {
    if (!e.IsCancelled) // Check if the event has been cancelled by another handler
    {
      e.Adjust();  // Apply any global adjustments first
      e.DamageAmount = (int)(e.DamageAmount * 0.5); // Further reduce damage by 50%
    }
  }
}

// Example of triggering a DamageEvent
var damageEvent = new DamageEvent(100); // Initial damage is 100
EventManager.TriggerGlobalEvent(damageEvent);

Console.WriteLine($"Final Damage: {damageEvent.DamageAmount}");
```

### Event Cancelling

When cancelling an event, it will stop propagating to the next handlers, you can see the behaviour [here](https://github.com/SanicBTW/MF-osuframework/blob/master/ModdingFramework/EventManager.cs#L304)

# IMod Usage

Now that you learnt the Modding Framework essentials, let's get to modding shall we?

Only by implementing the [IMod](https://github.com/SanicBTW/MF-osuframework/blob/master/ModdingFramework/IMod.cs) interface the mod will get automatically loaded into the framework, so let's get into the fields.

## Manifest

Refers to [ModManifest](https://github.com/SanicBTW/MF-osuframework/blob/master/ModdingFramework/Data/ModManifest.cs) for the Mods Manifest (duh) for the framework, it contains the essentials for a simple manifest object:

- Name: the name of the mod
- Description: the description of the mod (will most likely get used if there is something to list all the mods loaded).
- Author: the author of the mod (can be used with the name: `ModName by Author`).
- Intents: a list of intents, will get explained in the next section.
- Version: a version object that can be used for an auto updater or something.

## Intents

*automatically redirects to ModManifest.Intents*

This is used to let the framework know which parts of the application is going to access OR to listen to events that target that intent.

You can also use them in a different way in your application for increased security to parts of the code.

### Recommendation
I recommend having a class like [GameIntents](https://github.com/SanicBTW/MF-osuframework/blob/master/OFModTest/OFModTest.Game/GameIntents.cs) to ensure safety between intents.

## On Enable

This function will get executed once the mod gets enabled, it's recommended to overwrite any class the mod is going to use or register event handlers in here.

## On Disable

This function will get executed once the mod gets disabled, it's recommended to make any cleanups here, related to overwritten classes, registered event handlers, etc...

# Regarding the README

All of the usage documentation will be moved on to the Wiki of the repository, all the readme comes from the [original repo](https://github.com/SanicBTW/MF-osuframework) the framework was in, once I have time and the new example running, I'll work on moving the stuff to the Wiki, for now it's gonna stay here.
