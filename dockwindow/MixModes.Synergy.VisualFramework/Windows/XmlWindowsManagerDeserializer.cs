///
/// Copyright(C) MixModes Inc. 2010
/// 

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Xml;
using MixModes.Synergy.Utilities;

namespace MixModes.Synergy.VisualFramework.Windows
{
    /// <summary>
    /// Xml window manager deserializer
    /// </summary>
    public class XmlWindowsManagerDeserializer : WindowsManagerDeserializer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XmlWindowsManagerDeserializer"/> class.
        /// </summary>
        public XmlWindowsManagerDeserializer(Action<DockPane, string> dockPaneReader)
        {
            Validate.NotNull(dockPaneReader, "dockPaneReader");
            _dockPaneReader = dockPaneReader;
        }

        /// <summary>
        /// Reads the windows manager.
        /// </summary>
        protected override void ReadWindowsManager()
        {
            XmlElement windowsManagerElement = _document.DocumentElement;
            Validate.Assert<NullReferenceException>(windowsManagerElement != null);
            Validate.Assert<InvalidOperationException>(windowsManagerElement.Name == "WindowsManager");
            _elementStack.Push(windowsManagerElement);
        }

        /// <summary>
        /// Initializes the stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        protected override void InitializeStream(Stream stream)
        {
            _document.Load(stream);
        }

        /// <summary>
        /// Finalizes the deserialization.
        /// </summary>
        protected override void FinalizeDeserialization()
        {
            _elementStack.Pop();
        }

        /// <summary>
        /// Reads the header panes.
        /// </summary>
        /// <param name="dock">Dock point</param>
        /// <returns>DockPanes in order within header pane with specified dock point</returns>
        protected override IEnumerable<DockPane> ReadHeaderPanes(Dock dock)
        {
            XmlElement rootElement = _elementStack.Peek();
            Validate.Assert<InvalidOperationException>(rootElement.Name == "WindowsManager");
            XmlNodeList dockPaneList = rootElement.SelectNodes(string.Format(@"Headers/Header[@Dock='{0}']/DockPane", Enum.GetName(typeof (Dock), dock)));
            foreach (XmlElement dockPaneElement in dockPaneList.OfType<XmlElement>())
            {
                yield return ReadDockPane(dockPaneElement);
            }
        }
       
        /// <summary>
        /// Reads the pinned panes.
        /// </summary>
        /// <param name="dock">Dock point</param>
        /// <returns>DockPanes in order within pinned pane with specified dock point</returns>
        protected override IEnumerable<DockPane> ReadPinnedPanes(Dock dock)
        {
            XmlElement rootElement = _elementStack.Peek();
            Validate.Assert<InvalidOperationException>(rootElement.Name == "WindowsManager");
            XmlNodeList dockPaneList = rootElement.SelectNodes(string.Format(@"PinnedPanes/PinnedPane[@Dock='{0}']/DockPane", Enum.GetName(typeof(Dock), dock)));
            foreach (XmlElement dockPaneElement in dockPaneList.OfType<XmlElement>())
            {
                yield return ReadDockPane(dockPaneElement);
            }
        }

        /// <summary>
        /// Reads the floating panes.
        /// </summary>
        /// <returns>Floating panes</returns>
        protected override IEnumerable<DockPane> ReadFloatingPanes()
        {
            XmlElement rootElement = _elementStack.Peek();
            Validate.Assert<InvalidOperationException>(rootElement.Name == "WindowsManager");
            XmlNodeList dockPaneList = rootElement.SelectNodes(@"FloatingWindows/DockPane");
            foreach (XmlElement dockPaneElement in dockPaneList.OfType<XmlElement>())
            {
                yield return ReadDockPane(dockPaneElement);
            }
        }

        /// <summary>
        /// Reads the root document container and sets the State as well as dimensions for read DocumentContainer
        /// </summary>
        /// <returns>Read document container</returns>
        protected override DocumentContainer ReadRootDocumentContainer()
        {
            XmlElement rootElement = _elementStack.Peek();
            Validate.Assert<InvalidOperationException>(rootElement.Name == "WindowsManager");
            return ReadDocumentContainers().First();
        }

        /// <summary>
        /// Initializes the document container.
        /// </summary>
        /// <param name="documentContainer">The document container.</param>
        /// <returns>Document container state</returns>
        protected override DocumentContainerState InitializeDocumentContainer(DocumentContainer documentContainer)
        {
            ValidateStackNotEmpty();
            
            XmlElement documentContainerElement;
            Validate.Assert<InvalidOperationException>(_documentContainerToDefinitionMapping.TryGetValue(documentContainer, out documentContainerElement) && 
                                                       (documentContainerElement.Name == "DocumentContainer"));

            Grid.SetRow(documentContainer, int.Parse(documentContainerElement.GetAttribute("Row")));
            Grid.SetColumn(documentContainer, int.Parse(documentContainerElement.GetAttribute("Column")));
            
            _elementStack.Push(documentContainerElement);
            
            return (DocumentContainerState)Enum.Parse(typeof(DocumentContainerState), documentContainerElement.GetAttribute("State"));
        }

        /// <summary>
        /// Finalizes the document container.
        /// </summary>
        protected override void FinalizeDocumentContainer()
        {
            FinalizeElement("DocumentContainer");
        }

        /// <summary>
        /// Reads the documents for current document container
        /// </summary>
        /// <param name="documentContainer">The document container.</param>
        protected override void ReadDocuments(DocumentContainer documentContainer)
        {
            ValidateStackNotEmpty();

            XmlElement documentContainerElement = _elementStack.Peek();
            Validate.Assert<InvalidOperationException>(documentContainerElement.Name == "DocumentContainer");

            foreach (XmlElement documentElement in documentContainerElement.SelectNodes(@"Document").OfType<XmlElement>())
            {
                DockPane dockPane = new DockPane();
                _dockPaneReader(dockPane, documentElement.GetAttribute("Data"));
                documentContainer.AddDocument(dockPane);
            }
        }

        /// <summary>
        /// Initializes the split.
        /// </summary>
        /// <param name="parentContainer">The parent container.</param>
        protected override void InitializeSplit(DocumentContainer parentContainer)
        {
            ValidateStackNotEmpty();

            XmlElement documentContainerElement = _elementStack.Peek();
            Validate.Assert<InvalidOperationException>(documentContainerElement.Name == "DocumentContainer");

            XmlElement splitElement = documentContainerElement.FirstChild as XmlElement;
            Validate.Assert<NullReferenceException>(splitElement != null);
            Validate.Assert<InvalidOperationException>(splitElement.Name == "Split");

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
        /// Reads the document containers within current split and sets the State as well as dimensions for read DocumentContainer(s)
        /// </summary>
        /// <returns>Read document containers</returns>
        protected override IEnumerable<DocumentContainer> ReadDocumentContainers()
        {
            ValidateStackNotEmpty();
            XmlElement parentElement = _elementStack.Peek();
            foreach(XmlElement documentContainer in parentElement.SelectNodes("DocumentContainer").OfType<XmlElement>())
            {
                DocumentContainer container = new DocumentContainer();
                _documentContainerToDefinitionMapping[container] = documentContainer;
                yield return container;
            }
        }

        /// <summary>
        /// Reads the dock pane from the xml element
        /// </summary>
        /// <param name="dockPaneElement">The dock pane xml element.</param>
        /// <returns>Newly constructed DockPane</returns>
        private DockPane ReadDockPane(XmlElement dockPaneElement)
        {
            DockPane dockPane = new DockPane();            
            _dockPaneReader(dockPane, dockPaneElement.GetAttribute("Data"));

            double height = double.Parse(dockPaneElement.GetAttribute("Height"));
            dockPane.Height = height;

            double width = double.Parse(dockPaneElement.GetAttribute("Width"));
            dockPane.Width = width;

            XmlAttribute topAttribute = dockPaneElement.Attributes.GetNamedItem("Top") as XmlAttribute;
            if (topAttribute != null)
            {
                Canvas.SetTop(dockPane, double.Parse(topAttribute.Value));
            }

            XmlAttribute leftAttribute = dockPaneElement.Attributes.GetNamedItem("Left") as XmlAttribute;
            if (leftAttribute != null)
            {
                Canvas.SetLeft(dockPane, double.Parse(leftAttribute.Value));
            }

            return dockPane;
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
        private readonly Action<DockPane, string> _dockPaneReader;
        private readonly Func<string, DocumentContent> _documentReader;
        private readonly Stack<XmlElement> _elementStack = new Stack<XmlElement>();
        private readonly XmlDocument _document = new XmlDocument();
        private readonly Dictionary<DocumentContainer, XmlElement> _documentContainerToDefinitionMapping = new Dictionary<DocumentContainer, XmlElement>();
    }
}
