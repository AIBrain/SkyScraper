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
// "SkyScraper/Robots.cs" was last cleaned by Rick on 2014/07/06 at 4:36 PM
#endregion

namespace SkyScraper {
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    public static class Robots {
        private const String Allow = @"^Allow:\s";
        private const String Disallow = @"^Disallow:\s";
        private static readonly Regex AllowRegex = new Regex( Allow );
        private static readonly Regex Rules = new Regex( String.Format( "{0}|{1}", Disallow, Allow ) );
        private static ConcurrentQueue< Rule > aggregatedRules = new ConcurrentQueue< Rule >();

        public static String SiteMap { get; private set; }

        public static void Load( String robotsTxt, String userAgent = null ) {
            if ( String.IsNullOrEmpty( robotsTxt ) ) {
                return;
            }
            var allRulesList = new List< String >();
            var botRulesList = new List< String >();
            var currentAgents = new String[0];
            robotsTxt = Regex.Replace( robotsTxt, @"\r\n|\n\r|\n|\r", "\r\n" );
            var lines = new Queue< String >( robotsTxt.Split( new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries ) );
            while ( lines.Any() ) {
                SetSiteMap( lines );
                var readAgents = lines.ReadAgents()
                                      .ToArray();
                currentAgents = readAgents.Any() ? readAgents : currentAgents;
                if ( !lines.Any() ) {
                    continue;
                }
                var line = lines.Dequeue();
                if ( Rules.IsMatch( line ) && currentAgents.Any() && ( currentAgents.First() == "*" || currentAgents.Contains( userAgent ) ) ) {
                    if ( currentAgents.First() == "*" ) {
                        allRulesList.Add( line );
                    }
                    else {
                        botRulesList.Add( line );
                    }
                }
            }
            aggregatedRules = new ConcurrentQueue< Rule >( botRulesList.AsRules()
                                                                       .Concat( allRulesList.AsRules() ) );
        }

        public static bool PathIsAllowed( String path ) {
            foreach ( var rule in aggregatedRules.Where( rule => rule.Regex.IsMatch( path ) ) ) {
                return rule.IsAllowed;
            }
            return true;
        }

        private static Regex AsRegexRule( this String input ) {
            input = input.Split( ' ' )[ 1 ];
            input = Regex.Escape( input );
            input = input.Replace( "\\*", ".*" );
            if ( !input.EndsWith( ".*" ) ) {
                input += ".*";
            }
            input = String.Format( "^{0}$", input );
            return new Regex( input );
        }

        private static IEnumerable< Rule > AsRules( this IEnumerable< String > rules ) {
            return rules.Select( x => new Rule( x.AsRegexRule(), AllowRegex.IsMatch( x ) ) );
        }

        private static IEnumerable< String > ReadAgents( this Queue< String > lines ) {
            while ( lines.Any() && lines.Peek()
                                        .StartsWith( "User-agent: " ) ) {
                yield return lines.Dequeue()
                                  .Split( ' ' )[ 1 ];
            }
        }

        private static void SetSiteMap( this Queue< String > lines ) {
            if ( lines.Any() && lines.Peek()
                                     .StartsWith( "Sitemap: " ) ) {
                SiteMap = lines.Dequeue()
                               .Split( ' ' )[ 1 ];
            }
        }

        private class Rule {
            public Rule( Regex regex, bool isAllowed ) {
                this.Regex = regex;
                this.IsAllowed = isAllowed;
            }

            public bool IsAllowed { get; private set; }

            public Regex Regex { get; private set; }
        }
    }
}
