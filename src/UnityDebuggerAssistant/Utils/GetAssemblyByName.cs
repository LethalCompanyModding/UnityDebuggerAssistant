using System;
using System.Reflection;
using System.Linq;

namespace UnityDebuggerAssistant.Utils;

internal static partial class Helpers
{
    internal static Assembly GetAssemblyByName(string name)
    {
        return AppDomain.CurrentDomain.GetAssemblies().
               FirstOrDefault(assembly => assembly.GetName().Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
    }
}
