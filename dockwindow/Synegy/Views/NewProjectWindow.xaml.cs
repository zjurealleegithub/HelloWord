///
/// Copyright(C) MixModes Inc. 2010
/// 

using MixModes.Synergy.VisualFramework.Views;
using System.Windows.Input;

namespace Synegy.Views
{
    /// <summary>
    /// Interaction logic for NewProjectWindow.xaml
    /// </summary>
    public partial class NewProjectWindow : DialogWindow
    {
        public NewProjectWindow()
        {
            InitializeComponent();
            FocusManager.SetFocusedElement(this, ProjectName);
        }
    }
}
