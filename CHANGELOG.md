# UnityDebuggerAssistant Changelog

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
  - PluginInfo GUID, Name and Version for Assemblies that contain bepinex plugins
- Update package documents

## v1.0.1

- Initial Release
- Listens for Harmony or MonoMod Patches
- Outputs a list of all plugins that modify a specific method when an Exception is thrown targeting that method
