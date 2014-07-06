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
// "SkyScraper.Tests/When_reading_example2_robots_txt.cs" was last cleaned by Rick on 2014/07/06 at 4:00 PM
#endregion

namespace SkyScraper.Tests.RobotsFixtures {
    using System.Collections.Generic;
    using System.IO;
    using FluentAssertions;
    using NUnit.Framework;

    [TestFixture]
    internal class When_reading_example2_robots_txt {
        [Test]
        public void Then_disallows_should_be_respected() {
            const string robotsTxt = "RobotsFixtures\\example2.txt";
            Robots.Load( File.ReadAllText( robotsTxt ) );
            var lines = new Queue< string >( File.ReadAllLines( robotsTxt ) );
            while ( lines.Peek() != "User-agent: *" ) {
                lines.Dequeue();
            }
            lines.Dequeue();
            while ( !lines.Peek()
                          .StartsWith( "#" ) ) {
                var line = lines.Dequeue();
                if ( string.IsNullOrEmpty( line ) ) {
                    continue;
                }
                var rule = line.Split( ' ' )[ 1 ].Replace( "*", "foo" );
                Robots.PathIsAllowed( rule )
                      .Should()
                      .BeFalse();
            }

            Robots.PathIsAllowed( "/path" )
                  .Should()
                  .BeTrue();
        }
    }
}
