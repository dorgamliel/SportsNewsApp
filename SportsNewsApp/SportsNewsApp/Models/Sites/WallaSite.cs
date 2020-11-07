
using System.Collections.Generic;
using Xamarin.Forms.Internals;

namespace SportsNewsApp.Models
{
    class WallaSite : SiteImporter
    {
        public WallaSite()
        {
            //Page URL address.
            URL = "http://sports.walla.co.il/category/175";
            //A dictionary of delimiters, for HTML page parsing.
            Delimiters = new Dictionary<string, string>
            {
                {"Articles delimiter", "class=\"article fc common-article \""},
                {"Article indicator", "<footer>"},
                {"Title start", "data-cd-title>"},
                {"Title end", "</span>"},
                {"Subtitle start",  "<p>"},
                {"Subtitle end","</p>" },
                {"Image start", "<img src=\"//"},
                {"Image end", "\" class="},
                {"Author start", "class=\"author\">"},
                {"Author end", "</span>"},
                {"Date start", "<time datetime=\""},
                {"Date end", "\">"},
                {"Address start", "<a href=\""},
                {"Address end", "\"\tdata"},
                {"Site", "walla"}
            };
            //Get all articles from site, and put it in property.
            ArticleList = GetAllArticles();
            FixImages();
        }

        //Formatting date to app format (dd//mm//yy, hh:mm [if exists]).
        override internal void FormatDate(ref Article article)
        {
            Stack<string> dateElements = new Stack<string>();
            string newString;
            int i = 0, from = 0;
            string oldDate = article.Date;
            while (oldDate[i] != ' ')
            {
                if (oldDate[i] == '-')
                {
                    //Put day, month and year in a stack, to add to final date string.
                    dateElements.Push(oldDate.Substring(from, 2));
                    from = i + 1;
                }
                i++;
            }
            //Put last element in date.
            dateElements.Push(oldDate.Substring(from, 2));
            newString = dateElements.Pop() + "/" + dateElements.Pop() + "/" + dateElements.Pop();
            newString += "," + oldDate.Substring(i);
            article.Date = newString;
            TodayOrYesterday(ref article);
        }
        private void FixImages()
        {
            foreach (Article article in ArticleList)
            {
                article.Image = "https://" + article.Image;
            }
        }
    }

}
