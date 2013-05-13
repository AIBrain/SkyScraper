﻿using NSubstitute;
using SkyScraper.Observers.ImageScraper;

namespace SkyScraper.Tests.ImageScraperObserverFixtures
{
    abstract class ConcernForImageScraperObserverOnNext : ConcernFor<ImageScraperObserver>
    {
        protected IHttpClient HttpClient;
        protected IFileWriter FileWriter;
        protected HtmlDoc HtmlDoc;

        protected override void Context()
        {
            HttpClient = Substitute.For<IHttpClient>();
            FileWriter = Substitute.For<IFileWriter>();
            HtmlDoc = new HtmlDoc();
        }

        protected override ImageScraperObserver CreateClassUnderTest()
        {
            return new ImageScraperObserver(HttpClient, FileWriter);
        }

        protected override void Because()
        {
            SUT.OnNext(HtmlDoc);
        }
    }
}
