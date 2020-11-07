using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;

namespace SportsNewsApp.Models
{
    class OrangeBallSite : SiteImporter
    {
        public OrangeBallSite()
        {
            //Page URL address.
            URL = "https://www.orangeball.co.il/%D7%A2%D7%93%D7%9B%D7%95%D7%A0%D7%99%D7%9D/";
            //A dictionary of delimiters, for HTML page parsing.
            Delimiters = new Dictionary<string, string>
            {
                {"Articles delimiter", "article id"},
                {"Article indicator", ""},
                {"Title start", "<h2 class"},
                {"Title end", "</a></h2>"},
                {"Subtitle start",  ""},
                {"Subtitle end","" },
                {"Image start", ""},
                {"Image end", ""},
                {"Author start", ""},
                {"Author end", ""},
                {"Date start", "class=\"updated\">"},
                {"Date end", "</span> | <a "},
                {"Address start", "href=\""},
                {"Address end", "\">"},
                {"Site", "orangeBall"}
            };
            //Get all articles from site, and put it in property.
            ArticleList = GetAllArticles();
            AddImage();
            UpdateTitle();
        }

        internal override void FormatDate(ref Article article)
        {
            Dictionary<string, string> hebToEng = new Dictionary<string, string> {
                {"ינו", "01" },
                {"פבר", "02" },
                {"מרץ", "03" },
                {"אפר", "04" },
                {"מאי", "05" },
                {"יונ", "06" },
                {"יול", "07" },
                {"אוג", "08" },
                {"ספט", "09" },
                {"אוק", "10" },
                {"נוב", "11" },
                {"דצמ", "12" }
            };
            string targetMonth = article.Date.Split(new String[]{" "}, StringSplitOptions.RemoveEmptyEntries)[0];
            string month = hebToEng[targetMonth];
            string date = article.Date;
            string day = date.Substring(4, 2);
            string year = date.Substring(8, 2);
            article.Date = day + "/" + month + "/" + year + ", 00:00";
            TodayOrYesterday(ref article);
        }

        //Update title of all articles.
        internal void UpdateTitle()
        {
            foreach (Article article in ArticleList)
            {
                try
                {
                    article.Title = article.Title.Split(new[] { "/\">" }, StringSplitOptions.None)[1];
                } catch (Exception e) { }
            }
        }

        internal void AddImage()
        {
            foreach (Article article in ArticleList)
            {
                article.Image = "orangIcon.png";
            }
        }

    }
}
