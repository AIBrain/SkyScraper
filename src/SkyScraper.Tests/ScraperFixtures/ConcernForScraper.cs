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
// "SkyScraper.Tests/ConcernForScraper.cs" was last cleaned by Rick on 2014/07/06 at 4:00 PM
#endregion

namespace SkyScraper.Tests.ScraperFixtures {
    using System;
    using NSubstitute;

    internal abstract class ConcernForScraper : ConcernFor< Scraper > {
        protected IHttpClient HttpClient;
        protected Action< HtmlDoc > OnNext;
        protected Uri Uri;

        protected override void Context() {
            this.HttpClient = Substitute.For< IHttpClient >();
        }

        protected override Scraper CreateClassUnderTest() {
            this.SUT = new Scraper( this.HttpClient, new ScrapedUrisDictionary() );
            this.SUT.DisableRobotsProtocol = true;
            this.SUT.Subscribe( this.OnNext );
            return this.SUT;
        }

        protected override void Because() {
            this.SUT.Scrape( this.Uri )
                .Wait();
        }
    }
}
