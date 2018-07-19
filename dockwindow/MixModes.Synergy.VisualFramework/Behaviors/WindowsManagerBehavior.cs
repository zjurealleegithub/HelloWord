///
/// Copyright(C) MixModes Inc. 2010
/// 

using System;
using System.Windows;
using System.Windows.Interactivity;
using MixModes.Synergy.VisualFramework.Windows;

namespace MixModes.Synergy.VisualFramework.Behaviors
{
    public abstract class WindowsManagerBehavior : Behavior<FrameworkElement>
    {
        /// <summary>
        /// Called after the behavior is attached to an AssociatedObject.
        /// </summary>
        /// <remarks>Override this to hook up functionality to the AssociatedObject.</remarks>
        /// <exception cref="InvalidOperationException">WindowsManager does not exist in logical tree</exception>
        protected override void OnAttached()
        {
            base.OnAttached();
            FindWindowsManager();
        }

        /// <summary>
        /// Windows manager
        /// </summary>
        protected WindowsManager WindowsManager
        {
            get;
            private set;
        }

        /// <summary>
        /// Finds the windows manager
        /// </summary>
        /// <exception cref="InvalidOperationException">WindowsManager does not exist in logical tree</exception>
        private void FindWindowsManager()
        {
            DependencyObject currentElement = AssociatedObject;

            while (currentElement != null)
            {
                currentElement = LogicalTreeHelper.GetParent(currentElement);

                if (currentElement is WindowsManager)
                {
                    WindowsManager = currentElement as WindowsManager;
                    return;
                }
            }

            throw new InvalidOperationException("No WindowsManager found in logical tree");
        }
    }
}
