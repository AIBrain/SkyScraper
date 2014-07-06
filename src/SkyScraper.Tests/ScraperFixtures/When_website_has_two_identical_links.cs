using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SkyScraper.Tests.ScraperFixtures
{
    [TestFixture]
    class When_website_has_two_identical_links : ConcernForScraper
    {
        readonly List<HtmlDoc> htmlDocs = new List<HtmlDoc>();

        protected override void Context()
        {
            base.Context();
            Uri = new Uri("http://test");
            var page = @"<html>
                         <a href=""page1"">link1</a>
                         <a href=""page1"">link1</a>
                         </html>";
            HttpClient.GetString(Uri).Returns(Task.Factory.StartNew(() => page));
            HttpClient.GetString(Arg.Is<Uri>(x => x != Uri)).Returns(x => Task.Factory.StartNew(() => x.Arg<Uri>().PathAndQuery));
            OnNext = x => htmlDocs.Add(x);
        }

        [Test]
        public void Then_link_should_be_downloaded_once()
        {
            HttpClient.Received(1).GetString(Arg.Is<Uri>(x => x.ToString() == "http://test/page1"));
        }
    }
}