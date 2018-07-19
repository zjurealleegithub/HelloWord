///
/// Copyright(C) MixModes Inc. 2010
/// 

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using MixModes.Synergy.Utilities;
using MixModes.Synergy.VisualFramework.Framework;

namespace MixModes.Synergy.VisualFramework.Windows
{
    /// <summary>
    /// Interaction logic for WindowsManager.xaml
    /// </summary>
    [StyleTypedProperty(Property = "DockIllustrationContentStyleProperty", StyleTargetType = typeof(TabItem))]
    [StyleTypedProperty(Property = "DockPaneIllustrationStyle", StyleTargetType = typeof(Panel))]       
    public partial class WindowsManager
    {        
        /// <summary>
        /// Initializes a new instance of the <see cref="WindowsManager"/> class.
        /// </summary>
        public WindowsManager()
        {
            InitializeComponent();
            _dockPaneStateMonitorList = new ObservableDependencyPropertyCollection<DockPane>(DockPane.DockPaneStateProperty);
            _dockPaneStateMonitorList.DependencyPropertyChanged += OnDockPaneStateChanged;
        }
        
        /// <summary>
        /// Adds the dock pane.
        /// </summary>
        /// <param name="pane">The pane.</param>
        /// <param name="dock">The dock.</param>
        public void AddPinnedWindow(DockPane pane, Dock dock)
        {
            AddPinnedWindowInner(pane, dock);
            
            DetachDockPaneEvents(pane);            
            AttachDockPaneEvents(pane);
        }

        /// <summary>
        /// Adds the window in auto hide fashion
        /// </summary>
        /// <param name="pane">The pane.</param>
        /// <param name="dock">The dock.</param>
        public void AddAutoHideWindow(DockPane pane, Dock dock)
        {
            CondenceDockPanel(pane, dock);

            DetachDockPaneEvents(pane);
            AttachDockPaneEvents(pane);
        }

        /// <summary>
        /// Adds the floating window.
        /// </summary>
        /// <param name="pane">The pane.</param>
        public void AddFloatingWindow(DockPane pane)
        {
            DetachDockPaneEvents(pane);
            AttachDockPaneEvents(pane);

            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                // Setting state to non-floating so that OnPaneDragStared can 
                // set it to floating and execute related effects
                pane.DockPaneState = DockPaneState.Content;
                OnPaneDragStarted(pane, null);
            }
            else
            {                
                pane.DockPaneState = DockPaneState.Floating;
                FloatingPanel.Children.Add(pane);
                MonitorStateChangeForDockPane(pane);
            }
        }

        /// <summary>
        /// Removes the dock pane from windows manager alltogether and unsubscribes from all events
        /// </summary>
        /// <param name="pane">The pane.</param>
        public void RemoveDockPane(DockPane pane)
        {
            DetachDockPaneEvents(pane);
            RemoveCondencedDockPanel(DraggedPane.CondencedDockPanel);
            RemovePinnedWindow(DraggedPane);
            FloatingPanel.Children.Remove(pane);
        }

        /// <summary>
        /// Clears the windows manager
        /// </summary>
        public void Clear()
        {
            Action<Panel> clearAction = panel => panel.Children.Clear();
            clearAction(TopPinnedWindows);
            clearAction(TopWindowHeaders);
            clearAction(BottomPinnedWindows);
            clearAction(BottomWindowHeaders);
            clearAction(LeftPinnedWindows);
            clearAction(LeftWindowHeaders);
            clearAction(RightPinnedWindows);
            clearAction(RightWindowHeaders);
            clearAction(FloatingPanel);
            clearAction(PopupArea);
            DocumentContainer.Content = null;
            DocumentContainer.Clear();
            _dockPaneStateMonitorList.Clear();
            _popupTimer.Stop();
        }

        /// <summary>
        /// Starts the dock pane state change detection
        /// </summary>
        public void StartDockPaneStateChangeDetection()
        {
            MonitorStateChangeForDockPane(DraggedPane);
        }        

        /// <summary>
        /// Stops the dock pane state change detection
        /// </summary>
        public void StopDockPaneStateChangeDetection()
        {
            IgnoreStateChangeForDockPane(DraggedPane);
        }
        
        /// <summary>
        /// Style property for illustrating how a docked pane would look like in tab control
        /// </summary>
        public static readonly DependencyProperty DockPaneIllustrationStyleProperty = DependencyProperty.Register("DockPaneIllustrationStyle",
                                                                                                                  typeof(Style),
                                                                                                                  typeof(WindowsManager));

        /// <summary>
        /// Gets the dock pane illustration style
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static Style GetDockPaneIllustrationStyle(DependencyObject obj)
        {
            return (Style)obj.GetValue(DockPaneIllustrationStyleProperty);
        }

        /// <summary>
        /// Sets the dock pane illustration style
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="value">The value.</param>
        public static void SetDockPaneIllustrationStyle(DependencyObject obj, Style value)
        {
            obj.SetValue(DockPaneIllustrationStyleProperty, value);
        }

        /// <summary>
        /// Style property for illustrating how a docked content would look like in tab control
        /// </summary>
        public static readonly DependencyProperty DockIllustrationContentStyleProperty = DependencyProperty.Register("DockIllustrationContentStyle",
                                                                                                                     typeof(Style),
                                                                                                                     typeof(WindowsManager));

        /// <summary>
        /// Gets the dock content illustration style
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static Style GetDockIllustrationContentStyle(DependencyObject obj)
        {
            return (Style)obj.GetValue(DockIllustrationContentStyleProperty);
        }

        /// <summary>
        /// Sets the dock content illustration style.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="value">The value.</param>
        public static void SetDockIllustrationContentStyle(DependencyObject obj, Style value)
        {
            obj.SetValue(DockIllustrationContentStyleProperty, value);
        }

        /// <summary>
        /// Active windows manager
        /// </summary>
        public static WindowsManager ActiveWindowsManager
        {
            get;
            private set;
        }
        
        /// <summary>
        /// Dragged pane
        /// </summary>
        /// <remarks>Needs to be static since there can be only one dragged pane at any time
        /// and also since DraggedPanes can change ownership from one WindowsManager instance to another</remarks>
        public DockPane DraggedPane
        {
            get; 
            private set;
        }

        /// <summary>
        /// Monitors the state change for dock pane and ensures that
        /// only one instance of pane is monitored to prevent redundent events
        /// </summary>
        /// <param name="pane">Pane to monitor</param>
        private void MonitorStateChangeForDockPane(DockPane pane)
        {
            if ((pane != null) && (!_dockPaneStateMonitorList.Contains(pane)))
            {
                _dockPaneStateMonitorList.Add(pane);
            }
        }

        /// <summary>
        /// Ignores the state change for dock pane.
        /// </summary>
        /// <param name="pane">Dock pane to ignore state changes from</param>
        private void IgnoreStateChangeForDockPane(DockPane pane)
        {
            if (pane != null)
            {
                _dockPaneStateMonitorList.Remove(pane);
            }
        }

        /// <summary>
        /// Adds the pinned window without manipulating events
        /// </summary>
        /// <param name="pane">The pane.</param>
        /// <param name="dock">The dock.</param>
        private void AddPinnedWindowInner(DockPane pane, Dock dock)
        {
            switch (dock)
            {
                case Dock.Bottom:
                    AddPinnedWindowToBottom(pane);
                    break;
                case Dock.Left:
                    AddPinnedWindowToLeft(pane);
                    break;
                case Dock.Right:
                    AddPinnedWindowToRight(pane);
                    break;
                case Dock.Top:
                    AddPinnedWindowToTop(pane);
                    break;
                default:
                    break;
            }

            pane.DockPaneState = DockPaneState.Docked;
        }

        /// <summary>
        /// Adds the pinned window to left of content
        /// </summary>
        /// <param name="pane">Pane to add</param>
        private void AddPinnedWindowToLeft(DockPane pane)
        {
            DockPanel.SetDock(pane, Dock.Left);
            pane.Height = double.NaN;
            LeftPinnedWindows.Children.Add(pane);

            GridSplitter sizingThumb = new GridSplitter();
            sizingThumb.Width = 4;
            sizingThumb.Background = Brushes.Transparent;
            sizingThumb.Cursor = Cursors.SizeWE;
            DockPanel.SetDock(sizingThumb, Dock.Left);
            LeftPinnedWindows.Children.Add(sizingThumb);

            sizingThumb.DragDelta += (a, b) =>
            {
                if (pane.Width.Equals(double.NaN))
                {
                    pane.Width = pane.DesiredSize.Width;
                }

                if (pane.Width + b.HorizontalChange <= 0)
                {
                    return;
                }

                pane.Width += b.HorizontalChange;                
            };
        }

        /// <summary>
        /// Adds the pinned window to right of content
        /// </summary>
        /// <param name="pane">Pane to add</param>
        private void AddPinnedWindowToRight(DockPane pane)
        {
            DockPanel.SetDock(pane, Dock.Right);
            pane.Height = double.NaN;
            RightPinnedWindows.Children.Add(pane);

            GridSplitter sizingThumb = new GridSplitter();
            sizingThumb.Width = 4;
            sizingThumb.Background = Brushes.Transparent;
            sizingThumb.Cursor = Cursors.SizeWE;
            DockPanel.SetDock(sizingThumb, Dock.Right);
            RightPinnedWindows.Children.Add(sizingThumb);
            
            sizingThumb.DragDelta += (a, b) =>
            {
                if (pane.Width.Equals(double.NaN))
                {
                    pane.Width = pane.DesiredSize.Width;
                }

                if (pane.Width - b.HorizontalChange <= 0)
                {
                    return;
                }

                pane.Width -= b.HorizontalChange;
            };
        }

        /// <summary>
        /// Adds the pinned window to top of content
        /// </summary>
        /// <param name="pane">Pane to add</param>
        private void AddPinnedWindowToTop(DockPane pane)
        {
            DockPanel.SetDock(pane, Dock.Top);
            pane.Width = double.NaN;
            TopPinnedWindows.Children.Add(pane);

            GridSplitter sizingThumb = new GridSplitter();
            sizingThumb.Height = 4;
            sizingThumb.HorizontalAlignment = HorizontalAlignment.Stretch;
            sizingThumb.Background = Brushes.Transparent;
            sizingThumb.Cursor = Cursors.SizeNS;
            DockPanel.SetDock(sizingThumb, Dock.Top);
            TopPinnedWindows.Children.Add(sizingThumb);
            
            sizingThumb.DragDelta += (a, b) =>
            {
                if (pane.Height.Equals(double.NaN))
                {
                    pane.Height = pane.DesiredSize.Height;
                }

                if (pane.Height + b.VerticalChange <= 0)
                {
                    return;
                }

                pane.Height += b.VerticalChange;
            };
        }

        /// <summary>
        /// Adds the pinned window to bottom of content
        /// </summary>
        /// <param name="pane">Pane to add</param>
        private void AddPinnedWindowToBottom(DockPane pane)
        {
            DockPanel.SetDock(pane, Dock.Bottom);
            pane.Width = double.NaN;
            BottomPinnedWindows.Children.Add(pane);

            GridSplitter sizingThumb = new GridSplitter();
            sizingThumb.Height = 4;
            sizingThumb.HorizontalAlignment = HorizontalAlignment.Stretch;
            sizingThumb.Background = Brushes.Transparent;
            sizingThumb.Cursor = Cursors.SizeNS;
            DockPanel.SetDock(sizingThumb, Dock.Bottom);
            BottomPinnedWindows.Children.Add(sizingThumb);

            sizingThumb.DragDelta += (a, b) =>
            {
                if (pane.Height.Equals(double.NaN))
                {
                    pane.Height = pane.DesiredSize.Height;
                }

                if (pane.Height - b.VerticalChange <= 0)
                {
                    return;
                }

                pane.Height -= b.VerticalChange;
            };
        }

        /// <summary>
        /// Called when dock pane's state is changed
        /// </summary>
        /// <param name="sender">Event source</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private void OnDockPaneStateChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            DockPane pane = sender as DockPane;
            Validate.Assert<ArgumentException>(pane != null);

            IgnoreStateChangeForDockPane(pane);

            DockPaneState state = (DockPaneState)e.NewValue;

            if (state == DockPaneState.AutoHide)
            {
                DockPanel logicalParentDockPanel = LogicalTreeHelper.GetParent(pane) as DockPanel;
                RemovePinnedWindow(pane);
                CondenceDockPanel(pane, DockPanel.GetDock(logicalParentDockPanel));                
            }
            else if (state == DockPaneState.Docked)
            {
                PopupArea.Children.Remove(pane);
                RemoveCondencedDockPanel(pane.CondencedDockPanel);                

                AddPinnedWindowInner(pane, DockPanel.GetDock(pane));
            }

            MonitorStateChangeForDockPane(pane);
        }

        /// <summary>
        /// Condences the dock panel
        /// </summary>
        /// <param name="pane">The pane.</param>
        /// <param name="dock">The dock.</param>
        private void CondenceDockPanel(DockPane pane, Dock dock)
        {
            FrameworkElement condencedDockPanel = pane.CondencedDockPanel;
            condencedDockPanel.LayoutTransform = (dock == Dock.Left) || (dock == Dock.Right) ? new RotateTransform(90) : null;

            switch (dock)
            {
                case Dock.Bottom:
                    BottomWindowHeaders.Children.Add(condencedDockPanel);
                    break;
                case Dock.Left:
                    LeftWindowHeaders.Children.Add(condencedDockPanel);
                    break;
                case Dock.Right:
                    RightWindowHeaders.Children.Add(condencedDockPanel);
                    break;
                case Dock.Top:
                    TopWindowHeaders.Children.Add(condencedDockPanel);
                    break;
                default:
                    break;
            }

            DockPanel.SetDock(pane, dock);
            DetachEvents(condencedDockPanel);
            pane.DockPaneState = DockPaneState.AutoHide;
            AttachEvents(condencedDockPanel);            
        }

        /// <summary>
        /// Removes the condenced dock panel
        /// </summary>
        /// <param name="condencedDockPanel">The condenced dock panel</param>
        private void RemoveCondencedDockPanel(FrameworkElement condencedDockPanel)
        {
            BottomWindowHeaders.Children.Remove(condencedDockPanel);
            LeftWindowHeaders.Children.Remove(condencedDockPanel);
            RightWindowHeaders.Children.Remove(condencedDockPanel);
            TopWindowHeaders.Children.Remove(condencedDockPanel);
            DetachEvents(condencedDockPanel);
        }

        /// <summary>
        /// Attaches the events.
        /// </summary>
        /// <param name="condencedDockPanel">The condenced dock panel.</param>
        private void AttachEvents(FrameworkElement condencedDockPanel)
        {
            condencedDockPanel.MouseEnter += OnCondencedDockPanelMouseEnter;
            condencedDockPanel.MouseLeave += OnPopupAreaMouseLeave;
        }

        /// <summary>
        /// Detaches the events.
        /// </summary>
        /// <param name="condencedDockPanel">The condenced dock panel.</param>
        private void DetachEvents(FrameworkElement condencedDockPanel)
        {
            condencedDockPanel.MouseEnter -= OnCondencedDockPanelMouseEnter;
            condencedDockPanel.MouseLeave -= OnPopupAreaMouseLeave;
        }
        
        /// <summary>
        /// Called when mouse enters condenced dock panel
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="args">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void OnCondencedDockPanelMouseEnter(object source, RoutedEventArgs args)
        {
            FrameworkElement element = source as FrameworkElement;
            Validate.Assert<ArgumentException>(element != null);

            DockPane pane = element.DataContext as DockPane;
            Validate.Assert<ArgumentException>(pane != null);

            RemovePopupEvents();
            PopupArea.Children.Clear();
            Dock dock = DockPanel.GetDock(pane);

            PopupArea.Children.Add(pane);

            bool isLeftOrRightDock = (dock == Dock.Left) || (dock == Dock.Right);

            GridSplitter sizingThumb = new GridSplitter();

            if (isLeftOrRightDock)
            {
                sizingThumb.Width = 4;
                sizingThumb.VerticalAlignment = VerticalAlignment.Stretch;
            }
            else
            {
                sizingThumb.Height = 4;
                sizingThumb.HorizontalAlignment = HorizontalAlignment.Stretch;
            }

            sizingThumb.Background = Brushes.Transparent;
            sizingThumb.Cursor = isLeftOrRightDock ? Cursors.SizeWE : Cursors.SizeNS;
            DockPanel.SetDock(sizingThumb, dock);
            PopupArea.Children.Add(sizingThumb);

            sizingThumb.DragDelta += (c, d) =>
            {
                if (isLeftOrRightDock && (pane.Width.Equals(double.NaN)))
                {
                    pane.Width = pane.DesiredSize.Width;
                }
                else if ((!(isLeftOrRightDock))&&(pane.Height.Equals(double.NaN)))
                {
                    pane.Height = pane.DesiredSize.Height;
                }

                double result = 0;
                switch (dock)
                {
                    case Dock.Bottom:
                        result = pane.Height - d.VerticalChange;
                        break;
                    case Dock.Left:
                        result = pane.Width + d.HorizontalChange;
                        break;
                    case Dock.Right:
                        result = pane.Width - d.HorizontalChange;
                        break;
                    case Dock.Top:
                        result = pane.Height + d.VerticalChange;
                        break;
                }

                if (result <= 0)
                {
                    return;
                }

                if (isLeftOrRightDock)
                {
                    pane.Width = result;
                }
                else
                {
                    pane.Height = result;
                }
            };

            pane.MouseLeave += OnPopupAreaMouseLeave;
            sizingThumb.MouseLeave += OnPopupAreaMouseLeave;
            pane.MouseEnter += OnPopupAreaMouseEnter;
            sizingThumb.MouseEnter += OnPopupAreaMouseEnter;

            _mouseOverPopupPane = true;

            EnablePopupTimer();
        }

        /// <summary>
        /// Enables the popup timer
        /// </summary>
        private void EnablePopupTimer()
        {
            _popupTimer.Stop();
            _popupTimer.Interval = TimeSpan.FromMilliseconds(200);
            _popupTimer.Tick += OnPopupTimerElapsed;
            _popupTimer.Start();
        }

        /// <summary>
        /// Removes the popup events
        /// </summary>
        private void RemovePopupEvents()
        {
            if (PopupArea.Children.Count == 0)
            {
                return;
            }

            DockPane pane = PopupArea.Children[0] as DockPane;

            if (pane != null)
            {
                pane.MouseEnter -= OnPopupAreaMouseEnter;
                pane.MouseLeave -= OnPopupAreaMouseLeave;
            }
        }

        /// <summary>
        /// Called when mouse enters in current popup area (condenced dock panel or dock panel)
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        private void OnPopupAreaMouseEnter(object sender, MouseEventArgs e)
        {
            _mouseOverPopupPane = true;
        }

        /// <summary>
        /// Called when mouse leaves current popup area (condenced dock panel or dock panel)
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        private void OnPopupAreaMouseLeave(object sender, MouseEventArgs e)
        {
            _mouseOverPopupPane = false;
        }

        /// <summary>
        /// Called when popup timer has elapsed
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnPopupTimerElapsed(object sender, EventArgs e)
        {
            _popupTimer.Tick -= OnPopupTimerElapsed;
            _popupTimer.Stop();

            if (!_mouseOverPopupPane)
            {
                PopupArea.Children.Clear();
            }
            else
            {
                EnablePopupTimer();
            }
        }

        /// <summary>
        /// Called when dock pane is closed
        /// </summary>
        /// <param name="sender">Event source</param>
        /// <param name="args">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void OnDockPaneClosed(object sender, RoutedEventArgs args)
        {
            DockPane pane = args.OriginalSource as DockPane;
            Validate.Assert<InvalidOperationException>(pane != null);
            
            DetachDockPaneEvents(pane);

            // Pane is pinned, parent must be a dock panel)
            if (pane.DockPaneState == DockPaneState.Docked)
            {
                RemovePinnedWindow(pane);
            }
            else if (pane.DockPaneState == DockPaneState.AutoHide)
            {
                PopupArea.Children.Remove(pane);
                RemoveCondencedDockPanel(pane.CondencedDockPanel);                
            }
            else if (pane.DockPaneState == DockPaneState.Floating)
            {
                FloatingPanel.Children.Remove(pane);
            }
        }

        /// <summary>
        /// Removes the pinned window
        /// </summary>
        /// <param name="pane">Dock pane</param>
        private void RemovePinnedWindow(DockPane pane)
        {
            DockPanel logicalParentDockPanel = LogicalTreeHelper.GetParent(pane) as DockPanel;

            if (logicalParentDockPanel == null)
            {
                return;
            }

            int indexOfPane = logicalParentDockPanel.Children.IndexOf(pane);

            GridSplitter splitter = logicalParentDockPanel.Children[indexOfPane + 1] as GridSplitter;
            logicalParentDockPanel.Children.Remove(pane);
            logicalParentDockPanel.Children.Remove(splitter);
        }
        
        /// <summary>
        /// Called when dock pane drag has started
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void OnPaneDragStarted(object sender, MouseButtonEventArgs e)
        {
            ActiveWindowsManager = this;
            DraggedPane = sender as DockPane;
            _dockPaneDragging = true;
            _dragStartPointOffset = (e != null) ? e.GetPosition(DraggedPane) : Mouse.GetPosition(this);
            
            // Show the visibility
            DockingPanel.Visibility = Visibility.Visible;
            
            RemoveCondencedDockPanel(DraggedPane.CondencedDockPanel);
            RemovePinnedWindow(DraggedPane);

            // Dim the dragged window
            DraggedPane.Opacity = 0.25;

            if (DraggedPane.DockPaneState != DockPaneState.Floating)
            {
                IgnoreStateChangeForDockPane(DraggedPane);

                Point panePosition = e != null ? e.GetPosition(this) : Mouse.GetPosition(this);

                Canvas.SetTop(DraggedPane, panePosition.Y);
                Canvas.SetLeft(DraggedPane, panePosition.X);
                FloatingPanel.Children.Add(DraggedPane);
                MonitorStateChangeForDockPane(DraggedPane);

                // Set this last since until the DraggedPane is not added
                // to the FloatingPanel it will not be in the visual tree
                // and hence things like adorner layer will not be present
                DraggedPane.DockPaneState = DockPaneState.Floating;
            }

            DraggedPane.IsHitTestVisible = false;

            // This is necessary or when tab items are floated again
            // mouse up event never fires
            Mouse.Capture(this, CaptureMode.SubTree);
        }
        
        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.MouseMove"/> 
        /// attached event reaches an element in its route that is derived from this class. 
        /// Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseEventArgs"/> that contains the event data.</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (!_dockPaneDragging)
            {
                return;
            }            

            Point currentMousePosition = e.GetPosition(this);
            double currentPaneXPos = Canvas.GetLeft(DraggedPane);
            double currentPaneYPos = Canvas.GetTop(DraggedPane);

            Canvas.SetTop(DraggedPane, currentMousePosition.Y - _dragStartPointOffset.Y);
            Canvas.SetLeft(DraggedPane, currentMousePosition.X - _dragStartPointOffset.X);            
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.MouseUp"/> routed event 
        /// reaches an element in its route that is derived from this class. Implement this method to add class 
        /// handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs"/> that contains the event data. 
        /// The event data reports that the mouse button was released.</param>
        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);

            if (!_dockPaneDragging)
            {
                return;
            }

            ReleaseMouseCapture();
            
            DraggedPane.Opacity = 1;
            _dockPaneDragging = false;
            DockingPanel.Visibility = Visibility.Collapsed;

            DraggedPane.IsHitTestVisible = true;

            DraggedPane = null;
            ActiveWindowsManager = null;
        }               
        
        /// <summary>
        /// Attaches the events to dock pane
        /// </summary>
        /// <param name="pane">Dock pane</param>
        private void AttachDockPaneEvents(DockPane pane)
        {
            pane.Close += OnDockPaneClosed;

            MonitorStateChangeForDockPane(pane);

            pane.HeaderDrag += OnPaneDragStarted;
        }        

        /// <summary>
        /// Detaches the events from the dock pane
        /// </summary>
        /// <param name="pane">The pane.</param>
        private void DetachDockPaneEvents(DockPane pane)
        {
            pane.Close -= OnDockPaneClosed;

            IgnoreStateChangeForDockPane(pane);

            pane.HeaderDrag -= OnPaneDragStarted;
        }        

        // Private members
        private Point _dragStartPointOffset;        
        private bool _dockPaneDragging;
        private bool _mouseOverPopupPane = false;
        private DispatcherTimer _popupTimer = new DispatcherTimer();
        private ObservableDependencyPropertyCollection<DockPane> _dockPaneStateMonitorList;
    }
}
