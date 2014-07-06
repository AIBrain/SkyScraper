#region License
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified.
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
// 
// Royalties must be paid
//    via PayPal (paypal@aibrain.org)
//    via bitcoin (1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2)
//    via litecoin (LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9)
// 
// Usage of the source code or compiled binaries is AS-IS.
// 
// "SkyScraper/Scraper.cs" was last cleaned by Rick on 2014/07/06 at 4:36 PM
#endregion

namespace SkyScraper {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using CsQuery;

    public class Scraper : IScraper, IObservable< HtmlDoc > {
        private readonly IHttpClient _httpClient;
        private readonly IScrapedUris _scrapedUris;
        private Uri _baseUri;
        private DateTime? _endDateTime;

        public Scraper( IHttpClient httpClient, IScrapedUris scrapedUris ) {
            this._httpClient = httpClient;
            this._scrapedUris = scrapedUris;
            this.Observers = new List< IObserver< HtmlDoc > >();
        }

        public event Action< Uri > OnScrape = delegate { };
        public event Action< Exception > OnHttpClientException = delegate { };

        public List< IObserver< HtmlDoc > > Observers { get; set; }
        public TimeSpan TimeOut { set { this._endDateTime = DateTimeProvider.UtcNow + value; } }
        public int? MaxDepth { private get; set; }
        public Regex IgnoreLinks { private get; set; }
        public Regex IncludeLinks { private get; set; }
        public Regex ObserverLinkFilter { private get; set; }
        public bool DisableRobotsProtocol { get; set; }

        public IDisposable Subscribe( IObserver< HtmlDoc > observer ) {
            this.Observers.Add( observer );
            return new Unsubscriber( this.Observers, observer );
        }

        public async Task Scrape( Uri uri ) {
            this._baseUri = uri;

            if ( !this.DisableRobotsProtocol ) {
                var robotsUri = new Uri( uri.GetLeftPart( UriPartial.Authority ) + "/robots.txt" );
                var robotsTxt = await this._httpClient.GetString( robotsUri );
                Robots.Load( robotsTxt, this._httpClient.UserAgentName );
            }
            this.DoScrape( uri )
                .Wait();
        }

        private async Task DoScrape( Uri uri ) {
            this.OnScrape( uri );
            if ( this._endDateTime.HasValue && DateTimeProvider.UtcNow > this._endDateTime ) {
                return;
            }
            if ( !this._scrapedUris.TryAdd( uri ) ) {
                return;
            }
            if ( !this.DisableRobotsProtocol && !Robots.PathIsAllowed( uri.PathAndQuery ) ) {
                return;
            }
            var htmlDoc = new HtmlDoc {
                                          Uri = uri
                                      };
            try {
                htmlDoc.Html = await this._httpClient.GetString( uri );
            }
            catch ( Exception exception ) {
                this.OnHttpClientException( exception );
            }
            if ( String.IsNullOrEmpty( htmlDoc.Html ) ) {
                return;
            }
            if ( !( this.ObserverLinkFilter != null && !this.ObserverLinkFilter.IsMatch( uri.ToString() ) ) ) {
                this.NotifyObservers( htmlDoc );
            }

            var pageBase = htmlDoc.Uri.Segments.Last()
                                  .Contains( '.' ) ? htmlDoc.Uri.ToString()
                                                            .Substring( 0, htmlDoc.Uri.ToString()
                                                                                  .LastIndexOf( '/' ) ) : htmlDoc.Uri.ToString();
            if ( !pageBase.EndsWith( "/" ) ) {
                pageBase += "/";
            }
            var pageBaseUri = new Uri( pageBase );
            CQ cq = htmlDoc.Html;
            var links = cq[ "a" ].Select( x => x.GetAttribute( "href" ) )
                                 .Where( x => x != null );
            var localLinks = this.LocalLinks( links )
                                 .Select( x => this.NormalizeLink( x, pageBaseUri ) )
                                 .Where( x => x.ToString()
                                               .StartsWith( this._baseUri.ToString() ) && x.ToString()
                                                                                           .Length <= 2048 );
            if ( this.IncludeLinks != null ) {
                localLinks = localLinks.Where( x => this.IncludeLinks.IsMatch( x.ToString() ) );
            }
            if ( this.IgnoreLinks != null ) {
                localLinks = localLinks.Where( x => !this.IgnoreLinks.IsMatch( x.ToString() ) );
            }
            if ( this.MaxDepth.HasValue ) {
                localLinks = localLinks.Where( x => x.Segments.Length <= this.MaxDepth + 1 );
            }
            var tasks = localLinks.Select( this.DoScrape )
                                  .ToArray();
            Task.WaitAll( tasks );
        }

        private Uri NormalizeLink( String link, Uri pageBaseUri ) {
            if ( link.StartsWith( "/" ) ) {
                return new Uri( this._baseUri, link );
            }
            if ( link.StartsWith( this._baseUri.ToString() ) ) {
                return new Uri( link );
            }
            return new Uri( pageBaseUri, link );
        }

        private void NotifyObservers( HtmlDoc htmlDoc ) {
            this.Observers.ForEach( observer => observer.OnNext( htmlDoc ) );
        }

        private IEnumerable< String > LocalLinks( IEnumerable< String > links ) {
            return links.Select( WebUtility.HtmlDecode )
                        .Where( s => s.LinkIsLocal( this._baseUri.ToString() ) && s.LinkDoesNotContainAnchor() );
        }

        private class Unsubscriber : IDisposable {
            private readonly IObserver< HtmlDoc > _observer;
            private readonly List< IObserver< HtmlDoc > > _observers;

            public Unsubscriber( List< IObserver< HtmlDoc > > observers, IObserver< HtmlDoc > observer ) {
                this._observers = observers;
                this._observer = observer;
            }

            public void Dispose() {
                if ( this._observer != null && this._observers.Contains( this._observer ) ) {
                    this._observers.Remove( this._observer );
                }
            }
        }
    }
}
