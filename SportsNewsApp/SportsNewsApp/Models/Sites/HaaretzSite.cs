using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;

namespace SportsNewsApp.Models
{
    class HaaretzSite : SiteImporter
    {
        public HaaretzSite()
        {
            //Page URL address.
            URL = "https://www.haaretz.co.il/sport/nba";
            //A dictionary of delimiters, for HTML page parsing.
            Delimiters = new Dictionary<string, string>
            {
                {"Articles delimiter", "assetId"},
                {"Article indicator", ""},
                {"Title start", "\"titleMobile\":\""},
                {"Title end", "\",\"authors\":"},
                {"Subtitle start",  "subtitle\":\""},
                {"Subtitle end","\",\"subtitleMobile\""},
                {"Image start", ""},
                {"Image end", ""},
                {"Author start", "\"contentName\":\""},
                {"Author end", "\",\"__typename\""},
                {"Date start", "\"publishDate\":\""},
                {"Date end", "\",\"rank\""},
                {"Address start", "\"contentId\":\""},
                {"Address end", "\",\"hash\""},
                {"Site", "haaretz"}
            };
            //Get all articles from site, and put it in property.
            ArticleList = GetAllArticles();
            UpdateImages();
            UpdateAddress();
        }

        //Formatting date to app format (dd//mm//yy, hh:mm [if exists]).
        override internal void FormatDate(ref Article article)
        {
            string date = article.Date.Substring(0, 19);
            string time = date.Substring(11, 5);
            string year = date.Substring(2, 2);
            string month = date.Substring(5, 2);
            string day = date.Substring(8, 2);
            char[] dateCh = date.ToCharArray();
            dateCh[4] = '/';
            dateCh[7] = '/';
            article.Date = day + "/" +  month + "/" + year + ", " + time;
            TodayOrYesterday(ref article);
        }

        //Decoding image of all articles with http request, and update them in app.
        private string UpdateImages()
        {
            //http request
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
            request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip; //TODO remove?
            request.Proxy = null;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader sr = new StreamReader(response.GetResponseStream());
            string str = sr.ReadToEnd();
            List<string> buffer = SplitPage(str, ",\"Image:");
            List<string> l = new List<string>();
            int i = 0;
            foreach(string data in buffer)
            {
                string imgCode = GetImageCode(data);
                if (imgCode != "")
                    ArticleList[i++].Image = imgCode;
            }
            return str;
        }

        //Parsing HTML code, returning image address.
        private string GetImageCode(string str)
        {
            if (str.Contains("<!DOCTYPE") || !str.Contains("imgName"))
                return "";
            string s1 = str.Split(new[] { ".img" }, StringSplitOptions.None)[0];
            string s2 = str.Split(new[] { "image" }, StringSplitOptions.None)[1];
            s2 = "image" + s2;
            s2 = s2.Split(new[] { "\",\"aspects" }, StringSplitOptions.None)[0];
            return "https://www.haaretz.co.il/polopoly_fs/" + s1 + "!/" + s2;            
        }

        //Add prefix to address.
        private void UpdateAddress()
        {
            foreach (Article article in ArticleList)
                article.Address = "https://www.haaretz.co.il/" + article.Address;
        }
    }
}
