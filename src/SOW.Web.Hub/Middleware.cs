/**
* Copyright (c) 2018, SOW (https://www.facebook.com/safeonlineworld). (https://github.com/RKTUXYN) All rights reserved.
* @author {SOW}
* Copyrights licensed under the New BSD License.
* See the accompanying LICENSE file for terms.
*/
//Thanks to ==> https://github.com/bryceg/Owin.WebSocket
namespace SOW.Web.Hub.Core {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using System.Web.Script.Serialization;
    using Microsoft.Owin;
    
    public class Middleware<T> : OwinMiddleware where T : Hubs {
        private readonly Regex _mMatchPattern;
        private readonly object mServiceLocator;
        private static readonly Task completedTask = Task.FromResult( false );

        public Middleware( OwinMiddleware next, object locator )
            : base( next ) {
            mServiceLocator = locator;
        }

        public Middleware( OwinMiddleware next, object locator, Regex matchPattern )
            : this( next, locator ) {
            _mMatchPattern = matchPattern;
        }

        public override Task Invoke( IOwinContext context ) {
            if ( context.Request.Method == "POST" ) {
                context.Response.StatusCode = ( int )System.Net.HttpStatusCode.MethodNotAllowed;
                return context.Response.WriteAsync( "Not Found ==> {0}" );
            }
            string req = context.Request.Path.Value;
            IUrlReq request = Util.GetRequest( context.Request.Path.Value );
            if ( request == null ) {
                context.Response.StatusCode = ( int )System.Net.HttpStatusCode.NotFound;
                return context.Response.WriteAsync( string.Format( "Not Found ==> {0}", req ) );
            }
            var orgin = context.Request.Headers.Get( "Origin" );
            if ( orgin != null ) {
                var uri = new Uri( orgin );
                if ( context.Request.Host.Value != uri.Host ) {
                    if ( !Hubs.HubConfig.EnableCrossDomain ) {
                        context.Response.StatusCode = ( int )System.Net.HttpStatusCode.Forbidden;
                        return context.Response.WriteAsync( string.Format( "Not Allowed for Orgin ==> {0}", orgin ) );
                    }

                    if ( Hubs.HubConfig.CrossDomains != null ) {

                        var item = Hubs.HubConfig.CrossDomains.FirstOrDefault( a => a == uri.Host );
                        if ( item == null ) {
                            context.Response.StatusCode = ( int )System.Net.HttpStatusCode.Forbidden;
                            return context.Response.WriteAsync( string.Format( "Not Allowed for Orgin ==> {0}", orgin ) );
                        }
                    }
                    if ( request.M == "connect" ) {
                        context.Response.Headers.Add( "Access-Control-Allow-Origin", new string[] { "*" } );
                    } else {
                        context.Response.Headers.Add( "Access-Control-Allow-Origin", new string[] { orgin } );
                    }
                    context.Response.Headers.Add( "Access-Control-Allow-Credentials", new string[] { "true" } );
                    context.Response.Headers.Add( "Access-Control-Allow-Methods", new string[] { "GET" } );
                    context.Response.Headers.Add( "Access-Control-Max-Age", new string[] { "3600" } );
                    context.Response.Headers.Add( "Access-Control-Allow-Headers", new string[] { "Content-Type, Accept, X-Requested-With, remember-me" } );
                }
            }
            if ( request.M != "proxy" ) {
                if ( request.M != "connect" ) {
                    if ( request.M != "__internal__" ) {
                        if ( request.M == "heart" ) {
                            context.Response.ContentType = "application/json";
                            return context.Response.WriteAsync( new JavaScriptSerializer( ).Serialize( new {
                                beat = "Yes"
                            } ) );
                        }
                        context.Response.StatusCode = ( int )System.Net.HttpStatusCode.NotFound;
                        return context.Response.WriteAsync( string.Format( "Not Found ==> {0}", req ) );
                    }
                }
            }
            T socketConnection = Activator.CreateInstance<T>( );
            if ( request.M == "proxy" ) {
                if ( !Hubs.HubConfig.EnableJavaScriptProxies ) {
                    context.Response.StatusCode = ( int )System.Net.HttpStatusCode.NotFound;
                    return context.Response.WriteAsync( string.Format( "Not Found ==> {0}", req ) );
                }
                Type type;
                type = Util.GetType( request.H, socketConnection.GetType( ) );
                if ( type == null ) {
                    context.Response.StatusCode = ( int )System.Net.HttpStatusCode.NotFound;
                    return context.Response.WriteAsync( string.Format( "Not Found ==> {0}", req ) );
                }
                MethodInfo[] methodInfoArray = type.GetMethods( BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public );
                IList<string> serverProxy = new List<string>( );
                (
                    from a in ( IEnumerable<MethodInfo> )methodInfoArray
                    where ( a.IsAbstract ? false : !a.IsVirtual )
                    select a ).Select<MethodInfo, MethodInfo>( ( MethodInfo a ) => {
                        serverProxy.Add( a.Name );
                        return a;
                    } ).ToList<MethodInfo>( );
                IList<string> clientProxy = new List<string>( );

                context.Response.ContentType = "text/javascript";
                return context.Response.WriteAsync( string.Concat( new string[] { "/*[\r\n* Sow.Api.Hub==> Server Proxy\r\n* System Generated==>" + DateTime.Now.ToString( ) + "\r\n]*/\r\n( function () {\r\n\tif ( typeof ( Sow ) !== 'object' )\r\n\t\tthrow new Error( 'sow.framework.js required.' );\r\n\tif ( typeof ( Sow.define ) !== 'function' )\r\n\t\tthrow new Error( 'sow.framework.js required.' );\r\n\tSow.define( 'Sow.Api.Hub.", request.H, "', function ( ) {\r\n\t\treturn {\r\n\t\t\tgetServerProxy: function ( ) {\r\n\t\t\t\treturn ", socketConnection._jss.Serialize( serverProxy ), ";\r\n\t\t\t}\r\n\t\t};\r\n\t} );\r\n}() );" } ) );
            }
            if ( request.M == "__internal__" ) {
                if( !Hubs.HubConfig.AllowInternalRequest ) {
                    context.Response.StatusCode = ( int )System.Net.HttpStatusCode.NotFound;
                    return context.Response.WriteAsync( string.Format( "Not Found ==> {0}", req ) );
                }
                return socketConnection.AcceptSocketAsync( request, false, context, null );
            }
            IDictionary<string, string> matches = new Dictionary<string, string>( );

            if ( _mMatchPattern != null ) {
                Match match = _mMatchPattern.Match( context.Request.Path.Value );
                if ( !match.Success )
                    return Next?.Invoke( context ) ?? completedTask;
                for ( int i = 1 ; i <= match.Groups.Count ; i++ ) {
                    string name = _mMatchPattern.GroupNameFromNumber( i );
                    Group value = match.Groups[i];
                    matches.Add( name, value.Value );
                }
            }
            bool isReconnect = string.IsNullOrEmpty( context.Request.Query["r"] ) == false;
            //socketConnection.is
            return socketConnection.AcceptSocketAsync( request, isReconnect, context, matches );
        }
    }
}