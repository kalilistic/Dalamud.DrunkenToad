using System;
using System.Linq;

namespace DalamudPluginCommon
{
    public static class FontAwesomeUtil
    {
        public static FontAwesomeIcon[] Icons;
        public static string[] IconNames;

        public static void Init()
        {
            IconNames = Enum.GetNames(typeof(FontAwesomeIcon));
            Icons = Enum.GetValues(typeof(FontAwesomeIcon)).Cast<FontAwesomeIcon>().ToArray();
            Array.Sort(IconNames, Icons);
        }
    }
}