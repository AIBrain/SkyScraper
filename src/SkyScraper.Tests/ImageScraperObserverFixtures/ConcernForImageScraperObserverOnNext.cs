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
// "SkyScraper.Tests/ConcernForImageScraperObserverOnNext.cs" was last cleaned by Rick on 2014/07/06 at 4:00 PM
#endregion

namespace SkyScraper.Tests.ImageScraperObserverFixtures {
    using NSubstitute;
    using Observers.ImageScraper;

    internal abstract class ConcernForImageScraperObserverOnNext : ConcernFor< ImageScraperObserver > {
        protected IFileWriter FileWriter;
        protected HtmlDoc HtmlDoc;
        protected IHttpClient HttpClient;

        protected override void Context() {
            this.HttpClient = Substitute.For< IHttpClient >();
            this.FileWriter = Substitute.For< IFileWriter >();
            this.HtmlDoc = new HtmlDoc();
        }

        protected override ImageScraperObserver CreateClassUnderTest() {
            return new ImageScraperObserver( this.HttpClient, this.FileWriter );
        }

        protected override void Because() {
            this.SUT.OnNext( this.HtmlDoc );
        }
    }
}
