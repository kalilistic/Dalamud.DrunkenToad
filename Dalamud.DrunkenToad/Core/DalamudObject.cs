using System;
using System.Reflection;

namespace Dalamud.DrunkenToad.Core;

/// <summary>
/// Interface for working with instance of dalamud class.
/// </summary>
public abstract class DalamudObject
{
    protected const BindingFlags Flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance;
    protected object instance;
    protected Type instanceType;

    protected static void verify(string name, object property)
    {
        var result = property.ToString();
        Logger.LogVerbose($"Verified Prop|{name}|{result}");
    }

    protected static void verify(string name, Action func)
    {
        func.Invoke();
        Logger.LogVerbose($"Verified Func|{name}");
    }
}
