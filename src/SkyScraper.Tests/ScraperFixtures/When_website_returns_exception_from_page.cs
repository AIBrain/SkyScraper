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
// "SkyScraper.Tests/When_website_returns_exception_from_page.cs" was last cleaned by Rick on 2014/07/06 at 4:00 PM
#endregion

namespace SkyScraper.Tests.ScraperFixtures {
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;
    using FluentAssertions;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    internal class When_website_returns_exception_from_page : ConcernForScraper {
        private const string Page = @"<html><a href=""page1"">link1</a></html>";
        private readonly List< HtmlDoc > htmlDocs = new List< HtmlDoc >();
        private bool error;
        private Uri uri;

        protected override void Context() {
            base.Context();
            this.Uri = new Uri( "http://test" );
            this.HttpClient.GetString( this.Uri )
                .Returns( Task.Factory.StartNew( () => Page ) );
            this.HttpClient.GetString( Arg.Is< Uri >( x => x != this.Uri ) )
                .Returns( Task.Run( () => { throw new HttpRequestException(); } ) );
            this.OnNext = x => this.htmlDocs.Add( x );
        }

        protected override Scraper CreateClassUnderTest() {
            this.SUT = base.CreateClassUnderTest();
            this.SUT.OnHttpClientException += delegate { this.error = true; };
            this.SUT.OnScrape += x => this.uri = x;
            return this.SUT;
        }

        [Test]
        public void Then_htmldocs_should_contain_home_page() {
            this.htmlDocs.Should()
                .Contain( x => x.Uri.ToString() == "http://test/" && x.Html == Page );
        }

        [Test]
        public void Then_link_should_be_scraped() {
            this.HttpClient.Received()
                .GetString( Arg.Is< Uri >( x => x.ToString() == "http://test/page1" ) );
        }

        [Test]
        public void Then_one_htmldoc_should_be_returned() {
            this.htmlDocs.Count.Should()
                .Be( 1 );
        }

        [Test]
        public void Then_error_should_be_true() {
            this.error.Should()
                .BeTrue();
        }

        [Test]
        public void Then_uri_should_be_set() {
            this.uri.ToString()
                .Should()
                .Be( "http://test/page1" );
        }
    }
}
