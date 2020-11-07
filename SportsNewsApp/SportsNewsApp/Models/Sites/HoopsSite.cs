using System;
using System.Collections.Generic;
using System.Text;

namespace SportsNewsApp.Models
{
    class HoopsSite : SiteImporter
    {
        public HoopsSite()
        {
            //Page URL address.
            URL = "https://hoops.co.il/";
            //A dictionary of delimiters, for HTML page parsing.
            Delimiters = new Dictionary<string, string>
            {
                {"Articles delimiter", "<div class=\"thumbnail\""},
                {"Article indicator", ""},
                {"Title start", "title=\""},
                {"Title end", "\" rel"},
                {"Subtitle start",  "<p>"},
                {"Subtitle end","&hellip.or.</p>" },
                {"Image start", "background: url('"},
                {"Image end", "')"},
                {"Author start", "title=\"פוסטים מאת "},
                {"Author end", "\" rel=\"author\""},
                {"Date start", "icon-clock\"></i>"},
                {"Date end", "</li>"},
                {"Address start", "href=\""},
                {"Address end", "\" class"},
                {"Site", "hoops"}

            };
            //Get all articles from site, and put it in property.
            ArticleList = GetAllArticles();
            FixImagesURL();

        }

        //Formatting date to app format (dd//mm//yy, hh:mm [if exists]).
        override internal void FormatDate(ref Article article)
        {
            Dictionary<string, string> hebToEng = new Dictionary<string, string> {
                {"בינואר", "01" },
                {"בפברואר", "02" },
                {"מרץ", "03" },
                {"אפריל", "04" },
                {"מאי", "05" },
                {"ביוני", "06" },
                {"ביולי", "07" },
                {"באוגוסט", "08" },
                {"בספטמבר", "09" },
                {"באוקטובר", "10" },
                {"בנובמבר", "11" },
                {"בדצמבר", "12" }
            };
            string targetMonth = article.Date.Split(new String[] { " " }, StringSplitOptions.RemoveEmptyEntries)[1];
            string month = hebToEng[targetMonth];
            string date = article.Date;
            string day = date.Substring(0, 2);
            //Add zero to day in date.
            if (day[1] == ' ')
                day = "0" + day.Substring(0,1);
            string year = date.Substring(11, 2);
            try
            {
                article.Date = day + "/" + month + "/" + year + ", " + "00:00";
            } catch(Exception)
            {
                article.Date = "לא זמין";
            }
            TodayOrYesterday(ref article);
        }


        //Fix some images url.
        internal void FixImagesURL()
        {

            foreach (Article article in ArticleList)
            {
                if (article.Image.Contains("jpg&amp;"))
                    article.Image = article.Image.Split(new String[] { "&amp;" }, StringSplitOptions.None)[0];
            }
        }
    }
}
