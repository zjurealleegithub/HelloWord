///
/// Copyright(C) MixModes Inc. 2010
/// 

using System.Windows.Data;
using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows;
namespace MixModes.Synergy.VisualFramework.Converters
{
    /// <summary>
    /// Converts empty behavior of a panel to visibility
    /// </summary>
    public class EmptyPanelToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmptyPanelToVisibilityConverter"/> class.
        /// </summary>
        public EmptyPanelToVisibilityConverter()
        {
            EmptyVisibility = Visibility.Collapsed;    
        }

        /// <summary>
        /// Empty visibility mode
        /// </summary>
        public Visibility EmptyVisibility
        {
            get;
            set;
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object Convert(object value,
                              Type targetType, 
                              object parameter, 
                              CultureInfo culture)
        {
            Panel panel;
            if ((value == null) || ((panel = value as Panel) == null))
            {
                return EmptyVisibility;
            }

            return panel.Children.Count > 0 ? Visibility.Visible : EmptyVisibility;
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object ConvertBack(object value, 
                                  Type targetType, 
                                  object parameter, 
                                  CultureInfo culture)
        {
            throw new InvalidOperationException();
        }
    }
}
