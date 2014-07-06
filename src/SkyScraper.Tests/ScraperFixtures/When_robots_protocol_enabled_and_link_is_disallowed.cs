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
// "SkyScraper.Tests/When_robots_protocol_enabled_and_link_is_disallowed.cs" was last cleaned by Rick on 2014/07/06 at 4:00 PM
#endregion

namespace SkyScraper.Tests.ScraperFixtures {
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using FluentAssertions;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    internal class When_robots_protocol_enabled_and_link_is_disallowed : ConcernForScraper {
        private readonly List< HtmlDoc > _htmlDocs = new List< HtmlDoc >();
        private string page;

        protected override void Context() {
            base.Context();
            this.Uri = new Uri( "http://test" );
            this.page = @"<html>
                         <a href=""page1"">link1</a>
                         </html>";
            var path = Path.Combine( Environment.CurrentDirectory, "ScraperFixtures\\robots.txt" );
            var robots = File.OpenText( path )
                             .ReadToEnd();
            this.HttpClient.GetString( this.Uri )
                .Returns( Task.Factory.StartNew( () => this.page ) );
            this.HttpClient.GetString( Arg.Is< Uri >( x => x != this.Uri ) )
                .Returns( x => Task.Factory.StartNew( () => x.Arg< Uri >()
                                                             .PathAndQuery ) );
            this.HttpClient.GetString( Arg.Is< Uri >( x => x == new Uri( "http://test/robots.txt" ) ) )
                .Returns( Task.Factory.StartNew( () => robots ) );
            this.OnNext = x => this._htmlDocs.Add( x );
        }

        protected override void Because() {
            this.SUT.DisableRobotsProtocol = false;
            base.Because();
        }

        [Test]
        public void Then_htmldocs_should_contain_home_page() {
            this._htmlDocs.Should()
                .Contain( x => x.Uri.ToString() == "http://test/" && x.Html == this.page );
        }

        [Test]
        public void Then_htmldocs_should_not_contain_first_page() {
            this._htmlDocs.Should()
                .NotContain( x => x.Uri.ToString() == "http://test/page1" && x.Html == "/page1" );
        }
    }
}
