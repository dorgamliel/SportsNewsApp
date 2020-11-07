using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace SportsNewsApp.Models
{
    class Sport5Site : SiteImporter
    {
        public Sport5Site()
        {
            //Page URL address.
            URL = "https://nba.sport5.co.il/NBA.aspx?FolderId=402";
            //A dictionary of delimiters, for HTML page parsing.
            Delimiters = new Dictionary<string, string>
            {
                {"Articles delimiter", "btn-play\">"},
                {"Article indicator", ""},
                {"Title start", "target=\"_self\"><img.or.<div class=\"bomba-title\"><span>"},
                {"Title end", "</a></h.or.</span>"},
                {"Subtitle start",  "target=\"_self\"><img.or.<div class=\"bomba-subtitle\"><span>"},
                {"Subtitle end","</a></p.or.</span>" },
                {"Image start", "<img src=\""},
                {"Image end", "\" title="},
                {"Author start", "ancWriter\">.or.ancWriter_0\">.or.ancWriter_1\">.or.ancWriter_2\">.or." +
                "ancWriter_3\">.or.ancWriter_4\">.or.ancWriter_5\">.or.ancWriter_6\">"},
                {"Author end", "</a>"},
                {"Date start", "<em class=\"date\">.and.</a></spanc>"},
                {"Date end", "</em>\r\n\t</div>.and.<span.and.</em>\r\n                    "},
                {"Address start", "href=\""},
                {"Address end", "\" target"},
                {"Site", "sport5"}
            };
            //Get all articles from site, and put it in property.
            ArticleList = GetAllArticles();
            //Update title.
            UpdateTitleAndSubtitle();
            //Fix arcitle url address.
            FixAdress();
            //FixBombaArcitle();
        }

        //Formatting date to app format (dd//mm//yy, hh:mm [if exists]).
        override internal void FormatDate(ref Article article)
        {
            string date = article.Date;
            string day = date.Substring(0, 2);
            string month = date.Substring(3, 2);
            string year = date.Substring(6, 2);
            string time = date.Substring(11, 5);
            article.Date = day + "/" + month + "/" + year + ", " + time;
            TodayOrYesterday(ref article);
        }

        internal void UpdateTitleAndSubtitle()
        {
            foreach (Article article in ArticleList)
            {
                try
                {
                    article.Title = article.Title.Split(new[] { "_self\">" }, StringSplitOptions.None)[1];
                    article.Subtitle = article.Subtitle.Split(new[] { "_self\">" }, StringSplitOptions.None)[2];
                }
                catch (Exception e) { }
            }
        }

        //Remove redundant part in the middle on url.
        private void FixAdress()
        {
            foreach(Article article in ArticleList)
            {
                try
                {
                    string[] split = article.Address.Split(new String[] { "amp;" }, StringSplitOptions.None);
                    article.Address = split[0] + split[1];
                } catch (Exception e) { }
                    
            }
        }

        private void FixBombaArcitle()
        {
            var article = ArticleList.Find(x => x.Title == "");
            article.Title = "123";
        }
    }
}
