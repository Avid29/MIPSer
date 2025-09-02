// Avishai Dernis 2025

using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using MIPS.Models.Instructions.Enums.Registers;
using System;

namespace Mipser.Windows.Converters
{
    public partial class RegisterEncodingColorConverter : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is not Register reg)
                return null;

            return reg switch
            {
                Register.Zero or Register.AssemblerTemporary => OtherBrush,
                Register.ReturnValue0 or Register.ReturnValue1 => ReturnValueBrush,
                >= Register.Argument0 and <= Register.Argument3 => ArgBrush,
                (>= Register.Temporary0 and <= Register.Temporary7)
                or Register.Temporary8 or Register.Temporary9 => TempBrush,
                >= Register.Saved0 and <= Register.Saved7 => SavedBrush,
                Register.Kernel0 or Register.Kernel1 => KernelBrush,
                Register.GlobalPointer or Register.StackPointer or Register.FramePointer => EnvironmentBrush,
                Register.ReturnAddress => ReturnAddressBrush,
                _ => null
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }

        public Brush? ReturnValueBrush { get; set; }

        public Brush? ArgBrush { get; set; }
        
        public Brush? TempBrush { get; set; }
        
        public Brush? SavedBrush { get; set; }
        
        public Brush? KernelBrush { get; set; }
        
        public Brush? EnvironmentBrush { get; set; }
        
        public Brush? ReturnAddressBrush { get; set; }

        public Brush? OtherBrush { get; set; }
    }
}
