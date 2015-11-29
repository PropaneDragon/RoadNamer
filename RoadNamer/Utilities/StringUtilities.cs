using ColossalFramework.UI;
using System.Text.RegularExpressions;
using UnityEngine;

namespace RoadNamer.Utilities
{
    public static class StringUtilities
    {
        public static Color ExtractColourFromTags(string text, Color defaultColour)
        {
            Regex colourExtraction = new Regex("(?:<color)(#[0-9a-fA-F]{3,6})(>.*)");
            string extractedTag = colourExtraction.Replace(text, "$1");

            if(extractedTag != null && extractedTag != text && extractedTag != "")
            {
                defaultColour = UIMarkupStyle.ParseColor(extractedTag, defaultColour);
            }

            return defaultColour;
        }

        public static string RemoveTags(string text)
        {
            Regex tagRemover = new Regex("(<\\/?color.*?>)");
                        
            return tagRemover.Replace(text, "");
        }

        public static string WrapNameWithColorTags(string name, Color color)
        {
            string hexColour = UIMarkupStyle.ColorToHex(color);
            string returnName = "<color" + hexColour + ">" + name + "</color>";

            return returnName;
        }

        //Taken from http://stackoverflow.com/a/644115
        public static string PadCenter(this string s, int width, char c)
        {
            if (s == null || width <= s.Length) return s;

            int padding = width - s.Length;
            return s.PadLeft(s.Length + padding / 2, c).PadRight(width, c);
        }
    }
}
