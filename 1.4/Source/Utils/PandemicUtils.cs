using System;
using System.Text.RegularExpressions;
using Verse;

namespace Pandemics
{
    public static class PandemicUtils
    {
        private static Random rnd = new Random();
        public static string GenerateVirusName()
        {
            // Regex pattern to match three uppercase letters, a hyphen, and 2-3 digits
            string pattern = @"^([A-Z]{3}-\d{2,3})$"; 

            string virusName = Regex.Replace(pattern, @"[A-Z]{3}-\d{2,3}", match =>
            {
                char randomLetter = (char)rnd.Next('A', 'Z' + 1);
                int randomDigits = rnd.Next(10, 1000);
                return $"{randomLetter}{randomLetter}{randomLetter}-{randomDigits}";
            });
            return virusName;
        }
    }
}
