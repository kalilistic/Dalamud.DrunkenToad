# DalamudPluginCommon
A set of common utilities to use in dalamud plugins.
Refer to the PluginBase class and Util dir for available methods.

### How to Use
* Install nuget package.
* Merge using [Fody ILMerge](https://github.com/tom-englert/ILMerge.Fody).
* Implement PluginBase on your plugin class.
* Look at plugin base class (code or intellisense) and utils for what is available.


### Sample Code
```
using System.Reflection;

using Dalamud.Plugin;
using DalamudPluginCommon;
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