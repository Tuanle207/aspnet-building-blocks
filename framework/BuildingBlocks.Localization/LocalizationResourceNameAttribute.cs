namespace BuildingBlocks.Localization
{
    using System;

    public class LocalizationResourceNameAttribute : Attribute
    {
        public string ResourceName{ get; }

        public LocalizationResourceNameAttribute(string resourceName)
        {
            ResourceName = resourceName;
        }
    }
}
