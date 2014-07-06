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
// "SkyScraper.Tests/When_html_contains_a_local_image_and_httpclient_throws_exception.cs" was last cleaned by Rick on 2014/07/06 at 4:00 PM
#endregion

namespace SkyScraper.Tests.ImageScraperObserverFixtures {
    using System;
    using System.Threading.Tasks;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    internal class When_html_contains_a_local_image_and_httpclient_throws_exception : ConcernForImageScraperObserverOnNext {
        protected override void Context() {
            base.Context();
            this.HtmlDoc.Html = @"<html>
                         <img src=""image.png"" />
                         </html>";
            this.HtmlDoc.Uri = new Uri( "http://test/" );
            this.HttpClient.GetByteArray( Arg.Any< Uri >() )
                .Returns( new Task< byte[] >( () => { throw new Exception(); } ) );
        }

        [Test]
        public void Then_file_should_not_be_written() {
            this.FileWriter.DidNotReceive()
                .Write( Arg.Any< string >(), Arg.Any< byte[] >() );
        }
    }
}
