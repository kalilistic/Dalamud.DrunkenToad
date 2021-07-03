# Dalamud.DrunkenToad
A set of utilities to use in developing dalamud plugins. Equal in prestige to its in-game namesake.

![name-of-you-image](./assets/banner.png)

## How to Use
* Install nuget package.
* Merge using [Fody ILMerge](https://github.com/tom-englert/ILMerge.Fody).
* Implement PluginBase on your plugin class.
* Refer to the PluginBase class and Util dir for available methods.

## Sample Code
```
using System.Reflection;

using Dalamud.Plugin;
using Dalamud.DrunkenToad;
namespace SamplePlugin
{
    public class DalamudPlugin : PluginBase, IDalamudPlugin
    {
        public string Name { get; }

        public DalamudPlugin(string pluginName, DalamudPluginInterface pluginInterface, Assembly assembly)
            : base(
            pluginName, pluginInterface, Assembly.GetExecutingAssembly())
        {
        }

        public void Initialize(DalamudPluginInterface pluginInterface)
        {
            throw new System.NotImplementedException();
        
        }
    }
}
```