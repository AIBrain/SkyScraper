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
// "SkyScraper.Tests/When_website_contains_a_link_longer_than_2048_characters.cs" was last cleaned by Rick on 2014/07/06 at 4:00 PM
#endregion

namespace SkyScraper.Tests.ScraperFixtures {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    [TestFixture]
    internal class When_website_contains_a_link_longer_than_2048_characters : ConcernForScraper {
        private readonly List< HtmlDoc > htmlDocs = new List< HtmlDoc >();
        private string link;
        private string page;

        protected override void Context() {
            base.Context();
            this.Uri = new Uri( "http://test" );
            this.link = Enumerable.Repeat( "a", 2048 )
                                  .Aggregate( string.Empty, ( s, s1 ) => s += s1 );
            this.page = @"<html>
                         <a>link1</a>
                         <a href=""{0}"">link1</a>
                         </html>";
            this.page = string.Format( this.page, this.link );
            this.HttpClient.GetString( this.Uri )
                .Returns( Task.Factory.StartNew( () => this.page ) );
            this.HttpClient.GetString( Arg.Is< Uri >( x => x != this.Uri ) )
                .Returns( x => Task.Factory.StartNew( () => x.Arg< Uri >()
                                                             .PathAndQuery ) );
            this.OnNext = x => this.htmlDocs.Add( x );
        }

        [Test]
        public void Then_link_should_not_be_followed() {
            this.HttpClient.DidNotReceive()
                .GetString( Arg.Is< Uri >( x => x.ToString()
                                                 .EndsWith( this.link ) ) );
        }

        [Test]
        public void Then_one_htmldoc_should_be_returned() {
            this.htmlDocs.Count.Should()
                .Be( 1 );
        }
    }
}
