using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Documents;
using System.Windows.Media;
using Ysm.Core;
using Ysm.Data;
using Ysm.Properties;

namespace Ysm.Assets
{
    public static class Messages
    {
        public static FlowDocument ChangeSubscriptionView()
        {
            FlowDocument document = new FlowDocument();
            Paragraph paragraph = new Paragraph();


            Run r1 = new Run(Properties.Resources.Info_ChangeSubscriptionView1);
            r1.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF7B848B"));
            r1.FontSize = 15;

            Run r2 = new Run(Properties.Resources.Info_ChangeSubscriptionView2);
            r2.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#A2AAAF"));
            r2.FontSize = 14;

            paragraph.Inlines.Add(r1);
            paragraph.Inlines.Add(new LineBreak());
            paragraph.Inlines.Add(new LineBreak());
            paragraph.Inlines.Add(r2);

            document.Blocks.Add(paragraph);

            return document;
        }

        public static FlowDocument DoYouWantChangeVideoDisplayMode()
        {
            FlowDocument document = new FlowDocument();
            Paragraph paragraph = new Paragraph();

            Run r1 = new Run(Resources.Text_VideoNotVisible1);
            r1.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF7B848B"));
            r1.FontSize = 15;

            Run r2 = new Run(Resources.Text_VideoNotVisible2);
            r2.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#A2AAAF"));
            r2.FontSize = 14;

            paragraph.Inlines.Add(r1);
            paragraph.Inlines.Add(new LineBreak());
            paragraph.Inlines.Add(new LineBreak());
            paragraph.Inlines.Add(r2);

            document.Blocks.Add(paragraph);

            return document;
        }

        public static string GeUnsubscriptionMessage()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(Properties.Resources.Question_Unsub1);
            builder.AppendLine(Properties.Resources.Question_Unsub2);
            return builder.ToString();
        }

        public static FlowDocument PlaylistContainsVideo(string m1, string m2)
        {
            FlowDocument document = new FlowDocument();
            Paragraph paragraph = new Paragraph();

            Run r1 = new Run(m1);
            r1.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF7B848B"));
            r1.FontSize = 15;

            Run r2 = new Run(m2);
            r2.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#A2AAAF"));
            r2.FontSize = 14;

            paragraph.Inlines.Add(r1);
            paragraph.Inlines.Add(new LineBreak());
            paragraph.Inlines.Add(r2);

            document.Blocks.Add(paragraph);

            return document;
        }

        public static string GetDeleteMessage(List<Channel> channels)
        {
            string question;
            if (channels.Count == 1)
            {
                question = Properties.Resources.Question_Unsubscirbe_s;
                question = question.Replace("{xy}", channels[0].Title);
            }
            else
            {
                question = Properties.Resources.Question_Unsubscirbe_m;
                question = question.Replace("{xy}", channels.Count.ToString());
            }

            return question;
        }

        public static string GetOpenPagesMessage(int count)
        {
            string browser = DefaultBrowser.Get();
            if (browser == "Unknown") browser = "webbrowser";

            string question = Properties.Resources.Question_Open;
            question = question.Replace("_count_", count.ToString());
            question = question.Replace("_browser_", browser);


            return question;
        }

        public static FlowDocument DeleteChannelFromPlaylist(string channel)
        {
            string text = Resources.Question_Channel_Delete_From_Playlist;

            string t1 = text.Remove(text.IndexOf("|", StringComparison.Ordinal));
            string t2 = text.Substring(text.IndexOf("|", StringComparison.Ordinal) + 2);

            FlowDocument document = new FlowDocument();
            Paragraph paragraph = new Paragraph();

            Color dark = "#FF7B848B".ToColor();
            Color ligth = "#A2AAAF".ToColor();

            Run r1 = new Run(t1) { Foreground = new SolidColorBrush(dark), FontSize = 14 };
            Run r2 = new Run(" (") { Foreground = new SolidColorBrush(dark), FontSize = 14 };
            Run r3 = new Run(channel) { Foreground = new SolidColorBrush(ligth), FontSize = 14 };
            Run r4 = new Run(") ") { Foreground = new SolidColorBrush(dark), FontSize = 14 };
            Run r5 = new Run(t2) { Foreground = new SolidColorBrush(dark), FontSize = 14 };

            paragraph.Inlines.Add(r1);
            paragraph.Inlines.Add(r2);
            paragraph.Inlines.Add(r3);
            paragraph.Inlines.Add(r4);
            paragraph.Inlines.Add(r5);

            document.Blocks.Add(paragraph);

            return document;
        }

        public static FlowDocument DeletePlaylist(string playlist)
        {
            string text = Resources.Question_Playlist_Delete;

            FlowDocument document = new FlowDocument();
            Paragraph paragraph = new Paragraph();

            Color dark = "#FF7B848B".ToColor();
            Color ligth = "#A2AAAF".ToColor();

            Run r1 = new Run(text) { Foreground = new SolidColorBrush(dark), FontSize = 14 };
            Run r2 = new Run(" (") { Foreground = new SolidColorBrush(dark), FontSize = 14 };
            Run r3 = new Run(playlist) { Foreground = new SolidColorBrush(ligth), FontSize = 14 };
            Run r4 = new Run(")") { Foreground = new SolidColorBrush(dark), FontSize = 14 };
            Run r5 = new Run("?") { Foreground = new SolidColorBrush(dark), FontSize = 14 };

            paragraph.Inlines.Add(r1);
            paragraph.Inlines.Add(r2);
            paragraph.Inlines.Add(r3);
            paragraph.Inlines.Add(r4);
            paragraph.Inlines.Add(r5);

            document.Blocks.Add(paragraph);

            return document;
        }
    }
}
