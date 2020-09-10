using System.Windows;

namespace Ysm.Controls
{
    public class VideoRadioButton : System.Windows.Controls.RadioButton
    {
        #region Container

        public static readonly DependencyProperty ContainerProperty = DependencyProperty.Register(
            "Container", typeof(string), typeof(VideoRadioButton), new PropertyMetadata(default(string)));

        public string Container
        {
            get => (string) GetValue(ContainerProperty);
            set => SetValue(ContainerProperty, value);
        }

        #endregion

        #region VideoQualityLevel

        public static readonly DependencyProperty VideoQualityLevelProperty = DependencyProperty.Register(
            "VideoQualityLevel", typeof(string), typeof(VideoRadioButton), new PropertyMetadata(default(string)));

        public string VideoQualityLevel
        {
            get => (string) GetValue(VideoQualityLevelProperty);
            set => SetValue(VideoQualityLevelProperty, value);
        }

        #endregion

        #region Size

        public static readonly DependencyProperty SizeProperty = DependencyProperty.Register(
            "Size", typeof(string), typeof(VideoRadioButton), new PropertyMetadata(default(string)));

        public string Size
        {
            get => (string) GetValue(SizeProperty);
            set => SetValue(SizeProperty, value);
        }

        #endregion

        public object Audio { get; set; }

        public object Video { get; set; }

        public object Muxed { get; set; }
    }
}
