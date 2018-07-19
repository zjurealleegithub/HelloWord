///
/// Copyright(C) MixModes Inc. 2010
/// 

namespace MixModes.Synergy.VisualFramework.Windows
{
    /// <summary>
    /// Document container state
    /// </summary>
    public enum DocumentContainerState
    {
        /// <summary>
        /// Document container is empty
        /// </summary>
        Empty,

        /// <summary>
        /// Document container contains one or more documents
        /// </summary>
        ContainsDocuments,

        /// <summary>
        /// Document container contains other document containers and is split horizontally
        /// </summary>
        SplitHorizontally,

        /// <summary>
        /// Document container contains other document containers and is split vertically
        /// </summary>
        SplitVertically
    }
}
