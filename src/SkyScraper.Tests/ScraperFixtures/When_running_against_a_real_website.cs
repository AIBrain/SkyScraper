using System.IO;
using NUnit.Framework;
using SkyScraper.Observers.ConsoleWriter;
using System;
using SkyScraper.Observers.ImageScraper;

namespace SkyScraper.Tests.ScraperFixtures
{
    [TestFixture, Explicit]
    class When_running_against_a_real_website
    {
        [Test]
        public async void Then_images_should_be_saved()
        {
            var scraper = new Scraper(new AsyncHttpClient());
            var io = new ImageScraperObserver(new AsyncHttpClient(), new FileWriter(new DirectoryInfo("c:\\temp")));
            scraper.Subscribe(io);
            scraper.Subscribe(new ConsoleWriterObserver());
            scraper.Subscribe(x => Console.WriteLine(x.Uri));
            await scraper.Scrape(new Uri("http://www.cambridgecupcakes.com/"));
        }
    }
}