using System;
using Humanizer;
namespace mechanical.Utils
{
    public static class NumberToWordsConverter
    {
        public static string ConvertDoubleToWords(double number)
        {
            long wholeNumber = (long)number; // Get the whole number part
            int decimalPart = (int)((number - wholeNumber) * 100); // Get the decimal part (cents)

            // Convert the whole number part to words
            string words = wholeNumber.ToWords(); // Using Humanizer

            words += " Birrs";

            if (decimalPart > 0)
            {
                words += " and " + decimalPart.ToWords() + " Cents";
            }
            else
            {
                words += " and No Cents";
            }

            return words;
        }
    }
}
