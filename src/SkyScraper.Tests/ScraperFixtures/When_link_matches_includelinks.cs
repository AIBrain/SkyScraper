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
// "SkyScraper.Tests/When_link_matches_includelinks.cs" was last cleaned by Rick on 2014/07/06 at 4:00 PM
#endregion

namespace SkyScraper.Tests.ScraperFixtures {
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using FluentAssertions;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    internal class When_link_matches_includelinks : ConcernForScraper {
        private readonly List< HtmlDoc > htmlDocs = new List< HtmlDoc >();
        private String page;
        private IObserver< HtmlDoc > observer;

        protected override Scraper CreateClassUnderTest() {
            this.SUT = base.CreateClassUnderTest();
            this.observer = Substitute.For< IObserver< HtmlDoc > >();
            this.SUT.Subscribe( this.observer );
            this.SUT.IncludeLinks = new Regex( @"link1" );
            return this.SUT;
        }

        protected override void Context() {
            base.Context();
            this.Uri = new Uri( "http://test" );
            this.page = @"<html>
                    <a href=""link1"">link1</a>
                    <a href=""link2"">link2</a>
                    </html>";
            this.HttpClient.GetString( this.Uri )
                .Returns( Task.Factory.StartNew( () => this.page ) );
            this.HttpClient.GetString( Arg.Is< Uri >( x => x != this.Uri ) )
                .Returns( x => Task.Factory.StartNew( () => x.Arg< Uri >()
                                                             .PathAndQuery ) );
            this.OnNext = x => this.htmlDocs.Add( x );
        }

        [Test]
        public void Then_observer_should_be_notified_of_link2() {
            this.observer.Received()
                .OnNext( Arg.Is< HtmlDoc >( x => x.Uri.ToString()
                                                  .Contains( "link1" ) ) );
        }

        [Test]
        public void Then_two_htmldocs_should_be_returned() {
            this.htmlDocs.Count.Should()
                .Be( 2 );
        }
    }
}
