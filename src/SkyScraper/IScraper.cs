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
// "SkyScraper/IScraper.cs" was last cleaned by Rick on 2014/07/06 at 4:36 PM
#endregion

namespace SkyScraper {
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    public interface IScraper {
        List< IObserver< HtmlDoc > > Observers { get; set; }
        TimeSpan TimeOut { set; }
        Regex IgnoreLinks { set; }
        int? MaxDepth { set; }
        Regex IncludeLinks { set; }
        Regex ObserverLinkFilter { set; }
        bool DisableRobotsProtocol { get; set; }

        IDisposable Subscribe( IObserver< HtmlDoc > observer );

        Task Scrape( Uri uri );

        event Action< Uri > OnScrape;
        event Action< Exception > OnHttpClientException;
    }
}
