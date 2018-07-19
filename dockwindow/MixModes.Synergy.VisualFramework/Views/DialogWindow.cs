///
/// Copyright(C) MixModes Inc. 2010
/// 

using System;
using System.Windows;
using System.Windows.Interop;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace MixModes.Synergy.VisualFramework.Views
{
    /// <summary>
    /// A class that encapsulates the functionality for a Dialog window. It derives
    /// from the base WPF window and customizes the appearance to look like a typical
    /// Windows dialog.
    /// </summary>
    public class DialogWindow : Window
    {
        #region P/Invoke Methods
        [DllImport("user32.dll")]
        static extern int GetWindowLong(IntPtr hwnd, int index);

        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);

        [DllImport("user32.dll")]
        static extern bool SetWindowPos(IntPtr hwnd, IntPtr hwndInsertAfter, int x, int y, int width, int height, uint flags);

        [DllImport("user32.dll")]
        static extern IntPtr SendMessage(IntPtr hwnd, uint msg, IntPtr wParam, IntPtr lParam);

        const int GWL_EXSTYLE = -20;
        const int WS_EX_DLGMODALFRAME = 0x0001;
        const int SWP_NOSIZE = 0x0001;
        const int SWP_NOMOVE = 0x0002;
        const int SWP_NOZORDER = 0x0004;
        const int SWP_FRAMECHANGED = 0x0020;
        const uint WM_SETICON = 0x0080;
        #endregion

        /// <summary>
        /// Default constructor.
        /// </summary>
        public DialogWindow()
        {
            // Enable design time dialog properties
            // We need this condition since if window styles are set
            // after setting default dialog properties it has no effect
            // on the window and system icon is not hidden on the dialog
            if (DesignerProperties.GetIsInDesignMode(this))
            {
                SetDialogProperties();
            }
        }

        /// <summary>
        /// Overrides the base source initialization and sets the appropriate
        /// window styles for dialogs.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            SetWindowStyle();
        }

        /// <summary>
        /// Sets the appropriate window style for the dialog window based on the
        /// IsSystemMenuVisible and IsHelpButtonVisible properties.
        /// 
        /// Note: this makes several calls into native Windows methods to deliver
        /// this functionality. NativeMethods is a wrapper for native Windows 
        /// calls.
        /// </summary>
        protected virtual void SetWindowStyle()
        {
            // Get this window's handle
            IntPtr hwnd = new WindowInteropHelper(this).Handle;

            // Change the extended window style to not show a window icon
            int extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle | WS_EX_DLGMODALFRAME);

            // Update the window's non-client area to reflect the changes
            SetWindowPos(hwnd, IntPtr.Zero, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOZORDER | SWP_FRAMECHANGED);

            // Set default properties after setting window styles above
            // otherwise window icon won't be hidden
            ShowInTaskbar = false;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            ResizeMode = ResizeMode.NoResize;
            SizeToContent = SizeToContent.WidthAndHeight;

            // Set default properties after setting window styles above
            // otherwise window icon won't be hidden
            SetDialogProperties();
        }

        /// <summary>
        /// Sets the dialog properties.
        /// </summary>
        private void SetDialogProperties()
        {            
            ShowInTaskbar = false;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            ResizeMode = ResizeMode.NoResize;
            SizeToContent = SizeToContent.WidthAndHeight;
        }
    }
}
