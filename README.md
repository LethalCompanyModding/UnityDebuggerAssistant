# UnityDebuggerAssistant

A BepinEx plugin that captures Harmony and MonoMod hook information at runtime to ease debugging

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

## How Does This Help Me?

It might not help you directly but when you go to ask for support and a kind and helpful person asks for your logs this plugin will output more debug information before each exception is written out. It will make their life easier and they will thank you.
