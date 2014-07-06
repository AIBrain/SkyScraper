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
// "SkyScraper/HttpClient.cs" was last cleaned by Rick on 2014/07/06 at 4:36 PM
#endregion

namespace SkyScraper {
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;

    public class HttpClient : IHttpClient {
        private readonly System.Net.Http.HttpClient httpClient;
        private string userAgentName;

        public HttpClient() {
            this.httpClient = new System.Net.Http.HttpClient( new HttpClientHandler {
                                                                                        AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip
                                                                                    } ) {
                                                                                            Timeout = TimeSpan.FromMinutes( 1 )
                                                                                        };
        }

        public string UserAgentName {
            set {
                this.userAgentName = value;
                const string name = "User-Agent";
                this.httpClient.DefaultRequestHeaders.Remove( name );
                this.httpClient.DefaultRequestHeaders.Add( name, value );
            }
            get { return this.userAgentName; }
        }

        public async Task< string > GetString( Uri uri ) {
            var bytes = await this.Get( uri, x => x.ReadAsByteArrayAsync() );
            return bytes == null ? null : Encoding.UTF8.GetString( bytes );
        }

        public async Task< byte[] > GetByteArray( Uri uri ) {
            return await this.Get( uri, x => x.ReadAsByteArrayAsync() );
        }

        public async Task< Stream > GetStream( Uri uri ) {
            return await this.Get( uri, x => x.ReadAsStreamAsync() );
        }

        private async Task< T > Get< T >( Uri uri, Func< HttpContent, Task< T > > content ) {
            using ( var httpRequestMessage = new HttpRequestMessage( HttpMethod.Get, uri ) ) {
                using ( var httpResponseMessage = await this.httpClient.SendAsync( httpRequestMessage ) ) {
                    if ( httpResponseMessage.StatusCode != HttpStatusCode.OK || httpRequestMessage.RequestUri != uri ) {
                        return default( T );
                    }
                    return await content( httpResponseMessage.Content );
                }
            }
        }
    }
}
