// Avishai Dernis 2025

using Microsoft.UI.Xaml.Media;

namespace Mipser.Windows.Helpers;


public record InstructionTypeBrushPalette
{
    public SolidColorBrush? RType { get; set; }

    public SolidColorBrush? R2Type { get; set; }

    public SolidColorBrush? IType { get; set; }

    public SolidColorBrush? JType { get; set; }

    public SolidColorBrush? RegImmediate { get; set; }

    public SolidColorBrush? CoProcessor0 { get; set; }

    public SolidColorBrush? CoProcessor1 { get; set; }
}
