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
// "SkyScraper/Extensions.cs" was last cleaned by Rick on 2014/07/06 at 4:36 PM
#endregion

namespace SkyScraper {
    using System;

    public static class Extensions {
        public static bool LinkIsLocal( this String link, String baseUri ) {
            if ( Uri.IsWellFormedUriString( link, UriKind.Absolute ) ) {
                if ( !link.StartsWith( baseUri ) || link.StartsWith( "//" ) ) {
                    return false;
                }
            }
            return Uri.IsWellFormedUriString( link, UriKind.Relative ) || link.StartsWith( baseUri );
        }

        public static bool LinkDoesNotContainAnchor( this String str ) {
            return !str.Contains( "#" );
        }
    }
}
