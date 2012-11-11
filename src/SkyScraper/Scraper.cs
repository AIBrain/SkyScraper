using HtmlAgilityPack;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace SkyScraper
{
    public class Scraper : IObservable<HtmlDoc>
    {
        readonly ConcurrentDictionary<string, string> scrapedHtmlDocs;
        readonly ITaskRunner taskRunner;
        readonly IHttpClient httpClient;
        readonly List<IObserver<HtmlDoc>> observers;
        Uri baseUri;

        public Scraper()
        {
            taskRunner = taskRunner ?? new AsyncTaskRunner();
            httpClient = httpClient ?? new AsyncHttpClient();
            scrapedHtmlDocs = new ConcurrentDictionary<string, string>();
            observers = new List<IObserver<HtmlDoc>>();
        }

        public Scraper(ITaskRunner taskRunner, IHttpClient httpClient)
            : this()
        {
            this.taskRunner = taskRunner;
            this.httpClient = httpClient;
        }

        public void Scrape(Uri uri)
        {
            baseUri = uri;
            DownloadDocument(uri);
            taskRunner.WaitForAllTasks();
        }

        void DownloadDocument(Uri uri)
        {
            if (scrapedHtmlDocs.ContainsKey(uri.PathAndQuery)) return;
            var task = httpClient.GetString(uri);
            taskRunner.Run(task);
            task.Try(x =>
                         {
                             var html = task.Result;
                             taskRunner.Run(() => StoreHtmlDoc(uri, html));
                         });
        }

        void StoreHtmlDoc(Uri uri, string html)
        {
            var htmlDoc = new HtmlDoc
                              {
                                  Uri = uri,
                                  Html = html
                              };
            scrapedHtmlDocs.TryAdd(uri.PathAndQuery, null);
            observers.ForEach(o => o.OnNext(htmlDoc));
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);
            var linkNodeCollection = htmlDocument.DocumentNode.SelectNodes("//a[@href]");
            if (linkNodeCollection == null || !linkNodeCollection.Any())
                return;
            var localLinks = LocalLinks(linkNodeCollection);
            foreach (var downloadUri in localLinks.Select(href => new Uri(baseUri, href)))
            {
                DownloadDocument(downloadUri);
            }
        }

        IEnumerable<string> LocalLinks(IEnumerable<HtmlNode> linkNodeCollection)
        {
            return linkNodeCollection.Select(x => x.Attributes["href"].Value).Where(x => x.LinkIsLocal(baseUri.ToString()) && x.LinkDoesNotContainAnchor());
        }

        public IDisposable Subscribe(IObserver<HtmlDoc> observer)
        {
            observers.Add(observer);
            return new Unsubscriber(observers, observer);
        }

        private class Unsubscriber : IDisposable
        {
            private readonly List<IObserver<HtmlDoc>> observers;
            private readonly IObserver<HtmlDoc> observer;

            public Unsubscriber(List<IObserver<HtmlDoc>> observers, IObserver<HtmlDoc> observer)
            {
                this.observers = observers;
                this.observer = observer;
            }

            public void Dispose()
            {
                if (observer != null && observers.Contains(observer))
                    observers.Remove(observer);
            }
        }
    }
}