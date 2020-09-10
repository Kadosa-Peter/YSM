using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Ysm.Core;

namespace Ysm.Windows
{
    public partial class DialogWindow
    {
        public delegate void AnswerEventHandler(Answer answer);

        public event AnswerEventHandler UserAnswer;

        public new bool DialogResult { get; set; }

        public DialogWindow()
        {
            InitializeComponent();

            TextBlock.Visibility = Visibility.Visible;
        }

        public DialogWindow(FlowDocument document)
        {
            InitializeComponent();

            TextBlock.Visibility = Visibility.Collapsed;
            RichTextBox.Visibility = Visibility.Visible;

            if (Owner == null)
                WindowStartupLocation = WindowStartupLocation.CenterScreen;

            RichTextBox.Document = document;
        }

        public DialogWindow(string question, TextAlignment textAlignment = TextAlignment.Center)
        {
            InitializeComponent();

            TextBlock.Visibility = Visibility.Visible;
            RichTextBox.Visibility = Visibility.Collapsed;

            TextBlock.TextAlignment = textAlignment;

            if (Owner == null)
                WindowStartupLocation = WindowStartupLocation.CenterScreen;

            if (question.NotNull())
            {
                if (question.Contains(@"\n"))
                {
                    question = question.Replace(@"\n", System.Environment.NewLine);
                }

                TextBlock.Text = question;
            }
        }

        public DialogWindow(Dictionary<string, Brush> question)
        {
            InitializeComponent();

            if (Owner == null)
                WindowStartupLocation = WindowStartupLocation.CenterScreen;

            TextBlock.Text = string.Empty;

            if (question != null && question.Count > 0)
            {
                foreach (KeyValuePair<string, Brush> q in question)
                {
                    TextBlock.Inlines.Add(new Run(q.Key) { Foreground = q.Value });
                }
            }
        }

        public DialogWindow(List<string> lines)
        {
            InitializeComponent();

            if (Owner == null)
                WindowStartupLocation = WindowStartupLocation.CenterScreen;

            TextBlock.Text = string.Empty;

            foreach (string line in lines)
            {
                TextBlock.Inlines.Add(new Run(line));
            }
        }

        private void DialogWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            YesButton.ForceFocus();
        }

        private void DialogWindow_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                No_Click(null, null);
            }
        }

        private void Yes_Click(object sender, RoutedEventArgs e)
        {
            UserAnswer?.Invoke(Answer.Yes);

            DialogResult = true;

            Close();
        }

        private void No_Click(object sender, RoutedEventArgs e)
        {
            UserAnswer?.Invoke(Answer.No);

            DialogResult = false;

            Close();
        }

        private void Footer_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
    }

    public enum Answer
    {
        Yes,
        No
    }
}
