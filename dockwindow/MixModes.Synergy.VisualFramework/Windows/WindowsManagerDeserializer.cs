///
/// Copyright(C) MixModes Inc. 2010
/// 

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MixModes.Synergy.Utilities;
using MixModes.Synergy.VisualFramework.Windows;

namespace MixModes.Synergy.VisualFramework.Windows
{
    /// <summary>
    /// Deserializes a window manager
    /// </summary>
    /// <remarks>Deserialization should be atomic operation and must not leave windows manager is an unstable state</remarks>
    public abstract class WindowsManagerDeserializer
    {
        /// <summary>
        /// Deserializes the specified windows manager from the stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="windowsManager">The windows manager.</param>
        /// <exception cref="ArgumentNullException">stream or windowsManager are null</exception>
        /// <exception cref="InvalidOperationException">stream is not readable</exception>
        public void Deserialize(Stream stream, WindowsManager windowsManager)
        {
            Validate.NotNull(stream, "stream");
            Validate.NotNull(windowsManager, "windowsManager");
            Validate.Assert<InvalidOperationException>(stream.CanRead);

            _dockedWindows = new Dictionary<Dock, DockedWindows>();
            _floatingWindows = new List<DockPane>();
            _rootContainer = null;
            DockPositions.ForEach(dock => _dockedWindows[dock] = new DockedWindows());

            // Initialize stream
            InitializeStream(stream);

            // Navigate windows manager
            NavigateWindowsManager();

            // Finalize deserialization
            FinalizeDeserialization();

            // Transfer contents to windows manager
            TransferWindowsManagerContents(windowsManager);
        }

        /// <summary>
        /// Reads the windows manager.
        /// </summary>
        protected abstract void ReadWindowsManager();

        /// <summary>
        /// Initializes the stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        protected abstract void InitializeStream(Stream stream);

        /// <summary>
        /// Finalizes the deserialization.
        /// </summary>
        protected abstract void FinalizeDeserialization();

        /// <summary>
        /// Reads the header panes.
        /// </summary>
        /// <param name="dock">Dock point</param>
        /// <returns>DockPanes in order within header pane with specified dock point</returns>
        protected abstract IEnumerable<DockPane> ReadHeaderPanes(Dock dock);

        /// <summary>
        /// Reads the pinned panes.
        /// </summary>
        /// <param name="dock">Dock point</param>
        /// <returns>DockPanes in order within pinned pane with specified dock point</returns>
        protected abstract IEnumerable<DockPane> ReadPinnedPanes(Dock dock);

        /// <summary>
        /// Reads the floating panes.
        /// </summary>
        /// <returns>Floating panes</returns>
        protected abstract IEnumerable<DockPane> ReadFloatingPanes();

        /// <summary>
        /// Reads the root document container and sets the State as well as dimensions for read DocumentContainer
        /// </summary>
        /// <returns>Read document container</returns>
        protected abstract DocumentContainer ReadRootDocumentContainer();

        /// <summary>
        /// Initializes the document container.
        /// </summary>
        /// <param name="documentContainer">The document container.</param>
        /// <returns>Document container state</returns>
        protected abstract DocumentContainerState InitializeDocumentContainer(DocumentContainer documentContainer);

        /// <summary>
        /// Finalizes the document container.
        /// </summary>
        protected abstract void FinalizeDocumentContainer();

        /// <summary>
        /// Reads the documents for current document container
        /// </summary>
        /// <param name="documentContainer">The document container.</param>
        protected abstract void ReadDocuments(DocumentContainer documentContainer);

        /// <summary>
        /// Initializes the split.
        /// </summary>
        /// <param name="parentContainer">The parent container.</param>
        protected abstract void InitializeSplit(DocumentContainer parentContainer);

        /// <summary>
        /// Finalizes the split.
        /// </summary>
        protected abstract void FinalizeSplit();

        /// <summary>
        /// Reads the document containers within current split and sets the State as well as dimensions for read DocumentContainer(s)
        /// </summary>
        /// <returns>Read document containers</returns>
        protected abstract IEnumerable<DocumentContainer> ReadDocumentContainers();

        /// <summary>
        /// Navigates the windows manager.
        /// </summary>
        private void NavigateWindowsManager()
        {
            // Read windows manager itself
            ReadWindowsManager();

            // Read header panes
            ReadHeaderPanes();

            // Read pinned panes
            ReadPinnedPanes();

            // Read floating panes
            ReadFloatingPanesBase();

            // Read document container
            NavigateDocumentContainers();
        }

        /// <summary>
        /// Reads the header panes.
        /// </summary>
        private void ReadHeaderPanes()
        {
            foreach (Dock dock in DockPositions)
            {
                DockedWindows dockedWindows = _dockedWindows[dock];
                foreach (DockPane dockpane in ReadHeaderPanes(dock))
                {                    
                    dockedWindows.AutoHidePanes.Add(dockpane);
                }
            }
        }

        /// <summary>
        /// Reads the pinned panes.
        /// </summary>
        private void ReadPinnedPanes()
        {
            foreach (Dock dock in DockPositions)
            {
                DockedWindows dockedWindows = _dockedWindows[dock];
                foreach (DockPane dockpane in ReadPinnedPanes(dock))
                {                    
                    dockedWindows.PinnedPanes.Add(dockpane);
                }
            }
        }

        /// <summary>
        /// Reads the floating panes base.
        /// </summary>
        private void ReadFloatingPanesBase()
        {
            foreach (DockPane pane in ReadFloatingPanes())
            {
                _floatingWindows.Add(pane);
            }
        }

        /// <summary>
        /// Navigates the document containers.
        /// </summary>
        private void NavigateDocumentContainers()
        {
            _rootContainer = ReadRootDocumentContainer();
            NavigateDocumentContainer(_rootContainer);            
        }

        /// <summary>
        /// Navigates the document container
        /// </summary>
        /// <param name="documentContainer">The document container.</param>
        private void NavigateDocumentContainer(DocumentContainer documentContainer)
        {
            DocumentContainerState state = InitializeDocumentContainer(documentContainer);

            switch (state)
            {
                case DocumentContainerState.Empty:
                    // Do Nothing
                    break;
                case DocumentContainerState.ContainsDocuments:
                    ReadDocuments(documentContainer);
                    break;
                case DocumentContainerState.SplitHorizontally:
                    NavigateDocumentGrid(documentContainer, true);
                    break;
                case DocumentContainerState.SplitVertically:
                    NavigateDocumentGrid(documentContainer, false);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            FinalizeDocumentContainer();
        }

        /// <summary>
        /// Navigates the document grid.
        /// </summary>
        /// <param name="parentContainer">The parent container.</param>
        /// <param name="isHorizontal">if set to <c>true</c> orientation is horizontal otherwise orientation is vertical</param>
        private void NavigateDocumentGrid(DocumentContainer parentContainer, bool isHorizontal)
        {
            InitializeSplit(parentContainer);

            List<DocumentContainer> childContainers = new List<DocumentContainer>();

            foreach (DocumentContainer documentContainer in ReadDocumentContainers())
            {
                NavigateDocumentContainer(documentContainer);
                childContainers.Add(documentContainer);                
            }

            parentContainer.AddDocumentContainers(childContainers, isHorizontal);

            FinalizeSplit();
        }

        /// <summary>
        /// Transfers the windows manager contents after desealization has finished
        /// </summary>
        /// <param name="windowsManager">The windows manager.</param>
        private void TransferWindowsManagerContents(WindowsManager windowsManager)
        {
            windowsManager.Clear();

            // Transfer auto hide and pinned windows for all dock points);
            foreach (Dock dockPosition in DockPositions)
            {
                DockedWindows dockedWindows = _dockedWindows[dockPosition];
                
                foreach (DockPane pinnedPane in dockedWindows.PinnedPanes)
                {
                    windowsManager.AddPinnedWindow(pinnedPane, dockPosition);
                }

                foreach (DockPane autoHidePane in dockedWindows.AutoHidePanes)
                {
                    windowsManager.AddAutoHideWindow(autoHidePane, dockPosition);
                }
            }

            // Transfer floating windows
            foreach(DockPane floatingPane in _floatingWindows)
            {
                windowsManager.AddFloatingWindow(floatingPane);
            }

            // Transfer document content
            switch (_rootContainer.State)
            {
                case DocumentContainerState.Empty:
                    break;
                case DocumentContainerState.ContainsDocuments:
                    List<object> documents = new List<object>(_rootContainer.Documents);
                    _rootContainer.Clear();
                    foreach (object document in documents)
                    {
                        if (document is DocumentContent)
                        {
                            DocumentContent documentContent = (document as DocumentContent);
                            documentContent.DetachDockPane();
                            windowsManager.DocumentContainer.AddDocument(documentContent.DockPane);                                   
                        }
                    }
                    break;
                case DocumentContainerState.SplitHorizontally:
                    TransferDocumentGrid(windowsManager, true);
                    break;
                case DocumentContainerState.SplitVertically:
                    TransferDocumentGrid(windowsManager, false);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Transfers the document grid from _rootContainer to the main document container of specified windows manager
        /// </summary>
        /// <param name="windowsManager">The windows manager whose document content is the target of transferred child contents from _rootContainer</param>
        /// <param name="isHorizontal">if set to <c>true</c> indicates horizontal orientation otherwise vertical orientation</param>
        private void TransferDocumentGrid(WindowsManager windowsManager, bool isHorizontal)
        {
            Grid contentGrid = _rootContainer.Content as Grid;
            Validate.Assert<ArgumentNullException>(contentGrid != null);

            _rootContainer.Clear();
            List<DocumentContainer> documentContainers = new List<DocumentContainer>(contentGrid.Children.OfType<DocumentContainer>());
            contentGrid.Children.Clear();

            windowsManager.DocumentContainer.AddDocumentContainers(documentContainers, isHorizontal);
        }

        /// <summary>
        /// Gets the dock positions.
        /// </summary>
        /// <value>The dock positions.</value>
        private List<Dock> DockPositions
        {
            get { return new List<Dock>(new[] {Dock.Left, Dock.Top, Dock.Right, Dock.Bottom}); }
        }

        /// <summary>
        /// Docked Windows
        /// </summary>
        private class DockedWindows
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="DockedWindows"/> class.
            /// </summary>
            public DockedWindows()
            {
                PinnedPanes = new List<DockPane>();
                AutoHidePanes = new List<DockPane>();
            }

            /// <summary>
            /// Pinned Panes
            /// </summary>
            public readonly List<DockPane> PinnedPanes;

            /// <summary>
            /// Auto hide panes
            /// </summary>
            public readonly List<DockPane> AutoHidePanes;
        }

        // Private members
        private Dictionary<Dock, DockedWindows> _dockedWindows;
        private List<DockPane> _floatingWindows;
        private DocumentContainer _rootContainer;
    }
}
