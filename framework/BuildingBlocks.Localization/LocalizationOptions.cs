namespace BuildingBlocks.Localization
{
    using System.Collections.Generic;
    using LocalizationOptionsBase = Microsoft.Extensions.Localization.LocalizationOptions;

    public class LocalizationOptions : LocalizationOptionsBase
    {
        public List<string> SupportedCultures { get; set; }

        public string DefaultCulture { get; set; }
    }
}
