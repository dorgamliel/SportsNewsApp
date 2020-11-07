using System;
using System.Collections.Generic;
using System.Text;

namespace SportsNewsApp.Models.Sites
{
    class FullHighlights : SiteImporter
    {
        public FullHighlights()
        {
            //Page URL address.
            URL = "https://www.youtube.com/c/HNBMediaTV/videos";
            //A dictionary of delimiters, for HTML page parsing.
            Delimiters = new Dictionary<string, string>
            {
                {"Articles delimiter", "\"thumbnail\":{\"thumbnails\":"},
                {"Article indicator", ""},
                {"Title start", "\"title\":{\"runs\":[{\"text\":\""},
                {"Title end", "\"}]"},
                {"Subtitle start",  ""},
                {"Subtitle end","" },
                {"Image start", "[{\"url\":\"" },
                {"Image end", "\"," },
                {"Author start", "viewCountText\":{\"simpleText\":\""},
                {"Author end", "\"},"},
                {"Date start", "publishedTimeText\":{\"simpleText\":\""},
                {"Date end", "\"},"},
                {"Address start", "\"webCommandMetadata\":{\"url\":\"/w"},
                {"Address end", "\","},
                {"Site", "youtube"}
            };
            //Get all articles from site, and put it in property.
            ArticleList = GetAllArticles();
            FixAddress();
            FilterArticles();
        }

        internal override void FormatDate(ref Article article)
        {
            return;
        }
        internal void FixAddress()
        {
            foreach (Article article in ArticleList)
            {
                article.Address = "https://youtube.com/w" + article.Address;
            }
        }
        internal void FilterArticles()
        {
            List<Article> tempList = new List<Article>();
            foreach (Article article in ArticleList)
            {
                if (article.Title.Contains("Full Highlights"))
                {
                    tempList.Add(article);
                }
            }
            ArticleList = tempList;
        }
    }
}
