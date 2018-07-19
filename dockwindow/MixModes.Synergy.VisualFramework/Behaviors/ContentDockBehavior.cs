///
/// Copyright(C) MixModes Inc. 2010
/// 

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MixModes.Synergy.VisualFramework.Framework;
using MixModes.Synergy.VisualFramework.Windows;

namespace MixModes.Synergy.VisualFramework.Behaviors
{
    /// <summary>
    /// Illustrates the docking behavior for Content within DocumentContainer
    /// </summary>
    public class ContentDockBehavior : LogicalParentBehavior<DocumentContainer>
    {
        /// <summary>
        /// Dock point
        /// </summary>
        public ContentDockPoint DockPoint
        {
            get;
            set;
        }

        /// <summary>
        /// Called after the behavior is attached to an AssociatedObject.
        /// </summary>
        /// <remarks>Override this to hook up functionality to the AssociatedObject.</remarks>
        /// <exception cref="InvalidOperationException">WindowsManager does not exist in logical tree</exception>        
        protected override void OnAttached()
        {
            base.OnAttached();

            _tabItem = new TabItem();
            _parentStateChangeObserver = new ObservableDependencyProperty(typeof(DocumentContainer), DocumentContainer.StateProperty, OnParentStateChanged);
            _parentStateChangeObserver.AddValueChanged(LogicalParent);
            AssociatedObject.MouseEnter += OnMouseEnter;
            AssociatedObject.MouseUp += OnMouseUp;
            AssociatedObject.MouseLeave += OnMouseLeave;

            // This is important so content dock points can initialize their visibility
            OnParentStateChanged(LogicalParent.State);
        }       

        /// <summary>
        /// Called when the behavior is being detached from its AssociatedObject, but before it has actually occurred.
        /// </summary>
        /// <remarks>Override this to unhook functionality from the AssociatedObject.</remarks>
        protected override void OnDetaching()
        {
            base.OnDetaching();

            _tabItem = null;
            AssociatedObject.MouseEnter -= OnMouseEnter;
            AssociatedObject.MouseUp -= OnMouseUp;
            AssociatedObject.MouseLeave -= OnMouseLeave;
        }

        /// <summary>
        /// Called when mouse enters the associated object
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        private void OnMouseEnter(object sender, MouseEventArgs args)
        {
            if (WindowsManager.ActiveWindowsManager == null)
            {
                return;
            }

            switch (DockPoint)
            {
                case ContentDockPoint.Top:
                    IllustrateDocumentDock(Dock.Top, LogicalParent.ActualHeight / 2, double.NaN);
                    break;
                case ContentDockPoint.Left:
                    IllustrateDocumentDock(Dock.Left, double.NaN, LogicalParent.ActualWidth / 2);
                    break;
                case ContentDockPoint.Right:
                    IllustrateDocumentDock(Dock.Right, double.NaN, LogicalParent.ActualWidth / 2);
                    break;
                case ContentDockPoint.Bottom:
                    IllustrateDocumentDock(Dock.Bottom, LogicalParent.ActualHeight / 2, double.NaN);
                    break;
                case ContentDockPoint.Content:
                    IllustrateContentDock();
                    break;
                default:
                    break;
            }
        }        

        /// <summary>
        /// Called when mouse is up on the associated object
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            ClearIllustrations();

            if ((WindowsManager.ActiveWindowsManager != null) &&
                (WindowsManager.ActiveWindowsManager.DraggedPane != null))
            {
                LogicalParent.AddDockPane(DetachDockPaneFromWindowManager(), DockPoint);
            }
        }

        /// <summary>
        /// Called when mouse leaves the associated object
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        private void OnMouseLeave(object sender, MouseEventArgs args)
        {
            if (WindowsManager.ActiveWindowsManager == null)
            {
                return;
            }

            ClearIllustrations();            
        }

        /// <summary>
        /// Detaches the dock pane from window manager
        /// </summary>
        /// <returns></returns>
        private DockPane DetachDockPaneFromWindowManager()
        {
            DockPane dockPane = WindowsManager.ActiveWindowsManager.DraggedPane;
            WindowsManager.ActiveWindowsManager.RemoveDockPane(dockPane);
            return dockPane;
        }

        /// <summary>
        /// Illustrates the document dock in split windows
        /// </summary>
        /// <param name="dock">The dock.</param>
        /// <param name="height">The height.</param>
        /// <param name="width">The width.</param>
        private void IllustrateDocumentDock(Dock dock, double height, double width)
        {
            if (LogicalParent.DockIllustrationPanel == null)
            {
                return;
            }

            Grid dockPaneIllustratingGrid = new Grid();
            dockPaneIllustratingGrid.Width = width;
            dockPaneIllustratingGrid.Height = height;
            dockPaneIllustratingGrid.Style = WindowsManager.GetDockPaneIllustrationStyle(WindowsManager.ActiveWindowsManager);
            DockPanel.SetDock(dockPaneIllustratingGrid, dock);
            LogicalParent.DockIllustrationPanel.Children.Add(dockPaneIllustratingGrid);
        }

        /// <summary>
        /// Illustrates the content dock
        /// </summary>
        private void IllustrateContentDock()
        {            
            WindowsManager activeWindowsManager = WindowsManager.ActiveWindowsManager;
            _tabItem.Style = WindowsManager.GetDockIllustrationContentStyle(activeWindowsManager);
            LogicalParent.Documents.Add(_tabItem);

            if (LogicalParent.DocumentsTab != null)
            {
                LogicalParent.DocumentsTab.SelectedItem = _tabItem;
            }
        }

        /// <summary>
        /// Clears the illustrations
        /// </summary>
        private void ClearIllustrations()
        {
            if (LogicalParent.DockIllustrationPanel != null)
            {
                LogicalParent.DockIllustrationPanel.Children.Clear();
            }

            LogicalParent.Documents.Remove(_tabItem);            
        }

        /// <summary>
        /// Called when parent state has changed
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private void OnParentStateChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            OnParentStateChanged((DocumentContainerState)e.NewValue);            
        }

        /// <summary>
        /// Called when parent state has changed
        /// </summary>
        /// <param name="state">The state.</param>
        private void OnParentStateChanged(DocumentContainerState state)
        {
            if (IsDocumentStateCompatible(state))
            {
                AssociatedObject.Visibility = Visibility.Visible;
            }
            else
            {
                AssociatedObject.Visibility = Visibility.Hidden;
            }
        }

        /// <summary>
        /// Determines whether document state is compatible with current dockpoint
        /// </summary>
        /// <param name="documentContainerState">State of the document container</param>
        /// <returns>
        /// 	<c>true</c> if document state is compatible with current dockpoint; otherwise, <c>false</c>.
        /// </returns>
        private bool IsDocumentStateCompatible(DocumentContainerState documentContainerState)
        {
            switch (documentContainerState)
            {
                case DocumentContainerState.Empty:
                    return true;
                    break;
                case DocumentContainerState.ContainsDocuments:
                    return (DockPoint == ContentDockPoint.Content);
                    break;
                case DocumentContainerState.SplitHorizontally:
                    return (DockPoint == ContentDockPoint.Left) ||
                           (DockPoint == ContentDockPoint.Right);
                    break;
                case DocumentContainerState.SplitVertically:
                    return (DockPoint == ContentDockPoint.Top) ||
                           (DockPoint == ContentDockPoint.Bottom);
                    break;
                default:
                    break;
            }

            return false;
        }

        // Private members
        private TabItem _tabItem;
        private ObservableDependencyProperty _parentStateChangeObserver;
    }
}
