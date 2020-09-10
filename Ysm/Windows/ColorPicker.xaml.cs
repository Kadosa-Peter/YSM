using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Ysm.Core;
using Ysm.Data;
using Ysm.Models;

namespace Ysm.Windows
{
    public partial class ColorPicker
    {
        #region SelectedColor

        public static readonly DependencyProperty SelectedColorProperty = DependencyProperty.Register(
            "SelectedColor", typeof (Color), typeof (ColorPicker),
            new PropertyMetadata(default(Color), Color_Changed));

        private static void Color_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ColorPicker colorPicker)
            {
                colorPicker.Node.Color = colorPicker.SelectedColor;
            }
        }

        public Color SelectedColor
        {
            get => (Color) GetValue(SelectedColorProperty);
            set => SetValue(SelectedColorProperty, value);
        }

        #endregion

        #region ShowHeader

        public static readonly DependencyProperty ShowHeaderProperty = DependencyProperty.Register(
            "ShowHeader", typeof (bool), typeof (ColorPicker), new PropertyMetadata(true));

        public bool ShowHeader
        {
            get => (bool) GetValue(ShowHeaderProperty);
            set => SetValue(ShowHeaderProperty, value);
        }

        #endregion

        public Node Node { get; set; }

        public ColorPicker()
        {
            InitializeComponent();
        }

        public ColorPicker(Node node)
        {
            InitializeComponent();

            DataContext = this;

            Node = node;

            FolderTitle.Text = node.Title;
            
            SelectedColor = node.Color;
        }

        private void Header_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ColorPicker_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
                Close();
            if (e.Key == Key.Escape)
                Close();
        }

        private void ColorPicker_OnClosing(object sender, CancelEventArgs e)
        {
            if (Node.Color != ColorHelper.GetDefaultFolderColor())
            {
                if (Node.Category != null)
                {
                    Node.Category.Color = Node.Color.ToString();
                    Repository.Default.Categories.Update(Node.Category);
                }
                else if(Node.Playlist != null)
                {
                    Node.Playlist.Color = Node.Color.ToString();
                    Repository.Default.Playlists.Save(Node.Playlist);
                }

               
            }
        }
    }
}
