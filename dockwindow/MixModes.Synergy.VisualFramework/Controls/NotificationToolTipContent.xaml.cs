///
/// Copyright(C) MixModes Inc. 2010
/// 

using System.Windows.Controls;
using System.Windows;

namespace MixModes.Synergy.VisualFramework.Controls
{
    /// <summary>
    /// Interaction logic for NotificationTooltipContent.xaml
    /// </summary>
    public partial class NotificationToolTipContent : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationToolTipContent"/> class.
        /// </summary>
        public NotificationToolTipContent()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Icon Property
        /// </summary>
        public DependencyProperty IconProperty =
            DependencyProperty.Register("Icon",
                                        typeof(string),
                                        typeof(NotificationToolTipContent));

        /// <summary>
        /// Gets or sets the icon.
        /// </summary>
        /// <value>The icon.</value>
        public string Icon
        {
            get { return GetValue(IconProperty) as string; }
            set { SetValue(IconProperty, value); }
        }

        /// <summary>
        /// Title property
        /// </summary>
        public DependencyProperty TitleProperty =
            DependencyProperty.Register("Title",
                                        typeof(string),
                                        typeof(NotificationToolTipContent));

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        public string Title
        {
            get { return GetValue(TitleProperty) as string; }
            set { SetValue(TitleProperty, value); }
        }

        /// <summary>
        /// Description property
        /// </summary>
        public DependencyProperty DescriptionProperty =
            DependencyProperty.Register("Description",
                                        typeof(string),
                                        typeof(NotificationToolTipContent));

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description
        {
            get { return GetValue(DescriptionProperty) as string; }
            set { SetValue(DescriptionProperty, value); }
        }
    }
}
