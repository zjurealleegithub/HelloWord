///
/// Copyright(C) MixModes Inc. 2010
/// 

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MixModes.Synergy.VisualFramework.Extensions;
using System.Collections.Generic;

namespace MixModes.Synergy.VisualFramework.Adorners
{
    /// <summary>
    /// Content adorner base class that can be populated with any visual 
    /// </summary>
    internal abstract class ContentAdornerBase : AdornerBase
    {
        /// <summary>
        /// Initializes an Adorner
        /// </summary>
        /// <param name="adornedElement">The element to bind the adorner to</param>
        /// <exception cref="ArgumentNullException">adornedElement is null</exception>
        internal ContentAdornerBase(UIElement adornedElement, FrameworkElement content)
            : base(adornedElement)
        {
            _contentControl = new ContentControl();
            _contentControl.Content = content;
            _contentControl.ApplyTemplate();
            _visualChildren.Add(_contentControl);
        }

        /// <summary>
        /// Finds content with specified name
        /// </summary>
        /// <param name="name">Name of the element to find</param>
        /// <returns>An element with matching name if one exists; null otherwise</returns>
        protected T FindElement<T>(string name) where T:FrameworkElement
        {
            ContentPresenter contentPresenter = VisualTreeHelper.GetChild(_contentControl, 0) as ContentPresenter;
            FrameworkElement content = null;

            if ((contentPresenter == null) || 
                ((content = contentPresenter.Content as FrameworkElement) == null) ||
                (name == null))
            {
                return null;
            }

            Stack<FrameworkElement> searchStack = new Stack<FrameworkElement>();
            searchStack.Push(content);

            while (searchStack.Count > 0)
            {
                FrameworkElement element = searchStack.Pop();

                if (name.Equals(element.Tag))
                {
                    return element as T;
                }

                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
                {
                    FrameworkElement childElement = VisualTreeHelper.GetChild(element, i) as FrameworkElement;

                    if (childElement != null)
                    {
                        searchStack.Push(childElement);
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// When overridden in a derived class, positions child elements and 
        /// determines a size for a <see cref="T:System.Windows.FrameworkElement"/> derived class.
        /// </summary>
        /// <param name="finalSize">The final area within the parent that this element 
        /// should use to arrange itself and its children.</param>
        /// <returns>The actual size used.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {            
            (AdornedElement as FrameworkElement).EnforceSize();

            _contentControl.Arrange(new Rect(0, 0, DesiredSize.Width, DesiredSize.Height));

            return finalSize;
        }

        // Private members
        private readonly ContentControl _contentControl;
    }
}
