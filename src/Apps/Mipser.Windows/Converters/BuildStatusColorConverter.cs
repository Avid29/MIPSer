// Avishai Dernis 2025

using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using Mipser.Models.Enums;
using System;
using Windows.UI;

namespace Mipser.Windows.Converters;

/// <summary>
/// A converter that converts an <see cref="BuildSt"/> into a <see cref="Color"/>.
/// </summary>
public partial class BuildStatusColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object parameter, string language)
    {
        if (value is null)
            return null;

        if (value is not BuildStatus type)
            throw new ArgumentException("Value must be of type InstructionType", nameof(value));

        return type switch
        {
            BuildStatus.NotReady => NotReady,
            BuildStatus.Ready => Ready,

            BuildStatus.Preparing or
            BuildStatus.Assembling or
            BuildStatus.Linking => Running,

            BuildStatus.Completed => Done,
            BuildStatus.Failed => Failed,

            _ => NotReady,
        };
    }
    public object? ConvertBack(object value, Type targetType, object parameter, string language)
    {
        return null;
    }

    public Color? NotReady { get; set; }

    public Color? Ready { get; set; }

    public Color? Running { get; set; }

    public Color? Done { get; set; }

    public Color? Failed { get; set; }
}
