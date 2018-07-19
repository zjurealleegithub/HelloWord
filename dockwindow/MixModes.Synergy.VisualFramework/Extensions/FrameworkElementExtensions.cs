///
/// Copyright(C) MixModes Inc. 2010
/// 

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Collections.Generic;
using System.Windows.Media;

namespace MixModes.Synergy.VisualFramework.Extensions
{
    /// <summary>
    /// FrameworkElement extension methods
    /// </summary>
    public static class FrameworkElementExtensions
    {
        // Private members
        private const string NotificationToolTipResourceName = "NotificationToolTip";

        /// <summary>
        /// Shows the notification tool tip on a FrameworkElement at the bottom
        /// </summary>
        /// <param name="element">FrameworkElement instance to display notification on</param>
        /// <param name="content">Notification content</param>
        public static void ShowNotificationToolTip(this FrameworkElement element, object content)
        {
            ShowNotificationToolTip(element, content, PlacementMode.Bottom);
        }

        /// <summary>
        /// Shows the notification tool tip on a FrameworkElement at specified PlacementMode value
        /// </summary>
        /// <param name="element">FrameworkElement instance to display notification on</param>
        /// <param name="content">Notification content</param>
        /// <param name="placementMode">Placement mode.</param>
        public static void ShowNotificationToolTip(this FrameworkElement element, object content, PlacementMode placementMode)
        {
            ToolTip toolTip = Application.Current.Resources[NotificationToolTipResourceName] as ToolTip;
                                   
            if (toolTip == null)
            {
                throw new InvalidOperationException(string.Format("ToolTip resource with key {0} not found.", NotificationToolTipResourceName), null);
            }

            object contentClass = new { ToolTipContent = content };
            
            element.ToolTip = toolTip;
            
            toolTip.DataContext = contentClass;

            toolTip.PlacementTarget = element;
            toolTip.Placement = placementMode;
            toolTip.IsOpen = true;
        }

        /// <summary>
        /// This method ensures that the Widths and Heights are initialized.  
        /// Sizing to content produces Width and Height values of Double.NaN.  
        /// Because this Adorner explicitly resizes, the Width and Height
        /// need to be set first.  It also sets the maximum size of the adorned element.
        /// </summary>
        /// <param name="elementToResize">Element to resize</param>
        public static void EnforceSize(this FrameworkElement elementToResize)
        {
            if (elementToResize.Width.Equals(Double.NaN))
            {
                elementToResize.Width = elementToResize.DesiredSize.Width;
            }

            if (elementToResize.Height.Equals(Double.NaN))
            {
                elementToResize.Height = elementToResize.DesiredSize.Height;
            }
        }

        /// <summary>
        /// Clears the adorner layer for the element
        /// </summary>
        /// <param name="element">The element.</param>
        public static void ClearAdornerLayer(this FrameworkElement element)
        {
            AdornerLayer layer = AdornerLayer.GetAdornerLayer(element);            
            if (layer == null)
            {
                return;
            }

            Adorner[] adorners = layer.GetAdorners(element);

            if ((adorners == null) || (adorners.Length == 0))
            {
                return;
            }

            foreach (Adorner adorner in adorners)
            {
                layer.Remove(adorner);
            }
        }

        /// <summary>
        /// Gets the first logical parent of specified type or null if no parent of that type is found
        /// </summary>
        /// <typeparam name="T">Type of parent to search</typeparam>
        /// <param name="element">The element</param>
        /// <returns>First logical parent of specified type or null if no parent of that type is found</returns>
        public static T GetLogicalParent<T>(this FrameworkElement element) where T:FrameworkElement
        {
            DependencyObject parent = element;
            
            do
            {
                parent = LogicalTreeHelper.GetParent(parent);
            }
            while ((parent != null) && (!(parent is T)));

            return parent as T;
        }

        /// <summary>
        /// Gets the first visual parent of specified type or null if no parent of that type is found
        /// </summary>
        /// <typeparam name="T">Type of parent to search</typeparam>
        /// <param name="element">The element</param>
        /// <returns>First visual parent of specified type or null if no parent of that type is found</returns>
        public static T GetVisualParent<T>(this FrameworkElement element) where T : FrameworkElement
        {
            DependencyObject parent = element;

            do
            {
                parent = VisualTreeHelper.GetParent(parent);
            }
            while ((parent != null) && (!(parent is T)));

            return parent as T;
        }
    }
}
