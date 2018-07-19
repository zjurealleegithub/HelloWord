///
/// Copyright(C) MixModes Inc. 2010
/// 

using System.Windows.Input;
using MixModes.Synergy.VisualFramework.Commands;

namespace MixModes.Synergy.VisualFramework.Windows
{
    /// <summary>
    /// Document content
    /// </summary>
    public class DocumentContent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentContent"/> class.
        /// </summary>
        /// <param name="pane">The pane.</param>
        /// <param name="closeCommand">The close command.</param>
        public DocumentContent(DockPane pane, ICommand closeCommand)
        {
            Header = pane.Header;
            Content = pane.Content;
            DockPane = pane;
            CloseCommand = closeCommand;
            pane.Header = null;
            pane.Content = null;
        }

        /// <summary>
        /// Detaches the dock pane.
        /// </summary>
        public void DetachDockPane()
        {
            DockPane.Header = Header;
            DockPane.Content = Content;
        }

        /// <summary>
        /// Gets or sets the header.
        /// </summary>
        /// <value>The header.</value>
        public object Header { get; private set; }

        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        /// <value>The content.</value>
        public object Content { get; private set; }

        /// <summary>
        /// Gets or sets the dock pane.
        /// </summary>
        /// <value>The dock pane.</value>
        public DockPane DockPane { get; private set; }

        /// <summary>
        /// Gets or sets the close command.
        /// </summary>
        /// <value>The close command.</value>
        public ICommand CloseCommand { get; private set; }
    }
}
