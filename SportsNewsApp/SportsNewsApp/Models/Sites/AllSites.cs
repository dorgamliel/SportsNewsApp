using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;

namespace SportsNewsApp.Models.Sites
{
    //A class with static functions related to all sites handling.
    class AllSites
    {
        //Get information from sites by creating an instance of all site objects.
        public static List<Article> GetInfoFromSites()
        {
            //FullHighlights fh = null;
            WallaSite walla = null;
            HoopsSite hoops = null;
            HaaretzSite haaretz = null; ;
            YnetSite ynet = null;
            Sport5Site sport5 = null;
            OrangeBallSite orange = null;
            List<Thread> threads = new List<Thread>();
            //threads.Add(new Thread(() => { fh = new FullHighlights(); }));
            threads.Add(new Thread(() => { walla = new WallaSite(); }));
            threads.Add(new Thread(() => { hoops = new HoopsSite(); }));
            threads.Add(new Thread(() => { haaretz = new HaaretzSite(); }));
            threads.Add(new Thread(() => { ynet = new YnetSite(); }));
            threads.Add(new Thread(() => { sport5 = new Sport5Site(); }));
            threads.Add(new Thread(() => { orange = new OrangeBallSite(); }));
            //Constructing all sites objects.
            foreach (Thread thread in threads)
            {
                thread.Start();
            }
            //Wait 5 seconds for each thread to finish.
            foreach (Thread thread in threads)
            {
                if (!thread.Join(TimeSpan.FromSeconds(5)))
                {
                    Debug.WriteLine("thread didn't finish on time period given.");
                    continue;
                }
                thread.Join();
            }
            List<SiteImporter> sites = new List<SiteImporter> { walla, hoops, sport5, orange, haaretz, ynet };
            //Return sites arranged by date.
            return ArrangeByDate(sites);
        }

        //Arrange all articles by date, return the list ordered.
        public static List<Article> ArrangeByDate(List<SiteImporter> sites)
        {
            List<Article> all = new List<Article>();
            foreach (SiteImporter site in sites)
            {
                if (site != null)
                    all = all.Concat(site.ArticleList).ToList();
            }

            List<Article> today = all.FindAll(article => article.Date.Contains("היום")).
                OrderByDescending(article => article.Date).ToList();
            List<Article> yesterday = all.FindAll(article => article.Date.Contains("אתמול")).
                OrderByDescending(article => article.Date).ToList();
            List<Article> others = all.Except(today).Except(yesterday).ToList();
            //Sort all articles by date.
            try
            {
                others = others.OrderByDescending(x => DateTime.ParseExact(x.Date, "dd/MM/yy, HH:mm", CultureInfo.InvariantCulture)).ToList();
            } catch (Exception e)
            {

            }
            return today.Concat(yesterday).Concat(others).ToList();
        }
    }
}
