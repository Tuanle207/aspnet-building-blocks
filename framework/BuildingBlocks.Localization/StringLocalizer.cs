namespace BuildingBlocks.Localization
{
    using Microsoft.Extensions.Localization;
    using System.Collections.Generic;
    using System.Reflection;

    internal class StringLocalizer : IStringLocalizer
    {
        private readonly IStringLocalizer _localizer;

        public StringLocalizer(IStringLocalizerFactory factory)
        {
            var assembly = Assembly.GetEntryAssembly();
            _localizer = factory.Create(string.Empty, assembly.FullName);
        }

        public LocalizedString this[string name] => _localizer[name];

        public LocalizedString this[string name, params object[] arguments] => _localizer[name, arguments];

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) => _localizer.GetAllStrings(includeParentCultures);
    }
}
