using System;
using System.Collections.Generic;
using System.Text;

namespace SportsNewsApp.Models
{
    class OneSite : SiteImporter
    {
        public OneSite()
        {
            //Page URL address.
            URL = "https://www.one.co.il/Basketball/League/12";
            //A dictionary of delimiters, for HTML page parsing.
            Delimiters = new Dictionary<string, string>
            {
                {"Articles delimiter", "leagues-right-column"},
                {"Article indicator", ""},
                {"Title start", "<h2>"},
                {"Title end", "</h2>"},
                {"Subtitle start",  "<h3>"},
                {"Subtitle end","</h3>" },
                {"Image start", "<img src="},
                {"Image end", "\" alt="},
                {"Author start", ""},
                {"Author end", ""},
                {"Date start", ""},
                {"Date end", ""},
                {"Address start", "href=\""},
                {"Address end", "\"><img"}
            };
            //Get all articles from site, and put it in property.
            
            UpdateDateAndAuthor();
        }

        internal override void FormatDate(ref Article article)
        {
        }

        internal void UpdateDateAndAuthor()
        {
            string temp;
            foreach (Article article in ArticleList)
            {
                string page = GetWebPage(article.Address);
                string split = page.Split(new[] { "id=\"_ctl0_ContentHolder_Body_imgAuthorPhoto\"" }, StringSplitOptions.None)[1];
                split = split.Split(new[] { "</a>\r| \r" }, StringSplitOptions.None)[1];
                split = split.Split(new[] { "</div>" }, StringSplitOptions.None)[0];
                article.Date = split;
            }
        }
    }
}
