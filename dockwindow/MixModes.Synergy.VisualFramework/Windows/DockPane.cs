///
/// Copyright(C) MixModes Inc. 2010
/// 

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using MixModes.Synergy.VisualFramework.Adorners;
using MixModes.Synergy.VisualFramework.Commands;
using MixModes.Synergy.VisualFramework.Extensions;

namespace MixModes.Synergy.VisualFramework.Windows
{
    /// <summary>
    /// DockPanel class
    /// </summary>
    [TemplatePart(Name = "PART_DOCK_PANE_HEADER", Type = typeof(UIElement))]
    [TemplatePart(Name = "PART_CLOSE", Type = typeof(Button))]
    [TemplatePart(Name = "PART_PIN", Type = typeof(ToggleButton))]
    [TemplatePart(Name = "PART_DOCK_PANE_MENU", Type = typeof(Button))]
    public class DockPane : HeaderedContentControl
    {
        /// <summary>
        /// Initializes the <see cref="DockPane"/> class.
        /// </summary>
        static DockPane()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DockPane), new FrameworkPropertyMetadata(typeof(DockPane)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DockPane"/> class.
        /// </summary>
        public DockPane()
        {
            CreateCommands();
            Loaded += InitializeDockPane;
        }        

        /// <summary>
        /// Icon Property
        /// </summary>
        public static DependencyProperty IconProperty = DependencyProperty.Register("Icon",
                                                                                    typeof(string),
                                                                                    typeof(DockPane));
        /// <summary>
        /// Condenced dock panel template property
        /// </summary>
        public static DependencyProperty CondencedDockPanelTemplateProperty = DependencyProperty.Register("CondencedDockPanelTemplate",
                                                                                    typeof(DataTemplate),
                                                                                    typeof(DockPane));
        /// <summary>
        /// DockPaneState Property
        /// </summary>
        public static DependencyProperty DockPaneStateProperty = DependencyProperty.Register("DockPaneState",
                                                                                             typeof(DockPaneState),
                                                                                             typeof(DockPane),
                                                                                             new PropertyMetadata(OnDockPaneStateChanged));
        /// <summary>
        /// Close event
        /// </summary>
        public static readonly RoutedEvent CloseEvent =
            EventManager.RegisterRoutedEvent("Close",
                                             RoutingStrategy.Bubble,
                                             typeof(RoutedEventHandler),
                                             typeof(DockPane));

        /// <summary>
        /// Occurs when close button is clicked
        /// </summary>
        public event RoutedEventHandler Close
        {
            add { AddHandler(CloseEvent, value); }
            remove { RemoveHandler(CloseEvent, value); }
        }

        /// <summary>
        /// Toggle pin event
        /// </summary>
        public static readonly RoutedEvent TogglePinEvent =
            EventManager.RegisterRoutedEvent("TogglePin",
                                             RoutingStrategy.Bubble,
                                             typeof(RoutedEventHandler),
                                             typeof(DockPane));

        /// <summary>
        /// Occurs when dock pane's pin is toggled
        /// </summary>
        public event RoutedEventHandler TogglePin
        {
            add { AddHandler(TogglePinEvent, value); }
            remove { RemoveHandler(TogglePinEvent, value); }
        }

        /// <summary>
        /// Header drag event
        /// </summary>
        public static readonly RoutedEvent HeaderDragEvent = 
            EventManager.RegisterRoutedEvent("HeaderDrag",
                                             RoutingStrategy.Bubble,
                                             typeof(MouseButtonEventHandler),
                                             typeof(DockPane));

        /// <summary>
        /// Occurs when header is dragged
        /// </summary>
        public event MouseButtonEventHandler HeaderDrag
        {
            add { AddHandler(HeaderDragEvent, value); }
            remove { RemoveHandler(HeaderDragEvent, value); }
        }

        /// <summary>
        /// Condenced dock panel
        /// </summary>
        public FrameworkElement CondencedDockPanel
        {
            get
            {
                return CondencedDockPaneInstance ?? (CondencedDockPaneInstance = CreateCondencedDockPane());
            }
        }
        
        /// <summary>
        /// Gets or sets the icon.
        /// </summary>
        public string Icon
        {
            get { return GetValue(IconProperty) as string; }
            set { SetValue(IconProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating dock pane state
        /// </summary>
        /// <value>State of dock pane</value>
        public DockPaneState DockPaneState
        {
            get { return (DockPaneState)GetValue(DockPaneStateProperty); }
            set { SetValue(DockPaneStateProperty, value); }
        }

        /// <summary>
        /// Condenced dock panel template
        /// </summary>
        public DataTemplate CondencedDockPanelTemplate
        {
            get { return (DataTemplate) GetValue(CondencedDockPanelTemplateProperty); }
            set { SetValue(CondencedDockPanelTemplateProperty, value); }
        }       

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code 
        /// or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();            
            AttachToVisualTree();
        }

        /// <summary>
        /// Attaches to visual tree to the template
        /// </summary>
        private void AttachToVisualTree()
        {
            AttachDockPaneHeader();
            AttachCloseButton();
            AttachPinButton();            
        }

        /// <summary>
        /// Attaches the dock pane header
        /// </summary>
        private void AttachDockPaneHeader()
        {
            if (DockPaneHeader != null)
            {
                DockPaneHeader.MouseLeftButtonDown -= OnHeaderLeftMouseButtonDown;
                DockPaneHeader.MouseMove -= OnHeaderMouseMove;
            }

            DockPaneHeader = GetTemplateChild("PART_DOCK_PANE_HEADER") as UIElement;

            if (DockPaneHeader != null)
            {
                DockPaneHeader.MouseLeftButtonDown += OnHeaderLeftMouseButtonDown;
                DockPaneHeader.MouseMove += OnHeaderMouseMove;
            }            
        }        

        /// <summary>
        /// Called when left mouse button is down on header
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void OnHeaderLeftMouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            DraggedPane = this;
            DragStartPoint = e.GetPosition(this);
        }

        /// <summary>
        /// Called when mouse moves on the header
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        private void OnHeaderMouseMove(object sender, MouseEventArgs e)
        {
            if ((e.LeftButton != MouseButtonState.Pressed) || ((DraggedPane != null) && (DraggedPane != this)))
            {
               return;
            }

            // Check for minimum distance in order to start drag
            Vector distance = e.GetPosition(this) - DragStartPoint;

            if ((Math.Abs(distance.X) < SystemParameters.MinimumHorizontalDragDistance) &&
                (Math.Abs(distance.Y) < SystemParameters.MinimumVerticalDragDistance))
            {
                return;
            }

            RoutedEventArgs args = new MouseButtonEventArgs(e.MouseDevice, e.Timestamp, MouseButton.Left);
            args.RoutedEvent = HeaderDragEvent;
            RaiseEvent(args);
        }

        /// <summary>
        /// Attaches the close button
        /// </summary>
        private void AttachCloseButton()
        {
            if (CloseButton != null)
            {
                CloseButton.Command = null;
            }

            Button closeButton = GetTemplateChild("PART_CLOSE") as Button;
            if (closeButton != null)
            {
                closeButton.Command = CloseCommand;
                CloseButton = closeButton;
            }
        }

        /// <summary>
        /// Attaches the pin button
        /// </summary>
        private void AttachPinButton()
        {
            if (PinButton != null)
            {
                PinButton.Command = null;
            }

            ToggleButton pinButton = GetTemplateChild("PART_PIN") as ToggleButton;
            if (pinButton != null)
            {
                pinButton.Command = TogglePinCommand;
                PinButton = pinButton;
            }
        }

        /// <summary>
        /// Creates the commands
        /// </summary>
        private void CreateCommands()
        {
            CloseCommand = new CommandBase(arg => RaiseEvent(new RoutedEventArgs(CloseEvent)));
            TogglePinCommand = new CommandBase(arg =>
            {
                DockPaneState = PinButton.IsChecked.Value ? DockPaneState.AutoHide : DockPaneState.Docked;
            });
        }

        /// <summary>
        /// Creates the condenced dock pane
        /// </summary>
        private FrameworkElement CreateCondencedDockPane()
        {
            ContentControl content = new ContentControl();
            content.ContentTemplate = CondencedDockPanelTemplate;
            content.Content = new { Header = this.Header, Icon = this.Icon };
            content.DataContext = this;
            return content;
        }

        /// <summary>
        /// Called when state is changed
        /// </summary>
        /// <param name="d">Dependency object</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnDockPaneStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DockPane pane = d as DockPane;
            DockPaneState state = (DockPaneState)e.NewValue;
            OnDockPaneStateChange(pane, state);
        }

        /// <summary>
        /// Initializes the dock pane.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void InitializeDockPane(object sender, RoutedEventArgs e)
        {
            // Refresh state to ensure that proper controls (e.g. resizing adorners) are initialized since
            // before adorner layer was not present.
            OnDockPaneStateChange(this, DockPaneState);
        }

        /// <summary>
        /// Called when dock pane state has changed
        /// </summary>
        /// <param name="pane">Dock pane.</param>
        /// <param name="state">New state (may not be current yet)</param>
        private static void OnDockPaneStateChange(DockPane pane, DockPaneState state)
        {
            switch (state)
            {
                case DockPaneState.Docked:

                    pane.ClearAdornerLayer();

                    if (pane.PinButton != null)
                    {
                        pane.PinButton.Visibility = Visibility.Visible;
                        pane.PinButton.IsChecked = false;
                    }
                    break;

                case DockPaneState.AutoHide:

                    pane.ClearAdornerLayer();

                    if (pane.PinButton != null)
                    {
                        pane.PinButton.Visibility = Visibility.Visible;
                        pane.PinButton.IsChecked = true;
                    }
                    break;

                case DockPaneState.Floating:

                    pane.AddResizingAdorner();

                    if (pane.PinButton != null)
                    {
                        pane.PinButton.Visibility = Visibility.Collapsed;
                    }
                    break;

                case DockPaneState.Content:

                    pane.ClearAdornerLayer();
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Adds the resizing adorner to the dock pane
        /// </summary>
        private void AddResizingAdorner()
        {
            AdornerLayer layer = AdornerLayer.GetAdornerLayer(this);
            if (layer != null)
            {
                layer.Add(new ResizingAdorner(this));
            }
        }

        /// <summary>
        /// Close command
        /// </summary>
        private ICommand CloseCommand;

        /// <summary>
        /// Pin toggle command
        /// </summary>
        private ICommand TogglePinCommand;

        /// <summary>
        /// Close button
        /// </summary>
        private Button CloseButton { get; set; }

        /// <summary>
        /// Pin button
        /// </summary>
        private ToggleButton PinButton { get; set; }

        /// <summary>
        /// Condenced Dock Pane
        /// </summary>
        private FrameworkElement CondencedDockPaneInstance;

        /// <summary>
        /// Dock pane header
        /// </summary>
        private UIElement DockPaneHeader;

        /// <summary>
        /// Drag start point
        /// </summary>
        private Point DragStartPoint { get; set; }

        /// <summary>
        /// Gets or sets the dragged pane.
        /// </summary>
        /// <value>The dragged pane.</value>
        private static DockPane DraggedPane { get; set; }
    }
}
