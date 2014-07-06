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
// "SkyScraper.Tests/When_website_contains_an_html_encoded_link.cs" was last cleaned by Rick on 2014/07/06 at 4:00 PM
#endregion

namespace SkyScraper.Tests.ScraperFixtures {
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using FluentAssertions;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    internal class When_website_contains_an_html_encoded_link : ConcernForScraper {
        private readonly List< HtmlDoc > _htmlDocs = new List< HtmlDoc >();
        private String _page;

        protected override void Context() {
            base.Context();
            this.Uri = new Uri( "http://test" );
            this._page = @"<html>
                         <a href=""http://test/page1&amp;"">link1</a>
                         </html>";
            this.HttpClient.GetString( this.Uri )
                .Returns( Task.Factory.StartNew( () => this._page ) );
            this.HttpClient.GetString( Arg.Is< Uri >( x => x != this.Uri ) )
                .Returns( x => Task.Factory.StartNew( () => x.Arg< Uri >()
                                                             .PathAndQuery ) );
            this.OnNext = x => this._htmlDocs.Add( x );
        }

        [Test]
        public void Then_htmldocs_should_contain_first_page() {
            this._htmlDocs.Should()
                .Contain( x => x.Uri.ToString() == "http://test/page1&" && x.Html == "/page1&" );
        }

        [Test]
        public void Then_htmldocs_should_contain_home_page() {
            this._htmlDocs.Should()
                .Contain( x => x.Uri.ToString() == "http://test/" && x.Html == this._page );
        }

        [Test]
        public void Then_two_htmldocs_should_be_returned() {
            this._htmlDocs.Count.Should()
                .Be( 2 );
        }
    }
}
