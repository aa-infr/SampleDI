namespace Infrabel.ICT.Framework.Extension
{
    public static class IntegerExtensions
    {
        public static bool IsDClass(this int firstDigits) => firstDigits >= 224 && firstDigits <= 239;

        public static bool IsEClass(this int firstDigits) => firstDigits >= 240 && firstDigits <= 254;

        public static bool IsLocalhostRange(this int firstDigits) => firstDigits == 127;

        public static bool IsNetMaskRange(this int firstDigits) => firstDigits == 255;
    }
}