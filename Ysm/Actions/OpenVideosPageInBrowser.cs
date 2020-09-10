﻿using System.Collections.Generic;
using System.Linq;
using Ysm.Assets;
using Ysm.Core;
using Ysm.Windows;

namespace Ysm.Actions
{
    public class OpenVideosPageInBrowser : IAction
    {
        public string Name { get; } = "OpenVideosPageInBrowser";

        public void Execute(object obj)
        {
            if (obj is List<string> channels)
            {
                List<string> urls = channels.Select(UrlHelper.GetVideosPageUrl).ToList();

                if (urls.Count > 5)
                {
                    DialogWindow window = new DialogWindow(Messages.GetOpenPagesMessage(urls.Count));
                    window.UserAnswer += answer =>
                    {
                        if (answer == Answer.Yes)
                        {
                            BrowserOpen.OpenUrls(urls);
                        }
                    };
                    window.ShowDialog();
                }
                else
                {
                    BrowserOpen.OpenUrls(urls);
                }
            }
        }
    }
}
