///
/// Copyright(C) MixModes Inc. 2010
/// 

using System.ComponentModel;
using System.Linq.Expressions;
using System;

namespace MixModes.Synergy.VisualFramework.ViewModels
{
    /// <summary>
    /// Base class for view models
    /// </summary>
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        /// <summary>
        /// Occurs when a property value changes
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the property changed event
        /// </summary>
        /// <param name="x">Expression of the form: x=> this.PropertyName</param>
        protected void RaisePropertyChanged<R>(Expression<Func<object, R>> x)
        {
            var body = x.Body as MemberExpression;
            if (body == null)
            {
                throw new ArgumentException("Argument should be of the form: x=>this.Property");
            }

            string propertyName = body.Member.Name;

            PropertyChangedEventHandler handler = this.PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }   
    }
}
