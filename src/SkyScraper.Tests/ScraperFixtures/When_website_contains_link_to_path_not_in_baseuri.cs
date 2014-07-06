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
// "SkyScraper.Tests/When_website_contains_link_to_path_not_in_baseuri.cs" was last cleaned by Rick on 2014/07/06 at 4:00 PM
#endregion

namespace SkyScraper.Tests.ScraperFixtures {
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using FluentAssertions;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    internal class When_website_contains_link_to_path_not_in_baseuri : ConcernForScraper {
        private readonly List< HtmlDoc > htmlDocs = new List< HtmlDoc >();

        protected override void Context() {
            base.Context();
            this.Uri = new Uri( "http://test/foo/page1" );
            const String page = @"<html>
                         <a href=""/bar"">link1</a>
                         </html>";
            this.HttpClient.GetString( this.Uri )
                .Returns( Task.Factory.StartNew( () => page ) );
            this.HttpClient.GetString( Arg.Is< Uri >( x => x != this.Uri ) )
                .Returns( x => Task.Factory.StartNew( () => x.Arg< Uri >()
                                                             .PathAndQuery ) );
            this.OnNext = x => this.htmlDocs.Add( x );
        }

        [Test]
        public void Then_only_baseuri_should_be_downloaded() {
            this.htmlDocs.Should()
                .HaveCount( 1 )
                .And.Contain( x => x.Uri.ToString() == "http://test/foo/page1" );
        }
    }
}
