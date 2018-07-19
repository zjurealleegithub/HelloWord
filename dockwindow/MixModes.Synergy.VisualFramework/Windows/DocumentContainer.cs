///
/// Copyright(C) MixModes Inc. 2010
/// 

using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MixModes.Synergy.Utilities;
using MixModes.Synergy.VisualFramework.Commands;
using MixModes.Synergy.VisualFramework.Extensions;
using System.Collections.Generic;

namespace MixModes.Synergy.VisualFramework.Windows
{
    /// <summary>
    /// Encapsulates documents into tabbed and/or split views
    /// </summary>
    [TemplatePart(Name="PART_DOCK_POINTS", Type=typeof(Panel))]
    [TemplatePart(Name="PART_DOCK_ILLUSTRATION", Type=typeof(DockPanel))]
    [TemplatePart(Name="PART_DOCUMENTS", Type=typeof(TabControl))]
    public class DocumentContainer : ContentControl
    {
        /// <summary>
        /// Initializes the <see cref="DocumentContainer"/> class.
        /// </summary>
        static DocumentContainer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DocumentContainer), new FrameworkPropertyMetadata(typeof(DocumentContainer)));           
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentContainer"/> class.
        /// </summary>
        public DocumentContainer()
        {
            State = DocumentContainerState.Empty;
            Documents = new ObservableCollection<object>();
            RemoveDocumentCommand = new CommandBase(arg => RemoveDocument(arg as DocumentContent));
        }

        /// <summary>
        /// Clears the content panel
        /// </summary>
        public void Clear()
        {
            Content = null;
            Documents.Clear();
            State = DocumentContainerState.Empty;
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            DockPointsPanel = GetTemplateChild("PART_DOCK_POINTS") as Panel;
            DockIllustrationPanel = GetTemplateChild("PART_DOCK_ILLUSTRATION") as DockPanel;
            
            if (DocumentsTab != null)
            {
                DocumentsTab.RemoveHandler(TabItem.PreviewMouseDownEvent, new RoutedEventHandler(OnTabItemMouseDown));
                DocumentsTab.RemoveHandler(TabItem.PreviewMouseUpEvent, new RoutedEventHandler(OnTabItemMouseUp));
                DocumentsTab.RemoveHandler(TabItem.PreviewMouseMoveEvent, new RoutedEventHandler(OnTabItemMouseMove));
            }

            DocumentsTab = GetTemplateChild("PART_DOCUMENTS") as TabControl;

            if (DocumentsTab != null)
            {
                DocumentsTab.AddHandler(TabItem.PreviewMouseDownEvent, new RoutedEventHandler(OnTabItemMouseDown));
                DocumentsTab.AddHandler(TabItem.PreviewMouseUpEvent, new RoutedEventHandler(OnTabItemMouseUp));
                DocumentsTab.AddHandler(TabItem.PreviewMouseMoveEvent, new RoutedEventHandler(OnTabItemMouseMove));
            }            
        }

        /// <summary>
        /// Dock illustration panel containing content docking points
        /// </summary>
        public DockPanel DockIllustrationPanel
        {
            get;
            private set;
        }

        /// <summary>
        /// Documents tab
        /// </summary>
        public TabControl DocumentsTab
        {
            get;
            private set;
        }

        /// <summary>
        /// Document state property
        /// </summary>
        public static readonly DependencyProperty StateProperty = DependencyProperty.Register("State",
                                                                                              typeof(DocumentContainerState),
                                                                                              typeof(DocumentContainer),
                                                                                              new PropertyMetadata(DocumentContainerState.Empty));

        /// <summary>
        /// Document container state
        /// </summary>
        public DocumentContainerState State
        {
            get
            {
                return (DocumentContainerState)GetValue(StateProperty);
            }
            private set
            {
                SetValue(StateProperty, value);
            }
        }        

        /// <summary>
        /// Documents dependency property
        /// </summary>
        public static DependencyProperty DocumentsProperty = DependencyProperty.Register("Documents",
                                                                                         typeof(ObservableCollection<object>),
                                                                                         typeof(DocumentContainer),
                                                                                         new PropertyMetadata(new ObservableCollection<object>()));

        /// <summary>
        /// Documents
        /// </summary>
        public ObservableCollection<object> Documents
        {
            get
            {
                return (ObservableCollection<object>)GetValue(DocumentsProperty);
            }
            private set
            {
                SetValue(DocumentsProperty, value);
            }
        }

        /// <summary>
        /// Adds the dock pane to the document container via splitting and docks it to the specified dock point
        /// </summary>
        /// <param name="pane">Pane to add</param>
        /// <param name="dockPoint">The dock point for content</param>
        public void AddDockPane(DockPane pane, ContentDockPoint dockPoint)
        {            
            DocumentContainer targetContainer;
            if (CanMergeContent(dockPoint, out targetContainer))
            {
                targetContainer.AddDocument(pane);
            }
            else
            {
                SplitAndAddDocument(pane, dockPoint);
            }            
        }

        /// <summary>
        /// Adds the document containers by splitting the current document container
        /// </summary>
        /// <param name="childContainers">The child containers.</param>
        /// <param name="isHorizontal">if set to <c>true</c> indicates horizontal orientation otherwise vertical orientation</param>
        /// <exception cref="ArgumentNullException">childContainers is null</exception>
        /// <exception cref="InvalidOperationException">
        /// Current document container is not empty or
        /// current state is not SplitHorizontally or SplitVertically
        /// childContainers is empty or
        /// childContainers has more than two containers
        /// or the containers overlap with each other
        /// </exception>
        public void AddDocumentContainers(IEnumerable<DocumentContainer> childContainers, bool isHorizontal)
        {
            Validate.Assert<InvalidOperationException>(Content == null);

            Validate.NotNull(childContainers, "childContainers");
            List<DocumentContainer> childContainerList = new List<DocumentContainer>(childContainers);
            Validate.Assert<InvalidOperationException>((childContainerList.Count > 0) && (childContainerList.Count < 3));

            Grid splitGrid = isHorizontal ? SplitHorizontally(true) : SplitVertically(true);
            splitGrid.Children.RemoveAt(1);

            int position = -1;

            for (int i = 0; i < 2; i++)
            {
                DocumentContainer childContainer;

                if (i < childContainerList.Count)
                {
                    childContainer = childContainerList[i];
                    int row = Grid.GetRow(childContainer);
                    int column = Grid.GetColumn(childContainer);

                    if (isHorizontal)
                    {
                        Validate.Assert<InvalidOperationException>((row == 0) && (position != column));
                        position = column;
                    }
                    else
                    {
                        Validate.Assert<InvalidOperationException>((column == 0) && (position != row));
                        position = row;
                    }
                }
                else
                {
                    childContainer = new DocumentContainer();

                    int row = 0;
                    int column = 0;

                    if (isHorizontal)
                    {
                        column = position != 0 ? 0 : 2;
                    }
                    else
                    {
                        row = position != 0 ? 0 : 2;
                    }

                    Grid.SetColumn(childContainer, column);
                    Grid.SetRow(childContainer, row);
                }
                splitGrid.Children.Add(childContainer);
            }
        }

        /// <summary>
        /// Adds the dock panel as a document
        /// </summary>
        /// <param name="pane">The pane</param>
        public void AddDocument(DockPane pane)
        {
            DocumentContent documentContent = new DocumentContent(pane, RemoveDocumentCommand);
            pane.DockPaneState = DockPaneState.Content;
            Documents.Add(documentContent);

            if (DocumentsTab != null)
            {
                DocumentsTab.SelectedItem = documentContent;                
            }

            State = DocumentContainerState.ContainsDocuments;
        }

        /// <summary>
        /// Removes the document.
        /// </summary>
        /// <param name="documentContent">DocumentContent to remove</param>
        private void RemoveDocument(DocumentContent documentContent)
        {
            if (documentContent == null)
            {
                return;               
            }

            Documents.Remove(documentContent);

            if (Documents.Count == 0)
            {
                State = DocumentContainerState.Empty;

                WindowsManager parentWindowsManager = this.GetVisualParent<WindowsManager>();
                if (parentWindowsManager != null)
                {
                    ReduceChild(parentWindowsManager.DocumentContainer);                    
                }
            }
        }

        /// <summary>
        /// Called when tab item is dragged
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void OnTabItemMouseDown(object sender, RoutedEventArgs args)
        {
            DragStartPoint = Mouse.GetPosition(this);
            DragStarted = true;
        }

        /// <summary>
        /// Called when tab item is stopped dragging
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void OnTabItemMouseUp(object sender, RoutedEventArgs args)
        {
            DragStarted = false;
        }

        /// <summary>
        /// Called when tab item is dragged
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void OnTabItemMouseMove(object sender, RoutedEventArgs args)
        {
            if ((Mouse.LeftButton != MouseButtonState.Pressed) || (!DragStarted))
            {
                return;
            }

            // Check for minimum distance in order to start drag
            Vector distance = Mouse.GetPosition(this) - DragStartPoint;

            if ((Math.Abs(distance.X) < SystemParameters.MinimumHorizontalDragDistance) &&
                (Math.Abs(distance.Y) < SystemParameters.MinimumVerticalDragDistance))
            {
                return;
            }

            DragStarted = false;

            WindowsManager windowsManager = this.GetLogicalParent<WindowsManager>();
            TabItem currentItem = (args.OriginalSource as FrameworkElement).GetVisualParent<TabItem>();

            if (currentItem == null)
            {
                return;
            }

            currentItem.Header = null;
            currentItem.Content = null;

            DocumentContent dockPaneContent = currentItem.DataContext as DocumentContent;
            Documents.Remove(dockPaneContent);
            dockPaneContent.DetachDockPane();

            windowsManager.DocumentContainer.MergeEmptyDocumentContainers();

            Point currentMousePoint = Mouse.GetPosition(windowsManager);
            DockPane pane = dockPaneContent.DockPane;
            Canvas.SetLeft(pane, currentMousePoint.X - pane.ActualWidth / 2);
            Canvas.SetTop(pane, currentMousePoint.Y);
            windowsManager.AddFloatingWindow(pane);
        }

        /// <summary>
        /// Splits and adds document to the split window
        /// </summary>
        /// <param name="pane">The pane</param>
        /// <param name="dockPoint">The dock point</param>
        private void SplitAndAddDocument(DockPane pane, ContentDockPoint dockPoint)
        {
            DocumentContainer container = new DocumentContainer();
            Grid splitGrid = null;

            switch (dockPoint)
            {
                case ContentDockPoint.Top:
                    splitGrid = SplitVertically(false);
                    Grid.SetRow(container, 0);
                    break;
                case ContentDockPoint.Left:
                    splitGrid = SplitHorizontally(false);
                    Grid.SetColumn(container, 0);
                    break;
                case ContentDockPoint.Right:
                    splitGrid = SplitHorizontally(true);
                    Grid.SetColumn(container, 2);
                    break;
                case ContentDockPoint.Bottom:
                    splitGrid = SplitVertically(true);
                    Grid.SetRow(container, 2);
                    break;
                case ContentDockPoint.Content:
                    AddDocument(pane);
                    return;
                    break;
                default:
                    break;
            }

            container.AddDocument(pane);
            splitGrid.Children.Add(container);
        }

        /// <summary>
        /// Merges the empty document containers
        /// </summary>
        /// <returns>New content that should be child of parent DocumentContainer</returns>
        private object MergeEmptyDocumentContainers()
        {
            object newChild = null;
            switch (State)
            {
                case DocumentContainerState.ContainsDocuments:

                    if (Documents.Count == 0)
                    {
                        State = DocumentContainerState.Empty;
                    }
                    else
                    {
                        newChild = this;
                    }

                break;

                case DocumentContainerState.SplitHorizontally:
                case DocumentContainerState.SplitVertically:

                    Grid contentGrid = Content as Grid;

                    if (contentGrid != null)
                    {
                        DocumentContainer firstChild = contentGrid.Children[1] as DocumentContainer;
                        Validate.NotNull(firstChild, "firstChild");

                        DocumentContainer secondChild = contentGrid.Children[2] as DocumentContainer;
                        Validate.NotNull(secondChild, "secondChild");

                        object firstReduceResult = ReduceChild(firstChild);
                        object secondReduceResult = ReduceChild(secondChild);

                        if ((firstReduceResult != null) && (secondReduceResult != null))
                        {
                            newChild = this;
                        }
                        else if ((firstReduceResult == null) && (secondReduceResult == null))
                        {
                            Content = null;
                            State = DocumentContainerState.Empty;
                        }
                        else if (firstReduceResult != null)
                        {
                            secondChild.Clear();
                            newChild = this;
                        }
                        else /*if (secondReduceResult != null)*/
                        {
                            firstChild.Clear();
                            newChild = this;
                        }
                    }

                break;
            }

            return newChild;
        }

        /// <summary>
        /// Reduces the child element of a grid
        /// </summary>
        /// <param name="childElement">Child element.</param>
        /// <returns>Reduced child content</returns>
        private object ReduceChild(object childElement)
        {            
            if (childElement is DocumentContainer)
            {
                return (childElement as DocumentContainer).MergeEmptyDocumentContainers();
            }
            else            
            {
                return ReduceGrid(childElement as Grid);
            }            
        }

        /// <summary>
        /// Reduces the grid to minimal form
        /// </summary>
        /// <param name="contentGrid">Content grid to reduce</param>
        /// <returns>Null if grid has only splitter left; Grid itself otherwise</returns>
        private object ReduceGrid(Grid contentGrid)
        {
            object firstReduceResult = ReduceChild(contentGrid.Children[1]);
            object secondReduceResult = ReduceChild(contentGrid.Children[2]);

            if ((firstReduceResult != null) || (secondReduceResult != null))
            {
                return this;
            }

            return null;
        }

        /// <summary>
        /// Determines whether content can be merged at the specified dock point
        /// </summary>
        /// <param name="dockPoint">The dock point.</param>
        /// <param name="targetContainer">If merge can be performed, this is the target container for merge; otherwise this will be null</param>
        /// <returns>
        /// 	<c>true</c> if content can be merged at the specified dock point; otherwise, <c>false</c>.
        /// </returns>
        private bool CanMergeContent(ContentDockPoint dockPoint, out DocumentContainer targetContainer)
        {
            Grid contentGrid = Content as Grid;
            targetContainer = null;

            if (contentGrid == null)
            {
                return false;
            }

            foreach (object item in contentGrid.Children)
            {
                if (item is DocumentContainer)
                {
                    DocumentContainer container = item as DocumentContainer;

                    if (container.State == DocumentContainerState.Empty)
                    {
                        int column = Grid.GetColumn(container);
                        int row = Grid.GetRow(container);

                        if (((dockPoint == ContentDockPoint.Top) && (row == 0)) ||
                             ((dockPoint == ContentDockPoint.Bottom) && (row == 2)) ||
                             ((dockPoint == ContentDockPoint.Left) && (column == 0)) ||
                             ((dockPoint == ContentDockPoint.Right) && (column == 2)))
                        {
                            targetContainer = container;
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Splits the content horizontally
        /// </summary>
        /// <param name="contentIsInRightSplit">if set to <c>true</c>content is in right split.</param>
        /// <returns></returns>
        private Grid SplitHorizontally(bool contentIsInRightSplit)
        {
            Grid grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition());

            ColumnDefinition splitterColumnDefinition = new ColumnDefinition();
            splitterColumnDefinition.Width = GridLength.Auto;

            grid.ColumnDefinitions.Add(splitterColumnDefinition);
            grid.ColumnDefinitions.Add(new ColumnDefinition());

            GridSplitter splitter = new GridSplitter();
            splitter.VerticalAlignment = VerticalAlignment.Stretch;
            splitter.Width = 4;
            splitter.ResizeDirection = GridResizeDirection.Columns;
            splitter.ResizeBehavior = GridResizeBehavior.PreviousAndNext;
            Grid.SetColumn(splitter, 1);
            grid.Children.Add(splitter);

            UIElement content = Content as UIElement;

            if (content == null)
            {
                content = new DocumentContainer();
            }

            if (contentIsInRightSplit)
            {
                Grid.SetColumn(content, 0);
            }
            else
            {
                Grid.SetColumn(content, 2);
            }

            Content = grid;

            grid.Children.Add(content);

            State = DocumentContainerState.SplitHorizontally;

            return grid;
        }

        /// <summary>
        /// Splits the content vertically
        /// </summary>
        /// <param name="contentIsInTopSplit">if set to <c>true</c>content is in top split.</param>
        /// <returns></returns>
        private Grid SplitVertically(bool contentIsInTopSplit)
        {
            Grid grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition());

            RowDefinition splitterRowDefinition = new RowDefinition();
            splitterRowDefinition.Height = GridLength.Auto;

            grid.RowDefinitions.Add(splitterRowDefinition);
            grid.RowDefinitions.Add(new RowDefinition());

            GridSplitter splitter = new GridSplitter();
            splitter.HorizontalAlignment = HorizontalAlignment.Stretch;
            splitter.Height = 4;
            splitter.ResizeDirection = GridResizeDirection.Rows;
            splitter.ResizeBehavior = GridResizeBehavior.PreviousAndNext;
            Grid.SetRow(splitter, 1);
            grid.Children.Add(splitter);

            UIElement content = Content as UIElement;

            if (content == null)
            {
                content = new DocumentContainer();
            }

            if (contentIsInTopSplit)
            {
                Grid.SetRow(content, 0);
            }
            else
            {
                Grid.SetRow(content, 2);
            }

            Content = grid;

            grid.Children.Add(content);

            State = DocumentContainerState.SplitVertically;

            return grid;
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.MouseEnter"/> attached event is raised on this element. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseEventArgs"/> that contains the event data.</param>
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            
            if ((WindowsManager.ActiveWindowsManager != null) &&
                (WindowsManager.ActiveWindowsManager.DraggedPane != null) && 
                (e.LeftButton == MouseButtonState.Pressed) &&
                (DockPointsPanel != null))
            {
                DockPointsPanel.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.MouseLeave"/> attached event is raised on this element. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseEventArgs"/> that contains the event data.</param>
        protected override void OnMouseLeave(MouseEventArgs e)
        {
 	        base.OnMouseLeave(e);
            if (DockPointsPanel != null)
            {
                DockPointsPanel.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.MouseUp"/> routed event reaches an element in its route that is derived from this class. 
        /// Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs"/> that contains the event data. The event data reports that the mouse button was released.</param>
        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
            if (DockPointsPanel != null)
            {
                DockPointsPanel.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Gets or sets the drag start point.
        /// </summary>
        /// <value>The drag start point.</value>
        private Point DragStartPoint { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether drag has started
        /// </summary>
        /// <value><c>true</c> if drag has started; otherwise, <c>false</c>.</value>
        private bool DragStarted { get; set; }

        /// <summary>
        /// Gets or sets the remove document command.
        /// </summary>
        /// <value>The remove document command.</value>
        private ICommand RemoveDocumentCommand { get; set; }

        /// <summary>
        /// Dock points panel
        /// </summary>
        private Panel DockPointsPanel
        {
            get;
            set;
        }        
    }
}
