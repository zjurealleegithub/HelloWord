///
/// Copyright(C) MixModes Inc. 2010
/// 

using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Controls;
using System.Xml;
using MixModes.Synergy.Utilities;

namespace MixModes.Synergy.VisualFramework.Windows
{
    /// <summary>
    /// Xml window manager serializer
    /// </summary>
    public class XmlWindowsManagerSerializer : WindowsManagerSerializer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XmlWindowsManagerSerializer"/> class.
        /// </summary>
        /// <param name="dockPaneWriter">The dock pane writer.</param>
        /// <param name="documentWriter">The document writer.</param>
        public XmlWindowsManagerSerializer(Action<XmlElement, DockPane> dockPaneWriter, Func<DocumentContent, string> documentWriter)
        {
            Validate.NotNull(dockPaneWriter, "dockPaneWriter");
            Validate.NotNull(documentWriter, "documentWriter");
            _dockPaneWriter = dockPaneWriter;
            _documentWriter = documentWriter;
        }

        /// <summary>
        /// Initializes the stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        protected override void InitializeStream(Stream stream)
        {
            _stream = stream;
        }

        /// <summary>
        /// Writes the windows manager.
        /// </summary>
        /// <param name="windowsManager">The windows manager.</param>
        protected override void WriteWindowsManager(WindowsManager windowsManager)
        {
            XmlElement xmlElement = _document.CreateElement("WindowsManager");
            _document.AppendChild(xmlElement);
            _elementStack.Push(xmlElement);
        }

        /// <summary>
        /// Initializes the headers.
        /// </summary>
        protected override void InitializeHeaders()
        {
            ValidateStackNotEmpty();
            Validate.NotNull(_document.DocumentElement, "WindowsManager");
            XmlElement rootElement = _elementStack.Peek();
            Validate.Assert<InvalidOperationException>((rootElement == _document.DocumentElement) && (rootElement.Name == "WindowsManager"));
            XmlElement headersElement = _document.CreateElement("Headers");
            _elementStack.Push(headersElement);
            rootElement.AppendChild(headersElement);
        }

        /// <summary>
        /// Writes the header panes within a docked header
        /// </summary>
        /// <param name="panes">The panes.</param>
        /// <param name="headerDock">Header dock.</param>
        protected override void WriteHeaderPanes(IEnumerable<DockPane> panes, Dock headerDock)
        {
            ValidateStackNotEmpty();
            XmlElement headersElement = _elementStack.Peek();
            Validate.Assert<InvalidOperationException>(headersElement.Name == "Headers");
            XmlElement headerElement = _document.CreateElement("Header");
            headerElement.SetAttribute("Dock", Enum.GetName(typeof (Dock), headerDock));

            foreach (XmlElement dockElement in WriteDockPanes(panes))
            {
                headerElement.AppendChild(dockElement);
            }

            headersElement.AppendChild(headerElement);
        }

        /// <summary>
        /// Finalizes the headers.
        /// </summary>
        protected override void FinalizeHeaders()
        {
            FinalizeElement("Headers");
        }

        /// <summary>
        /// Initializes the pinned panes.
        /// </summary>
        protected override void InitializePinnedPanes()
        {
            ValidateStackNotEmpty();
            Validate.NotNull(_document.DocumentElement, "WindowsManager");
            XmlElement rootElement = _elementStack.Peek();
            Validate.Assert<InvalidOperationException>((rootElement == _document.DocumentElement) && (rootElement.Name == "WindowsManager"));
            XmlElement pinnedPanes = _document.CreateElement("PinnedPanes");
            _elementStack.Push(pinnedPanes);
            rootElement.AppendChild(pinnedPanes);
        }

        /// <summary>
        /// Writes the pinned panes.
        /// </summary>
        /// <param name="panes">The panes.</param>
        /// <param name="headerDock">The header dock.</param>
        protected override void WritePinnedPanes(IEnumerable<DockPane> panes, Dock headerDock)
        {
            ValidateStackNotEmpty();
            XmlElement pinnedPanesElement = _elementStack.Peek();
            Validate.Assert<InvalidOperationException>(pinnedPanesElement.Name == "PinnedPanes");
            XmlElement pinnedPaneElement = _document.CreateElement("PinnedPane");
            pinnedPaneElement.SetAttribute("Dock", Enum.GetName(typeof(Dock), headerDock));

            foreach (XmlElement dockElement in WriteDockPanes(panes))
            {
                pinnedPaneElement.AppendChild(dockElement);
            }

            pinnedPanesElement.AppendChild(pinnedPaneElement);
        }

        /// <summary>
        /// Finalizes the pinned panes.
        /// </summary>
        protected override void FinalizePinnedPanes()
        {
            FinalizeElement("PinnedPanes");
        }

        /// <summary>
        /// Writes the floating panes.
        /// </summary>
        /// <param name="floatingPanes">The floating panes.</param>
        protected override void WriteFloatingPanes(IEnumerable<DockPane> floatingPanes)
        {
            ValidateStackNotEmpty();
            Validate.NotNull(_document.DocumentElement, "WindowsManager");
            XmlElement rootElement = _elementStack.Peek();
            Validate.Assert<InvalidOperationException>((rootElement == _document.DocumentElement) && (rootElement.Name == "WindowsManager"));
            XmlElement floatingWindows = _document.CreateElement("FloatingWindows");
            rootElement.AppendChild(floatingWindows);

            foreach (XmlElement floatingWindow in WriteDockPanes(floatingPanes))
            {
                floatingWindows.AppendChild(floatingWindow);
            }
        }

        /// <summary>
        /// Initializes the document container.
        /// </summary>
        /// <param name="documentContainer">The document container.</param>
        protected override void InitializeDocumentContainer(DocumentContainer documentContainer)
        {
            ValidateStackNotEmpty();
            XmlElement documentContainerElement = _document.CreateElement("DocumentContainer");
            documentContainerElement.SetAttribute("State", Enum.GetName(typeof(DocumentContainerState), documentContainer.State));
            documentContainerElement.SetAttribute("Row", Grid.GetRow(documentContainer).ToString());
            documentContainerElement.SetAttribute("Column", Grid.GetColumn(documentContainer).ToString());
            _elementStack.Peek().AppendChild(documentContainerElement);
            _elementStack.Push(documentContainerElement);
        }

        /// <summary>
        /// Writes the documents within a document container
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="documents">The documents.</param>
        protected override void WriteDocuments(DocumentContainer container, IEnumerable<DocumentContent> documents)
        {
            ValidateStackNotEmpty();
            XmlElement documentContainerElement = _elementStack.Peek();
            Validate.Assert<InvalidOperationException>(documentContainerElement.Name == "DocumentContainer");
            foreach (DocumentContent document in documents)
            {
                XmlElement documentElement = _document.CreateElement("Document");
                documentElement.SetAttribute("Data", _documentWriter(document));
                documentContainerElement.AppendChild(documentElement);
            }
        }

        /// <summary>
        /// Initializes the split.
        /// </summary>
        protected override void InitializeSplit()
        {
            ValidateStackNotEmpty();
            XmlElement splitElement = _document.CreateElement("Split");
            _elementStack.Peek().AppendChild(splitElement);
            _elementStack.Push(splitElement);
        }

        /// <summary>
        /// Finalizes the split.
        /// </summary>
        protected override void FinalizeSplit()
        {
            FinalizeElement("Split");
        }

        /// <summary>
        /// Finalizes the document container.
        /// </summary>
        /// <param name="documentContainer">The document container.</param>
        protected override void FinalizeDocumentContainer(DocumentContainer documentContainer)
        {
            FinalizeElement("DocumentContainer");
        }

        /// <summary>
        /// Finalizes the serialization.
        /// </summary>
        protected override void FinalizeSerialization()
        {
            _document.Save(_stream);
        }

        /// <summary>
        /// Writes the dock panes.
        /// </summary>
        /// <param name="dockPanes">The dock panes.</param>
        /// <returns>XMLElement(s) corresponding to the dock panes in order</returns>
        private IEnumerable<XmlElement> WriteDockPanes(IEnumerable<DockPane> dockPanes)
        {
            foreach (DockPane dockPane in dockPanes)
            {
                yield return WriteDockPane(dockPane);
            }
        }

        /// <summary>
        /// Writes the dock pane.
        /// </summary>
        /// <param name="dockPane">The dock pane.</param>
        /// <returns>XMLElement corresponding to the dock pane</returns>
        private XmlElement WriteDockPane(DockPane dockPane)
        {
            XmlElement dockPaneElement = _document.CreateElement("DockPane");
            dockPaneElement.SetAttribute("Height", dockPane.ActualHeight.ToString());
            dockPaneElement.SetAttribute("Width", dockPane.ActualWidth.ToString());

            if (dockPane.DockPaneState == DockPaneState.Floating)
            {
                dockPaneElement.SetAttribute("Top", Canvas.GetTop(dockPane).ToString());
                dockPaneElement.SetAttribute("Left", Canvas.GetLeft(dockPane).ToString());
            }

            _dockPaneWriter(dockPaneElement, dockPane);
            return dockPaneElement;
        }

        /// <summary>
        /// Validates that the stack is not empty.
        /// </summary>
        private void ValidateStackNotEmpty()
        {
            Validate.Assert<InvalidOperationException>(_elementStack.Count > 0);
        }

        /// <summary>
        /// Finalizes the element.
        /// </summary>
        /// <param name="elementName">Name of the element.</param>
        /// <exception cref="InvalidOperationException">Top element on the stack does not match element name</exception>
        private void FinalizeElement(string elementName)
        {
            ValidateStackNotEmpty();
            Validate.Assert<InvalidOperationException>(_elementStack.Peek().Name == elementName);
            _elementStack.Pop();
        }

        // Private members
        private readonly XmlDocument _document = new XmlDocument();
        private Stream _stream;
        private readonly Stack<XmlElement> _elementStack = new Stack<XmlElement>();
        private readonly Action<XmlElement, DockPane> _dockPaneWriter;
        private readonly Func<DocumentContent, string> _documentWriter;
    }
}
