///
/// Copyright(C) MixModes Inc. 2010
/// 

using System;
using System.ComponentModel;
using System.Windows;

namespace MixModes.Synergy.VisualFramework.Framework
{
    /// <summary>
    /// Wrapper class that creates a wrapper for DependencyPropertyDescriptor
    /// </summary>
    public class ObservableDependencyProperty
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableDependencyPropertyCollection&lt;T&gt;.ObservableDependencyProperty"/> class.
        /// </summary>
        /// <param name="targetType">Type of the target</param>
        /// <param name="dependencyProperty">Dependency property.</param>
        /// <param name="OnDependencyPropertyChanged">Dependency property changed callback</param>
        public ObservableDependencyProperty(Type targetType,
                                 DependencyProperty dependencyProperty,
                                 DependencyPropertyChangedEventHandler OnDependencyPropertyChanged)
        {
            _descriptor = DependencyPropertyDescriptor.FromProperty(dependencyProperty, targetType);
            _dependencyProperty = dependencyProperty;
            _onDependencyPropertyChanged = OnDependencyPropertyChanged;
        }

        /// <summary>
        /// Enables property monitoring for a dependency object
        /// </summary>
        /// <param name="dependencyObject">The dependency object</param>
        public void AddValueChanged(DependencyObject dependencyObject)
        {
            _oldValue = dependencyObject.GetValue(_dependencyProperty);
            _descriptor.AddValueChanged(dependencyObject, OnValueChanged);
        }

        /// <summary>
        /// Disables property monitoring for a dependency object
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        public void RemoveValueChanged(DependencyObject dependencyObject)
        {
            _descriptor.RemoveValueChanged(dependencyObject, OnValueChanged);
        }

        /// <summary>
        /// Called when value of dependency property has changed
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnValueChanged(object sender, EventArgs args)
        {
            if (_changeEventInProgress)
            {
                return;
            }

            _changeEventInProgress = true;

            object oldValue = _oldValue;
            _oldValue = (sender as DependencyObject).GetValue(_dependencyProperty);

            _onDependencyPropertyChanged(sender,
                new DependencyPropertyChangedEventArgs(_dependencyProperty,
                                                       oldValue,
                                                       _oldValue));

            _changeEventInProgress = false;
        }

        // Private members
        private DependencyPropertyChangedEventHandler _onDependencyPropertyChanged;
        private DependencyPropertyDescriptor _descriptor;
        private DependencyProperty _dependencyProperty;
        private bool _changeEventInProgress = false;
        private object _oldValue;
    }
}
