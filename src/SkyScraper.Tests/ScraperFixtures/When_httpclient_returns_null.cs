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
// "SkyScraper.Tests/When_httpclient_returns_null.cs" was last cleaned by Rick on 2014/07/06 at 4:00 PM
#endregion

namespace SkyScraper.Tests.ScraperFixtures {
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using FluentAssertions;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    internal class When_httpclient_returns_null : ConcernForScraper {
        private readonly List< HtmlDoc > htmlDocs = new List< HtmlDoc >();
        private String page;

        protected override void Context() {
            base.Context();
            this.Uri = new Uri( "http://test" );
            this.page = null;
            this.HttpClient.GetString( this.Uri )
                .Returns( Task.Factory.StartNew( () => this.page ) );
            this.OnNext = x => this.htmlDocs.Add( x );
        }

        [Test]
        public void Then_no_htmldoc_should_be_returned() {
            this.htmlDocs.Should()
                .BeEmpty();
        }
    }
}
