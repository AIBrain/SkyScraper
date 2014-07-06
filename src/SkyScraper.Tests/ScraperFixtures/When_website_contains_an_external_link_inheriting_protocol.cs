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
// "SkyScraper.Tests/When_website_contains_an_external_link_inheriting_protocol.cs" was last cleaned by Rick on 2014/07/06 at 4:00 PM
#endregion

namespace SkyScraper.Tests.ScraperFixtures {
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    [TestFixture]
    internal class When_website_contains_an_external_link_inheriting_protocol : ConcernForScraper {
        private readonly List< HtmlDoc > htmlDocs = new List< HtmlDoc >();
        private string page;

        protected override void Context() {
            base.Context();
            this.Uri = new Uri( "http://test" );
            this.page = @"<html>
                         <a href=""//foo.com/"">link1</a>
                         </html>";
            this.HttpClient.GetString( this.Uri )
                .Returns( Task.Factory.StartNew( () => this.page ) );
            this.OnNext = x => this.htmlDocs.Add( x );
        }

        [Test]
        public void Then_htmldocs_should_contain_home_page() {
            this.htmlDocs.Should()
                .Contain( x => x.Uri.ToString() == "http://test/" && x.Html == this.page );
        }

        [Test]
        public void Then_link_should_not_be_scraped() {
            this.HttpClient.DidNotReceive()
                .GetString( Arg.Is< Uri >( x => x.ToString() == "//file://foo.com/" ) );
        }

        [Test]
        public void Then_one_htmldoc_should_be_returned() {
            this.htmlDocs.Count.Should()
                .Be( 1 );
        }
    }
}
