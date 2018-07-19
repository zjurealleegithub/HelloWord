///
/// Copyright(C) MixModes Inc. 2010
/// 

using System.Windows;
using System.Windows.Input;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using MixModes.Synergy.VisualFramework.Extensions;
using System.Collections.Generic;
using System.Windows.Media;

namespace MixModes.Synergy.VisualFramework.Adorners
{
    /// <summary>
    /// Resizing adorner
    /// </summary>
    internal class ResizingAdorner : ContentAdornerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResizingAdorner"/> class.
        /// </summary>
        /// <param name="adornedElement">The element to bind the adorner to</param>
        /// <exception cref="ArgumentNullException">adornedElement is null</exception>
        internal ResizingAdorner(UIElement adornedElement)
            : base(adornedElement, Application.Current.Resources[ResourceKey] as FrameworkElement)
        {
            AttachSizers();
        }
        
        /// <summary>
        /// Attaches the sizers.
        /// </summary>
        private void AttachSizers()
        {
            AttachTopSizer();
            AttachLeftSizer();
            AttachRightSizer();
            AttachBottomSizer();
            AttachTopLeftSizer();
            AttachTopRightSizer();
            AttachBottomLeftSizer();
            AttachBottomRightSizer();
        }        

        /// <summary>
        /// Attaches the top sizer
        /// </summary>
        private void AttachTopSizer()
        {
            Thumb thumb = FindElement<Thumb>("PART_SIZE_TOP");

            if (thumb == null)
            {
                return;
            }

            thumb.DragDelta += (a, b) =>
            {
                double heightDelta = ChangeHeight(-1 * b.VerticalChange);
                ChangeTop(-1 * heightDelta);
            };
        }

        /// <summary>
        /// Attaches the left sizer
        /// </summary>        
        private void AttachLeftSizer()
        {
            Thumb thumb = FindElement<Thumb>("PART_SIZE_LEFT");

            if (thumb == null)
            {
                return;
            }

            thumb.DragDelta += (a, b) =>
            {
                double widthDelta = ChangeWidth(-1 * b.HorizontalChange);
                ChangeLeft(-1 * widthDelta);
            };
        }        

        /// <summary>
        /// Attaches the right sizer.
        /// </summary>
        private void AttachRightSizer()
        {
            Thumb thumb = FindElement<Thumb>("PART_SIZE_RIGHT");

            if (thumb == null)
            {
                return;
            }

            thumb.DragDelta += (a, b) => ChangeWidth(b.HorizontalChange);            
        }

        /// <summary>
        /// Attaches the bottom sizer
        /// </summary>
        private void AttachBottomSizer()
        {
            Thumb thumb = FindElement<Thumb>("PART_SIZE_BOTTOM");

            if (thumb == null)
            {
                return;
            }

            thumb.DragDelta += (a, b) => ChangeHeight(b.VerticalChange);
        }

        /// <summary>
        /// Attaches the top left sizer
        /// </summary>
        private void AttachTopLeftSizer()
        {
            Thumb thumb = FindElement<Thumb>("PART_SIZE_TOP_LEFT");

            if (thumb == null)
            {
                return;
            }

            thumb.DragDelta += (a, b) =>
            {
                double heightDelta = ChangeHeight(-1 * b.VerticalChange);
                ChangeTop(-1 * heightDelta);

                double widthDelta = ChangeWidth(-1 * b.HorizontalChange);
                ChangeLeft(-1 * widthDelta);
            };
        }

        /// <summary>
        /// Attaches the top right sizer
        /// </summary>
        private void AttachTopRightSizer()
        {
            Thumb thumb = FindElement<Thumb>("PART_SIZE_TOP_RIGHT");

            if (thumb == null)
            {
                return;
            }

            thumb.DragDelta += (a, b) =>
            {
                double heightDelta = ChangeHeight(-1 * b.VerticalChange);
                ChangeTop(-1 * heightDelta);

                ChangeWidth(b.HorizontalChange);
            };
        }

        /// <summary>
        /// Attaches the bottom left sizer
        /// </summary>
        private void AttachBottomLeftSizer()
        {
            Thumb thumb = FindElement<Thumb>("PART_SIZE_BOTTOM_LEFT");

            if (thumb == null)
            {
                return;
            }

            thumb.DragDelta += (a, b) =>
            {
                ChangeHeight(b.VerticalChange);

                double widthDelta = ChangeWidth(-1 * b.HorizontalChange);
                ChangeLeft(-1 * widthDelta);
            };
        }

        /// <summary>
        /// Attaches the bottom right sizer
        /// </summary>
        private void AttachBottomRightSizer()
        {
            Thumb thumb = FindElement<Thumb>("PART_SIZE_BOTTOM_RIGHT");

            if (thumb == null)
            {
                return;
            }

            thumb.DragDelta += (a, b) =>
            {
                ChangeHeight(b.VerticalChange);
                ChangeWidth(b.HorizontalChange);
            };
        }

        /// <summary>
        /// Changes the top
        /// </summary>
        /// <param name="delta">The delta.</param>
        /// <returns>Actual amount the top is increased by</returns>
        private double ChangeTop(double delta)
        {
            FrameworkElement parentElement = AdornedElement as FrameworkElement;
            parentElement.EnforceSize();

            double top = Canvas.GetTop(parentElement);
            top = double.IsNaN(top) ? 0 : top;

            double newTop = top + delta;
            double appliedDelta = delta;
            if (newTop > 0)
            {
                Canvas.SetTop(parentElement, newTop);
            }
            else
            {
                appliedDelta = -1 * top;
                Canvas.SetTop(parentElement, 0);
            }

            return appliedDelta;
        }

        /// <summary>
        /// Changes the left
        /// </summary>
        /// <param name="delta">The delta.</param>
        /// <returns>Actual amount the left is increased by</returns>
        private double ChangeLeft(double delta)
        {
            FrameworkElement parentElement = AdornedElement as FrameworkElement;
            parentElement.EnforceSize();

            double left = Canvas.GetLeft(parentElement);
            left = double.IsNaN(left) ? 0 : left;

            double newLeft = left + delta;
            double appliedDelta = delta;
            if (newLeft > 0)
            {
                Canvas.SetLeft(parentElement, newLeft);
            }
            else
            {
                appliedDelta = -1 * left;
                Canvas.SetLeft(parentElement, 0);
            }

            return appliedDelta;
        }

        /// <summary>
        /// Changes the width.
        /// </summary>
        /// <param name="delta">The delta.</param>
        /// <returns>Actual amount the width is increased by</returns>
        private double ChangeWidth(double delta)
        {
            FrameworkElement parentElement = AdornedElement as FrameworkElement;
            parentElement.EnforceSize();

            double newWidth = delta + parentElement.ActualWidth;
            double appliedDelta = delta;
            if (newWidth > parentElement.MinWidth)
            {
                parentElement.Width = newWidth;
            }
            else
            {
                appliedDelta = parentElement.Width - parentElement.MinWidth;
                parentElement.Width = parentElement.MinWidth;
            }

            return appliedDelta;
        }

        /// <summary>
        /// Changes the height.
        /// </summary>
        /// <param name="delta">The delta.</param>
        /// <returns>Actual amount the width is increased by</returns>
        private double ChangeHeight(double delta)
        {
            FrameworkElement parentElement = AdornedElement as FrameworkElement;
            parentElement.EnforceSize();

            double newHeight = delta + parentElement.ActualHeight;
            double appliedDelta = delta;
            if (newHeight > parentElement.MinHeight)
            {
                parentElement.Height = newHeight;
            }
            else
            {
                appliedDelta = parentElement.Height - parentElement.MinHeight;
                parentElement.Height = parentElement.MinHeight;
            }

            return appliedDelta;
        }

        // Private members
        private const string ResourceKey = "ResizableAdorner"; 
    }
}
