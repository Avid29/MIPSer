// Avishai Dernis 2025

using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;

namespace Zarem.Assembler.Localization;

/// <summary>
/// A base class for a <see cref="IStringLocalizer"/> that uses a <see cref="ResourceManager"/> to load resources.
/// </summary>
public class SetLocalizer : IStringLocalizer
{
    private ResourceManager _resourceManager;
    private ResourceSet? _neutralSet;

    /// <summary>
    /// Initializes a new instance of the <see cref="SetLocalizer"/> class.
    /// </summary>
    public SetLocalizer(string @namespace, Assembly assembly)
    {
        _resourceManager = new ResourceManager(@namespace, assembly);

        // Create fallback using neutral set
        var neutral = assembly.GetCustomAttributes<NeutralResourcesLanguageAttribute>().FirstOrDefault()?.CultureName;
        if (neutral is null)
            return;

        _neutralSet = _resourceManager.GetResourceSet(CultureInfo.GetCultureInfo(neutral), true, true);
    }

    /// <summary>
    /// Gets the localized string for the given key.
    /// </summary>
    public string? TryGet(string key, params object?[] args)
    {
        var localized = _resourceManager.GetString(key);
        if (!string.IsNullOrEmpty(localized))
            return localized;

        if (_neutralSet is not null)
        {
            var fallback = _neutralSet.GetString(key);
            if (!string.IsNullOrEmpty(fallback))
                return fallback;
        }

        return null;
    }
}
