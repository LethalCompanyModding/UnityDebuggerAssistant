# UnityDebuggerAssistant

A BepinEx plugin that captures Harmony and MonoMod hook information at runtime to ease debugging

## Supported Games

- [Lethal Company](https://thunderstore.io/c/lethal-company/p/LethalCompanyModding/UnityDebuggerAssistant/)
- [Valheim](https://thunderstore.io/c/valheim/p/LethalCompanyModding/UnityDebuggerAssistant/)
- [ULTRAKILL](https://thunderstore.io/c/ultrakill/p/LethalCompanyModding/UnityDebuggerAssistant/)
- [Cult of The Lamb](https://thunderstore.io/c/cult-of-the-lamb/p/LethalCompanyModding/UnityDebuggerAssistant/)
- [Last Train Outta Wormtown](https://thunderstore.io/c/last-train-outta-wormtown/p/LethalCompanyModding/UnityDebuggerAssistant/)
- [Ultimate Chicken Horse](https://thunderstore.io/c/ultimate-chicken-horse/p/LethalCompanyModding/UnityDebuggerAssistant/)

## Maintainers

**Original Author**: Robyn

**Current Maintainer(s)**: Robyn

This mod has been dedicated to the [Lethal Company Modding community repo](https://github.com/LethalCompanyModding/UnityDebuggerAssistant) and may be maintained by any willing community member with a github account.

## Current Features

- Enumerates patches from both Harmony and MonoMod/HookGenPatcher
- Outputs a list of all plugins that modify a specific method when an Exception is thrown within that method
- Includes useful symbols for debugging such as:
  - Calling Method for the throwing method
  - Defining Assembly Name for the throwing method
    - PluginInfo GUID, Name and Version for Assemblies that contain BepinEx plugins
  
## Configuration

> **Note**
>
> The blacklist works for either stage by matching the given string with the start of the assembly name that either produced the exception (in the exception context) or defined the method (in the frame context). This means that e.g. `Unity.Engine` will match any assembly name that begins with `Unity.Engine` such as `Unity.Engine.CoreModule`

- Exception Filter
  
  > **Note**
  >
  > Any match at this level discards the entire stack without writing it. This is for filtering whole mods or assemblies that produce useless or ignorable errors.

  - Whitelist
    - Enable Whitelist: enables or disables the whitelist on an exception basis
    - The whitelist cannot be configured but comes pre-loaded with common game assemblies. Enable this to substantially reduce log spam from UDA. Enabled by default
  - Blacklist
    - Enable Blacklist: enables or disables the blacklist on an exception basis
    - The exception blacklist is a comma separated list of assembly names that will be pattern matched against the assembly that threw the exception. Disabled by default.

- Frame Filter

  (those parts of the log that say `---FrameX`)

  > **Note**
  >
  > Any match at this level discards only the single frame. This is mostly intended for filtering system or engine assemblies that never seem to have anything useful to say inside a stack frame.

  - Whitelist
    - Enable Whitelist: enables or disables the whitelist on a frame basis
    - The whitelist cannot be configured but comes pre-loaded with common game assemblies. Enable this to slightly reduce log spam from UDA at the cost of less complete frame output when an error occurs. Disabled by default.
  - Blacklist
    - Enable Blacklist: enables or disables the blacklist on a frame basis
    - The black list is a comma separated list of assembly names that will be pattern matched from the start of each frame's method's defining assembly. This is rarely useful except for filtering out specific system assemblies e.g. UnityEngine or mscorlib from your the stack. Disabled by default.

## How Does This Help Me?

It might not help you directly but when you go to ask for support and a kind and helpful person asks for your logs this plugin will output more debug information before each exception is written out. It will make their life easier and they will thank you.
