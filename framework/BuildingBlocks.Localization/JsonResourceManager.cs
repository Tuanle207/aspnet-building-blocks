namespace BuildingBlocks.Localization
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text.Json;

    internal class JsonResourceManager
    {
        private static readonly JsonDocumentOptions JSON_DOCUMENT_OPTIONS = new JsonDocumentOptions
        {
            CommentHandling = JsonCommentHandling.Skip,
            AllowTrailingCommas = true,
        };

        private ConcurrentDictionary<string, ConcurrentDictionary<string, string>> _resourcesCache = new ConcurrentDictionary<string, ConcurrentDictionary<string, string>>();

        private string ResourceName { get; }

        public string ResourcesPath { get; }

        public JsonResourceManager(string resourcesPath, string resourceName = null)
        {
            ResourcesPath = resourcesPath;
            ResourceName = resourceName;
        }

        public string GetString(string name)
        {
            return GetString(name, CultureInfo.CurrentUICulture);
        }

        public string GetString(string name, CultureInfo culture)
        {
            GetResourceSet(culture);

            if (_resourcesCache.Count == 0)
            {
                return null;
            }

            if (!_resourcesCache.ContainsKey(culture.Name))
            {
                return null;
            }

            return _resourcesCache[culture.Name].TryGetValue(name, out string value)
                ? value
                : null;
        }

        public IEnumerable<KeyValuePair<string, string>> GetAllStrings(CultureInfo culture)
        {
            TryLoadResourceSet(culture);

            foreach (var (key, value) in _resourcesCache[culture.Name])
            {
                yield return new KeyValuePair<string, string>(key, value);
            }
        }

        public ConcurrentDictionary<string, string> GetResourceSet(CultureInfo culture)
        {
            TryLoadResourceSet(culture);

            if (!_resourcesCache.ContainsKey(culture.Name))
            {
                return null;
            }

            _resourcesCache.TryGetValue(culture.Name, out ConcurrentDictionary<string, string> resources);
            return resources;
        }

        private void TryLoadResourceSet(CultureInfo culture)
        {
            if (_resourcesCache.ContainsKey(culture.Name))
            {
                return;
            }

            var file = Path.Combine(ResourcesPath, ResourceName, $"{culture.Name}.json");
            var resources = LoadJsonResources(file);
            _resourcesCache.TryAdd(culture.Name, new ConcurrentDictionary<string, string>(resources.ToDictionary(r => r.Key, r => r.Value)));
        }

        private static IDictionary<string, string> LoadJsonResources(string filePath)
        {
            var resources = new Dictionary<string, string>();
            if (File.Exists(filePath))
            {
                using var reader = new StreamReader(filePath);

                using var document = JsonDocument.Parse(reader.BaseStream, JSON_DOCUMENT_OPTIONS);

                ProcessJsonElement(document.RootElement, resources);
            }

            return resources;
        }

        private static void ProcessJsonElement(JsonElement element, Dictionary<string, string> resources, string currentKey = "")
        {
            switch (element.ValueKind)
            {
                case JsonValueKind.Object:
                    foreach (var property in element.EnumerateObject())
                    {
                        var key = string.IsNullOrEmpty(currentKey) ? property.Name : $"{currentKey}.{property.Name}";
                        ProcessJsonElement(property.Value, resources, key);
                    }
                    break;
                case JsonValueKind.String:
                    resources[currentKey] = element.GetString();
                    break;
                default:
                    // Ignore other value kinds
                    break;
            }
        }
    }
}
