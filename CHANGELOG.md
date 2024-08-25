# UnityDebuggerAssistant Changelog

## v1.4.2

- UDA will now guess what the declaring assembly is based on context clues if the exception is missing the required information to obtain it normally. This will be noted in the output with `(Guess)`. This feature is good for when small methods get inlined and _merged_ with their calling methods or when dynamic methods clobber the stacktrace

## v1.4.1

- Correctly determines the patching assembly for harmony patches. (No more blaming mscorlib for everything, whoops!)
- The exception broker will now send up to 3 exceptions each frame to the handler for processing. This should result in faster responses to exceptions without a substantial increase in overhead.
- Split whitelist into exception whitelist and frame whitelist
- Added exception blacklist and frame blacklist

## v1.4.0

- Fixes a potential exception that could occur in ExceptionHandler with `SingleOrDefault` by replacing it with `FirstOrDefault`. Thanks: @p1xel8ted on github
- Switches from non thread safe list to ConcurrentStack to fix potential sync issues in UDAPatchCollector. Thanks: NutDaddy on Discord

## v1.3.2

- Applies whitelist filter setting to main assembly in a given exception

## v1.3.1

- Fixes some missing methods on older games using net standard < 2.1 such as Peaks of Yore

## v1.3.0

- Experimental mode has been folded into the standard handling loop
  - Experimental flag is now removed from the config file
- The exception handler now processes each frame of each exception and outputs any potential blames for even better debugging

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
