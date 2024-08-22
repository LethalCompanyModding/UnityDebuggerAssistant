# UnityDebuggerAssistant Changelog

## v1.2.1

- Documentation updated

## v1.2.0

- Adds two new config options
  - EnableWhiteList, enables or disables the built in whitelist for exceptions
  - EnableExperimentalMod, turns on an aggressive but experimental filter mode. Useful for deep debugging but should probably not be used in actual gameplay

## v1.1.2

- Adds a check for null targets in ILHooks which can be sent by MonoMod IL patches

## v1.1.1

- Removes LC specific symbol that stopped it from running on other unity games, whoopsie!

## v1.1.0

- Complete rewrite of Harmony patch handling
  - Understands patches from Prefixes, Postfixes and Finalizers

- Outputs the caller's defining method for better debugging

- Should now be game-agnostic

## v1.0.3

- Now checks for broken patches and refuses to handle them
  - Removes crashing
  - No idea if this effects overall quality of logs

- Now outputs even better symbols
  - Target Method
  - Caller (if applicable)

## v1.0.2

- Now outputs better symbols
  - Declaring Assembly Name for the throwing method
  - PluginInfo GUID, Name and Version for Assemblies that contain BepinEx plugins
- Update package documents

## v1.0.1

- Initial Release
- Listens for Harmony or MonoMod Patches
- Outputs a list of all plugins that modify a specific method when an Exception is thrown targeting that method
