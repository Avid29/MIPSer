// Avishai Dernis 2026

using System.Collections.Generic;

namespace Zarem.Assembler.Localization;

/// <summary>
/// An <see cref="IStringLocalizer"/> that searches through multiple <see cref="IStringLocalizer"/>s to find the appropriate key.
/// </summary>
public class CompositeLocalizer : IStringLocalizer
{
    private readonly List<IStringLocalizer> _localizers;

    /// <summary>
    /// Initializes a new instance of the <see cref="CompositeLocalizer"/> class.
    /// </summary>
    public CompositeLocalizer()
    {
        _localizers = [];
    }

    /// <inheritdoc/>
    public string? TryGet(string key, params object?[] args)
    {
        foreach (var l in _localizers)
        {
            var result = l.TryGet(key);
            if (result is not null)
            {
                return string.Format(result, args);
            }
        }

        return null;
    }

    /// <summary>
    /// Registers a new potential string source.
    /// </summary>
    public void Register(IStringLocalizer localizer)
    {
        _localizers.Add(localizer);
    }
}
