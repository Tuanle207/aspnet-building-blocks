namespace BuildingBlocks.Localization
{
    using Microsoft.Extensions.Localization;
    using Microsoft.Extensions.Options;
    using System.Collections.Concurrent;
    using System.IO;
    using System;
    using System.Reflection;

    internal class JsonResourceManagerStringLocalizerFactory : IStringLocalizerFactory
    {
        private readonly ConcurrentDictionary<string, JsonResourceManagerStringLocalizer> _localizerCache =
            new ConcurrentDictionary<string, JsonResourceManagerStringLocalizer>();
        private readonly string _resourcesRelativePath;

        public JsonResourceManagerStringLocalizerFactory(IOptions<LocalizationOptions> localizationOptions)
        {
            if (localizationOptions == null)
            {
                throw new ArgumentNullException(nameof(localizationOptions));
            }

            _resourcesRelativePath = localizationOptions.Value.ResourcesPath ?? string.Empty;
        }

        public IStringLocalizer Create(Type resourceSource)
        {
            if (resourceSource == null)
            {
                throw new ArgumentException(nameof(resourceSource));
            }

            // Get without Add to prevent unnecessary lambda allocation
            if (!_localizerCache.TryGetValue(resourceSource.AssemblyQualifiedName!, out var localizer))
            {
                string resourcesPath = GetResourcesPath();
                string resourceName = GetLocalizationResourceNameAttribute(resourceSource);

                localizer = CreateJsonResourceManagerStringLocalizer(resourcesPath, resourceName);

                _localizerCache[resourceSource.AssemblyQualifiedName] = localizer;
            }

            return localizer;
        }

        public IStringLocalizer Create(string baseName, string location)
        {
            if (location == null)
            {
                throw new ArgumentNullException(nameof(location));
            }

            return _localizerCache.GetOrAdd($"B={baseName},L={location}", _ =>
            {
                var assemblyName = new AssemblyName(location);
                var assembly = Assembly.Load(assemblyName);
                var resourcesPath = Path.Combine(GetApplicationRoot(), _resourcesRelativePath);

                return CreateJsonResourceManagerStringLocalizer(resourcesPath, baseName);
            });
        }

        private static JsonResourceManagerStringLocalizer CreateJsonResourceManagerStringLocalizer(string resourcesPath, string resourceName)
        {
            var resourceManager = new JsonResourceManager(resourcesPath, resourceName);
            return new JsonResourceManagerStringLocalizer(resourceManager);
        }

        private static string GetLocalizationResourceNameAttribute(Type resourceSource)
        {
            var localizationResourceNameAttribute = resourceSource.GetCustomAttribute<LocalizationResourceNameAttribute>();
            return localizationResourceNameAttribute.ResourceName;
        }

        private string GetResourcesPath()
        {
            var resourceLocation = _resourcesRelativePath;
            resourceLocation = Path.Combine(
                resourceLocation
                    .Replace(Path.DirectorySeparatorChar, '.')
                    .Replace(Path.AltDirectorySeparatorChar, '.')
                    .Split('.')
            );
            resourceLocation = Path.Combine(GetApplicationRoot(), resourceLocation);
            return resourceLocation;
        }

        private static string GetApplicationRoot()
        {
            return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }
    }
}
