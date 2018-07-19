///
/// Copyright(C) MixModes Inc. 2010
/// 

using System.IO;
using MixModes.Synergy.VisualFramework.Windows;
using System.Windows.Controls;
using System.Windows.Media;
namespace Synegy
{
    /// <summary>
    /// Main Window
    /// </summary>
    public partial class MainWindow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();           
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            DockPane pane = new DockPane();
            pane.MinHeight = 24;
            pane.MinWidth = 100;
            pane.Header = "Solution Explorer";
            Grid g = new Grid();
            g.Background = Brushes.White;
            TextBlock text = new TextBlock();
            text.Text = "Some content - " + (i + 1);
            g.Children.Add(text);
            pane.Content = g;
            Dock direction = Dock.Top;
            if (i % 4 == 1)
                direction = Dock.Right;
            else if (i % 4 == 2)
                direction = Dock.Left;
            else if (i % 4 == 3)
                direction = Dock.Bottom;
            i++;

            pane.Tag = i;

            pane.MouseEnter += (a, b) =>
                {
                    var x = WindowsManager;
                    System.Diagnostics.Debug.WriteLine((a as DockPane).Tag);
                };

            WindowsManager.AddPinnedWindow(pane, direction);            
        }

        private void Button_Save(object sender, System.Windows.RoutedEventArgs e)
        {            
            _stream.SetLength(0);
            _stream.Seek(0, SeekOrigin.Begin);
            new XmlWindowsManagerSerializer((xmlElement, dockPane)=> xmlElement.SetAttribute("Data", dockPane.Tag.ToString()), arg=> arg.DockPane.Tag.ToString()).Serialize(_stream, WindowsManager);
        }

        private void Button_Load(object sender, System.Windows.RoutedEventArgs e)
        {
            _stream.Seek(0, SeekOrigin.Begin);

            WindowsManager.Clear();
            
            if (_stream.Length == 0)
            {
                return;
            }

            new XmlWindowsManagerDeserializer((dockpane, data)=>
                                                  {
                                                      dockpane.Header = "Solution Explorer";
                                                      dockpane.Tag = int.Parse(data);
                                                      Grid g = new Grid();
                                                      g.Background = Brushes.White;
                                                      TextBlock text = new TextBlock();
                                                      text.Text = "Some content - " + data;
                                                      g.Children.Add(text);
                                                      dockpane.Content = g;
                                                  }).Deserialize(_stream, WindowsManager);
        }

        int i = 0;
        private MemoryStream _stream = new MemoryStream();
    }
}
