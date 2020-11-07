using System;
using System.Collections.Generic;
using System.Text;

namespace SportsNewsApp.Models
{
    class YnetSite : SiteImporter
    {
        public YnetSite()
        {
            //Page URL address.
            URL = "https://www.ynet.co.il/sport/worldbasketball";
            //A dictionary of delimiters, for HTML page parsing.
            Delimiters = new Dictionary<string, string>
            {
                {"Articles delimiter", "slotView"},
                {"Article indicator", "imageView"},
                {"Title start", "slotTitle"},
                {"Title end", "</a></div>"},
                {"Subtitle start",  "slotSubTitle"},
                {"Subtitle end","</a></div>" },
                {"Image start", "<img src=\""},
                {"Image end", "\" width"},
                {"Author start", "moreDetails\">"},
                {"Author end", "<span class=\"commaView\">"},
                {"Date start", "dateView\">"},
                {"Date end", "</span></div>"},
                {"Address start", "<a href=\""},
                {"Address end", "\"><div"},
                {"Site", "ynet"}
            };
            //Get all articles from site, and put it in property.
            ArticleList = GetAllArticles();
            UpdateTitleAndSubtitle();
        }

        //Updating title and subtitle for each article.
        internal void UpdateTitleAndSubtitle()
        {
            string temp;
            foreach (Article article in ArticleList)
            {
                try
                {
                    //Updating title.
                    article.Title = article.Title.Split(new[] { "\">" }, StringSplitOptions.None)[2];
                    //Decoding phase.
                    temp = article.Title;
                    DecodeHTMLEncoding(ref temp);
                    article.Title = temp;
                    //Updating subtitle.
                    article.Subtitle = article.Subtitle.Split(new[] { "\">" }, StringSplitOptions.None)[2];
                    //Decoding phase.
                    temp = article.Subtitle;
                    DecodeHTMLEncoding(ref temp);
                    article.Subtitle = temp;
                } catch (Exception e) { }
            }
        }

        //Formatting date to app format (dd//mm//yy, hh:mm [if exists]).
        override internal void FormatDate(ref Article article)
        {
            char[] date = article.Date.ToCharArray();
            date[11] = '/';
            date[14] = '/';
            article.Date = new String(date, 9, 8) + ", " + new String(date, 0, 5);
            TodayOrYesterday(ref article);
        }
    }
}
