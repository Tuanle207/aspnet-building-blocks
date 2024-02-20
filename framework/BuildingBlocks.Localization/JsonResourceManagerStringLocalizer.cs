namespace BuildingBlocks.Localization
{
    using Microsoft.Extensions.Localization;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    internal class JsonResourceManagerStringLocalizer : IStringLocalizer
    {
        private readonly JsonResourceManager _jsonResourceManager;

        public JsonResourceManagerStringLocalizer(JsonResourceManager jsonResourceManager)
        {
            _jsonResourceManager = jsonResourceManager;
        }

        public LocalizedString this[string name]
        {
            get
            {
                if (name == null)
                {
                    throw new ArgumentNullException(nameof(name));
                }

                var value = GetSafelyString(name, currentCulture: null) ?? name;
                return new LocalizedString(name, value, resourceNotFound: value == null);
            }
        }

        public LocalizedString this[string name, params object[] arguments]
        {
            get
            {
                if (name == null)
                {
                    throw new ArgumentNullException(nameof(name));
                }

                var format = GetSafelyString(name, currentCulture: null);
                var value = string.Format(format ?? name, arguments);

                return new LocalizedString(name, value, resourceNotFound: value == null);
            }
        }

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            return GetAllStrings(includeParentCultures, CultureInfo.CurrentCulture);
        }

        private IEnumerable<LocalizedString> GetAllStrings(bool _, CultureInfo culture)
        {
            foreach (var (key, value) in _jsonResourceManager.GetAllStrings(culture))
            {
                yield return new LocalizedString(key, value);
            }
        }

        private string GetSafelyString(string name, CultureInfo currentCulture)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            var culture = currentCulture ?? CultureInfo.CurrentUICulture;
            try
            {
                return culture == null ? _jsonResourceManager.GetString(name) : _jsonResourceManager.GetString(name, culture);
            }
            catch
            {
                return null;
            }
        }
    }
}
