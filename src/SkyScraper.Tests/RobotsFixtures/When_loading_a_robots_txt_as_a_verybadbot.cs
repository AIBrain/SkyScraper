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
// "SkyScraper.Tests/When_loading_a_robots_txt_as_a_verybadbot.cs" was last cleaned by Rick on 2014/07/06 at 4:00 PM
#endregion

namespace SkyScraper.Tests.RobotsFixtures {
    using System;
    using System.IO;
    using FluentAssertions;
    using NUnit.Framework;

    [TestFixture]
    internal class When_loading_a_robots_txt_as_a_verybadbot {
        [TestFixtureSetUp]
        public void TestFixtureSetUp() {
            var path = Path.Combine( Environment.CurrentDirectory, "RobotsFixtures\\robots.txt" );
            var robots = File.OpenText( path )
                             .ReadToEnd();
            Robots.Load( robots, "verybadbot" );
        }

        [Test]
        public void Then_botpath_should_not_be_allowed() {
            Robots.PathIsAllowed( "/botpath" )
                  .Should()
                  .BeFalse();
        }

        [Test]
        public void Then_foo_path3_should_not_be_allowed() {
            Robots.PathIsAllowed( "/foo/path3" )
                  .Should()
                  .BeFalse();
        }

        [Test]
        public void Then_path1_filetxt_should_not_be_allowed() {
            Robots.PathIsAllowed( "/path1/file.txt" )
                  .Should()
                  .BeFalse();
        }

        [Test]
        public void Then_path1_foo_should_not_be_allowed() {
            Robots.PathIsAllowed( "/path1/foo" )
                  .Should()
                  .BeFalse();
        }

        [Test]
        public void Then_path1_should_not_be_allowed() {
            Robots.PathIsAllowed( "/path1" )
                  .Should()
                  .BeFalse();
        }

        [Test]
        public void Then_path2_should_not_be_allowed() {
            Robots.PathIsAllowed( "/path2" )
                  .Should()
                  .BeFalse();
        }
    }
}
