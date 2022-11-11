using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using Dalamud.Interface;

namespace Dalamud.DrunkenToad;

/// <summary>
/// Font Awesome Icon utility.
/// </summary>
public static class FontAwesomeUtil
{
    static FontAwesomeUtil()
    {
        // initial arrays from default list
        var iconNames = Enum.GetNames(typeof(FontAwesomeIcon)).ToList();
        var icons = Enum.GetValues(typeof(FontAwesomeIcon)).Cast<FontAwesomeIcon>().ToList();

        // get excluded icon
        var excludedIcons = new List<string>();
        using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Dalamud.DrunkenToad.ExcludedIcons.txt");
        using var reader = new StreamReader(stream!);
        {
            while (!reader.EndOfStream)
            {
                excludedIcons.Add(reader.ReadLine() !);
            }
        }

        // remove excluded icons
        for (var i = 0; i < iconNames.Count; i++)
        {
            if (excludedIcons.Contains(iconNames[i]))
            {
                iconNames.RemoveAt(i);
                icons.RemoveAt(i);
                i--;
            }
        }

        // save as sorted arrays
        IconNames = iconNames.ToArray();
        Icons = icons.ToArray();
        Array.Sort(IconNames, Icons);
    }

    /// <summary>
    /// Gets filtered fontAwesomeIcon list.
    /// </summary>
    public static FontAwesomeIcon[] Icons { get; private set; }

    /// <summary>
    /// Gets filtered fontAwesomeIcon names.
    /// </summary>
    public static string[] IconNames { get; private set; }
}
