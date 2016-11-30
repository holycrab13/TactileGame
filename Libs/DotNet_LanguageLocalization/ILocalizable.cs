using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace tud.mci.LanguageLocalization
{
    public interface ILocalizable
    {
        /// <summary>
        /// Change the standard culture used for localizing texts..
        /// </summary>
        /// <param name="culture">The culture to use for localization.</param>
        void SetLocalizationCulture(System.Globalization.CultureInfo culture);
    }
}
