using System.Globalization;

namespace Infrabel.ICT.Framework.Service
{
    public interface ITranslationService
    {
        bool Contains(string value, CultureInfo cultureInfo);

        string Translate(string value, CultureInfo cultureInfo);
    }
}