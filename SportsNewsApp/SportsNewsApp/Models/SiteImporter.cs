using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using Xamarin.Forms;

namespace SportsNewsApp.Models
{
    public abstract class SiteImporter
    {
        //Property for storing all articles.
        internal List<Article> ArticleList { get; set; }
        //Site URL.
        internal string URL { get; set; }
        //An array of all delimiters, for HTML page parsing.
        internal Dictionary<string, string> Delimiters { get; set; }
        //Site's icon.
        internal string Icon { get; set; }

        //Gets all articles from sites main page.
        internal List<Article> GetAllArticles()
        {
            List<Article> listOfArticles = new List<Article>();
            try
            {
                Timer t = new Timer();
                t.ElapseStart();
                string page = GetWebPage(URL);
                List<string> buffer = SplitPage(page, Delimiters["Articles delimiter"]);
                //Gathering all articles from site.
                foreach (string data in buffer)
                {
                    try
                    {
                        Article article = new Article();
                        UpdateArticleInfo(data, ref article, Delimiters);
                        if (article.Title == null)
                            continue;
                        listOfArticles.Add(article);
                    }
                    catch (Exception e) { }
                }
                t.ElapseEnd(t.GetCurrentMethod());
            } catch (Exception) { Console.WriteLine("Error trying to get articles from " + URL); }
            return listOfArticles;
        }

        //Decoding HTML encoded characters into string.
        internal void DecodeHTMLEncoding(ref string text)
        {
            if (text.Contains("&#8211;"))
                text = text.Replace("&#8211;", WebUtility.HtmlDecode("&#8211;"));
            if (text.Contains("&#039;"))
                text = text.Replace("&#039;", WebUtility.HtmlDecode("&#039;"));
            if (text.Contains("&nbsp;"))
                text = text.Replace("&nbsp;", WebUtility.HtmlDecode("&nbsp;"));
            if (text.Contains("&#8230;"))
                text = text.Replace("&#8230;", WebUtility.HtmlDecode("&#8230;"));
            if (text.Contains("&quot;"))
                text = text.Replace("&quot;", WebUtility.HtmlDecode("&quot;"));
            if (text.Contains("&#x27;"))
                text = text.Replace("&#x27;", WebUtility.HtmlDecode("&#x27;"));

        }

        //Remove redundant spaces and new lines.
        internal void HandleRedundancy(ref string text)
        {
            int i = 0;
            bool changed = false;
            //Check from beginning of string.
            while (text[i] == ' ' || text[i] == '\n' || text[i] == '\r')
            {
                changed = true;
                i++;
            }
            if (changed)
                text = text.Substring(i);
            changed = false;
            i = text.Length - 1;
            //Check from end of string.
            while (text[i] == ' ' || text[i] == '\n')
            {
                changed = true;
                i--;
            }
            if (changed)
                text = text.Substring(0, i + 1);
        }

        //If there is more than one option for delimiter in certain property, update it with the correct one.
        internal void CheckDelimiterOptions(string text, ref string delimStart, ref string delimEnd)
        {
            //Check ".or." delimiter in beginning and end strings.
            if (delimStart.Contains(".or."))
            {
                string first = GetFirstDelimiter(text, delimStart);
                if (first != "")
                    delimStart = first;
            }
            if (delimEnd.Contains(".or."))
            {
                string first = GetFirstDelimiter(text, delimEnd);
                if (first != "")
                    delimEnd = first;
            }
            //Check ".and." delimiter in beginning and end strings.
            if (delimStart.Contains(".and."))
            {
                string last = GetLastDelimiter(text, delimStart);
                if (last != "")
                    delimStart = last;
            }
            if (delimEnd.Contains(".and."))
            {
                string last = GetLastDelimiter(text, delimEnd);
                if (last != "")
                    delimEnd = last;
            }
        }

        //Get first delimiter from delimiters list.
        internal string GetFirstDelimiter(string text, string delim)
        {
            long min = 9999999;
            string first = "";
            string[] split = delim.Split(new String[] { ".or." }, StringSplitOptions.None);
            foreach (string str in split)
            {
                int index = text.IndexOf(str);
                if (index != -1 && index < min)
                {
                    min = index;
                    first = str;
                }
            }
            return first;
        }

        //Get last delimiter from delimiters list.
        internal string GetLastDelimiter(string text, string delim)
        {
            long max = -1;
            string last = "";
            string[] split = delim.Split(new String[] { ".and." }, StringSplitOptions.None);
            foreach (string str in split)
            {
                int index = text.IndexOf(str);
                if (index != -1 && index > max)
                {
                    max = index;
                    last = str;
                }
            }
            return last;
        }

        //Split page into sections, easier to work with.
        public static List<string> SplitPage(string str, string delimiter)
        {
            string[] split = str.Split(new String[] { delimiter }, StringSplitOptions.None);
            List<string> list = split.OfType<string>().ToList();
            return list;
        }

        //Get entire page source from web.
        public static string GetWebPage(string address)
        {
            HttpWebResponse response = null;
            Timer t = new Timer();
            string str = "";
            string url = address;
            do
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip; //TODO remove?
                request.Proxy = null;
                t.ElapseStart();
                try
                {
                    response = (HttpWebResponse)request.GetResponse();
                } catch (WebException e)
                {
                    Console.Error.WriteLine("Site " + url + "failed to connect. Error: " + e.Message);
                    try
                    {
                        response = (HttpWebResponse)request.GetResponse();
                    }
                    catch (WebException e2) { continue; }
                }
                t.ElapseEnd(t.GetCurrentMethod());
                StreamReader sr = new StreamReader(response.GetResponseStream());
                str = sr.ReadToEnd();
                string decoded = HttpUtility.UrlDecode(str); //TODO remove?
                Console.WriteLine("test1: " + str.Substring(0, 9)); //TODO remove.
                Console.WriteLine("test2: " + str); //TODO remove.
                Console.WriteLine("test3: " + decoded); //TODO remove.
            } while (!str.Contains("<!DOCTYPE") && !str.Contains("<!doctype"));
            return str;
        }

        //Get string and update article info by parsing the data in the string.
        internal void UpdateArticleInfo(string text, ref Article article, Dictionary<string, string> delimiters)
        {
            /*Filtering irrelevant HTML code sections 
            (irrelevant - Beginning of HTML page or does not contain article indicator).*/
            if (!text.Contains("<!DOCTYPE") && text.Contains(delimiters["Article indicator"]))
            {
                article.Title = ParseElement(text, delimiters["Title start"], delimiters["Title end"]);
                article.Subtitle = ParseElement(text, delimiters["Subtitle start"], delimiters["Subtitle end"]);
                article.Image = ParseElement(text, delimiters["Image start"], delimiters["Image end"]);
                article.Author = ParseElement(text, delimiters["Author start"], delimiters["Author end"]);
                article.Date = ParseElement(text, delimiters["Date start"], delimiters["Date end"]);
                FormatDate(ref article);
                article.Address = ParseElement(text, delimiters["Address start"], delimiters["Address end"]);
                article.Icon = GetIcon(delimiters["Site"]);
            }
        }

        //Formatting date to app format (dd//mm//yy, hh:mm [if exists]).
        internal abstract void FormatDate(ref Article article);

        //Get string, parse it and return relevant substring.
        internal string ParseElement(string text, string delimStart, string delimEnd)
        {
            if (delimStart.Length == 0 || delimEnd.Length == 0)
                return "";
            CheckDelimiterOptions(text, ref delimStart, ref delimEnd);
            try
            {
                text = text.Split(new[] { delimStart }, StringSplitOptions.None)[1];
                text = text.Split(new[] { delimEnd }, StringSplitOptions.None)[0];
            }
            catch (Exception) {
                return "";
            }
            
            //Handles redundancy in string (spaces/new lines) if necessary.
            HandleRedundancy(ref text);
            //Decode encoded HTML characters into string.
            DecodeHTMLEncoding(ref text);
            return text;
        }

        //Checks if given string represents today's date or yesterday's.
        internal void TodayOrYesterday(ref Article article)
        {
            int i;
            string time = "";
            Boolean notToday = false;
            //Defining today's date.
            string today = DateTime.Now.ToString("dd/MM/yy");
            //Splitting both dates to compare their values.
            string[] todaysDate = today.Split(new String[] { "/" }, StringSplitOptions.None);
            string[] givenDate = article.Date.Split(new String[] { "/" }, StringSplitOptions.None);
            givenDate[2] = givenDate[2].Split(new String[] { "," }, StringSplitOptions.None)[0];
            //If days are following, check if date is yesterday's.
            if (Int32.Parse(todaysDate[0]) - Int32.Parse(givenDate[0]) == 1)
            {
                notToday = true;
                givenDate[0] = todaysDate[0];
            }
            //Compare date by day, month and year.
            for (i = 0; i < 2; i++) {
                if (!todaysDate[i].Equals(givenDate[i]))
                    return;
            }
            if (article.Date.Contains(","))
                //Editing new time format (today/yesterday and time).
                time = ", " + article.Date.Split(new String[] { ", " }, StringSplitOptions.None)[1];
            if (notToday)
                article.Date = "אתמול" + time;
            else
                article.Date = "היום" + time;

        }

        //Add icon according to site.
        internal string GetIcon(string icon)
        {
            switch (icon)
            {
                case "walla":
                    return "wallaIcon.png";
                case "ynet":
                    return "ynetIcon.png";
                case "haaretz":
                    return "haaretzIcon.png";
                case "sport5":
                    return "sport5Icon.png";
                case "orangeBall":
                    return "orangeIcon.png";
                case "hoops":
                    return "hoopsIcon.png";
                case "youtube":
                    return "youtube.png";
                default:
                    return "noImageIcon.png";
            }
        }
    }
}
