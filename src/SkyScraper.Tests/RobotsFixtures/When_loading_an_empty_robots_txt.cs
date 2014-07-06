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
// "SkyScraper.Tests/When_loading_an_empty_robots_txt.cs" was last cleaned by Rick on 2014/07/06 at 4:00 PM
#endregion

namespace SkyScraper.Tests.RobotsFixtures {
    using System;
    using FluentAssertions;
    using NUnit.Framework;

    [TestFixture]
    internal class When_loading_an_empty_robots_txt {
        [Test]
        public void Then_PathIsAllowed_should_return_true() {
            Robots.Load( null );
            Robots.PathIsAllowed( "" )
                  .Should()
                  .BeTrue();
        }

        [Test]
        public void Then_no_exception_is_thrown() {
            Action action = () => Robots.Load( null );
            action.ShouldNotThrow();
        }
    }
}
