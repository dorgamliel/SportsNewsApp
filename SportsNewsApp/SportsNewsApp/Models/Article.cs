using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Xamarin.Forms;

namespace SportsNewsApp.Models
{
    public class Article
    {
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string Image { get; set; }
        public string Author { get; set; }
        public string Date { get; set; }
        public string Address { get; set; }
        public string Icon { get; set; }
        public string AuthorAndDate
        {
            get
            {
                if (Author.Contains("מערכת אתר"))
                    Author = Author.Replace("מערכת אתר ", "");
                else if (Author.Contains("מערכת"))
                    Author = Author.Replace("מערכת ", "");
                else if (Address.Contains("ynet"))
                    Author = Author.Replace("ynet ספורט", "ספורט ynet");
                //in case no available time was set.
                if (Date.Contains("00:00"))
                    Date = Date.Replace(", 00:00", "");
                return string.Format("\n {0} | {1}", Author, Date);
            }
        }
        public override string ToString()
        {
            return Title;
        }
    }
}
