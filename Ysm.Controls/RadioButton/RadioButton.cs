using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Ysm.Core;

namespace Ysm.Controls
{
    public class RadioButton : CheckBox
    {
        public static readonly DependencyProperty GroupNameProperty;

        [Category("Common")]
        public string GroupName
        {
            get => (string)GetValue(GroupNameProperty);
            set => SetValue(GroupNameProperty, value);
        }

        private DependencyObject root;

        static RadioButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RadioButton), new FrameworkPropertyMetadata(typeof(RadioButton)));

            GroupNameProperty = DependencyProperty.Register("GroupName", typeof(string), typeof(RadioButton), new PropertyMetadata(default(string)));
        }

        public RadioButton()
        {
            Loaded += RadioButton_Loaded;
        }

        private void RadioButton_Loaded(object sender, RoutedEventArgs e)
        {
            if(root == null)
                root = FindRoot(this);
        }

        public DependencyObject FindRoot(DependencyObject obj)
        {
            DependencyObject ancestor = obj;

            do
            {
                ancestor = VisualTreeHelper.GetParent(ancestor);

            } while (ancestor != null && VisualTreeHelper.GetParent(ancestor) != null);

            return ancestor;
        }

        protected override void OnClick()
        {
            if (IsChecked == false)
                base.OnClick();
        }

        protected override void OnChecked(RoutedEventArgs e)
        {
            base.OnChecked(e);

            if(root == null)
                root = FindRoot(this);

            if (root != null && !string.IsNullOrEmpty(GroupName))
                UncheckGroup(root);
        }

        private void UncheckGroup(DependencyObject obj)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);

                if (child is RadioButton && !Equals(child, this))
                {
                    RadioButton radioButton = child as RadioButton;

                    if (radioButton.GroupName == GroupName)
                        radioButton.IsChecked = false;
                }

                UncheckGroup(child);
            }
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);

            if (e.Key == Key.Tab && ModifierKeyHelper.IsCtrlDown)
            {
                TraversalRequest request = new TraversalRequest(FocusNavigationDirection.Previous);
                MoveFocus(request);
                e.Handled = true;
            }
        }
    }
}
