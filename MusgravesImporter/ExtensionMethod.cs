using System;
using System.Collections.Generic;
using System.Text;

namespace MusgravesImporter
{
   public static class ExtensionMethod
    {
        public static string SplitByApostrophe(this string value)
        {
            if (value.Contains("'"))
            {
                var result = $"{value.Replace("'", "")}";
                return result;
            }

            return value;
        }
    }
}
