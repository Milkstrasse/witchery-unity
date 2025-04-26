using System;

namespace UnityEngine.Localization.Settings
{
    [Serializable]
    public class CustomLocaleSelector : IStartupLocaleSelector
    {
        public Locale GetStartupLocale(ILocalesProvider availableLocales)
        {
            for (int i = 0; i < LocalizationSettings.AvailableLocales.Locales.Count; i++)
            {
                if (LocalizationSettings.AvailableLocales.Locales[i].ToString().Split(' ')[0] == Application.systemLanguage.ToString())
                {
                    return LocalizationSettings.AvailableLocales.Locales[i];
                }
            }
            
            // No locale could be found.
            return null;
        }
    }
}