///
/// Copyright(C) MixModes Inc. 2010
/// 

using System.Windows;
using System.Windows.Controls;

namespace MixModes.Synergy.VisualFramework.Windows
{
    /// <summary>
    /// Dock illustration grid
    /// </summary>
    public class DockIllustrationGrid : Grid
    {
        /// <summary>
        /// Initializes the <see cref="DockIllustrationGrid"/> class.
        /// </summary>
        static DockIllustrationGrid()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DockIllustrationGrid), 
                                                     new FrameworkPropertyMetadata(typeof(DockIllustrationGrid)));
        }
    }
}
