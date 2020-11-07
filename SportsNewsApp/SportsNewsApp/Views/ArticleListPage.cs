using SportsNewsApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace SportsNewsApp.Views
{

    class ArticleListPage
    {
        ObservableCollection<Article> articles = new ObservableCollection<Article>();
        public ObservableCollection<Article> Articles { get { return articles; } }

        public ArticleListPage()
        {
            Article a = new Article();
            a.Title = "hello";
            articles.Add(a);
        }

    }
}
