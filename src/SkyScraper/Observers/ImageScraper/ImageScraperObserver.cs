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
// "SkyScraper/ImageScraperObserver.cs" was last cleaned by Rick on 2014/07/06 at 4:36 PM
#endregion

namespace SkyScraper.Observers.ImageScraper {
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using CsQuery;

    public class ImageScraperObserver : IObserver< HtmlDoc > {
        private readonly ConcurrentDictionary< String, String > _downloadedImages = new ConcurrentDictionary< String, String >();
        private readonly IFileWriter _fileWriter;
        private readonly IHttpClient _httpClient;

        public ImageScraperObserver( IHttpClient httpClient, IFileWriter fileWriter ) {
            this._httpClient = httpClient;
            this._fileWriter = fileWriter;
        }

        public void OnNext( HtmlDoc htmlDoc ) {
            var baseUri = new Uri( htmlDoc.Uri.GetLeftPart( UriPartial.Path ) );
            if ( baseUri.Segments.Last()
                        .Contains( '.' ) ) {
                baseUri = new Uri( baseUri.ToString()
                                          .Substring( 0, baseUri.ToString()
                                                                .LastIndexOf( '/' ) ) );
            }
            CQ html = htmlDoc.Html;
            var imgSrcs = html[ "img" ].Select( x => x.GetAttribute( "src" ) )
                                       .Where( x => x.LinkIsLocal( baseUri.ToString() ) );
            var downloadUris = imgSrcs.Select( imgSrc => Uri.IsWellFormedUriString( imgSrc, UriKind.Absolute ) ? new Uri( imgSrc ) : new Uri( baseUri, imgSrc ) );
            downloadUris.AsParallel()
                        .ForAll( this.DownloadImage );
        }

        public void OnError( Exception error ) { }

        public void OnCompleted() { }

        private async void DownloadImage( Uri uri ) {
            var fileName = uri.Segments.Last();
            if ( !this._downloadedImages.TryAdd( fileName, null ) ) {
                return;
            }
            var imgBytes = await this._httpClient.GetByteArray( uri );
            this._fileWriter.Write( fileName, imgBytes );
        }
    }
}
