//Thanks to ==> https://github.com/bryceg/Owin.WebSocket
/**
* Copyright (c) 2018, SOW (https://www.facebook.com/safeonlineworld). (https://github.com/RKTUXYN) All rights reserved.
* @author {SOW}
* Copyrights licensed under the New BSD License.
* See the accompanying LICENSE file for terms.
*/
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Owin;
namespace SOW.Web.Hub.Core.Extensions {
    public static class OwinExtension {
        /// <summary>
        /// Maps a static URI to a web socket consumer
        /// </summary>
        /// <typeparam name="T">Type of WebSocketHubConnection</typeparam>
        /// <param name="app">Owin App</param>
        /// <param name="route">Static URI to map to the hub</param>
        /// <param name="serviceLocator">Service locator to use for getting instances of T</param>
        public static void MapWebSocketRoute<T>( this IAppBuilder app, string route, IHubConfiguration hubConfig, object serviceLocator = null )
            where T : Hubs {
            if( hubConfig == null ) {
                hubConfig = new HubConfiguration { EnableDetailedErrors = false, EnableJavaScriptProxies = false };
            }
            Hubs.HubConfig = hubConfig;
            app.Map( route, config => config.Use<Middleware<T>>( serviceLocator ) );
        }

        /// <summary>
        /// Maps a URI pattern to a web socket consumer using a Regex pattern mach on the URI
        /// </summary>
        /// <typeparam name="T">Type of WebSocketHubConnection</typeparam>
        /// <param name="app">Owin app</param>        /// 
        /// <param name="regexPatternMatch">Regex pattern of the URI to match.  Capture groups will be sent to the hub on the Arguments property</param>
        /// <param name="serviceLocator">Service locator to use for getting instances of T</param>
        public static void MapWebSocketPattern<T>( this IAppBuilder app, string regexPatternMatch, object serviceLocator = null )
            where T : Hubs {
            app.Use<Middleware<T>>( serviceLocator, new Regex( regexPatternMatch, RegexOptions.Compiled | RegexOptions.IgnoreCase ) );
        }

        /// <summary>
        /// Maps a static URI route to the web socket connection using the WebSocketRouteAttribute
        /// </summary>
        /// <typeparam name="T">Type of WebSocketHubConnection</typeparam>
        /// <param name="app">Owin App</param>
        /// <param name="serviceLocator">Service locator to use for getting instances of T</param>
        public static void MapWebSocketRoute<T>( this IAppBuilder app, object serviceLocator = null )
            where T : Hubs {
            var routeAttributes = typeof( T ).GetCustomAttributes( typeof( RouteAttribute ), true );

            if ( routeAttributes.Length == 0 )
                throw new InvalidOperationException( typeof( T ).Name + " type must have attribute of WebSocketRouteAttribute for mapping" );

            foreach ( var routeAttribute in routeAttributes.Cast<RouteAttribute>( ) ) {
                app.Map( routeAttribute.Route, config => config.Use<Middleware<T>>( serviceLocator ) );
            }
        }

        internal static T Get<T>( this IDictionary<string, object> dictionary, string key ) {
            object item;
            if ( dictionary.TryGetValue( key, out item ) ) {
                return ( T )item;
            }

            return default( T );
        }
    }
}
