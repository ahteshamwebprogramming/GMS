namespace GMS.Infrastructure.Helper
{
    // File: Helpers/AmountConverter.cs
    public static class AmountConverter
    {
        public static string ConvertAmountToWords(decimal amount)
        {
            if (amount == 0)
                return "Zero Rupees Only";

            var n = (long)Math.Floor(amount);
            var paise = (int)((amount - n) * 100);

            string rupees = ConvertToWords(n);
            string result = $"{rupees} Rupees";

            if (paise > 0)
            {
                result += $" and {ConvertToWords(paise)} Paise";
            }

            return result + " Only";
        }

        private static string ConvertToWords(long number)
        {
            string[] unitsMap = { "", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten",
                              "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen",
                              "Seventeen", "Eighteen", "Nineteen" };
            string[] tensMap = { "", "", "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety" };

            if (number < 20)
                return unitsMap[number];
            if (number < 100)
                return tensMap[number / 10] + (number % 10 > 0 ? " " + unitsMap[number % 10] : "");
            if (number < 1000)
                return ConvertToWords(number / 100) + " Hundred" + (number % 100 > 0 ? " " + ConvertToWords(number % 100) : "");
            if (number < 100000)
                return ConvertToWords(number / 1000) + " Thousand" + (number % 1000 > 0 ? " " + ConvertToWords(number % 1000) : "");
            if (number < 10000000)
                return ConvertToWords(number / 100000) + " Lakh" + (number % 100000 > 0 ? " " + ConvertToWords(number % 100000) : "");

            return ConvertToWords(number / 10000000) + " Crore" + (number % 10000000 > 0 ? " " + ConvertToWords(number % 10000000) : "");
        }
    }

}
