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

namespace MixModes.Synergy.VisualFramework.Windows
{
    /// <summary>
    /// Serializes a window manager
    /// </summary>
    public abstract class WindowsManagerSerializer
    {
        /// <summary>
        /// Serializes the specified windows manager to the stream
        /// </summary>
        /// <remarks>Caller must be responsible for closing the stream</remarks>
        /// <param name="stream">Stream to serialize contents of windows manager to</param>
        /// <param name="windowsManager">Windows manager to serialize</param>
        /// <exception cref="ArgumentNullException">stream or windowsManager are null</exception>
        /// <exception cref="InvalidOperationException">stream is not writable</exception>
        public void Serialize(Stream stream, WindowsManager windowsManager)
        {
            Validate.NotNull(stream, "stream");
            Validate.NotNull(windowsManager, "windowsManager");
            Validate.Assert<InvalidOperationException>(stream.CanWrite);

            InitializeStream(stream);
            NavigateWindowsManager(windowsManager);
        }

        /// <summary>
        /// Initializes the stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        protected abstract void InitializeStream(Stream stream);

        /// <summary>
        /// Writes the windows manager.
        /// </summary>
        /// <param name="windowsManager">The windows manager.</param>
        protected abstract void WriteWindowsManager(WindowsManager windowsManager);

        /// <summary>
        /// Initializes the headers.
        /// </summary>
        protected abstract void InitializeHeaders();

        /// <summary>
        /// Writes the header panes within a docked header
        /// </summary>
        /// <param name="panes">The panes.</param>
        /// <param name="headerDock">Header dock.</param>
        protected abstract void WriteHeaderPanes(IEnumerable<DockPane> panes, Dock headerDock);

        /// <summary>
        /// Finalizes the headers.
        /// </summary>
        protected abstract void FinalizeHeaders();

        /// <summary>
        /// Initializes the pinned panes.
        /// </summary>
        protected abstract void InitializePinnedPanes();

        /// <summary>
        /// Writes the pinned panes.
        /// </summary>
        /// <param name="panes">The panes.</param>
        /// <param name="headerDock">The header dock.</param>
        protected abstract void WritePinnedPanes(IEnumerable<DockPane> panes, Dock headerDock);

        /// <summary>
        /// Finalizes the pinned panes.
        /// </summary>
        protected abstract void FinalizePinnedPanes();

        /// <summary>
        /// Writes the floating panes.
        /// </summary>
        /// <param name="floatingPanes">The floating panes.</param>
        protected abstract void WriteFloatingPanes(IEnumerable<DockPane> floatingPanes);

        /// <summary>
        /// Initializes the document container.
        /// </summary>
        /// <param name="documentContainer">The document container.</param>
        protected abstract void InitializeDocumentContainer(DocumentContainer documentContainer);

        /// <summary>
        /// Writes the documents within a document container
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="documents">The documents.</param>
        protected abstract void WriteDocuments(DocumentContainer container, IEnumerable<DocumentContent> documents);

        /// <summary>
        /// Initializes the split.
        /// </summary>
        protected abstract void InitializeSplit();

        /// <summary>
        /// Finalizes the split.
        /// </summary>
        protected abstract void FinalizeSplit();

        /// <summary>
        /// Finalizes the document container.
        /// </summary>
        /// <param name="documentContainer">The document container.</param>
        protected abstract void FinalizeDocumentContainer(DocumentContainer documentContainer);

        /// <summary>
        /// Finalizes the serialization.
        /// </summary>
        protected abstract void FinalizeSerialization();

        /// <summary>
        /// Navigates the windows manager and serializes content
        /// </summary>
        /// <param name="windowsManager">The windows manager.</param>
        private void NavigateWindowsManager(WindowsManager windowsManager)
        {
            // Write windows manager itself
            WriteWindowsManager(windowsManager);

            // Write header panes
            WriteHeaderPanes(windowsManager);

            // Write pinned panes
            WritePinnedPanes(windowsManager);

            // Write floating panes
            WriteFloatingPanes(windowsManager);

            // Write document container
            WriteDocumentContainers(windowsManager.DocumentContainer);

            FinalizeSerialization();
        }

        /// <summary>
        /// Writes the header panes
        /// </summary>
        /// <param name="windowsManager">The windows manager.</param>
        private void WriteHeaderPanes(WindowsManager windowsManager)
        {
            InitializeHeaders();
            Dock[] docks = new[] { Dock.Left, Dock.Top, Dock.Right, Dock.Bottom };

            foreach (Dock dock in docks)
            {
                WriteHeaderPanes(GetDockPanelFromHeaders(windowsManager, dock), dock);
            }
            FinalizeHeaders();
        }

        /// <summary>
        /// Gets the dock panel from headers.
        /// </summary>
        /// <param name="windowsManager">The windows manager.</param>
        /// <param name="headerDock">The header dock.</param>
        /// <returns></returns>
        private IEnumerable<DockPane> GetDockPanelFromHeaders(WindowsManager windowsManager, Dock headerDock)
        {
            StackPanel headerHost;

            switch (headerDock)
            {
                case Dock.Top:
                    headerHost = windowsManager.TopWindowHeaders;
                    break;

                case Dock.Bottom:
                    headerHost = windowsManager.BottomWindowHeaders;
                    break;

                case Dock.Left:
                    headerHost = windowsManager.LeftWindowHeaders;
                    break;

                default:
                    headerHost = windowsManager.RightWindowHeaders;
                    break;
            }

            return (from FrameworkElement element in headerHost.Children select element.DataContext).OfType<DockPane>().OrderBy(Panel.GetZIndex);
        }

        private void WritePinnedPanes(WindowsManager windowsManager)
        {
            InitializePinnedPanes();
            Dock[] docks = new[] { Dock.Left, Dock.Top, Dock.Right, Dock.Bottom };

            foreach (Dock dock in docks)
            {
                WritePinnedPanes(GetPinnedDockPanes(windowsManager, dock), dock);
            }
            FinalizePinnedPanes();
        }

        private IEnumerable<DockPane> GetPinnedDockPanes(WindowsManager windowsManager, Dock dock)
        {
            DockPanel dockPanel;

            switch (dock)
            {
                case Dock.Top:
                    dockPanel = windowsManager.TopPinnedWindows;
                    break;

                case Dock.Bottom:
                    dockPanel = windowsManager.BottomPinnedWindows;
                    break;

                case Dock.Left:
                    dockPanel = windowsManager.LeftPinnedWindows;
                    break;

                default:
                    dockPanel = windowsManager.RightPinnedWindows;
                    break;
            }

            return (from FrameworkElement element in dockPanel.Children select element).OfType<DockPane>().OrderBy(Panel.GetZIndex);
        }

        private void WriteFloatingPanes(WindowsManager windowsManager)
        {
            IEnumerable<DockPane> floatingPanes = (from FrameworkElement element in windowsManager.FloatingPanel.Children select element).OfType<DockPane>();
            WriteFloatingPanes(floatingPanes);
        }

        private void WriteDocumentContainers(DocumentContainer documentContainer)
        {
            NavigateDocumentContainer(documentContainer);
        }

        private void NavigateDocumentContainer(DocumentContainer documentContainer)
        {           
            InitializeDocumentContainer(documentContainer);

            switch (documentContainer.State)
            {
                case DocumentContainerState.Empty:
                    // Do nothing
                    break;
                case DocumentContainerState.ContainsDocuments:
                    WriteDocuments(documentContainer, documentContainer.Documents.OfType<DocumentContent>());
                    break;
                case DocumentContainerState.SplitHorizontally:
                    NavigateDocumentGrid(documentContainer.Content as Grid);
                    break;
                case DocumentContainerState.SplitVertically:
                    NavigateDocumentGrid(documentContainer.Content as Grid);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            FinalizeDocumentContainer(documentContainer);
        }

        private void NavigateDocumentGrid(Grid grid)
        {            
            InitializeSplit();
            for (int i = 0; i < grid.Children.Count; i++)
            {
                FrameworkElement childElement = grid.Children[i] as FrameworkElement;
                if (childElement is DocumentContainer)
                {
                    NavigateDocumentContainer(childElement as DocumentContainer);
                }
            }
            FinalizeSplit();
        }       
    }
}
