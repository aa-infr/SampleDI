namespace Infrabel.ICT.Framework.Entity
{
    public enum SearchableType
    {
        None = 1,
        Digit = 2,    // extract only digits 
        LetterOrDigit = 4,
        LetterOrDigitWithReplacement = 5
    }
}