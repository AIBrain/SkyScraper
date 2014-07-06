using System;

namespace SkyScraper.Tests.RobotsFixtures
{
    using FluentAssertions;
    using NUnit.Framework;

    [TestFixture]
    class When_loading_an_empty_robots_txt
    {
        [Test]
        public void Then_no_exception_is_thrown()
        {
            Action action = () => Robots.Load(null);
            action.ShouldNotThrow();
        }

        [Test]
        public void Then_PathIsAllowed_should_return_true()
        {
            Robots.Load(null);
            Robots.PathIsAllowed("").Should().BeTrue();
        }
    }
}